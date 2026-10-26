using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

namespace aweXpect;

public static partial class ThatDictionary
{
	private const string ContainsEntrySummary = "Verifies that the dictionary contains the <paramref name=\"expected\" /> entry.";

	private const string DoesNotContainEntrySummary =
		"Verifies that the dictionary does not contain the <paramref name=\"unexpected\" /> entry.";

	private const string ContainsKeyAndValueSummary =
		"Verifies that the dictionary contains the <paramref name=\"expectedKey\" /> with the <paramref name=\"expectedValue\" />.";

	private const string DoesNotContainKeyAndValueSummary =
		"Verifies that the dictionary does not contain the <paramref name=\"unexpectedKey\" /> with the <paramref name=\"unexpectedValue\" />.";

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true,
		GuaranteesNotNull = true, Summary = ContainsEntrySummary, NegatedSummary = DoesNotContainEntrySummary)]
	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true, KeyAndValue = true,
		GuaranteesNotNull = true, Summary = ContainsKeyAndValueSummary, NegatedSummary = DoesNotContainKeyAndValueSummary)]
	internal static ObjectEqualityResult<TCollection, IThat<TCollection?>, TValue>
		ContainsCore<TCollection, TKey, TValue>(
			IThat<TCollection?> subject,
			KeyValuePair<TKey, TValue> expected,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		expected.Key.ThrowIfNull(negated);
		OptionsHolder<ObjectMatchType<TValue>> options = new(ObjectToleranceMatchTypeFactory.ForValuesOf<TValue>());
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection?>, TValue>(
			expectationBuilder.AddConstraint((Expected: expected, Options: options, Negated: negated),
				static (state, it, grammars)
					=> new ContainsConstraint<TCollection, TKey, TValue>(it, grammars, state.Negated ? "unexpected" : "expected",
						state.Expected, state.Options).InvertIf(state.Negated)),
			subject,
			options);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true,
		Factory = typeof(ObjectToleranceMatchTypeFactory),
		GuaranteesNotNull = true, Summary = ContainsEntrySummary, NegatedSummary = DoesNotContainEntrySummary)]
	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true, KeyAndValue = true,
		Factory = typeof(ObjectToleranceMatchTypeFactory),
		GuaranteesNotNull = true, Summary = ContainsKeyAndValueSummary, NegatedSummary = DoesNotContainKeyAndValueSummary)]
	internal static ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection?>, TValue, TTolerance>
		ContainsWithToleranceCore<TCollection, TKey, TValue, TTolerance>(
			IThat<TCollection?> subject,
			KeyValuePair<TKey, TValue> expected,
			ObjectToleranceMatchType<TValue, TTolerance> initialOptions,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		OptionsHolder<ObjectMatchType<TValue>> options = new(initialOptions);
		expected.Key.ThrowIfNull(negated);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection?>, TValue, TTolerance>(
			expectationBuilder.AddConstraint((Expected: expected, Options: options, Negated: negated),
				static (state, it, grammars)
					=> new ContainsConstraint<TCollection, TKey, TValue>(it, grammars, state.Negated ? "unexpected" : "expected",
						state.Expected, state.Options).InvertIf(state.Negated)),
			subject,
			options);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true,
		GuaranteesNotNull = true, Summary = ContainsEntrySummary, NegatedSummary = DoesNotContainEntrySummary)]
	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true, KeyAndValue = true,
		GuaranteesNotNull = true, Summary = ContainsKeyAndValueSummary, NegatedSummary = DoesNotContainKeyAndValueSummary)]
	internal static StringEqualityTypeResult<TCollection, IThat<TCollection?>>
		ContainsForStringsCore<TCollection, TKey>(
			IThat<TCollection?> subject,
			KeyValuePair<TKey, string?> expected,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, string?>>
	{
		expected.Key.ThrowIfNull(negated);
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<TCollection, IThat<TCollection?>>(
			expectationBuilder.AddConstraint((Expected: expected, Options: options, Negated: negated),
				static (state, it, grammars)
					=> new ContainsConstraint<TCollection, TKey, string?>(it, grammars, state.Negated ? "unexpected" : "expected",
						state.Expected, state.Options).InvertIf(state.Negated)),
			subject,
			options);
	}

	private sealed class ContainsConstraint<TDictionary, TKey, TValue>(
		string it,
		ExpectationGrammars grammars,
		string parameterName,
		KeyValuePair<TKey, TValue> expected,
		IReadOnlyOptionsHolder<IEqualityOptions<TValue>> options)
		: ConstraintResult.WithNotNullValue<TDictionary?>(it, grammars),
			IAsyncContextConstraint<TDictionary?>
		where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private TValue? _actualValue;
		private IEqualityEvaluator<TValue>? _evaluationOptions;
		private bool _hasKey;

		/// <remarks>
		///     The expected value is validated first, so that an unusable pattern is rejected whichever entries the
		///     subject has.
		/// </remarks>
		public async ValueTask<ConstraintResult> IsMetBy(TDictionary? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			options.ValidateExpectedItems([expected.Value,], parameterName);
			_evaluationOptions = options.ForEvaluation(context, cancellationToken);
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_hasKey = TryLookUp(GetLookup(actual), expected.Key, out _actualValue);
			Outcome = _hasKey && await _evaluationOptions.AreConsideredEqual(_actualValue!, expected.Value)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			AddDictionaryContext(contexts, Actual);
			contexts.AddEqualityOptionsContexts(_evaluationOptions ?? options.WithoutEvaluation());
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("contains ", "contain "));
			stringBuilder.Append(Formatter.Format(expected).Indent(indentation, false));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!_hasKey)
			{
				stringBuilder.Append(It).Append(" did not contain key ");
				stringBuilder.Append(Formatter.Format(expected.Key).Indent(indentation, false));
				stringBuilder.AppendClosestMatchHint(expected.Key, GetKeys(Actual));
				return;
			}

			stringBuilder.Append(It).Append(" contained key ");
			stringBuilder.Append(Formatter.Format(expected.Key).Indent(indentation, false));
			stringBuilder.Append(" with value ");
			stringBuilder.Append(Formatter.Format(_actualValue).Indent(indentation, false));
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not contain ", "do not contain "));
			stringBuilder.Append(Formatter.Format(expected).Indent(indentation, false));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
