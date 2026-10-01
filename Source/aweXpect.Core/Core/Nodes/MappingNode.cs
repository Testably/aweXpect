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

internal class MappingNode<TSource, TTarget> : ExpectationNode
{
	private readonly Action<MemberAccessor<TSource, TTarget>, StringBuilder>
		_expectationTextGenerator;

	private readonly MemberAccessor<TSource, TTarget> _memberAccessor;

	public MappingNode(MemberAccessor<TSource, TTarget> memberAccessor,
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
		if (context is ExpectationTextEvaluationContext)
		{
			return await GetExpectationResult(context, cancellationToken);
		}

		if (value is null || value is DelegateValue { IsNull: true, })
		{
			ConstraintResult result = await GetExpectationResult(context, cancellationToken);
			return NullSubjectResult.Create(result, value);
		}

		if (value is TSource typedValue)
		{
			TTarget matchingValue;
			try
			{
				matchingValue = _memberAccessor.AccessMember(typedValue);
			}
			catch (Exception exception) when (!MemberExceptionResult.IsCancellationOf(exception, cancellationToken))
			{
				ConstraintResult result = await GetExpectationResult(context, cancellationToken);
				return MemberExceptionResult.Create(result, exception, _memberAccessor.ToString().Trim(), value);
			}

			ConstraintResult memberResult = await IsMetByMember(matchingValue, context, cancellationToken);
			return memberResult.UseValue(value);
		}

		// The value only has another type after a failed type check (e.g. `Is<T>()`), which reports the mismatch.
		ConstraintResult expectationResult = await GetExpectationResult(context, cancellationToken);
		return new NotApplicableConstraintResult(expectationResult.AppendExpectation);
	}

	/// <inheritdoc />
	/// <remarks>
	///     The expectations on the member are held in a separate node, so that a combination node created by
	///     <c>And</c>/<c>Or</c> while registering them can replace it.
	/// </remarks>
	public override void AddNode(Node node, string? separator = null) => SetInnerNode(node);

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		StringBuilder separator = new();
		_expectationTextGenerator(_memberAccessor, separator);
		stringBuilder.AppendSeparatedExpectation(separator.ToString(), sb => AppendMemberExpectation(sb, indentation));
	}

	/// <summary>
	///     Appends the expectations on the member, without the text for the member itself.
	/// </summary>
	/// <remarks>
	///     Results flowing through <see cref="CombineResults" /> already get the member text prepended there.
	/// </remarks>
	protected void AppendMemberExpectation(StringBuilder stringBuilder, string? indentation)
		=> base.AppendExpectation(stringBuilder, indentation);

	/// <summary>
	///     Verifies if the <paramref name="value" /> of the member satisfies the expectations of the node.
	/// </summary>
	protected virtual Task<ConstraintResult> IsMetByMember(TTarget? value,
		IEvaluationContext context,
		CancellationToken cancellationToken)
		=> IsMetByExpectations(value, context, cancellationToken);

	/// <summary>
	///     Returns the expectations on the member, without evaluating them, for when the member value is not available.
	/// </summary>
	private Task<ConstraintResult> GetExpectationResult(IEvaluationContext context,
		CancellationToken cancellationToken)
		=> IsMetByMember(default, ExpectationTextEvaluationContext.For(context), cancellationToken);

	/// <summary>
	///     Verifies if the <paramref name="value" /> satisfies the expectations of the node, without accessing the member.
	/// </summary>
	protected Task<ConstraintResult> IsMetByExpectations<TValue>(TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken)
		=> base.IsMetBy(value, context, cancellationToken);

	/// <inheritdoc cref="object.Equals(object?)" />
	public override bool Equals(object? obj) => obj is MappingNode<TSource, TTarget> other && Equals(other);

	private bool Equals(MappingNode<TSource, TTarget> other) => _memberAccessor.Equals(other._memberAccessor);

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

		return new MappingConstraintResult(combinedResult, result, _expectationTextGenerator, _memberAccessor);
	}

	private static void DefaultExpectationTextGenerator(
		MemberAccessor<TSource, TTarget> memberAccessor,
		StringBuilder expectation)
		=> expectation.Append(memberAccessor);

	private sealed class MappingConstraintResult : ConstraintResult
	{
		private readonly Action<MemberAccessor<TSource, TTarget>, StringBuilder>? _expectationTextGenerator;
		private readonly ConstraintResult _left;
		private readonly MemberAccessor<TSource, TTarget> _memberAccessor;
		private readonly ConstraintResult _right;
		private bool _isNegated;
		private bool _rightFailsAlsoWhenNegated;

		/// <summary>
		///     The positive expectation text of the member, which a negated result keeps, as only the left part renders
		///     the negation.
		/// </summary>
		private string? _negatedRightExpectation;

		public MappingConstraintResult(ConstraintResult left,
			ConstraintResult right,
			Action<MemberAccessor<TSource, TTarget>, StringBuilder>? expectationTextGenerator,
			MemberAccessor<TSource, TTarget> memberAccessor) : base(FurtherProcessingStrategy.Continue)
		{
			_left = left;
			_right = right;
			_expectationTextGenerator = expectationTextGenerator;
			_memberAccessor = memberAccessor;
			Outcome = Combine(left.Outcome, right.Outcome, false);
		}

		public override Exception? FailureCause
			=> Outcome == Outcome.Failure ? _left.FailureCause ?? _right.FailureCause : null;

		/// <remarks>
		///     An operand which only contributes an expectation text does not take part in the combination.
		/// </remarks>
		private Outcome Combine(Outcome left, Outcome right, bool isNegated)
		{
			if (_left.IsExpectationOnly)
			{
				return right;
			}

			if (_right.IsExpectationOnly)
			{
				return left;
			}

			return isNegated ? Or(left, right) : And(left, right);
		}

		private static Outcome And(Outcome left, Outcome right)
			=> (left, right) switch
			{
				(Outcome.Success, Outcome.Success) => Outcome.Success,
				(_, Outcome.Failure) => Outcome.Failure,
				(Outcome.Failure, _) => Outcome.Failure,
				(_, _) => Outcome.Undecided,
			};

		private static Outcome Or(Outcome left, Outcome right)
			=> (left, right) switch
			{
				(Outcome.Failure, Outcome.Failure) => Outcome.Failure,
				(_, Outcome.Success) => Outcome.Success,
				(Outcome.Success, _) => Outcome.Success,
				(_, _) => Outcome.Undecided,
			};

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			_left.AppendExpectation(stringBuilder);
			if (_negatedRightExpectation is not null)
			{
				stringBuilder.Append(_negatedRightExpectation);
				return;
			}

			AppendRightExpectation(stringBuilder);
		}

		private void AppendRightExpectation(StringBuilder stringBuilder)
		{
			StringBuilder separator = new();
			_expectationTextGenerator?.Invoke(_memberAccessor, separator);
			stringBuilder.AppendSeparatedExpectation(separator.ToString(), _right);
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			bool rendersLeft = _left.ExplainsOutcomeOf(this);
			// Under negation both parts were met, so the left part explains the failure, unless the member failed in
			// both cases.
			bool rendersRight = _right.ExplainsOutcomeOf(this) &&
			                    (!_isNegated || _rightFailsAlsoWhenNegated || !rendersLeft);
			if (rendersLeft)
			{
				_left.AppendResult(stringBuilder, indentation);
				if (rendersRight &&
				    _left.FurtherProcessingStrategy == FurtherProcessingStrategy.Continue &&
				    !_left.HasSameResultTextAs(_right))
				{
					stringBuilder.Append(" and ");
					_right.AppendResult(stringBuilder, indentation);
				}
			}
			else if (rendersRight)
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
			_isNegated = !_isNegated;
			_left.Negate();
			_negatedRightExpectation = _isNegated ? GetRightExpectation() : null;
			Outcome rightOutcome = _right.Outcome;
			_right.Negate();
			_rightFailsAlsoWhenNegated = rightOutcome == Outcome.Failure && _right.Outcome == Outcome.Failure;
			// De Morgan, so that an operand which stays failed under negation keeps the combination failed.
			Outcome = Combine(_left.Outcome, _right.Outcome, _isNegated);
			return this;
		}

		private string GetRightExpectation()
		{
			StringBuilder stringBuilder = new();
			AppendRightExpectation(stringBuilder);
			return stringBuilder.ToString();
		}
	}
}
