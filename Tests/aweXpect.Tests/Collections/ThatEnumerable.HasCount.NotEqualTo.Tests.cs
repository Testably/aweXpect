using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasCount
	{
		public sealed class NotEqualTo
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IEnumerable<int> subject = GetCancellingEnumerable(4, cts);

					async Task Act()
						=> await That(subject).HasCount().NotEqualTo(6)
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that subject
						             does not have exactly 6 items,
						             but could not verify, because it was already cancelled

						             Collection:
						             [
						               0,
						               1,
						               2,
						               3,
						               4,
						               5,
						               6,
						               7,
						               8,
						               9,
						               (… and maybe others)
						             ]
						             """);
				}

				[Fact]
				public async Task WhenArrayContainsMatchingItems_ShouldFail()
				{
					int[] subject = [1, 2,];

					async Task Act()
						=> await That(subject).HasCount().NotEqualTo(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have exactly 2 items,
						             but it did

						             Collection:
						             [1, 2]
						             """);
				}

				[Fact]
				public async Task WhenArrayContainsTooFewItems_ShouldSucceed()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().NotEqualTo(4);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenArrayContainsTooManyItems_ShouldSucceed()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().NotEqualTo(2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsMatchingItems_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).HasCount().NotEqualTo(3);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have exactly 3 items,
						             but it did

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsTooFewItems_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).HasCount().NotEqualTo(4);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsTooManyItems_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).HasCount().NotEqualTo(2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).HasCount().NotEqualTo(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have exactly 2 items,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
