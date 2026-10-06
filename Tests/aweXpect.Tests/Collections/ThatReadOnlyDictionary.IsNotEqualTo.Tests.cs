using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenBothAreNull_ShouldFail()
			{
				IReadOnlyDictionary<string, int>? subject = null;
				IReadOnlyDictionary<string, int>? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldSucceed()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);
				IReadOnlyDictionary<string, int>? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("a dictionary that is there is not equal to a null dictionary");
			}

			[Test]
			public async Task WhenSubjectHasADifferentValue_ShouldSucceed()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IReadOnlyDictionary<string, int> unexpected = ToDictionary(["a", "b",], [1, 3,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("the value for key b differs");
			}

			[Test]
			public async Task WhenSubjectHasTheSamePairsInADifferentOrder_ShouldFail()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a", "b",], [1, 2,]);
				IReadOnlyDictionary<string, int> unexpected = ToDictionary(["b", "a",], [2, 1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected,
					             but it was

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				IReadOnlyDictionary<string, int>? subject = null;
				IReadOnlyDictionary<string, int> unexpected = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("a null dictionary is not equal to a dictionary that is there");
			}

			[Test]
			public async Task WhenSubjectLacksAKey_ShouldSucceed()
			{
				ReadOnlyOnlyDictionary<string, int> subject = new(new Dictionary<string, int> { { "a", 1 }, });
				IReadOnlyDictionary<string, int> unexpected = ToDictionary(["a", "b",], [1, 2,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("the subject has no entry for key b");
			}

			[Test]
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

			[Test]
			public async Task WhenTheComparerCannotBeRead_WithADifferentlyCasedKey_ShouldSucceed()
			{
				ReadOnlyOnlyDictionary<string, int> subject =
					new(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, });
				IReadOnlyDictionary<string, int> unexpected = ToDictionary(["A",], [1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("without the comparer of the subject, the key \"a\" matched no unexpected key");
			}

			[Test]
			public async Task WhenTheComparerCannotBeRead_WithTwoUnexpectedKeysForOneEntry_ShouldSucceed()
			{
				ReadOnlyOnlyDictionary<string, int> subject =
					new(new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { { "a", 1 }, { "b", 1 }, });
				IReadOnlyDictionary<string, int> unexpected = ToDictionary(["a", "A",], [1, 1,]);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("the key \"b\" matched no unexpected key");
			}

			[Test]
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

			[Test]
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

		public sealed class ThrowingSubjectTests
		{
			[Test]
			public async Task WhenTheEnumerationThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("enumeration failed");
				IReadOnlyDictionary<string, int> subject = new ReadOnlyOnlyDictionary<string, int>(
					new ThrowingDictionary<string, int>(exception, ThrowingMembers.Enumeration) { ["a"] = 1, ["b"] = 2, });
				Dictionary<string, int> unexpected = new() { ["a"] = 1, ["c"] = 3, };

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected,
					             but it did throw an InvalidOperationException:
					               enumeration failed

					             Dictionary:
					             [the enumeration did throw an InvalidOperationException: enumeration failed]
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a dictionary that answered nothing cannot prove the negation either");
			}

			[Test]
			public async Task WhenTheKeyComparerThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("comparer failed");
				ThrowingKeyComparer<string> comparer = new(exception);
				IReadOnlyDictionary<string, int> subject = new ReadOnlyOnlyDictionary<string, int>(
					new Dictionary<string, int>(comparer) { ["a"] = 1, ["b"] = 2, });
				comparer.IsArmed = true;
				Dictionary<string, int> unexpected = new() { ["a"] = 1, ["c"] = 3, };

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected,
					             but it did throw an InvalidOperationException:
					               comparer failed

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a dictionary that answered nothing cannot prove the negation either");
			}

			[Test]
			public async Task WhenTheLookupThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("lookup failed");
				IReadOnlyDictionary<string, int> subject = new ReadOnlyOnlyDictionary<string, int>(
					new ThrowingDictionary<string, int>(exception, ThrowingMembers.TryGetValue) { ["a"] = 1, ["b"] = 2, });
				Dictionary<string, int> unexpected = new() { ["a"] = 1, ["c"] = 3, };

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to dictionary unexpected,
					             but it did throw an InvalidOperationException:
					               lookup failed

					             Dictionary:
					             {["a"] = 1, ["b"] = 2}
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a dictionary that answered nothing cannot prove the negation either");
			}
		}
	}
}
