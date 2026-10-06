#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class Exactly
	{
		public sealed class ItemsTests
		{
			[Test]
			public async Task ConsidersCancellationToken()
			{
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;
				IAsyncEnumerable<int> subject =
					GetCancellingAsyncEnumerable(6, cts, CancellationToken.None);

				async Task Act()
					=> await That(subject).Exactly(6).Satisfy(y => y < 6)
						.WithCancellation(token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that subject
					             satisfies y => y < 6 for exactly 6 items,
					             but it could not be verified, because the evaluation was already canceled

					             Collection:
					             [0, 1, 2, 3, 4, 5, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

				async Task Act()
					=> await That(subject).Exactly(1).AreEqualTo(1)
						.And.Exactly(1).AreEqualTo(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).Exactly(1).AreEqualTo(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for exactly one item,
					             but at least 2 of at least 2 were

					             Collection:
					             [1, 1, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsExpectedNumberOfEqualItems_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1, 1, 2, 2, 3);

				async Task Act()
					=> await That(subject).Exactly(4).AreEqualTo(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsTooFewEqualItems_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1, 1, 2, 2, 3);

				async Task Act()
					=> await That(subject).Exactly(4).AreEqualTo(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2 for exactly 4 items,
					             but only 2 of 7 were

					             Collection:
					             [1, 1, 1, 1, 2, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsTooManyEqualItems_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1, 1, 2, 2, 3);

				async Task Act()
					=> await That(subject).Exactly(3).AreEqualTo(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for exactly 3 items,
					             but at least 4 of at least 4 were

					             Collection:
					             [1, 1, 1, 1, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_NegatedShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Exactly(1).AreEqualTo(0));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 0 for not exactly one item,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).Exactly(1).AreEqualTo(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 0 for exactly one item,
					             but it was <null>
					             """);
			}
		}

		public sealed class StringTests
		{
			[Test]
			public async Task ShouldSupportIgnoringCase()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "FOO", "bar",]);

				async Task Act()
					=> await That(subject).Exactly(1).AreEqualTo("foo").IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" ignoring case for exactly one item,
					             but at least 2 of at least 2 were

					             Collection:
					             [
					               "foo",
					               "FOO",
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsExpectedNumberOfEqualItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "foo", "bar",]);

				async Task Act()
					=> await That(subject).Exactly(2).AreEqualTo("foo");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsTooFewEqualItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "FOO", "foo", "bar",]);

				async Task Act()
					=> await That(subject).Exactly(3).AreEqualTo("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" for exactly 3 items,
					             but only 2 of 4 were

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
			public async Task WhenEnumerableContainsTooManyEqualItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "foo", "bar",]);

				async Task Act()
					=> await That(subject).Exactly(1).AreEqualTo("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" for exactly one item,
					             but at least 2 of at least 2 were

					             Collection:
					             [
					               "foo",
					               "foo",
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).Exactly(1).AreEqualTo("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" for exactly one item,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedComplyWithTests
		{
			[Test]
			public async Task WhenExactlyTheExpectedNumberComplies_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 4, 5);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.Exactly(3).ComplyWith(x => x.IsGreaterThan(2)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2 for not exactly 3 items,
					             but 3 of 5 were

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
#endif
