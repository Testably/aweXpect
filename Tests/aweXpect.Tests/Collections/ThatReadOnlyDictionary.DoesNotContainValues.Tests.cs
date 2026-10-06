using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aweXpect.Tests;

public sealed partial class ThatReadOnlyDictionary
{
	public sealed class DoesNotContainValues
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAllValuesDoNotExist_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValues(0, 2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAllValuesDoNotExist_WithNull_ShouldSucceed()
			{
				IReadOnlyDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValues(2, null);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAtLeastOneValueExists_ShouldFail()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValues(42, 2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values [42, 2],
					             but it contained [
					               42
					             ]

					             Dictionary:
					             {[1] = 41, [2] = 42, [3] = 43}
					             """);
			}

			[Test]
			public async Task WhenAtLeastOneValueExists_WithNull_ShouldFail()
			{
				IReadOnlyDictionary<int, int?> subject = ToDictionary<int, int?>([1, 2, 3,], [null, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValues(2, null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values [2, <null>],
					             but it contained [
					               <null>
					             ]

					             Dictionary:
					             {[1] = <null>, [2] = 42, [3] = 43}
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				ReadOnlyDictionary<int, string>? subject = null;

				async Task Act()
					=> await That(subject)!.DoesNotContainValues("foo", "bar");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values ["foo", "bar"],
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUnexpectedIsEmpty_ShouldThrowArgumentException()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);

				async Task Act()
					=> await That(subject).DoesNotContainValues();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldThrowArgumentNullException()
			{
				IReadOnlyDictionary<int, int> subject = ToDictionary([1, 2, 3,], [41, 42, 43,]);
				int[]? unexpected = null;

				async Task Act()
					=> await That(subject).DoesNotContainValues(unexpected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
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

				async Task Act()
					=> await That(subject).DoesNotContainValues(7, 8);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not contain values [7, 8],
					             but it did throw an InvalidOperationException:
					               enumeration failed

					             Dictionary:
					             [the enumeration did throw an InvalidOperationException: enumeration failed]
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a dictionary that answered nothing cannot prove the negation either");
			}
		}
	}
}
