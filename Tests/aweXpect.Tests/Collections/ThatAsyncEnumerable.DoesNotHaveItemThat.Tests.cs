#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class DoesNotHaveItemThat
	{
		public sealed class Tests
		{
			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(5)).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsMatchingItemAtGivenIndex_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

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
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(1)).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(2)).AtIndex(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

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
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

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

		public sealed class FromEndTests
		{
			[Fact]
			public async Task WhenEnumerableContainsMatchingItemAtGivenIndex_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(1)).AtIndex(1).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item that is equal to 1 at index 1 from end,
					             but it had item 1 at index 1 from end

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(1)).AtIndex(3).FromEnd();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
