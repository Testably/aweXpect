using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class DoesNotContain
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenKeyIsMissing_ShouldSucceed()
			{
				ReadOnlyOnlyDictionary<string, int> subject = new(new Dictionary<string, int> { { "a", 1 }, });

				async Task Act()
					=> await That(subject).DoesNotContain(new KeyValuePair<string, int>("b", 1));

				await That(Act).DoesNotThrow()
					.Because("the dictionary has no entry for key b");
			}

			[Test]
			public async Task WhenPairExists_ShouldFail()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).DoesNotContain(new KeyValuePair<string, int>("a", 1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain ["a"] = 1,
					             but it did

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IReadOnlyDictionary<string, int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotContain(new KeyValuePair<string, int>("a", 1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain ["a"] = 1,
					             but it was <null>
					             """);
			}
		}

		public sealed class KeyAndValueTests
		{
			[Test]
			public async Task WhenEntryExists_ShouldFail()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).DoesNotContain("a", 1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain ["a"] = 1,
					             but it did

					             Dictionary:
					             {["a"] = 1}
					             """);
			}

			[Test]
			public async Task WhenKeyIsMissing_ShouldSucceed()
			{
				IReadOnlyDictionary<string, int> subject = ToDictionary(["a",], [1,]);

				async Task Act()
					=> await That(subject).DoesNotContain("b", 1);

				await That(Act).DoesNotThrow()
					.Because("the key and value overload looks the entry up like the pair overload");
			}
		}

		public sealed class ThrowingSubjectTests
		{
			[Test]
			public async Task WhenTheKeyComparerThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("comparer failed");
				ThrowingKeyComparer<string> comparer = new(exception);
				IReadOnlyDictionary<string, int> subject = new ReadOnlyOnlyDictionary<string, int>(
					new Dictionary<string, int>(comparer) { ["a"] = 1, ["b"] = 2, });
				comparer.IsArmed = true;

				async Task Act()
					=> await That(subject).DoesNotContain("a", 7);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain ["a"] = 7,
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

				async Task Act()
					=> await That(subject).DoesNotContain("a", 7);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain ["a"] = 7,
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
