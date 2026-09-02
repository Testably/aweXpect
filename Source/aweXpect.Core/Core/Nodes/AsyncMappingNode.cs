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

internal class AsyncMappingNode<TSource, TTarget> : ExpectationNode
{
	private readonly Action<MemberAccessor<TSource, Task<TTarget>>, StringBuilder>
		_expectationTextGenerator;

	private readonly MemberAccessor<TSource, Task<TTarget>> _memberAccessor;

	public AsyncMappingNode(MemberAccessor<TSource, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
	{
		_memberAccessor = memberAccessor;
		if (expectationTextGenerator == null)
		{
			_expectationTextGenerator = DefaultExpectationTextGenerator;
		}
		else
		{
			_expectationTextGenerator = expectationTextGenerator;
		}
	}

	/// <inheritdoc />
	public override async Task<ConstraintResult> IsMetBy<TValue>(
		TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken) where TValue : default
	{
		if (value is null || value is DelegateValue { IsNull: true, })
		{
			ConstraintResult result = await base.IsMetBy<TTarget>(default, context, cancellationToken);
			return result.Fail("it was <null>", value);
		}

		if (value is TSource typedValue)
		{
			TTarget matchingValue = await _memberAccessor.AccessMember(typedValue);
			ConstraintResult memberResult = await base.IsMetBy(matchingValue, context, cancellationToken);
			return memberResult.UseValue(value);
		}

		throw new InvalidOperationException(
				$"The member type for the actual value in the which node did not match.{Environment.NewLine}Expected: {Formatter.Format(typeof(TSource))},{Environment.NewLine}   Found: {Formatter.Format(value.GetType())}")
			.LogTrace();
	}

	/// <inheritdoc cref="object.Equals(object?)" />
	public override bool Equals(object? obj) => obj is AsyncMappingNode<TSource, TTarget> other && Equals(other);

	private bool Equals(AsyncMappingNode<TSource, TTarget> other) => _memberAccessor.Equals(other._memberAccessor);

	/// <inheritdoc cref="object.GetHashCode()" />
	public override int GetHashCode() => _memberAccessor.GetHashCode();

	internal ConstraintResult CombineResults(
		ConstraintResult? combinedResult,
		ConstraintResult result)
	{
		if (combinedResult == null)
		{
			return result.PrependExpectationText(e => _expectationTextGenerator(_memberAccessor, e));
		}

		return new AsyncMappingConstraintResult(combinedResult, result, _expectationTextGenerator, _memberAccessor);
	}

	private static void DefaultExpectationTextGenerator(
		MemberAccessor<TSource, Task<TTarget>> memberAccessor,
		StringBuilder expectation)
		=> expectation.Append(memberAccessor);

	private sealed class AsyncMappingConstraintResult : ConstraintResult
	{
		private readonly Action<MemberAccessor<TSource, Task<TTarget>>, StringBuilder>? _expectationTextGenerator;
		private readonly ConstraintResult _left;
		private readonly MemberAccessor<TSource, Task<TTarget>> _memberAccessor;
		private readonly ConstraintResult _right;

		public AsyncMappingConstraintResult(ConstraintResult left,
			ConstraintResult right,
			Action<MemberAccessor<TSource, Task<TTarget>>, StringBuilder>? expectationTextGenerator,
			MemberAccessor<TSource, Task<TTarget>> memberAccessor) : base(FurtherProcessingStrategy.Continue)
		{
			_left = left;
			_right = right;
			_expectationTextGenerator = expectationTextGenerator;
			_memberAccessor = memberAccessor;
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
			if (_expectationTextGenerator is not null)
			{
				_expectationTextGenerator(_memberAccessor, stringBuilder);
			}

			_right.AppendExpectation(stringBuilder);
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_left.Outcome == Outcome.Failure)
			{
				_left.AppendResult(stringBuilder, indentation);
				if (_right.Outcome == Outcome.Failure &&
				    _left.FurtherProcessingStrategy == FurtherProcessingStrategy.Continue &&
				    !_left.HasSameResultTextAs(_right))
				{
					stringBuilder.Append(" and ");
					_right.AppendResult(stringBuilder, indentation);
				}
			}
			else if (_right.Outcome == Outcome.Failure)
			{
				_right.AppendResult(stringBuilder, indentation);
			}
		}

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value)
			where TValue : default
		{
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
			return false;
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
