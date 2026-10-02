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
	private const string IsEqualToSummary =
		"Verifies that the dictionary is equal to the <paramref name=\"expected\" /> dictionary.";

	private const string IsNotEqualToSummary =
		"Verifies that the dictionary is not equal to the <paramref name=\"unexpected\" /> dictionary.";

	private const string KeyComparerRemarks =
		"The expected keys are looked up through the dictionary, so its key comparer decides which keys are the same, and\n" +
		"two expected keys that it considers the same cannot both be matched by one of its keys. Detecting this needs the\n" +
		"comparer itself, which is only read from known dictionary types such as\n" +
		"<see cref=\"System.Collections.Generic.Dictionary{TKey,TValue}\" /> or\n" +
		"<see cref=\"System.Collections.Generic.SortedDictionary{TKey,TValue}\" />, and from the dictionary that a\n" +
		"<see cref=\"System.Collections.ObjectModel.ReadOnlyDictionary{TKey,TValue}\" /> wraps, not from a custom dictionary.\n" +
		"Without it, a key of the dictionary that equals no expected key is reported, even when the comparer considers it\n" +
		"the same as one.";

	[CreateExpectationFamily("Is{Not}EqualTo", PerSubject = true, Remarks = KeyComparerRemarks,
		Summary = IsEqualToSummary, NegatedSummary = IsNotEqualToSummary)]
	internal static ObjectEqualityResult<TCollection?, IThat<TCollection?>, TValue>
		IsEqualToCore<TCollection, TKey, TValue>(
			IThat<TCollection?> subject,
			IEnumerable<KeyValuePair<TKey, TValue>>? expected,
			string expectedExpression,
			bool negated)
		where TCollection : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		ICollection<KeyValuePair<TKey, TValue>>? expectedEntries =
			ThrowHelper.EnsureDistinctKeys(expected, negated);
		ObjectEqualityOptions<TValue> options = ObjectEqualityWithToleranceOptionsFactory.ForValuesOf<TValue>();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection?, IThat<TCollection?>, TValue>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<TCollection, TKey, TValue>(expectationBuilder, it, grammars,
					expectedExpression.TrimCommonWhiteSpace(),
					expectedEntries,
					options).InvertIf(negated)),
			subject,
			options);
	}

	private sealed class IsEqualToConstraint<TDictionary, TKey, TValue>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		string? expectedExpression,
		IEnumerable<KeyValuePair<TKey, TValue>>? expected,
		ObjectEqualityOptions<TValue> options)
		: ConstraintResult.WithEqualToValue<TDictionary?>(it, grammars, expected is null),
			IAsyncConstraint<TDictionary?>
		where TDictionary : IEnumerable<KeyValuePair<TKey, TValue>>
	{
		private string? _failure;

		public async Task<ConstraintResult> IsMetBy(TDictionary? actual,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = expected is null ? Outcome.Success : Outcome.Failure;
				return this;
			}

			AddDictionaryContext(expectationBuilder, actual);
			if (expected is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			ValueLookup<TKey, TValue> tryGetValue = GetLookup(actual);
			List<TKey> missingKeys = [];
			List<TKey> collapsedKeys = [];
			List<string> incorrectValues = [];
			ISet<TKey> matchedKeys = KeyComparers.CreateKeySet(actual);
			foreach (KeyValuePair<TKey, TValue> pair in expected)
			{
				if (!tryGetValue(pair.Key, out TValue? value))
				{
					missingKeys.Add(pair.Key);
					continue;
				}

				if (!matchedKeys.Add(pair.Key))
				{
					collapsedKeys.Add(pair.Key);
				}

				if (!await options.AreConsideredEqual(value!, pair.Value))
				{
					incorrectValues.Add(
						$"contained key {Formatter.Format(pair.Key)} with value {Formatter.Format(value)} instead of {Formatter.Format(pair.Value)}");
				}
			}

			List<string> errors = [];
			errors.AddRange(MissingKeysError(missingKeys));
			errors.AddRange(CollapsedKeysError(collapsedKeys));
			errors.AddRange(incorrectValues);
			errors.AddRange(AdditionalKeysError(actual, matchedKeys));
			_failure = errors.Count == 0 ? null : $"{It} {string.Join(" and ", errors)}";
			Outcome = _failure is null ? Outcome.Success : Outcome.Failure;
			return this;
		}

		private static IEnumerable<string> MissingKeysError(List<TKey> missingKeys)
		{
			if (missingKeys.Count == 1)
			{
				yield return $"lacked key {Formatter.Format(missingKeys[0])}";
			}
			else if (missingKeys.Count > 1)
			{
				yield return
					$"lacked {missingKeys.Count} keys: {Formatter.Format(missingKeys, FormattingOptions.SingleLine)}";
			}
		}

		private static IEnumerable<string> CollapsedKeysError(List<TKey> collapsedKeys)
		{
			if (collapsedKeys.Count == 1)
			{
				yield return $"lacked a distinct key for {Formatter.Format(collapsedKeys[0])}";
			}
			else if (collapsedKeys.Count > 1)
			{
				yield return
					$"lacked a distinct key for each of {collapsedKeys.Count} keys: {Formatter.Format(collapsedKeys, FormattingOptions.SingleLine)}";
			}
		}

		/// <summary>
		///     The names are only reported as additional when as many were found as the entry count asks for, because for a
		///     subject whose key comparer cannot be read, the <paramref name="matchedKeys" /> use the default equality, and a
		///     key comparer that considers more keys equal than the default one lets the scan for names overshoot.
		/// </summary>
		/// <remarks>
		///     Without the comparer, a key that equals no expected key can be an additional key hidden by two expected keys
		///     that the comparer considers the same, or merely spelled differently than the expected key it stands for. The
		///     two cannot be told apart, so such a key is reported even when the entry count matches.
		/// </remarks>
		private static IEnumerable<string> AdditionalKeysError(TDictionary actual, ISet<TKey> matchedKeys)
		{
			int count = 0;
			List<TKey> additionalKeys = [];
			foreach (KeyValuePair<TKey, TValue> pair in actual)
			{
				count++;
				if (!matchedKeys.Contains(pair.Key))
				{
					additionalKeys.Add(pair.Key);
				}
			}

			int matchedCount = matchedKeys.Count;
			if (count == matchedCount && additionalKeys.Count == 0)
			{
				yield break;
			}

			yield return (additionalKeys.Count == count - matchedCount, count == matchedCount, additionalKeys.Count) switch
			{
				(true, _, 1) => $"contained additional key {Formatter.Format(additionalKeys[0])}",
				(true, _, _) =>
					$"contained {additionalKeys.Count} additional keys: {Formatter.Format(additionalKeys, FormattingOptions.SingleLine)}",
				(false, true, 1) => $"contained key {Formatter.Format(additionalKeys[0])} that matched no expected key",
				(false, true, _) =>
					$"contained {additionalKeys.Count} keys that matched no expected key: {Formatter.Format(additionalKeys, FormattingOptions.SingleLine)}",
				_ => $"contained {count} {KeyNoun(count)} and matched {matchedCount} expected {KeyNoun(matchedCount)}",
			};
		}

		private static string KeyNoun(int count) => count == 1 ? "key" : "keys";

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is ", "are ")).Append("equal to dictionary ");
			stringBuilder.Append(expectedExpression ?? Formatter.Format(expected, FormattingOptions.SingleLine));
			stringBuilder.Append(options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(ExpectedDictionaryWasNull);
			}
			else
			{
				stringBuilder.Append(_failure);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is ", "are ")).Append("not equal to dictionary ");
			stringBuilder.Append(expectedExpression ?? Formatter.Format(expected, FormattingOptions.SingleLine));
			stringBuilder.Append(options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (expected is null)
			{
				stringBuilder.Append(ExpectedDictionaryWasNull);
			}
			else
			{
				stringBuilder.Append(It).Append(" was");
			}
		}
	}
}
