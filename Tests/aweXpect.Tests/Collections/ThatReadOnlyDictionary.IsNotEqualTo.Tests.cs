using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenBothAreNull_ShouldFail()
			{
				IReadOnlyDictionary<string, int>? subject = null;
				IReadOnlyDictionary<string, int>? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected!);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldSucceed()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				IReadOnlyDictionary<string, int>? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected!);

				await That(Act).DoesNotThrow()
					.Because("a dictionary that is there is not equal to a null dictionary");
			}

			[Fact]
			public async Task WhenSubjectHasADifferentValue_ShouldSucceed()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IReadOnlyDictionary<string, int> unexpected = ToDictionary(["a", "b",], [1, 3,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("the value for key b differs");
			}

			[Fact]
			public async Task WhenSubjectHasTheSamePairsInADifferentOrder_ShouldFail()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IReadOnlyDictionary<string, int> unexpected = ToDictionary(["b", "a",], [2, 1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected,
					             but it was

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				IReadOnlyDictionary<string, int>? subject = null;
				IReadOnlyDictionary<string, int> unexpected = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("a null dictionary is not equal to a dictionary that is there");
			}

			[Fact]
			public async Task WhenSubjectLacksAKey_ShouldSucceed()
			{
				ReadOnlyOnlyDictionary<string, int> subject = new(new Dictionary<string, int> { { "a", 1 }, });
				IReadOnlyDictionary<string, int> unexpected = ToDictionary(["a", "b",], [1, 2,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("the subject has no entry for key b");
			}

			[Fact]
			public async Task WhenSubjectUsesACaseInsensitiveComparer_WithTwoUnexpectedKeysForOneEntry_ShouldSucceed()
			{
				IReadOnlyDictionary<string, int> subject =
					new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, { "b", 1 }, };
				IReadOnlyDictionary<string, int> unexpected = ToDictionary(["a", "A",], [1, 1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("both unexpected keys are matched by the key \"a\", so the key \"b\" is left over");
			}

			[Fact]
			public async Task WhenUnexpectedContainsADuplicateKeyWithADifferentValue_ShouldThrowArgumentException()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				List<KeyValuePair<string, int>> unexpected = [new("a", 1), new("a", 2),];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The key \"a\" must not occur more than once.").AsPrefix()
					.Because("no dictionary can hold both entries, so the expectation would pass for every subject");
			}

			[Fact]
			public async Task WhenUnexpectedContainsADuplicateKeyWithTheSameValue_ShouldThrowArgumentException()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				List<KeyValuePair<string, int>> unexpected = [new("a", 1), new("a", 1),];

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The key \"a\" must not occur more than once.").AsPrefix()
					.Because("repeating an entry says nothing that the first one did not already say");
			}
		}
	}
}
