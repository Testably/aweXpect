using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatDictionary
{
	/// <summary>
	///     Verifies that the dictionary is equal to the <paramref name="expected" /> dictionary.
	/// </summary>
	public static ObjectEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TValue>
		IsEqualTo<TKey, TValue>(
			this IThat<IDictionary<TKey, TValue>?> subject,
			IEnumerable<KeyValuePair<TKey, TValue>> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ICollection<KeyValuePair<TKey, TValue>>? expectedEntries = ThrowHelper.EnsureDistinctKeys(expected);
		ObjectEqualityOptions<TValue> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TValue>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expectedEntries,
					options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the dictionary is equal to the <paramref name="expected" /> dictionary.
	/// </summary>
	/// <remarks>
	///     Most dictionaries implement both dictionary interfaces, so the two overloads must share a declaring type for the
	///     priority to decide between them.
	/// </remarks>
	[OverloadResolutionPriority(-1)]
	public static ObjectEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TValue>
		IsEqualTo<TKey, TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> subject,
			IEnumerable<KeyValuePair<TKey, TValue>> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ICollection<KeyValuePair<TKey, TValue>>? expectedEntries = ThrowHelper.EnsureDistinctKeys(expected);
		ObjectEqualityOptions<TValue> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TValue>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expectedEntries,
					options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the dictionary is not equal to the <paramref name="unexpected" /> dictionary.
	/// </summary>
	public static ObjectEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TValue>
		IsNotEqualTo<TKey, TValue>(
			this IThat<IDictionary<TKey, TValue>?> subject,
			IEnumerable<KeyValuePair<TKey, TValue>> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ICollection<KeyValuePair<TKey, TValue>>? unexpectedEntries = ThrowHelper.EnsureDistinctKeys(unexpected);
		ObjectEqualityOptions<TValue> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IDictionary<TKey, TValue>, IThat<IDictionary<TKey, TValue>?>, TValue>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<IDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpectedEntries,
					options).Invert()),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the dictionary is not equal to the <paramref name="unexpected" /> dictionary.
	/// </summary>
	/// <remarks>
	///     Most dictionaries implement both dictionary interfaces, so the two overloads must share a declaring type for the
	///     priority to decide between them.
	/// </remarks>
	[OverloadResolutionPriority(-1)]
	public static ObjectEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TValue>
		IsNotEqualTo<TKey, TValue>(
			this IThat<IReadOnlyDictionary<TKey, TValue>?> subject,
			IEnumerable<KeyValuePair<TKey, TValue>> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		ICollection<KeyValuePair<TKey, TValue>>? unexpectedEntries = ThrowHelper.EnsureDistinctKeys(unexpected);
		ObjectEqualityOptions<TValue> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IReadOnlyDictionary<TKey, TValue>, IThat<IReadOnlyDictionary<TKey, TValue>?>,
			TValue>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToConstraint<IReadOnlyDictionary<TKey, TValue>, TKey, TValue>(expectationBuilder, it,
					grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpectedEntries,
					options).Invert()),
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
			List<string> incorrectValues = [];
			HashSet<TKey> matchedKeys = [];
			foreach (KeyValuePair<TKey, TValue> pair in expected)
			{
				if (!tryGetValue(pair.Key, out TValue? value))
				{
					missingKeys.Add(pair.Key);
				}
				else
				{
					matchedKeys.Add(pair.Key);
					if (!await options.AreConsideredEqual(value!, pair.Value))
					{
						incorrectValues.Add(
							$"contained key {Formatter.Format(pair.Key)} with value {Formatter.Format(value)} instead of {Formatter.Format(pair.Value)}");
					}
				}
			}

			List<string> errors = [];
			errors.AddRange(MissingKeysError(missingKeys));
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

		/// <summary>
		///     The names are only reported when as many were found as the entry count asks for, because a key comparer that
		///     considers more keys equal than the default one lets the scan for names overshoot.
		/// </summary>
		private static IEnumerable<string> AdditionalKeysError(TDictionary actual, HashSet<TKey> matchedKeys)
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
			if (count == matchedCount)
			{
				yield break;
			}

			yield return (additionalKeys.Count == count - matchedCount, additionalKeys.Count) switch
			{
				(true, 1) => $"contained additional key {Formatter.Format(additionalKeys[0])}",
				(true, _) =>
					$"contained {additionalKeys.Count} additional keys: {Formatter.Format(additionalKeys, FormattingOptions.SingleLine)}",
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
				stringBuilder.Append(It).Append(" did");
			}
		}
	}
}
