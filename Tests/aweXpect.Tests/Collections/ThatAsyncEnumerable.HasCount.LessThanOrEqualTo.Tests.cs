#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class HasCount
	{
		public sealed class LessThanOrEqualTo
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
						=> await That(subject).HasCount().LessThanOrEqualTo(6)
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that subject
						             has at most 6 items,
						             but could not verify, because it was already cancelled

						             Collection:
						             [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, (… and maybe others)]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsMatchingItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(3);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsTooFewItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(4);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsTooManyItems_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has at most 2 items,
						             but found at least 3

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has at most 2 items,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
#endif
