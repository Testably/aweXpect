using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsEmpty
	{
		public sealed class Tests
		{
			[Test]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was [
					               1,
					               1,
					               2,
					               3,
					               5,
					               8,
					               13,
					               21,
					               34,
					               55,
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task ShouldCallEnumeratorOnlyOnce()
			{
				IEnumerable<int> subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was [
					               1
					             ]
					             """);
			}

			[Test]
			public async Task WhenAnEarlierAttemptHadAnotherSubject_ShouldDescribeTheLastOne()
			{
				int calls = 0;
				Func<IEnumerable<int>> subject = () => calls++ == 0 ? [1, 2,] : ToEnumerable([3,]);

				async Task Act()
					=> await That(subject).Eventually().WithinTwoAttempts(5.Seconds())
						.IsEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             eventually is empty within 0:05,
					             but it was [
					               3
					             ]
					             """);
			}

			[Test]
			public async Task WhenArrayContainsValues_ShouldFail()
			{
				string[] subject = ["foo",];

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was [
					               "foo"
					             ]
					             """);
			}

			[Test]
			public async Task WhenArrayIsEmpty_ShouldSucceed()
			{
				string[] subject = [];

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsValues_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 2,]);

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was [
					               1,
					               1,
					               2
					             ]
					             """);
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable((int[])[]);

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSourceThrowsAfterTwoItems_ShouldListTheItemsBeforeTheException()
			{
				IEnumerable<int> subject = ThrowAfter(new InvalidOperationException("src"), 1, 2);

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was [
					               1,
					               2,
					               (the enumeration did throw an InvalidOperationException: src)
					             ]
					             """)
					.Because("the items that were read before the exception are listed as well");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was <null>
					             """);
			}
		}
	}
}
