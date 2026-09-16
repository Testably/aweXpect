using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotHaveItemThat
	{
		public sealed class Tests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				ThrowWhenIteratingTwiceEnumerable subject = new();

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(42))
						.And.DoesNotHaveItemThat(it => it.IsEqualTo(42)).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsMatchingItemAtGivenIndex_ShouldFail()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(2)).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have item that is equal to 2 at index 2,
					              but it had item 2 at index 2

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenEnumerableContainsOtherItemAtGivenIndex_ShouldSucceed()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(1)).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(2)).AtIndex(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldSucceed()
			{
				int[] subject = [];

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsGreaterThan(0));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(1)).AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item that is equal to 1 at index 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithAnyIndex_WhenAnItemMatches_ShouldFail()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have item that is equal to 1,
					              but it had item 1

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class FromEndTests
		{
			[Fact]
			public async Task WhenEnumerableContainsMatchingItemAtGivenIndex_ShouldFail()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(1)).AtIndex(1).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have item that is equal to 1 at index 1 from end,
					              but it had item 1 at index 1 from end

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
			{
				int[] subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(1)).AtIndex(3).FromEnd();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
