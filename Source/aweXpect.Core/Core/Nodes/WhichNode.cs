using System;
using System.Diagnostics.CodeAnalysis;
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
	private readonly Node? _parent;
	private readonly string? _separator;
	private Node? _inner;

	public WhichNode(
		Node? parent,
		Func<TSource, TMember?> memberAccessor,
		string? separator = null)
	{
		_parent = parent;
		_memberAccessor = memberAccessor;
		_separator = separator;
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
	public override Node AddAsyncMapping<TValue, TTarget>(
		MemberAccessor<TValue, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
		=> _inner?.AddAsyncMapping(memberAccessor, expectationTextGenerator) ?? this;

	/// <inheritdoc />
	public override void AddNode(Node node, string? separator = null)
		=> _inner = node;

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
			throw new InvalidOperationException("No inner node specified for the which node.")
				.LogTrace();
		}

		if (value is null || value is DelegateValue { IsNull: true, })
		{
			ConstraintResult nullResult = await _inner.IsMetBy<TMember>(default, context, cancellationToken);
			return CombineResults(parentResult, nullResult, _separator ?? "",
				FurtherProcessingStrategy.IgnoreResult, default);
		}

		TSource? source = ResolveSource(parentResult, value);
		TMember? matchingValue = await ComputeMatchingValueAsync(source);

		ConstraintResult innerResult = await _inner.IsMetBy(matchingValue, context, cancellationToken);
		return CombineResults(parentResult, innerResult, _separator ?? "", FurtherProcessingStrategy.IgnoreResult,
			matchingValue);
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

		throw new InvalidOperationException(
				$"The member type for the actual value in the which node did not match.{Environment.NewLine}     Found: {Formatter.Format(value.GetType())}{Environment.NewLine}  Expected: {Formatter.Format(typeof(TSource))}")
			.LogTrace();
	}

	private async Task<TMember?> ComputeMatchingValueAsync(TSource? source)
	{
#pragma warning disable S2583
		if (source is null)
		{
			return default;
		}
#pragma warning restore S2583

		return _memberAccessor != null
			? _memberAccessor(source)
			: await _asyncMemberAccessor!.Invoke(source);
	}

	/// <inheritdoc cref="object.Equals(object?)" />
	public override bool Equals(object? obj) => obj is WhichNode<TSource, TMember> other && Equals(other);

	private bool Equals(WhichNode<TSource, TMember> other) =>
		_parent?.Equals(other._parent) != false &&
		_inner?.Equals(other._inner) != false;

	/// <inheritdoc cref="object.GetHashCode()" />
	public override int GetHashCode() => _parent?.GetHashCode() ?? 17;

	private static ConstraintResult CombineResults(ConstraintResult? leftResult,
		ConstraintResult rightResult,
		string separator,
		FurtherProcessingStrategy? furtherProcessingStrategy,
		TMember? value)
	{
		if (leftResult == null)
		{
			return rightResult;
		}

		return new WhichConstraintResult(leftResult, rightResult, separator,
			furtherProcessingStrategy ?? FurtherProcessingStrategy.Continue,
			value);
	}

	/// <inheritdoc cref="Node.SetReason(IBecauseReason)" />
	public override void SetReason(IBecauseReason becauseReason)
		=> _inner?.SetReason(becauseReason);

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_separator != null)
		{
			stringBuilder.Append(_separator);
		}

		_inner?.AppendExpectation(stringBuilder, indentation);
	}

	private sealed class WhichConstraintResult : ConstraintResult
	{
		private readonly ConstraintResult _left;
		private readonly ConstraintResult _right;
		private readonly string _separator;

		// ReSharper disable once ReplaceWithPrimaryConstructorParameter
		private readonly TMember? _value;

		public WhichConstraintResult(ConstraintResult left,
			ConstraintResult right,
			string separator,
			FurtherProcessingStrategy furtherProcessingStrategy,
			TMember? value) : base(furtherProcessingStrategy)
		{
			_left = left;
			_right = right;
			_separator = separator;
			_value = value;
			Outcome = And(left.Outcome, right.Outcome);
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
			stringBuilder.Append(_separator);
			_right.AppendExpectation(stringBuilder);
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_left.Outcome == Outcome.Failure)
			{
				_left.AppendResult(stringBuilder, indentation);
			}
			else if (_right.Outcome == Outcome.Failure)
			{
				_right.AppendResult(stringBuilder, indentation);
			}
		}

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value)
			where TValue : default
		{
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
			Outcome = Outcome switch
			{
				Outcome.Failure => Outcome.Success,
				Outcome.Success => Outcome.Failure,
				_ => Outcome,
			};
			return this;
		}
	}
}
