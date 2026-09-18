#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class Between
	{
		public sealed class ItemsTests
		{
			[Fact]
			public async Task ConsidersCancellationToken()
			{
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;
				IAsyncEnumerable<int> subject =
					GetCancellingAsyncEnumerable(6, cts, CancellationToken.None);

				async Task Act()
					=> await That(subject).Between(6).And(8).Satisfy(y => y < 6)
						.WithCancellation(token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that subject
					             satisfies y => y < 6 for between 6 and 8 items,
					             but it could not be verified, because it was already cancelled

					             Collection:
					             [0, 1, 2, 3, 4, 5, (… and maybe others)]
					             """);
			}

			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

				async Task Act()
					=> await That(subject).Between(0).And(2).AreEqualTo(1)
						.And.Between(0).And(1).AreEqualTo(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).Between(0).And(1).AreEqualTo(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for between 0 and 1 items,
					             but at least 2 of at least 11 were

					             Collection:
					             [
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
					               (… and maybe others)
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsSufficientlyEqualItems_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1, 1, 2, 2, 3);

				async Task Act()
					=> await That(subject).Between(3).And(4).AreEqualTo(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsTooFewEqualItems_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1, 1, 2, 2, 3);

				async Task Act()
					=> await That(subject).Between(3).And(4).AreEqualTo(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2 for between 3 and 4 items,
					             but only 2 of 7 were

					             Collection:
					             [1, 1, 1, 1, 2, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsTooManyEqualItems_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1, 1, 2, 2, 3);

				async Task Act()
					=> await That(subject).Between(1).And(3).AreEqualTo(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for between 1 and 3 items,
					             but 4 of 7 were

					             Collection:
					             [1, 1, 1, 1, 2, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_NegatedShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Between(0).And(1).AreEqualTo(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 0 for not between 0 and 1 items,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).Between(0).And(1).AreEqualTo(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 0 for between 0 and 1 items,
					             but it was <null>
					             """);
			}
		}

		public sealed class StringTests
		{
			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "FOO", "bar",]);

				async Task Act()
					=> await That(subject).Between(0).And(1).AreEqualTo("foo").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" ignoring case for between 0 and 1 items,
					             but 2 of 3 were

					             Collection:
					             [
					               "foo",
					               "FOO",
					               "bar"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedNumberOfEqualItems_ShouldSucceed()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "foo", "bar",]);

				async Task Act()
					=> await That(subject).Between(2).And(3).AreEqualTo("foo");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsTooFewEqualItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "FOO", "foo", "bar",]);

				async Task Act()
					=> await That(subject).Between(3).And(4).AreEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" for between 3 and 4 items,
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

			[Fact]
			public async Task WhenEnumerableContainsTooManyEqualItems_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "foo", "bar",]);

				async Task Act()
					=> await That(subject).Between(0).And(1).AreEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" for between 0 and 1 items,
					             but 2 of 3 were

					             Collection:
					             [
					               "foo",
					               "foo",
					               "bar"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).Between(1).And(3).AreEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" for between 1 and 3 items,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedComplyWithTests
		{
			[Fact]
			public async Task WhenTheNumberOfComplyingItemsIsInRange_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 4, 5);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.Between(2).And(4).ComplyWith(x => x.IsGreaterThan(2)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2 for not between 2 and 4 items,
					             but 3 of 5 were

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
#endif
