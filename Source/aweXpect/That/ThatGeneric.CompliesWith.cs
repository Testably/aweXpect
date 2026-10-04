using System;
using System.Collections.Generic;
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
				.AddConstraint((Expectations: expectations, Options: options), static (state, _, grammars) =>
					new CompliesWithConstraint<T>(grammars, state.Expectations, state.Options)),
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
				.AddConstraint((Expectations: expectations, Options: options), static (state, _, grammars) =>
					new CompliesWithConstraint<T>(grammars, state.Expectations, state.Options).Invert()),
			subject,
			options);
	}

	private sealed class CompliesWithConstraint<T>
		: ConstraintResult,
			IAsyncContextConstraint<T>,
			IExpectationTextConstraint
	{
		private readonly ManualExpectationBuilder<T> _itemExpectationBuilder;
		private readonly RepeatedCheckOptions _options;
		private bool _isNegated;
		private ConstraintResult? _negatedResult;

		public CompliesWithConstraint(ExpectationGrammars grammars,
			Action<IThatSubject<T>> expectations, RepeatedCheckOptions options)
			: base(grammars)
		{
			_options = options;
			_itemExpectationBuilder = new ManualExpectationBuilder<T>(grammars);
			expectations.Invoke(new ThatSubject<T>(_itemExpectationBuilder));
		}

		public async ValueTask<ConstraintResult> IsMetBy(
			T actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			RevertPreviousNegation();
			ConstraintResult? isMatch = null;
			Outcome outcome = await _options.CheckRepeatedly(async checkContext =>
			{
				isMatch = await _itemExpectationBuilder.IsMetBy(actual, checkContext, cancellationToken);
				return IsMet(isMatch);
			}, context);
			ConstraintResult result = KeepSubjectAsValue(NegateIfNegated(isMatch!), actual)
				.AppendExpectationText(AppendSuffix);
			return outcome == Outcome.Undecided ? new CanceledResult(result) : result;
		}

		/// <remarks>
		///     The <paramref name="isMatch" /> is not negated yet, so a negated expectation is met when it is not met, unless
		///     it could not be answered, which fails the negation as well.
		/// </remarks>
		private bool IsMet(ConstraintResult isMatch)
		{
			if (_isNegated)
			{
				return isMatch.Outcome is Outcome.Failure or Outcome.Undecided;
			}

			return isMatch.Outcome == Outcome.Success;
		}

		private void AppendSuffix(StringBuilder stringBuilder)
		{
			stringBuilder.Append(_options);
			_itemExpectationBuilder.AppendReasons(stringBuilder);
		}

		/// <summary>
		///     The expectations may have another value (e.g. the single item of <c>HasSingle()</c>), even one of the
		///     subject's type, but the result of <c>CompliesWith</c> is the subject.
		/// </summary>
		/// <remarks>
		///     A result that already stores the subject is kept, which avoids the wrapper on the common path.
		/// </remarks>
		private static ConstraintResult KeepSubjectAsValue(ConstraintResult result, T actual)
			=> result.TryGetStoredValue(out T? value) && IsSubject(value, actual) ? result : result.UseValue(actual);

		private static bool IsSubject(T? value, T actual)
			=> typeof(T).IsValueType
				? EqualityComparer<T>.Default.Equals(value!, actual)
				: ReferenceEquals(value, actual);

		public async Task<ConstraintResult> GetExpectationResult(IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			RevertPreviousNegation();
			return NegateIfNegated(await _itemExpectationBuilder.IsMetBy(default!, context, cancellationToken))
				.AppendExpectationText(AppendSuffix);
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
			}
			else
			{
				_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
			}

			_itemExpectationBuilder.AppendReasons(stringBuilder);
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

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
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

	/// <summary>
	///     The undecided result of a repeated check that the cancellation of the evaluation ended before its timeout.
	/// </summary>
	private sealed class CanceledResult(ConstraintResult inner) : ConstraintResult(inner.Grammars)
	{
		public override Outcome Outcome
		{
			get => Outcome.Undecided;
			// The outcome of a canceled check is always undecided, so the value is discarded.
			protected set => _ = value;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> inner.AppendExpectation(stringBuilder, indentation);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> AppendCanceledResult(stringBuilder, "it");

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
			=> inner.TryGetStoredValue(out value);

		public override ConstraintResult Negate()
		{
			inner.Negate();
			return this;
		}
	}
}
