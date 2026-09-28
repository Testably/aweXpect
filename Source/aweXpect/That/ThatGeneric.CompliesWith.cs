using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatGeneric
{
	/// <summary>
	///     Verifies that the subject complies with the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expectations" /> decide about a <see langword="null" /> subject as well, so
	///     <c>CompliesWith(it =&gt; it.IsNull())</c> succeeds.
	/// </remarks>
	public static RepeatedCheckResult<T, IThat<T>> CompliesWith<T>(this IThat<T> subject,
		Action<IThatSubject<T>> expectations)
	{
		expectations.ThrowIfNull();
		RepeatedCheckOptions options = new();
		return new RepeatedCheckResult<T, IThat<T>>(subject.Get().ExpectationBuilder
				.AddConstraint((expectationBuilder, _, grammars) =>
					new CompliesWithConstraint<T>(expectationBuilder, grammars, expectations, options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject does not comply with the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expectations" /> decide about a <see langword="null" /> subject as well, so
	///     <c>DoesNotComplyWith(it =&gt; it.IsNotNull())</c> succeeds.
	/// </remarks>
	public static RepeatedCheckResult<T, IThat<T>> DoesNotComplyWith<T>(this IThat<T> subject,
		Action<IThatSubject<T>> expectations)
	{
		expectations.ThrowIfNull();
		RepeatedCheckOptions options = new();
		return new RepeatedCheckResult<T, IThat<T>>(subject.Get().ExpectationBuilder
				.AddConstraint((expectationBuilder, _, grammars) =>
					new CompliesWithConstraint<T>(expectationBuilder, grammars, expectations, options).Invert()),
			subject,
			options);
	}

	private sealed class CompliesWithConstraint<T>
		: ConstraintResult,
			IAsyncContextConstraint<T>,
			IExpectationTextConstraint
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly ManualExpectationBuilder<T> _itemExpectationBuilder;
		private readonly RepeatedCheckOptions _options;
		private bool _isNegated;
		private ConstraintResult? _negatedResult;

		public CompliesWithConstraint(ExpectationBuilder expectationBuilder, ExpectationGrammars grammars,
			Action<IThatSubject<T>> expectations, RepeatedCheckOptions options)
			: base(grammars)
		{
			_expectationBuilder = expectationBuilder;
			_options = options;
			_itemExpectationBuilder = new ManualExpectationBuilder<T>(expectationBuilder, grammars);
			expectations.Invoke(new ThatSubject<T>(_itemExpectationBuilder));
		}

		public async Task<ConstraintResult> IsMetBy(
			T actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			RevertPreviousNegation();
			ConstraintResult? isMatch = null;
			await _options.CheckRepeatedly(async () =>
			{
				isMatch = await _itemExpectationBuilder.IsMetBy(actual, context, cancellationToken);
				return isMatch.Outcome == Outcome.Success != _isNegated;
			}, _expectationBuilder, cancellationToken);
			return NegateIfNegated(isMatch!).AppendExpectationText(sb => sb.Append(_options));
		}

		public async Task<ConstraintResult> GetExpectationResult(IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			RevertPreviousNegation();
			return NegateIfNegated(await _itemExpectationBuilder.IsMetBy(default!, context, cancellationToken))
				.AppendExpectationText(sb => sb.Append(_options));
		}

		/// <remarks>
		///     The expectation text of the expectations is not aware of the negation, so the negated result of the last
		///     evaluation is rendered instead, which applies De Morgan to combinations.
		/// </remarks>
		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_negatedResult is not null)
			{
				_negatedResult.AppendExpectation(stringBuilder, indentation);
				return;
			}

			_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
		}

		private ConstraintResult NegateIfNegated(ConstraintResult constraintResult)
		{
			if (_isNegated)
			{
				_negatedResult = constraintResult;
				return constraintResult.Negate();
			}

			return constraintResult;
		}

		/// <summary>
		///     The constraints of the expectations are reused by every evaluation (e.g. for each item of a collection) and
		///     keep the negation of the previous result, so it is undone before evaluating again.
		/// </summary>
		private void RevertPreviousNegation()
		{
			_negatedResult?.Negate();
			_negatedResult = null;
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
		}

		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			value = default;
			return false;
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			return this;
		}
	}
}
