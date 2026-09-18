#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class HasCount
	{
		public sealed class Between
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IAsyncEnumerable<int> subject = GetCancellingAsyncEnumerable(6, cts, token);

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6)
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that subject
						             has between 3 and 6 items,
						             but it could not be verified, because it was already cancelled

						             Collection:
						             [0, 1, 2, 3, 4, 5, 6, (… and maybe others)]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsMatchingItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsTooFewItems_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2);

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has between 3 and 6 items,
						             but found only 2

						             Collection:
						             [1, 2]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsTooManyItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 4, 5, 6, 7);

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has between 3 and 6 items,
						             but found at least 7

						             Collection:
						             [1, 2, 3, 4, 5, 6, 7]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).HasCount().Between(2).And(4);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has between 2 and 4 items,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
#endif
