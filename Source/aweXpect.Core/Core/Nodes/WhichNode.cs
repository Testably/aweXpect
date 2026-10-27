using System;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Core.Sources;

namespace aweXpect.Core.Nodes;

internal class WhichNode<TSource, TMember> : Node
{
	/// <summary>
	///     The accessor of a member that returns a <see cref="Task{TResult}" /> or a
	///     <see cref="ValueTask{TResult}" /> of <typeparamref name="TMember" />, and that receives either the source
	///     value only or also the evaluation context and the cancellation token.
	/// </summary>
	private readonly Delegate? _asyncMemberAccessor;

	private readonly string? _contextMember;
	private readonly Func<TSource, TMember?>? _memberAccessor;
	private readonly string _memberName = "it";
	private readonly bool _negateMemberOnly;
	private readonly Node? _parent;
	private readonly string? _separator;
	private Node? _inner;

	public WhichNode(
		Node? parent,
		Func<TSource, TMember?> memberAccessor,
		string? separator = null,
		bool negateMemberOnly = false,
		string? memberName = null,
		string? contextMember = null)
		: this(parent, separator, negateMemberOnly, memberName, contextMember)
	{
		_memberAccessor = memberAccessor;
	}

	public WhichNode(
		Node? parent,
		Func<TSource, Task<TMember?>> asyncMemberAccessor,
		string? separator = null,
		bool negateMemberOnly = false,
		string? memberName = null,
		string? contextMember = null)
		: this(parent, separator, negateMemberOnly, memberName, contextMember)
	{
		_asyncMemberAccessor = asyncMemberAccessor;
	}

	public WhichNode(
		Node? parent,
		Func<TSource, ValueTask<TMember?>> asyncMemberAccessor,
		string? separator = null,
		bool negateMemberOnly = false,
		string? memberName = null,
		string? contextMember = null)
		: this(parent, separator, negateMemberOnly, memberName, contextMember)
	{
		_asyncMemberAccessor = asyncMemberAccessor;
	}

	public WhichNode(
		Node? parent,
		Func<TSource, IEvaluationContext, CancellationToken, Task<TMember?>> asyncMemberAccessor,
		string? separator = null,
		bool negateMemberOnly = false,
		string? memberName = null,
		string? contextMember = null)
		: this(parent, separator, negateMemberOnly, memberName, contextMember)
	{
		_asyncMemberAccessor = asyncMemberAccessor;
	}

	public WhichNode(
		Node? parent,
		Func<TSource, IEvaluationContext, CancellationToken, ValueTask<TMember?>> asyncMemberAccessor,
		string? separator = null,
		bool negateMemberOnly = false,
		string? memberName = null,
		string? contextMember = null)
		: this(parent, separator, negateMemberOnly, memberName, contextMember)
	{
		_asyncMemberAccessor = asyncMemberAccessor;
	}

	private WhichNode(
		Node? parent,
		string? separator,
		bool negateMemberOnly,
		string? memberName,
		string? contextMember)
	{
		_contextMember = contextMember;
		_parent = parent;
		_separator = separator;
		_negateMemberOnly = negateMemberOnly;
		_memberName = memberName ?? _memberName;
	}

	/// <inheritdoc />
	public override void AddConstraint(IConstraint constraint, string it = "it")
		=> _inner?.AddConstraint(constraint, it);

	/// <inheritdoc />
	public override Node AddMapping(MappingNode mappingNode)
		=> _inner?.AddMapping(mappingNode) ?? this;

	/// <inheritdoc />
	public override void AddNode(Node node, string? separator = null)
		=> _inner = node;

	/// <inheritdoc />
	/// <remarks>
	///     When the expectations on the member are combined with <c>And</c>/<c>Or</c>, a further continuation applies
	///     to their right-most operand. Otherwise, it continues this node and projects from its member.
	/// </remarks>
	public override Node ReplaceRightMostOperand<TState>(TState state, Func<TState, Node, Node> replace)
	{
		if (_inner is JunctionNode or SubjectContextsNode { Inner: JunctionNode, })
		{
			_inner = _inner.ReplaceRightMostOperand(state, replace);
			return this;
		}

		return replace(state, this);
	}

	/// <inheritdoc />
	public override async ValueTask<ConstraintResult> IsMetBy<TValue>(
		TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken) where TValue : default
	{
		ConstraintResult? parentResult = null;
		if (_parent != null)
		{
			parentResult = await _parent.IsMetBy(value, context, cancellationToken);
			if (parentResult.FurtherProcessingStrategy == FurtherProcessingStrategy.IgnoreCompletely)
			{
				return parentResult;
			}
		}

		if (_inner == null)
		{
			throw Tracing.WriteException(
				new InvalidOperationException("No inner node specified for the which node."));
		}

		if (context is ExpectationTextEvaluationContext)
		{
			ConstraintResult expectationResult = await _inner.IsMetBy<TMember>(default, context, cancellationToken);
			return CombineResults(parentResult, expectationResult, _separator ?? "",
				FurtherProcessingStrategy.IgnoreResult, default);
		}

		if (value is null || value is DelegateValue { IsNull: true, })
		{
			ConstraintResult nullResult = NullSubjectResult.Create(
				await _inner.IsMetBy<TMember>(default, ExpectationTextEvaluationContext.For(context),
					cancellationToken), default(TMember));
			return CombineResults(parentResult, nullResult, _separator ?? "",
				FurtherProcessingStrategy.IgnoreResult, default);
		}

		if (parentResult is { Outcome: not Outcome.Success, })
		{
			// The member only exists when the parent was met (e.g. the single item or the parsed value).
			ConstraintResult memberExpectation = await _inner.IsMetBy<TMember>(default,
				ExpectationTextEvaluationContext.For(context), cancellationToken);
			return new WhichConstraintResult(parentResult, memberExpectation, _separator ?? "",
				FurtherProcessingStrategy.IgnoreResult, default, _negateMemberOnly, true);
		}

		TSource? source = ResolveSource(parentResult, value);
		(TMember? Value, NullSubjectKind NullKind) matching;
		try
		{
			matching = await ComputeMatchingValueAsync(source, context, cancellationToken);
		}
		catch (Exception exception)
		{
			UserCodeException? userCodeException = exception as UserCodeException;
			Exception cause = userCodeException?.Exception ?? exception;
			if (MemberExceptionResult.IsCancellationOf(cause, cancellationToken))
			{
				ExceptionDispatchInfo.Capture(cause).Throw();
			}

			ConstraintResult exceptionResult = MemberExceptionResult.Create(
				await _inner.IsMetBy<TMember>(default, ExpectationTextEvaluationContext.For(context),
					cancellationToken), cause, userCodeException?.Thrower ?? _memberName, default(TMember));
			return CombineResults(parentResult, exceptionResult, _separator ?? "",
				FurtherProcessingStrategy.IgnoreResult, default);
		}

		if (matching.NullKind == NullSubjectKind.NullTaskReturned)
		{
			ConstraintResult nullTaskResult = NullSubjectResult.CreateForNullTask(
				await _inner.IsMetBy<TMember>(default, ExpectationTextEvaluationContext.For(context),
					cancellationToken), _memberName, default(TMember));
			return CombineResults(parentResult, nullTaskResult, _separator ?? "",
				FurtherProcessingStrategy.IgnoreResult, default);
		}

		ConstraintResult innerResult;
		using (EvaluationContext.EvaluationContext.StartNestedEvaluation(context))
		{
			innerResult = await _inner.IsMetBy(matching.Value, context, cancellationToken);
		}

		return CombineResults(parentResult, innerResult, _separator ?? "", FurtherProcessingStrategy.IgnoreResult,
			matching.Value);
	}

	private static TSource? ResolveSource(ConstraintResult? parentResult, object value)
	{
		if (parentResult != null && parentResult.TryGetStoredValue(out TSource? projectedValue))
		{
			return projectedValue;
		}

		if (value is TSource directValue)
		{
			return directValue;
		}

		throw Tracing.WriteException(
			new InvalidOperationException(
				$"The member type for the actual value in the which node did not match.{Environment.NewLine}Expected: {Formatter.Format(typeof(TSource))}{Environment.NewLine}   Found: {Formatter.Format(value.GetType())}"));
	}

	private async ValueTask<(TMember? Value, NullSubjectKind NullKind)> ComputeMatchingValueAsync(TSource? source,
		IEvaluationContext context, CancellationToken cancellationToken)
	{
#pragma warning disable S2583
		if (source is null)
		{
			return (default, NullSubjectKind.None);
		}
#pragma warning restore S2583

		if (_memberAccessor != null)
		{
			return (_memberAccessor(source), NullSubjectKind.None);
		}

		Task<TMember?>? task;
		switch (_asyncMemberAccessor)
		{
			case Func<TSource, Task<TMember?>> taskMemberAccessor:
				task = taskMemberAccessor.Invoke(source);
				break;
			case Func<TSource, IEvaluationContext, CancellationToken, Task<TMember?>> taskMemberAccessor:
				task = taskMemberAccessor.Invoke(source, context, cancellationToken);
				break;
			default:
				ValueTask<TMember?> valueTask =
					_asyncMemberAccessor is Func<TSource, ValueTask<TMember?>> valueTaskMemberAccessor
						? valueTaskMemberAccessor.Invoke(source)
						: ((Func<TSource, IEvaluationContext, CancellationToken, ValueTask<TMember?>>)
							_asyncMemberAccessor!).Invoke(source, context, cancellationToken);
				if (valueTask.IsCompletedSuccessfully)
				{
					return (valueTask.Result, NullSubjectKind.None);
				}

				return (await valueTask.AsTask().AbandonOnCancellation(cancellationToken), NullSubjectKind.None);
		}

		return task is null
			? (default, NullSubjectKind.NullTaskReturned)
			: (await task.AbandonOnCancellation(cancellationToken), NullSubjectKind.None);
	}

	/// <inheritdoc cref="object.Equals(object?)" />
	public override bool Equals(object? obj) => obj is WhichNode<TSource, TMember> other && Equals(other);

	private bool Equals(WhichNode<TSource, TMember> other) =>
		Equals(_parent, other._parent) &&
		Equals(_inner, other._inner);

	/// <inheritdoc cref="object.GetHashCode()" />
	public override int GetHashCode() => _parent?.GetHashCode() ?? 17;

	private ConstraintResult CombineResults(ConstraintResult? leftResult,
		ConstraintResult rightResult,
		string separator,
		FurtherProcessingStrategy? furtherProcessingStrategy,
		TMember? value)
	{
		if (leftResult == null)
		{
			if (separator.Length == 0)
			{
				return _contextMember is null ? rightResult : rightResult.PrependExpectationText(null, _contextMember);
			}

			return PrependSeparator(rightResult, separator, _contextMember);
		}

		return new WhichConstraintResult(leftResult, rightResult, separator,
			furtherProcessingStrategy ?? FurtherProcessingStrategy.Continue,
			value, _negateMemberOnly, false, _contextMember);
	}

	/// <remarks>
	///     A separate method, so that the closure over the <paramref name="separator" /> is only allocated when it is
	///     prepended, and not on every combination.
	/// </remarks>
	private static ConstraintResult PrependSeparator(ConstraintResult result, string separator, string? contextMember)
		=> result.PrependExpectationText(sb => sb.Append(separator.TrimStart()), contextMember);

	/// <inheritdoc />
	/// <remarks>
	///     Without a parent the separator is trimmed at the start, so that the member stays part of the expectation.
	/// </remarks>
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		_parent?.AppendExpectation(stringBuilder, indentation);
		stringBuilder.AppendSeparatedExpectation(
			_parent == null ? _separator?.TrimStart() ?? "" : _separator ?? "",
			sb => _inner?.AppendExpectation(sb, indentation));
	}

	private sealed class WhichConstraintResult : CombinedResult
	{
		private readonly string? _contextMember;

		/// <summary>
		///     The formatters of the evaluation that created this result, which format the expectation text of the member
		///     also when the result is negated after that evaluation, e.g. by <c>DoesNotComplyWith</c>.
		/// </summary>
		private readonly IValueFormatter[]? _formatters = ValueFormatter.ScopedFormatters;

		private readonly bool _isMemberSkipped;
		private readonly bool _negateMemberOnly;
		private readonly string _separator;

		// ReSharper disable once ReplaceWithPrimaryConstructorParameter
		private readonly TMember? _value;

		/// <summary>
		///     The positive expectation text of the member, which a negated result keeps, as only the left part renders
		///     the negation.
		/// </summary>
		private string? _negatedRightExpectation;

		public WhichConstraintResult(ConstraintResult left,
			ConstraintResult right,
			string separator,
			FurtherProcessingStrategy furtherProcessingStrategy,
			TMember? value,
			bool negateMemberOnly,
			bool isMemberSkipped,
			string? contextMember = null) : base(left, right, true, furtherProcessingStrategy)
		{
			_contextMember = contextMember;
			_separator = separator;
			_value = value;
			_negateMemberOnly = negateMemberOnly;
			_isMemberSkipped = isMemberSkipped;
			Outcome = isMemberSkipped ? left.Outcome : CombineOutcomes();
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			Left.AppendExpectation(stringBuilder, indentation);
			if (_negatedRightExpectation is not null)
			{
				stringBuilder.Append(_negatedRightExpectation);
				return;
			}

			stringBuilder.AppendSeparatedExpectation(_separator, Right, indentation);
		}

		/// <inheritdoc />
		/// <remarks>
		///     Only one part explains the outcome. Under negation both parts were met, so the left part explains the
		///     failure, unless the member could not be answered.
		/// </remarks>
		protected override (bool Left, bool Right) GetExplainingParts()
		{
			if (IsNegated && Right.Outcome == Outcome.FailureBothWays)
			{
				return (false, true);
			}

			if (Left.ExplainsOutcomeOf(this))
			{
				return (true, false);
			}

			return (false, Right.ExplainsOutcomeOf(this));
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			(bool rendersLeft, bool rendersRight) = GetExplainingParts();
			if (rendersLeft)
			{
				Left.AppendResult(stringBuilder, indentation);
			}
			else if (rendersRight)
			{
				Right.AppendResult(stringBuilder, indentation);
			}
		}

		/// <inheritdoc />
		protected override void VisitRight(ResultContextCollector contexts)
			=> contexts.VisitOptionalMember(_contextMember, Right);

		public override bool TryGetStoredValue<TValue>(out TValue? value)
			where TValue : default
		{
			if (_isMemberSkipped)
			{
				return Left.TryGetStoredValue(out value);
			}

			if (_value is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			if (base.TryGetStoredValue(out value))
			{
				return true;
			}

			// When neither this result nor its sub-chains carry a TValue, fall through to a
			// type-compatibility check so chained WhichNodes can keep propagating projections
			// even when the recorded matching value happens to be null (e.g. an outer
			// `Which(p => p.Address)` projecting `null`).
			return typeof(TValue).IsAssignableFrom(typeof(TMember));
		}

		protected internal override void NegateInPlace()
		{
			if (_isMemberSkipped)
			{
				NegateWithoutMember();
				return;
			}

			if (_negateMemberOnly)
			{
				// The parent keeps its positive form, so a failed parent still fails the combination.
				Right.NegateInPlace();
				Outcome = CombineOutcomes();
				return;
			}

			IsNegated = !IsNegated;
			Left.NegateInPlace();
			_negatedRightExpectation = IsNegated ? GetRightExpectation() : null;
			Right.NegateInPlace();
			Outcome = CombineOutcomes();
		}

		/// <remarks>
		///     Without a member, the parent alone decides the outcome, also when it stays failed under negation.
		/// </remarks>
		private void NegateWithoutMember()
		{
			if (_negateMemberOnly)
			{
				Right.NegateInPlace();
			}
			else
			{
				Left.NegateInPlace();
				IsNegated = !IsNegated;
			}

			Outcome = Left.Outcome;
		}

		private string GetRightExpectation()
		{
			using ValueFormatter.FormatterScope _ = ValueFormatter.UseScopedFormatters(_formatters);
			StringBuilder stringBuilder = new();
			stringBuilder.AppendSeparatedExpectation(_separator, Right, null);
			return stringBuilder.ToString();
		}
	}
}
