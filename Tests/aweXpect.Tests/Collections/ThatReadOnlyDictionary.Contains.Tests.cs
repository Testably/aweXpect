using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class Contains
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenKeyIsMissing_ShouldFail()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);

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
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("a", 1));

				await That(Act).DoesNotThrow()
					.Because("the dictionary holds the key with that value");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IReadOnlyDictionary<string, int>? subject = null;

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

		public sealed class KeyAndValueTests
		{
			[Fact]
			public async Task WhenEntryExists_ShouldSucceed()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).Contains("a", 1);

				await That(Act).DoesNotThrow()
					.Because("the key and value overload looks the entry up like the pair overload");
			}

			[Fact]
			public async Task WhenKeyExistsWithADifferentValue_ShouldFail()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);

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

		public sealed class ReadOnlyOnlyTests
		{
			[Fact]
			public async Task WhenKeyExistsWithADifferentValue_ShouldFail()
			{
				ReadOnlyOnlyDictionary<string, int> subject = new(new Dictionary<string, int> { { "a", 1 }, });

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("a", 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains ["a"] = 2,
					             but it contained the key "a" with value 1

					             Dictionary:
					             [["a"] = 1]
					             """);
			}

			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_ShouldLookTheKeyUpThroughIt()
			{
				ReadOnlyOnlyDictionary<string, int> subject =
					new(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, });

				async Task Act()
					=> await That(subject).Contains(new KeyValuePair<string, int>("A", 1));

				await That(Act).DoesNotThrow()
					.Because("a type that implements no IDictionary is looked up through its own TryGetValue");
			}
		}
	}
}
