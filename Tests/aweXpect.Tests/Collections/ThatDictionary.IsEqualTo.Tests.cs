using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class IsEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenBothAreNull_ShouldSucceed()
			{
				IDictionary<string, int>? subject = null;
				IDictionary<string, int>? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("a null dictionary is equal to a null dictionary");
			}

			[Fact]
			public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldStillCompareTheEntries()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IEnumerable<KeyValuePair<string, int>> expected =
					Factory.GetSingleUseEnumerable<KeyValuePair<string, int>>(new("a", 1), new("b", 2));

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("the duplicate guard must not consume the only enumeration of the expected entries");
			}

			[Fact]
			public async Task WhenExpectedContainsADuplicateKeyWithADifferentValue_ShouldThrowArgumentException()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				List<KeyValuePair<string, int>> expected = [new("a", 1), new("a", 2),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The key \"a\" must not occur more than once.").AsPrefix()
					.Because("a dictionary holds one value per key, so no subject could ever satisfy both entries");
			}

			[Fact]
			public async Task WhenExpectedContainsADuplicateKeyWithTheSameValue_ShouldThrowArgumentException()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				List<KeyValuePair<string, int>> expected = [new("a", 1), new("a", 1),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The key \"a\" must not occur more than once.").AsPrefix()
					.Because("repeating an entry says nothing that the first one did not already say");
			}

			[Fact]
			public async Task WhenExpectedContainsTwoDuplicateKeys_ShouldNameTheOneThatAppearsFirst()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				List<KeyValuePair<string, int>> expected =
					[new("b", 2), new("a", 1), new("a", 3), new("b", 4),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The key \"b\" must not occur more than once.").AsPrefix()
					.Because("the first key that is written twice is the one to point at");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				IDictionary<string, int>? expected = null;

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but the expected dictionary was <null>

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Fact]
			public async Task WhenSubjectHasAnAdditionalKey_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IDictionary<string, int> expected = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained additional key "b"

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """);
			}

			[Fact]
			public async Task WhenSubjectHasTheSamePairsInADifferentOrder_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IDictionary<string, int> expected = ToDictionary(["b", "a",], [2, 1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("a dictionary is a keyed lookup without a contractual enumeration order");
			}

			[Fact]
			public async Task WhenSubjectHasTwoAdditionalKeys_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b", "c",], [1, 2, 3,]);
				IDictionary<string, int> expected = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained 2 additional keys: ["b", "c"]

					             Dictionary:
					             {["a"] = 1, ["b"] = 2, ["c"] = 3}
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_AndExpectedContainsADuplicateKey_ShouldThrowArgumentException()
			{
				IDictionary<string, int>? subject = null;
				List<KeyValuePair<string, int>> expected = [new("a", 1), new("a", 2),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The key \"a\" must not occur more than once.").AsPrefix()
					.Because("the expectation is rejected when it is written, so no subject can make it valid");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IDictionary<string, int>? subject = null;
				IDictionary<string, int> expected = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectLacksAKey_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				IDictionary<string, int> expected = ToDictionary(["a", "b",], [1, 2,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it lacked key "b"

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Fact]
			public async Task WhenSubjectLacksTwoKeys_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				IDictionary<string, int> expected = ToDictionary(["a", "b", "c",], [1, 2, 3,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it lacked 2 keys: ["b", "c"]

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Fact]
			public async Task WhenTheDefaultTimeToleranceIsSet_ShouldApplyIt()
			{
				DateTime value = new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
				Dictionary<int, DateTime> subject = new() { [1] = value, };
				Dictionary<int, DateTime> expected = new() { [1] = value.AddMilliseconds(500), };

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Seconds());
					await That(subject).IsEqualTo(expected);
				}

				await That(Act).DoesNotThrow()
					.Because("the values fall back to the default tolerance, as the items of a collection do");
			}

			[Fact]
			public async Task WhenTheDefaultTimeToleranceIsSet_ShouldMentionIt()
			{
				DateTime value = new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc);
				Dictionary<int, DateTime> subject = new() { [1] = value, };
				Dictionary<int, DateTime> expected = new() { [1] = value.AddSeconds(2), };

				async Task Act()
				{
					using IDisposable __ =
						Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(1.Seconds());
					await That(subject).IsEqualTo(expected);
				}

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is equal to dictionary expected ± 0:01,
					              but it contained key 1 with value {Formatter.Format(value)} instead of {Formatter.Format(expected[1])}

					              Dictionary:
					              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
					              """)
					.Because("the applied default tolerance is part of the expectation");
			}

			[Fact]
			public async Task WhenTheValueForAKeyDiffers_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IDictionary<string, int> expected = ToDictionary(["a", "b",], [1, 3,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained key "b" with value 2 instead of 3

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """);
			}
		}

		public sealed class ComparerTests
		{
#if NETFRAMEWORK
			[Fact]
			public async Task WhenSubjectIsAConcurrentDictionaryOnNetFramework_WithTwoExpectedKeysForOneEntry_ShouldFail()
			{
				IDictionary<string, int> subject =
					new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase) { ["a"] = 1, ["b"] = 1, };
				IDictionary<string, int> expected = ToDictionary(["a", "A",], [1, 1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained key "b" that matched no expected key
					             """).AsPrefix()
					.Because("the concurrent dictionary of .NET Framework does not expose its comparer, so the two expected keys must not hide the key \"b\"");
			}

#endif
			[Fact]
			public async Task WhenSubjectIsACustomDictionaryWrapper_WithADifferentlyCasedKey_ShouldFail()
			{
				IReadOnlyDictionary<string, int> subject = new ThatReadOnlyDictionary.ReadOnlyOnlyDictionary<string, int>(
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "A", 1 }, });
				IDictionary<string, int> expected = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained key "A" that matched no expected key

					             Dictionary:
					             {["A"] = 1}
					             """)
					.Because("without the comparer of the wrapped dictionary, the key \"A\" cannot be told apart from an additional key");
			}

			[Fact]
			public async Task WhenSubjectIsACustomDictionaryWrapper_WithTwoDifferentlyCasedKeys_ShouldFail()
			{
				IReadOnlyDictionary<string, int> subject = new ThatReadOnlyDictionary.ReadOnlyOnlyDictionary<string, int>(
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "A", 1 }, { "B", 1 }, });
				IDictionary<string, int> expected = ToDictionary(["a", "b",], [1, 1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained 2 keys that matched no expected key: ["A", "B"]

					             Dictionary:
					             {["A"] = 1, ["B"] = 1}
					             """)
					.Because("without the comparer of the wrapped dictionary, neither key can be told apart from an additional key");
			}

			[Fact]
			public async Task WhenSubjectIsACustomDictionaryWrapper_WithTwoExpectedKeysForOneEntry_ShouldFail()
			{
				IReadOnlyDictionary<string, int> subject = new ThatReadOnlyDictionary.ReadOnlyOnlyDictionary<string, int>(
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, { "b", 1 }, });
				IDictionary<string, int> expected = ToDictionary(["a", "A",], [1, 1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained key "b" that matched no expected key

					             Dictionary:
					             {["a"] = 1, ["b"] = 1}
					             """)
					.Because("without the comparer of the wrapped dictionary, the two expected keys must not hide the key \"b\"");
			}

			[Fact]
			public async Task WhenSubjectIsAReadOnlyDictionaryWrapper_ShouldUseTheComparerOfTheWrappedDictionary()
			{
				ReadOnlyDictionary<string, int> subject = new(
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { ["Let it be"] = 5, });

				async Task Act()
					=> await That(subject).IsEqualTo(new Dictionary<string, int> { ["LET IT BE"] = 5, });

				await That(Act).DoesNotThrow()
					.Because("IsEquivalentTo and ContainsKey use the comparer of the wrapped dictionary as well");
			}

			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_ShouldLookTheKeysUpThroughIt()
			{
				IDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, };
				IDictionary<string, int> expected = ToDictionary(["A",], [1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("the key comparer of the subject decides which keys are the same");
			}

			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_WithADifferentValue_ShouldFail()
			{
				IDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, };
				IDictionary<string, int> expected = ToDictionary(["A",], [2,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained key "A" with value 1 instead of 2

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_WithAnAdditionalKey_ShouldFail()
			{
				IDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, { "b", 2 }, };
				IDictionary<string, int> expected = ToDictionary(["A",], [1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained additional key "b"

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """)
					.Because("the comparer of the subject tells which of its keys were matched");
			}

			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_WithDifferentlyCasedKeysThatStayDistinct_ShouldSucceed()
			{
				IDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, { "b", 1 }, };
				IDictionary<string, int> expected = ToDictionary(["a", "B",], [1, 1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("every expected key is matched by a different key of the subject");
			}

			[Theory]
			[MemberData(nameof(CaseInsensitiveDictionaries), DisableDiscoveryEnumeration = true)]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_WithTwoExpectedKeysForOneEntry_ShouldFail(
				IDictionary<string, int> subject)
			{
				IDictionary<string, int> expected = ToDictionary(["a", "A",], [1, 1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it lacked a distinct key for "A" and contained additional key "b"
					             """).AsPrefix()
					.Because($"both expected keys are matched by the key \"a\" of the {subject.GetType().Name}");
			}

			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_WithTwoKeysThatOnlyItUnifies_ShouldNotReject()
			{
				IDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, };
				List<KeyValuePair<string, int>> expected = [new("a", 1), new("A", 1),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it lacked a distinct key for "A"

					             Dictionary:
					             {["a"] = 1}
					             """)
					.Because("the duplicate guard uses the default key equality, not the comparer of the subject");
			}

			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_WithTwoPairsOfExpectedKeysForTwoEntries_ShouldFail()
			{
				IDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, { "b", 1 }, { "c", 1 }, };
				IDictionary<string, int> expected = ToDictionary(["a", "A", "b", "B",], [1, 1, 1, 1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it lacked a distinct key for each of 2 keys: ["A", "B"] and contained additional key "c"

					             Dictionary:
					             {["a"] = 1, ["b"] = 1, ["c"] = 1}
					             """);
			}

			public static TheoryData<IDictionary<string, int>> CaseInsensitiveDictionaries()
			{
				Dictionary<string, int> entries = new() { { "a", 1 }, { "b", 1 }, };
				TheoryData<IDictionary<string, int>> dictionaries = new(
					new Dictionary<string, int>(entries, StringComparer.OrdinalIgnoreCase),
					new SortedDictionary<string, int>(entries, StringComparer.OrdinalIgnoreCase),
					new SortedList<string, int>(entries, StringComparer.OrdinalIgnoreCase),
					entries.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase),
					entries.ToImmutableSortedDictionary(StringComparer.OrdinalIgnoreCase),
					entries.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase),
					new ReadOnlyDictionary<string, int>(new Dictionary<string, int>(entries, StringComparer.OrdinalIgnoreCase)),
					new ReadOnlyDictionary<string, int>(
						new SortedDictionary<string, int>(entries, StringComparer.OrdinalIgnoreCase)));
#if NET8_0_OR_GREATER
				dictionaries.Add(new ConcurrentDictionary<string, int>(entries, StringComparer.OrdinalIgnoreCase));
#endif
				return dictionaries;
			}
		}

		public sealed class ValueComparerTests
		{
			[Fact]
			public async Task WhenUsingAComparerForTheValues_ShouldApplyItToTheValues()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				IDictionary<string, int> expected = ToDictionary(["a",], [2,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Using(new AllEqualComparer());

				await That(Act).DoesNotThrow()
					.Because("the equality options of the result apply to the values behind the keys");
			}

			private sealed class AllEqualComparer : IEqualityComparer<object>
			{
				public new bool Equals(object? x, object? y) => true;

				public int GetHashCode(object obj) => 0;
			}
		}

		public sealed class StringTests
		{
			[Fact]
			public async Task WhenAValueDiffersInMoreThanCase_WithIgnoringCase_ShouldFail()
			{
				Dictionary<int, string> subject = new() { [1] = "Let It Be", [2] = "Help!", };
				Dictionary<int, string> expected = new() { [1] = "LET IT BE", [2] = "YESTERDAY", };

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected ignoring case,
					             but it contained key 2 with value "Help!" instead of "YESTERDAY"

					             Dictionary:
					             {
					               [1] = "Let It Be",
					               [2] = "Help!"
					             }
					             """);
			}

			[Fact]
			public async Task WhenTheValuesDifferOnlyInCase_WithIgnoringCase_ShouldSucceed()
			{
				Dictionary<int, string> subject = new() { [1] = "Let It Be", };
				Dictionary<int, string> expected = new() { [1] = "LET IT BE", };

				async Task Act()
					=> await That(subject).IsEqualTo(expected).IgnoringCase();

				await That(Act).DoesNotThrow()
					.Because("the values have the same string options as ContainsValue");
			}
		}

		public sealed class WithinTests
		{
			[Fact]
			public async Task WhenAValueLiesOutsideTheTolerance_ShouldFail()
			{
				Dictionary<string, double> subject = new() { ["a"] = 1.05, ["b"] = 2.2, };
				Dictionary<string, double> expected = new() { ["a"] = 1.0, ["b"] = 2.0, };

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(0.1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected ± 0.1,
					             but it contained key "b" with value 2.2 instead of 2.0

					             Dictionary:
					             {["a"] = 1.05, ["b"] = 2.2}
					             """);
			}

			[Fact]
			public async Task WhenTheValuesLieWithinTheTolerance_ShouldSucceed()
			{
				Dictionary<string, DateTime> subject = new() { ["a"] = new DateTime(2024, 1, 1, 0, 0, 1), };
				Dictionary<string, DateTime> expected = new() { ["a"] = new DateTime(2024, 1, 1), };

				async Task Act()
					=> await That(subject).IsEqualTo(expected).Within(1.Seconds());

				await That(Act).DoesNotThrow()
					.Because("the values have the same tolerance as the items of a collection");
			}
		}

		public sealed class OverloadTests
		{
			[Fact]
			public async Task ForADictionary_ShouldBindToTheDictionaryOverload()
			{
				Dictionary<string, int> subject = new() { { "a", 1 }, { "b", 2 }, };
				Dictionary<string, int> expected = new() { { "b", 2 }, { "a", 1 }, };

				async Task Act()
					=> await (ObjectEqualityWithToleranceResult<Dictionary<string, int>?, IThat<Dictionary<string, int>?>, int, int>)
						That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("the compile-time result type pins the expectation to the dictionary overload");
			}

			[Fact]
			public async Task ForADictionary_ShouldKeepTheSubjectType()
			{
				Dictionary<string, int> subject = new() { { "a", 1 }, };
				Dictionary<string, int> expected = new() { { "a", 1 }, };

				Dictionary<string, int>? result = await That(subject).IsEqualTo(expected);

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task ForAListOfPairs_ShouldStillBindToTheCollectionOverload()
			{
				List<KeyValuePair<string, int>> subject = [new("a", 1), new("b", 2),];
				List<KeyValuePair<string, int>> expected = [new("b", 2), new("a", 1),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected).InAnyOrder();

				await That(Act).DoesNotThrow()
					.Because("a collection of pairs that is no dictionary keeps the collection result with InAnyOrder");
			}

			[Fact]
			public async Task ForAListOfPairsInADifferentOrder_ShouldFail()
			{
				List<KeyValuePair<string, int>> subject = [new("a", 1), new("b", 2),];
				List<KeyValuePair<string, int>> expected = [new("b", 2), new("a", 1),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("*is equal to collection expected in order*").AsWildcard()
					.Because("a collection of pairs that is no dictionary is still compared in order");
			}

			[Fact]
			public async Task ForAListOfPairsWithADuplicateKey_ShouldStillCompareInOrder()
			{
				List<KeyValuePair<string, int>> subject = [new("a", 1), new("a", 2),];
				List<KeyValuePair<string, int>> expected = [new("a", 1), new("a", 2),];

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("a repeated key is a legitimate sequence entry as long as no dictionary is involved");
			}

			[Fact]
			public async Task ForASequenceOfDictionaryPairs_ShouldCompareInOrder()
			{
				SortedDictionary<string, int> subject = new() { { "a", 1 }, { "b", 2 }, };
				List<KeyValuePair<string, int>> expected = [new("b", 2), new("a", 1),];

				async Task Act()
					=> await That(subject.AsEnumerable()).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("*is equal to collection expected in order*").AsWildcard()
					.Because("comparing a dictionary as a sequence of pairs keeps the ordered comparison available");
			}

			[Fact]
			public async Task ForASortedDictionary_ShouldBindToTheDictionaryOverload()
			{
				SortedDictionary<string, int> subject = new() { { "b", 2 }, { "a", 1 }, };
				Dictionary<string, int> expected = new() { { "a", 1 }, { "b", 2 }, };

				async Task Act()
					=> await (ObjectEqualityWithToleranceResult<IDictionary<string, int>?, IThat<IDictionary<string, int>?>, int, int>)
						That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("a type that implements both dictionary interfaces must not become ambiguous");
			}

			[Fact]
			public async Task ForASortedDictionaryOfStrings_ShouldBindToTheStringOverload()
			{
				SortedDictionary<string, string> subject = new() { { "a", "foo" }, };
				Dictionary<string, string> expected = new() { { "a", "FOO" }, };

				async Task Act()
					=> await (StringEqualityResult<IDictionary<string, string?>?, IThat<IDictionary<string, string?>?>>)
						That(subject).IsEqualTo(expected).IgnoringCase();

				await That(Act).DoesNotThrow()
					.Because("the string overloads of both dictionary interfaces must not become ambiguous");
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectHasDifferentEntries_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IDictionary<string, int> expected = ToDictionary(["a", "b",], [1, 3,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectHasTheSameEntries_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IDictionary<string, int> expected = ToDictionary(["b", "a",], [2, 1,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(expected));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary expected,
					             but it was

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """);
			}
		}

		public sealed class ThrowingSubjectTests
		{
			[Fact]
			public async Task WhenTheEnumerationThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("enumeration failed");
				IDictionary<string, int> subject =
					new ThrowingDictionary<string, int>(exception, ThrowingMembers.Enumeration) { ["a"] = 1, ["b"] = 2, };
				Dictionary<string, int> expected = new() { ["a"] = 1, ["b"] = 2, };

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it did throw an InvalidOperationException:
					               enumeration failed

					             Dictionary:
					             [the enumeration did throw an InvalidOperationException: enumeration failed]
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenTheKeyComparerThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("comparer failed");
				ThrowingKeyComparer<string> comparer = new(exception);
				Dictionary<string, int> subject = new(comparer) { ["a"] = 1, ["b"] = 2, };
				comparer.IsArmed = true;
				Dictionary<string, int> expected = new() { ["a"] = 1, ["b"] = 2, };

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it did throw an InvalidOperationException:
					               comparer failed

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenTheLookupThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("lookup failed");
				IDictionary<string, int> subject =
					new ThrowingDictionary<string, int>(exception, ThrowingMembers.TryGetValue) { ["a"] = 1, ["b"] = 2, };
				Dictionary<string, int> expected = new() { ["a"] = 1, ["b"] = 2, };

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it did throw an InvalidOperationException:
					               lookup failed

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}
		}
	}
}
