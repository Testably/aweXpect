using System;
using System.Diagnostics.CodeAnalysis;
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
		string? memberName = null)
	{
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
	public override Node AddMapping<TValue, TTarget>(MemberAccessor<TValue, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
		=> _inner?.AddMapping(memberAccessor, expectationTextGenerator) ?? this;

	/// <inheritdoc />
	public override Node AddNarrowingMapping<TValue, TTarget, TNarrowed>(
		MemberAccessor<TValue, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
		where TNarrowed : default
		=> _inner?.AddNarrowingMapping<TValue, TTarget, TNarrowed>(memberAccessor, expectationTextGenerator) ?? this;

	/// <inheritdoc />
	public override Node AddAsyncMapping<TValue, TTarget>(
		MemberAccessor<TValue, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
		=> _inner?.AddAsyncMapping(memberAccessor, expectationTextGenerator) ?? this;

	/// <inheritdoc />
	public override Node AddAsyncNarrowingMapping<TValue, TTarget, TNarrowed>(
		MemberAccessor<TValue, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
		where TNarrowed : default
		=> _inner?.AddAsyncNarrowingMapping<TValue, TTarget, TNarrowed>(memberAccessor, expectationTextGenerator)
		   ?? this;

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
		if (_inner is OrNode or AndNode)
		{
			_inner = _inner.ReplaceRightMostOperand(replace);
			return this;
		}

		return replace(this);
	}

	/// <inheritdoc />
	public override async Task<ConstraintResult> IsMetBy<TValue>(
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
		if (parentResult != null && parentResult.TryGetValue(out TSource? projectedValue))
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

	private async Task<(TMember? Value, bool IsNullTask)> ComputeMatchingValueAsync(TSource? source)
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
			return separator.Length == 0
				? rightResult
				: rightResult.PrependExpectationText(sb => sb.Append(separator.TrimStart()));
		}

		return new WhichConstraintResult(leftResult, rightResult, separator,
			furtherProcessingStrategy ?? FurtherProcessingStrategy.Continue,
			value, _negateMemberOnly, false);
	}

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

	private sealed class WhichConstraintResult : ConstraintResult
	{
		private readonly bool _isMemberSkipped;
		private readonly bool _negateMemberOnly;
		private readonly string _separator;

		// ReSharper disable once ReplaceWithPrimaryConstructorParameter
		private readonly TMember? _value;

		private bool _isNegated;
		private ConstraintResult _left;
		private ConstraintResult _right;

		public WhichConstraintResult(ConstraintResult left,
			ConstraintResult right,
			string separator,
			FurtherProcessingStrategy furtherProcessingStrategy,
			TMember? value,
			bool negateMemberOnly,
			bool isMemberSkipped) : base(furtherProcessingStrategy)
		{
			_left = left;
			_right = right;
			_separator = separator;
			_value = value;
			_negateMemberOnly = negateMemberOnly;
			_isMemberSkipped = isMemberSkipped;
			Outcome = isMemberSkipped ? left.Outcome : And(left.Outcome, right.Outcome);
		}

		public override Exception? FailureCause
			=> Outcome == Outcome.Failure ? _left.FailureCause ?? _right.FailureCause : null;

		private static Outcome And(Outcome left, Outcome right)
			=> (left, right) switch
			{
				(Outcome.Success, Outcome.Success) => Outcome.Success,
				(_, Outcome.Failure) => Outcome.Failure,
				(Outcome.Failure, _) => Outcome.Failure,
				(_, _) => Outcome.Undecided,
			};

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			_left.AppendExpectation(stringBuilder);
			stringBuilder.AppendSeparatedExpectation(_separator, _right);
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_isNegated)
			{
				// A negated whole phrase only fails when both parts were met, or when the member was not evaluated.
				(_right is IUnevaluatedMemberResult ? _right : _left).AppendResult(stringBuilder, indentation);
			}
			else if (_left.ExplainsOutcomeOf(this))
			{
				_left.AppendResult(stringBuilder, indentation);
			}
			else if (_right.ExplainsOutcomeOf(this))
			{
				_right.AppendResult(stringBuilder, indentation);
			}
		}

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value)
			where TValue : default
		{
			if (_isMemberSkipped)
			{
				return _left.TryGetValue(out value);
			}

			if (_value is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			if (_left.TryGetValue(out TValue? leftValue))
			{
				value = leftValue;
				return true;
			}

			if (_right.TryGetValue(out TValue? rightValue))
			{
				value = rightValue;
				return true;
			}

			value = default;
			// When neither this result nor its sub-chains carry a TValue, fall through to a
			// type-compatibility check so chained WhichNodes can keep propagating projections
			// even when the recorded matching value happens to be null (e.g. an outer
			// `Which(p => p.Address)` projecting `null`). The caller is expected to guard
			// against null `value` even though `[NotNullWhen(true)]` is annotated on the base.
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
				_right = _right.Negate();
				Outcome = And(_left.Outcome, _right.Outcome);
				return this;
			}

			if (_right is not IUnevaluatedMemberResult)
			{
				Outcome = Outcome switch
				{
					Outcome.Failure => Outcome.Success,
					Outcome.Success => Outcome.Failure,
					_ => Outcome,
				};
			}

			_left = _left.Negate();
			_isNegated = !_isNegated;
			return this;
		}

		/// <remarks>
		///     Without a member, the parent alone decides the outcome, also when it stays failed under negation.
		/// </remarks>
		private ConstraintResult NegateWithoutMember()
		{
			if (_negateMemberOnly)
			{
				_right = _right.Negate();
			}
			else
			{
				_left = _left.Negate();
				_isNegated = !_isNegated;
			}

			Outcome = _left.Outcome;
			return this;
		}
	}
}
