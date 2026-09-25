using System.Collections;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotHaveItemThat
	{
		public sealed class EnumerableTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(42))
						.And.DoesNotHaveItemThat(it => it.IsEqualTo(42)).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsMatchingItemAtGivenIndex_ShouldFail()
			{
				IEnumerable subject = new[] { 0, 1, 2, };

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(2)).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item that is equal to 2 at index 2,
					             but it had item 2 at index 2

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsOtherItemAtGivenIndex_ShouldSucceed()
			{
				IEnumerable subject = new[] { 0, 1, 2, };

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(1)).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
			{
				IEnumerable subject = new[] { 0, 1, 2, };

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(2)).AtIndex(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldSucceed()
			{
				IEnumerable subject = Array.Empty<int>();

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsNotNull());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable subject = new[] { 1, 2, 3, };

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

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
				IEnumerable subject = new[] { 0, 1, 2, };

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item that is equal to 1,
					             but it had item 1

					             Collection:
					             [0, 1, 2]
					             """);
			}
		}

		public sealed class EnumerableNegatedTests
		{
			[Fact]
			public async Task WhenEnumerableContainsMatchingItem_ShouldSucceed()
			{
				IEnumerable subject = new[] { 0, 1, 2, };

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.DoesNotHaveItemThat(x => x.IsEqualTo(1)));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.DoesNotHaveItemThat(x => x.IsEqualTo(1)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is equal to 1,
					             but it was <null>
					             """);
			}
		}
	}
}
