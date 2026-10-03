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
	private readonly Func<TSource, Task<TMember?>>? _asyncMemberAccessor;
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
	{
		_contextMember = contextMember;
		_parent = parent;
		_memberAccessor = memberAccessor;
		_separator = separator;
		_negateMemberOnly = negateMemberOnly;
		_memberName = memberName ?? _memberName;
	}

	public WhichNode(
		Node? parent,
		Func<TSource, Task<TMember?>> asyncMemberAccessor,
		string? separator = null)
	{
		_parent = parent;
		_asyncMemberAccessor = asyncMemberAccessor;
		_separator = separator;
	}

	/// <inheritdoc />
	public override void AddConstraint(IConstraint constraint)
		=> _inner?.AddConstraint(constraint);

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
	public override Node ReplaceRightMostOperand(Func<Node, Node> replace)
	{
		if (_inner is JunctionNode)
		{
			_inner = _inner.ReplaceRightMostOperand(replace);
			return this;
		}

		return replace(this);
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
		(TMember? Value, bool IsNullTask) matching;
		try
		{
			matching = await ComputeMatchingValueAsync(source);
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

		if (matching.IsNullTask)
		{
			ConstraintResult nullTaskResult = NullSubjectResult.CreateForNullTask(
				await _inner.IsMetBy<TMember>(default, ExpectationTextEvaluationContext.For(context),
					cancellationToken), _memberName, default(TMember));
			return CombineResults(parentResult, nullTaskResult, _separator ?? "",
				FurtherProcessingStrategy.IgnoreResult, default);
		}

		ConstraintResult innerResult = await _inner.IsMetBy(matching.Value, context, cancellationToken);
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

	private async ValueTask<(TMember? Value, bool IsNullTask)> ComputeMatchingValueAsync(TSource? source)
	{
#pragma warning disable S2583
		if (source is null)
		{
			return (default, false);
		}
#pragma warning restore S2583

		if (_memberAccessor != null)
		{
			return (_memberAccessor(source), false);
		}

		Task<TMember?>? task = _asyncMemberAccessor!.Invoke(source);
		return task is null ? (default, true) : (await task, false);
	}

	/// <inheritdoc cref="object.Equals(object?)" />
	public override bool Equals(object? obj) => obj is WhichNode<TSource, TMember> other && Equals(other);

	private bool Equals(WhichNode<TSource, TMember> other) =>
		_parent?.Equals(other._parent) != false &&
		_inner?.Equals(other._inner) != false;

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
			Left.AppendExpectation(stringBuilder);
			if (_negatedRightExpectation is not null)
			{
				stringBuilder.Append(_negatedRightExpectation);
				return;
			}

			stringBuilder.AppendSeparatedExpectation(_separator, Right);
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

		public override ConstraintResult Negate()
		{
			if (_isMemberSkipped)
			{
				return NegateWithoutMember();
			}

			if (_negateMemberOnly)
			{
				// The parent keeps its positive form, so a failed parent still fails the combination.
				Right = Right.Negate();
				Outcome = CombineOutcomes();
				return this;
			}

			IsNegated = !IsNegated;
			Left = Left.Negate();
			_negatedRightExpectation = IsNegated ? GetRightExpectation() : null;
			Right = Right.Negate();
			Outcome = CombineOutcomes();
			return this;
		}

		/// <remarks>
		///     Without a member, the parent alone decides the outcome, also when it stays failed under negation.
		/// </remarks>
		private WhichConstraintResult NegateWithoutMember()
		{
			if (_negateMemberOnly)
			{
				Right = Right.Negate();
			}
			else
			{
				Left = Left.Negate();
				IsNegated = !IsNegated;
			}

			Outcome = Left.Outcome;
			return this;
		}

		private string GetRightExpectation()
		{
			StringBuilder stringBuilder = new();
			stringBuilder.AppendSeparatedExpectation(_separator, Right);
			return stringBuilder.ToString();
		}
	}
}
