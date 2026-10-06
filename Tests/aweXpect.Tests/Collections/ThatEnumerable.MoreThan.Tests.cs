using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class MoreThan
	{
		public sealed class ItemsTests
		{
			[Test]
			public async Task ConsidersCancellationToken()
			{
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;
				IEnumerable<int> subject = GetCancellingEnumerable(6, cts);

				async Task Act()
					=> await That(subject).MoreThan(6).Satisfy(y => y < 6)
						.WithCancellation(token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that subject
					             satisfies y => y < 6 for more than 6 items,
					             but it could not be verified, because the evaluation was already canceled

					             Collection:
					             [0, 1, 2, 3, 4, 5, 6, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceEnumerable subject = new();

				async Task Act()
					=> await That(subject).MoreThan(0).AreEqualTo(1)
						.And.MoreThan(0).AreEqualTo(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).MoreThan(1).AreEqualTo(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsEnoughEqualItems_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 1, 1, 2, 2, 3,]);

				async Task Act()
					=> await That(subject).MoreThan(3).AreEqualTo(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsTooFewEqualItems_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 1, 1, 2, 2, 3,]);

				async Task Act()
					=> await That(subject).MoreThan(5).AreEqualTo(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for more than 5 items,
					             but only 4 of 7 were

					             Not matching items:
					             [2, 2, 3]

					             Collection:
					             [1, 1, 1, 1, 2, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).MoreThan(1).AreEqualTo(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 0 for more than one item,
					             but it was <null>
					             """);
			}
		}

		public sealed class StringTests
		{
			[Test]
			public async Task ShouldSupportIgnoringCase()
			{
				IEnumerable<string> subject = ToEnumerable(["foo", "FOO", "bar",]);

				async Task Act()
					=> await That(subject).MoreThan(2).AreEqualTo("foo").IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" ignoring case for more than 2 items,
					             but only 2 of 3 were

					             Not matching items:
					             [
					               "bar"
					             ]

					             Collection:
					             [
					               "foo",
					               "FOO",
					               "bar"
					             ]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsExpectedNumberOfEqualItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["foo", "foo", "bar",]);

				async Task Act()
					=> await That(subject).MoreThan(2).AreEqualTo("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" for more than 2 items,
					             but only 2 of 3 were

					             Not matching items:
					             [
					               "bar"
					             ]

					             Collection:
					             [
					               "foo",
					               "foo",
					               "bar"
					             ]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsTooFewEqualItems_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["foo", "FOO", "foo", "bar",]);

				async Task Act()
					=> await That(subject).MoreThan(2).AreEqualTo("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" for more than 2 items,
					             but only 2 of 4 were

					             Not matching items:
					             [
					               "FOO",
					               "bar"
					             ]

					             Collection:
					             [
					               "foo",
					               "FOO",
					               "foo",
					               "bar"
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).MoreThan(1).AreEqualTo("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" for more than one item,
					             but it was <null>
					             """);
			}
		}
	}
}
