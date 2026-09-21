using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
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
					=> await That(subject).IsEqualTo(expected!);

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
					=> await That(subject).IsEqualTo(expected!);

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
					             but it contained 2 keys and matched 1 expected key

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """)
					.Because("naming the additional keys would overshoot when the comparer is coarser than the default");
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
					             but it contained 1 key and matched 2 expected keys

					             Dictionary:
					             {["a"] = 1}
					             """)
					.Because("the duplicate guard uses the default key equality, not the comparer of the subject");
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

		public sealed class OverloadTests
		{
			[Fact]
			public async Task ForADictionary_ShouldBindToTheDictionaryOverload()
			{
				Dictionary<string, int> subject = new() { { "a", 1 }, { "b", 2 }, };
				Dictionary<string, int> expected = new() { { "b", 2 }, { "a", 1 }, };

				async Task Act()
					=> await (ObjectEqualityResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>, int>)
						That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("the compile-time result type pins the expectation to the dictionary overload");
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
					=> await (ObjectEqualityResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>, int>)
						That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("a type that implements both dictionary interfaces must not become ambiguous");
			}
		}
	}
}
