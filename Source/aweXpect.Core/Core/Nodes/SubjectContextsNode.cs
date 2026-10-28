using System;
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
///     The result is also wrapped when it is met, as an outer negation can still fail it, unless
///     <paramref name="wrapsMetResult" /> is <see langword="false" /> for the whole expectation, whose result nothing
///     negates.
///     <para />
///     For the expectations on a member, the result is not wrapped when <paramref name="isOfMember" /> is set, because
///     the <see cref="MappingNode" /> adds the <see cref="SubjectContexts" /> to the result that it creates anyway.
/// </remarks>
internal sealed class SubjectContextsNode(
	Node inner,
	SubjectContext subjectContexts,
	bool wrapsMetResult = true,
	bool isOfMember = false)
	: Node
{
	private readonly SubjectContext _subjectContexts = subjectContexts;

	/// <summary>
	///     The node with the expectations on the subject.
	/// </summary>
	public Node Inner { get; private set; } = inner;

	/// <summary>
	///     Whether the <see cref="SubjectContexts" /> are added by the <see cref="MappingNode" /> of the member, instead
	///     of by this node.
	/// </summary>
	public bool IsOfMember { get; } = isOfMember;

	/// <summary>
	///     The subject contexts of the subject.
	/// </summary>
	public SubjectContext SubjectContexts => _subjectContexts;

	/// <inheritdoc />
	public override void AddConstraint(IConstraint constraint, string it = "it")
		=> Inner.AddConstraint(constraint, it);

	/// <inheritdoc />
	public override Node AddMapping(MappingNode mappingNode)
		=> Inner.AddMapping(mappingNode);

	/// <inheritdoc />
	public override void AddNode(Node node, string? separator = null)
		=> Inner.AddNode(node, separator);

	/// <inheritdoc />
	public override Node ReplaceRightMostOperand<TState>(TState state, Func<TState, Node, Node> replace)
	{
		Inner = Inner.ReplaceRightMostOperand(state, replace);
		return this;
	}

	/// <inheritdoc />
	public override ValueTask<ConstraintResult> IsMetBy<TValue>(TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken) where TValue : default
	{
		ValueTask<ConstraintResult> isMet = Inner.IsMetBy(value, context, cancellationToken);
		if (IsOfMember || context is ExpectationTextEvaluationContext)
		{
			return isMet;
		}

		if (!isMet.IsCompletedSuccessfully)
		{
			return AwaitAndWrap(isMet, value);
		}

		ConstraintResult result = isMet.Result;
		return !wrapsMetResult && result.Outcome == Outcome.Success
			? isMet
			: new ValueTask<ConstraintResult>(new SubjectContextsResult<TValue>(result, value, _subjectContexts));
	}

	private async ValueTask<ConstraintResult> AwaitAndWrap<TValue>(ValueTask<ConstraintResult> isMet, TValue? value)
	{
		ConstraintResult result = await isMet;
		return !wrapsMetResult && result.Outcome == Outcome.Success
			? result
			: new SubjectContextsResult<TValue>(result, value, _subjectContexts);
	}

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> Inner.AppendExpectation(stringBuilder, indentation);

	/// <inheritdoc cref="object.Equals(object?)" />
	public override bool Equals(object? obj)
		=> obj is SubjectContextsNode other && Inner.Equals(other.Inner) &&
		   HaveSameCallbacks(_subjectContexts, other._subjectContexts);

	private static bool HaveSameCallbacks(SubjectContext? left, SubjectContext? right)
	{
		while (left is not null && right is not null)
		{
			if (!left.Callback.Equals(right.Callback))
			{
				return false;
			}

			left = left.Next;
			right = right.Next;
		}

		return left is null && right is null;
	}

	/// <inheritdoc cref="object.GetHashCode()" />
	public override int GetHashCode() => Inner.GetHashCode();

	/// <summary>
	///     The result of the expectations on a subject, which adds the contexts of the subject.
	/// </summary>
	internal sealed class SubjectContextsResult<TValue> : ConstraintResult
	{
		private readonly SubjectContext _subjectContexts;
		private readonly TValue? _value;
		private readonly ConstraintResult _inner;

		public SubjectContextsResult(ConstraintResult inner, TValue? value, SubjectContext subjectContexts)
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

		/// <inheritdoc cref="ConstraintResult.NegateInPlace()" />
		protected internal override void NegateInPlace()
		{
			_inner.NegateInPlace();
		}

		/// <inheritdoc cref="ConstraintResult.AppendContexts(ResultContextCollector)" />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			for (SubjectContext? subjectContext = _subjectContexts;
			     subjectContext is not null;
			     subjectContext = subjectContext.Next)
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
/// <remarks>
///     The subject contexts of a subject are chained in the order they were added, as a subject rarely has more than
///     one and a list would cost more.
/// </remarks>
internal abstract class SubjectContext
{
	/// <summary>
	///     The subject context that was added after this one, or <see langword="null" />.
	/// </summary>
	public SubjectContext? Next { get; set; }

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
