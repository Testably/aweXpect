using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class DoesNotContainKeys
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllKeysDoNotExist_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotContainKeys(42, 43);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAtLeastOneKeyExists_ShouldFail()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotContainKeys(42, 2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain keys [42, 2],
					             but it contained [
					               2
					             ]

					             Dictionary:
					             {[1] = 0, [2] = 0, [3] = 0}
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ReadOnlyDictionary<string, int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotContainKeys("foo", "bar");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain keys ["foo", "bar"],
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUnexpectedIsEmpty_ShouldThrowArgumentException()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotContainKeys();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);
				int[]? unexpected = null;

				async Task Act()
					=> await That(subject).DoesNotContainKeys(unexpected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenOneKeyIsMissingAndOneExists_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [0, 0, 0,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(d => d.DoesNotContainKeys(42, 2));

				await That(Act).DoesNotThrow();
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
					new Dictionary<string, int>(comparer)
					{
						["a"] = 1,
						["b"] = 2,
					});
				comparer.IsArmed = true;

				async Task Act()
					=> await That(subject).DoesNotContainKeys("x", "y");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain keys ["x", "y"],
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
					new ThrowingDictionary<string, int>(exception, ThrowingMembers.ContainsKey)
					{
						["a"] = 1,
						["b"] = 2,
					});

				async Task Act()
					=> await That(subject).DoesNotContainKeys("x", "y");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain keys ["x", "y"],
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
