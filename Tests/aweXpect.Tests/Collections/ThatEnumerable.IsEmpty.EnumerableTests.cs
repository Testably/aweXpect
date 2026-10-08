using System.Collections;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsEmpty
	{
		public sealed class EnumerableTests
		{
			[Test]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

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
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

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
			public async Task WhenArrayContainsValues_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					"foo",
				};

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
				IEnumerable subject = Array.Empty<object>();

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsValues_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 1, 2,]);

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
				IEnumerable subject = ToEnumerable((int[])[]);

				async Task Act()
					=> await That(subject).IsEmpty();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSourceHasFurtherItems_ShouldNotReadThemForTheFailureMessage()
			{
				int readItems = 0;
				IEnumerable subject = CountReadItems();

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
				IEnumerable CountReadItems()
				{
					while (true)
					{
						yield return ++readItems;
					}
				}
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

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
