using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatString
{
	/// <summary>
	///     Verifies that the subject contains the <paramref name="expected" /> <see langword="string" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringOccurrenceCountResult<string, IThat<string?>> Contains(
		this IThat<string?> subject,
		string expected)
	{
		expected.ThrowIfNull();
		if (expected == string.Empty)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException("The 'expected' string cannot be empty.", nameof(expected)));
		}

		Quantifier quantifier = new();
		StringEqualityOptions options = new(nameof(expected));
		return new StringOccurrenceCountResult<string, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(Expected: expected, Quantifier: quantifier, Options: options),
				static (state, it, grammars) =>
					new ContainsConstraint(it, grammars, state.Expected, state.Quantifier, state.Options)),
			subject,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the subject does not contain the <paramref name="unexpected" /> <see langword="string" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringOccurrenceCountResult<string, IThat<string?>> DoesNotContain(
		this IThat<string?> subject,
		string unexpected)
	{
		unexpected.ThrowIfNull();
		if (unexpected == string.Empty)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException("The 'unexpected' string cannot be empty.", nameof(unexpected)));
		}

		Quantifier quantifier = new();
		StringEqualityOptions options = new(nameof(unexpected));
		return new StringOccurrenceCountResult<string, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(Unexpected: unexpected, Quantifier: quantifier, Options: options),
				static (state, it, grammars) =>
					new ContainsConstraint(it, grammars, state.Unexpected, state.Quantifier, state.Options).Invert()),
			subject,
			quantifier,
			options);
	}

	private sealed class ContainsConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		Quantifier quantifier,
		StringEqualityOptions options)
		: ConstraintResult(grammars),
			IAsyncContextConstraint<string?>
	{
		private string? _actual;
		private StringEqualityOptions? _evaluationOptions;
		private int _actualCount;
		private bool _isFinished;
		private bool _isNegated;

		/// <inheritdoc />
		/// <remarks>
		///     The occurrences are also counted for a <see langword="null" /> subject, as in an empty one, because the
		///     options validate the expected value while they count, so an unusable pattern is rejected whichever
		///     subject it is searched in.
		/// </remarks>
		public async ValueTask<ConstraintResult> IsMetBy(string? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_actual = actual;
			int decisiveAmount = GetDecisiveAmount();
			_evaluationOptions = options.ForEvaluation(context, cancellationToken);
			_actualCount = await _evaluationOptions.CountOccurrences(actual ?? string.Empty, expected, decisiveAmount);
			_isFinished = _actualCount < decisiveAmount;
			if (actual is null)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			Outcome = quantifier.Check(_actualCount, true, _isNegated) ?? _isNegated ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <summary>
		///     The number of occurrences from which on no further occurrence can change the outcome, so that the rest of
		///     the subject does not have to be searched.
		/// </summary>
		/// <remarks>
		///     At an upper bound that includes the <see cref="Quantifier.DeterminableAmount" />, only one more occurrence
		///     decides the outcome. The options validate the expected value while they count, so at least one occurrence
		///     is searched for.
		/// </remarks>
		private int GetDecisiveAmount()
		{
			int amount = quantifier.DeterminableAmount;
			if (quantifier.Check(amount, false) is null && amount < int.MaxValue)
			{
				return amount + 1;
			}

			return Math.Max(amount, 1);
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(quantifier.ToContainsExpectation(Grammars, $"{Formatter.Format(expected)}{options}",
				_isNegated));

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (!string.IsNullOrEmpty(_actual))
			{
				contexts.AddStringContext("Actual", _actual, this);
				contexts.AddStringContext("Expected", expected, this);
			}

			(_evaluationOptions ?? options).AppendContexts(contexts);
		}

		/// <inheritdoc cref="ConstraintResult.TryGetStoredValue{TValue}(out TValue)" />
		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(string));
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else
			{
				if (_actualCount == 0)
				{
					stringBuilder.Append(it).Append(" did not contain ");
					Formatter.Format(stringBuilder, expected);
					stringBuilder.Append(" in ");
				}
				else
				{
					stringBuilder.Append(it).Append(" contained ");
					Formatter.Format(stringBuilder, expected);
					stringBuilder.Append(_isFinished ? " " : " at least ").AppendOccurrences(_actualCount).Append(" in ");
				}

				Formatter.Format(stringBuilder, _actual);
				stringBuilder.Append((_evaluationOptions ?? options).GetOccurrenceDetails(_actual, expected, _actualCount)
					.Indent(indentation, false));
			}
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
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
