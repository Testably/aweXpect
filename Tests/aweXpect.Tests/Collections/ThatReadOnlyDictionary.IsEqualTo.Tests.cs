using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class IsEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectHasTheSamePairsInADifferentOrder_ShouldSucceed()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IReadOnlyDictionary<string, int> expected = ToDictionary(["b", "a",], [2, 1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("a dictionary is a keyed lookup without a contractual enumeration order");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IReadOnlyDictionary<string, int>? subject = null;
				IReadOnlyDictionary<string, int> expected = ToDictionary(["a",], [1,]);

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
			public async Task WhenTheValueForAKeyDiffers_ShouldFail()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				IReadOnlyDictionary<string, int> expected = ToDictionary(["a",], [2,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained key "a" with value 1 instead of 2

					             Dictionary:
					             {["a"] = 1}
					             """);
			}
		}

		public sealed class ReadOnlyOnlyTests
		{
			[Fact]
			public async Task WhenSubjectHasAnAdditionalKey_ShouldFail()
			{
				ReadOnlyOnlyDictionary<string, int> subject =
					new(new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, });
				IReadOnlyDictionary<string, int> expected = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to dictionary expected,
					             but it contained additional key "b"

					             Dictionary:
					             [["a"] = 1, ["b"] = 2]
					             """);
			}

			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_ShouldLookTheKeysUpThroughIt()
			{
				ReadOnlyOnlyDictionary<string, int> subject =
					new(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, });
				IReadOnlyDictionary<string, int> expected = ToDictionary(["A",], [1,]);

				async Task Act()
					=> await (ObjectEqualityResult<IReadOnlyDictionary<string, int>,
						IThat<IReadOnlyDictionary<string, int>?>, int>)That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow()
					.Because("a type that implements no IDictionary is looked up through its own TryGetValue");
			}
		}
	}
}
