using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
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
		ObjectEqualityOptions<TValue> options = ObjectEqualityWithToleranceOptionsFactory.ForValuesOf<TValue>();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection?>, TValue>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new ContainsConstraint<TCollection, TKey, TValue>(expectationBuilder, it, grammars,
					expected, options).InvertIf(negated)),
			subject,
			options);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		GuaranteesNotNull = true, Summary = ContainsEntrySummary, NegatedSummary = DoesNotContainEntrySummary)]
	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true, KeyAndValue = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		GuaranteesNotNull = true, Summary = ContainsKeyAndValueSummary, NegatedSummary = DoesNotContainKeyAndValueSummary)]
	internal static ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection?>, TValue, TTolerance>
		ContainsWithToleranceCore<TCollection, TKey, TValue, TTolerance>(
			IThat<TCollection?> subject,
			KeyValuePair<TKey, TValue> expected,
			ObjectEqualityWithToleranceOptions<TValue, TTolerance> options,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		expected.Key.ThrowIfNull(negated);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection?>, TValue, TTolerance>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new ContainsConstraint<TCollection, TKey, TValue>(expectationBuilder, it, grammars,
					expected, options).InvertIf(negated)),
			subject,
			options);
	}

	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true,
		GuaranteesNotNull = true, Summary = ContainsEntrySummary, NegatedSummary = DoesNotContainEntrySummary)]
	[CreateExpectationFamily("Contains", NegatedName = "DoesNotContain", PerSubject = true, KeyAndValue = true,
		GuaranteesNotNull = true, Summary = ContainsKeyAndValueSummary, NegatedSummary = DoesNotContainKeyAndValueSummary)]
	internal static StringEqualityResult<TCollection, IThat<TCollection?>>
		ContainsForStringsCore<TCollection, TKey>(
			IThat<TCollection?> subject,
			KeyValuePair<TKey, string?> expected,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, string?>>
	{
		expected.Key.ThrowIfNull(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityResult<TCollection, IThat<TCollection?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new ContainsConstraint<TCollection, TKey, string?>(expectationBuilder, it, grammars,
					expected, options).InvertIf(negated)),
			subject,
			options);
	}

	private sealed class ContainsConstraint<TDictionary, TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		KeyValuePair<TKey, TValue> expected,
		IOptionsEquality<TValue> options)
		: ConstraintResult.WithNotNullValue<TDictionary?>(it, grammars),
			IAsyncConstraint<TDictionary?>
		where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private TValue? _actualValue;
		private bool _hasKey;

		public async Task<ConstraintResult> IsMetBy(TDictionary? actual,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_hasKey = GetLookup(actual)(expected.Key, out _actualValue);
			Outcome = _hasKey && await options.AreConsideredEqual(_actualValue!, expected.Value)
				? Outcome.Success
				: Outcome.Failure;
			AddDictionaryContext(expectationBuilder, actual);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("contains ", "contain "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!_hasKey)
			{
				stringBuilder.Append(It).Append(" did not contain key ");
				Formatter.Format(stringBuilder, expected.Key);
				return;
			}

			stringBuilder.Append(It).Append(" contained key ");
			Formatter.Format(stringBuilder, expected.Key);
			stringBuilder.Append(" with value ");
			Formatter.Format(stringBuilder, _actualValue);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not contain ", "do not contain "));
			Formatter.Format(stringBuilder, expected);
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did");
	}
}
