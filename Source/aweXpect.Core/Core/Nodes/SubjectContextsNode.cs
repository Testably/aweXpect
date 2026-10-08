using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core.Nodes;

/// <summary>
///     Adds the contexts of the subject, see <see cref="ExpectationBuilder.AddSubjectContexts{TValue}" />, to the
///     result of the <see cref="Inner" /> node, which holds all expectations on the subject.
/// </summary>
/// <remarks>
///     The result is also wrapped when it is met, as an outer negation can still fail it.
/// </remarks>
internal sealed class SubjectContextsNode(Node inner, List<SubjectContext> subjectContexts) : Node
{
	private readonly List<SubjectContext> _subjectContexts = subjectContexts;

	/// <summary>
	///     The node with the expectations on the subject.
	/// </summary>
	public Node Inner { get; private set; } = inner;

	/// <inheritdoc />
	public override void AddConstraint(IConstraint constraint)
		=> Inner.AddConstraint(constraint);

	/// <inheritdoc />
	public override Node AddMapping(MappingNode mappingNode)
		=> Inner.AddMapping(mappingNode);

	/// <inheritdoc />
	public override void AddNode(Node node, string? separator = null)
		=> Inner.AddNode(node, separator);

	/// <inheritdoc />
	public override Node ReplaceRightMostOperand(Func<Node, Node> replace)
	{
		Inner = Inner.ReplaceRightMostOperand(replace);
		return this;
	}

	/// <inheritdoc />
	public override ValueTask<ConstraintResult> IsMetBy<TValue>(TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken) where TValue : default
	{
		ValueTask<ConstraintResult> isMet = Inner.IsMetBy(value, context, cancellationToken);
		if (context is ExpectationTextEvaluationContext)
		{
			return isMet;
		}

		if (!isMet.IsCompletedSuccessfully)
		{
			return AwaitAndWrap(isMet, value);
		}

		ConstraintResult result = isMet.Result;
		return new ValueTask<ConstraintResult>(new SubjectContextsResult<TValue>(result, value, _subjectContexts));
	}

	private async ValueTask<ConstraintResult> AwaitAndWrap<TValue>(ValueTask<ConstraintResult> isMet, TValue? value)
		=> new SubjectContextsResult<TValue>(await isMet, value, _subjectContexts);

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> Inner.AppendExpectation(stringBuilder, indentation);

	/// <inheritdoc cref="object.Equals(object?)" />
	public override bool Equals(object? obj)
		=> obj is SubjectContextsNode other && Inner.Equals(other.Inner) &&
		   _subjectContexts.Select(c => c.Callback).SequenceEqual(other._subjectContexts.Select(c => c.Callback));

	/// <inheritdoc cref="object.GetHashCode()" />
	public override int GetHashCode() => Inner.GetHashCode();

	private sealed class SubjectContextsResult<TValue> : ConstraintResult
	{
		private readonly List<SubjectContext> _subjectContexts;
		private readonly TValue? _value;
		private ConstraintResult _inner;

		public SubjectContextsResult(ConstraintResult inner, TValue? value, List<SubjectContext> subjectContexts)
			: base(inner.FurtherProcessingStrategy)
		{
			_inner = inner;
			_value = value;
			_subjectContexts = subjectContexts;
		}

		/// <inheritdoc cref="ConstraintResult.Outcome" />
		public override Outcome Outcome => _inner.Outcome;

		/// <inheritdoc cref="ConstraintResult.FailureCause" />
		public override Exception? FailureCause => _inner.FailureCause;

		/// <inheritdoc cref="ConstraintResult.LeadingSubject" />
		public override string? LeadingSubject => _inner.LeadingSubject;

		/// <inheritdoc cref="ConstraintResult.TrailingSubject" />
		public override string? TrailingSubject => _inner.TrailingSubject;

		/// <inheritdoc cref="ConstraintResult.IsExpectationOnly" />
		internal override bool IsExpectationOnly => _inner.IsExpectationOnly;

		/// <inheritdoc cref="ConstraintResult.IsNegatedAnd" />
		internal override bool IsNegatedAnd => _inner.IsNegatedAnd;

		/// <inheritdoc cref="ConstraintResult.AppendExpectation(StringBuilder, string?)" />
		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> _inner.AppendExpectation(stringBuilder, indentation);

		/// <inheritdoc cref="ConstraintResult.AppendResult(StringBuilder, string?)" />
		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> _inner.AppendResult(stringBuilder, indentation);

		/// <inheritdoc cref="ConstraintResult.TryGetStoredValue{TValue}(out TValue)" />
		public override bool TryGetStoredValue<T>(out T? value) where T : default
			=> _inner.TryGetStoredValue(out value);

		/// <inheritdoc cref="ConstraintResult.Negate()" />
		public override ConstraintResult Negate()
		{
			_inner = _inner.Negate();
			return this;
		}

		/// <inheritdoc cref="ConstraintResult.AppendContexts(ResultContextCollector)" />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			foreach (SubjectContext subjectContext in _subjectContexts)
			{
				subjectContext.AppendContexts(_value, contexts);
			}

			contexts.Visit(_inner);
		}
	}
}

/// <summary>
///     The callback of <see cref="ExpectationBuilder.AddSubjectContexts{TValue}" />.
/// </summary>
internal abstract class SubjectContext
{
	/// <summary>
	///     The callback, which identifies the subject context, so that it is only added once.
	/// </summary>
	public abstract Delegate Callback { get; }

	/// <summary>
	///     Adds the contexts of the <paramref name="value" /> to the <paramref name="contexts" />, unless it is not of the
	///     type of the callback.
	/// </summary>
	public abstract void AppendContexts<TValue>(TValue? value, ResultContextCollector contexts);
}

/// <inheritdoc cref="SubjectContext" />
internal sealed class SubjectContext<TSubject>(Action<TSubject, ResultContextCollector> appendContexts)
	: SubjectContext
{
	/// <inheritdoc />
	public override Delegate Callback => appendContexts;

	/// <inheritdoc />
	public override void AppendContexts<TValue>(TValue? value, ResultContextCollector contexts) where TValue : default
	{
		if (value is TSubject subject)
		{
			appendContexts(subject, contexts);
		}
	}
}
