#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class None
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ConsidersCancellationToken()
			{
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;
				IAsyncEnumerable<int> subject =
					GetCancellingAsyncEnumerable(6, cts, CancellationToken.None);

				async Task Act()
					=> await That(subject).None().Satisfy(y => y < 0)
						.WithCancellation(token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that subject
					             satisfies y => y < 0 for no items,
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
					=> await That(subject).None().AreEqualTo(15)
						.And.None().AreEqualTo(81);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).None().AreEqualTo(5);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 5 for no items,
					             but at least one was

					             Matching items:
					             [5, (… and maybe others)]

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
			public async Task WhenEnumerableContainsEqualValues_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1, 1, 2, 2, 3);

				async Task Act()
					=> await That(subject).None().AreEqualTo(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for no items,
					             but 4 of 7 were

					             Matching items:
					             [1, 1, 1, 1]

					             Collection:
					             [1, 1, 1, 1, 2, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableOnlyContainsDifferentValues_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 1, 1, 1, 2, 2, 3);

				async Task Act()
					=> await That(subject).None().AreEqualTo(42);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_NegatedShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.None().AreEqualTo(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 0 for no items,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).None().AreEqualTo(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 0 for no items,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedComplyWithTests
		{
			[Fact]
			public async Task WhenNoItemComplies_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 4, 5);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.None().ComplyWith(x => x.IsGreaterThan(5)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 5 for at least one item,
					             but found 0

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
#endif
