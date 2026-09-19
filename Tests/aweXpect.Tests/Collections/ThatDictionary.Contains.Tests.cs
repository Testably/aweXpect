using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class Contains
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenKeyExistsWithADifferentValue_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("a", 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["a"] = 2,
					             but it contained the key "a" with value 1

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Fact]
			public async Task WhenKeyIsMissing_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("b", 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["b"] = 1,
					             but it did not contain the key "b"

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Fact]
			public async Task WhenPairExists_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("b", 2));

				await That(Act).DoesNotThrow()
					.Because("the dictionary holds the key with that value");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IDictionary<string, int>? subject = null;

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("a", 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["a"] = 1,
					             but it was <null>
					             """);
			}
		}

		public sealed class ComparerTests
		{
			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_ShouldLookTheKeyUpThroughIt()
			{
				IDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, };

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("A", 1));

				await That(Act).DoesNotThrow()
					.Because("ContainsKey finds the key through the comparer, and Contains must not contradict it");
			}

			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_WithADifferentValue_ShouldFail()
			{
				IDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, };

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("A", 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["A"] = 2,
					             but it contained the key "A" with value 1

					             Dictionary:
					             {["a"] = 1}
					             """);
			}
		}

		public sealed class KeyAndValueTests
		{
			[Fact]
			public async Task WhenEntryExists_ShouldSucceed()
			{
				IDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);

				async Task Act()
					=> await That(subject).Contains("b", 2);

				await That(Act).DoesNotThrow()
					.Because("the key and value overload looks the entry up like the pair overload");
			}

			[Fact]
			public async Task WhenKeyExistsWithADifferentValue_ShouldFail()
			{
				IDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).Contains("a", 2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["a"] = 2,
					             but it contained the key "a" with value 1

					             Dictionary:
					             {["a"] = 1}
					             """);
			}
		}

		public sealed class OverloadTests
		{
			[Fact]
			public async Task ForAListOfPairs_ShouldStillBindToTheCollectionOverload()
			{
				List<KeyValuePair<string, int>> subject = [new("a", 1),];

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("a", 1)).Once();

				await That(Act).DoesNotThrow()
					.Because("a collection of pairs that is no dictionary keeps the quantifier of the collection result");
			}

			[Fact]
			public async Task ForASortedDictionary_ShouldBindToTheDictionaryOverload()
			{
				SortedDictionary<string, int> subject = new() { { "a", 1 }, };

				async Task Act()
					=> await (ObjectEqualityResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>, int>)
						That(subject).Contains(new KeyValuePair<string, int>("a", 1));

				await That(Act).DoesNotThrow()
					.Because("a type that implements both dictionary interfaces must not become ambiguous");
			}

			[Fact]
			public async Task ForASortedDictionary_WithKeyAndValue_ShouldBindToTheDictionaryOverload()
			{
				SortedDictionary<string, int> subject = new() { { "a", 1 }, };

				async Task Act()
					=> await (ObjectEqualityResult<IDictionary<string, int>, IThat<IDictionary<string, int>?>, int>)
						That(subject).Contains("a", 1);

				await That(Act).DoesNotThrow()
					.Because("the key and value overloads need the same priority to stay unambiguous");
			}
		}
	}
}
