using System.Collections.Generic;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;

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
					               1,
					               (… and maybe more)
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
						.IsEmpty().WithTimeSystem(new VirtualTimeSystem());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             eventually is empty within 0:05,
					             but it was [
					               3,
					               (… and maybe more)
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
					               (… and maybe more)
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
			public async Task WhenSourceHasFurtherItems_ShouldNotReadThemForTheFailureMessage()
			{
				int readItems = 0;
				IEnumerable<int> subject = CountReadItems();

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is empty,
					             but it was [
					               1,
					               (… and maybe more)
					             ]
					             """);
				await That(readItems).IsEqualTo(1)
					.Because("a source that blocks after the first item must not hang the failure message");

				// ReSharper disable once IteratorNeverReturns
				IEnumerable<int> CountReadItems()
				{
					while (true)
					{
						yield return ++readItems;
					}
				}
			}

			[Test]
			public async Task WhenSourceThrowsAfterTheFirstItem_ShouldListTheItem()
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
					               (… and maybe more)
					             ]
					             """)
					.Because("the first item already decides the result, so the exception of the source must not replace it");
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
