using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
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
					IEnumerable<int> subject = GetCancellingEnumerable(4, cts);

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6)
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that subject
						             has between 3 and 6 items,
						             but it could not be verified, because it was already cancelled

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
				public async Task WhenArrayContainsMatchingItems_ShouldSucceed()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenArrayContainsTooFewItems_ShouldFail()
				{
					int[] subject = [1, 2,];

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
				public async Task WhenArrayContainsTooManyItems_ShouldSucceed()
				{
					int[] subject = [1, 2, 3, 4, 5, 6, 7,];

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has between 3 and 6 items,
						             but found 7

						             Collection:
						             [1, 2, 3, 4, 5, 6, 7]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsMatchingItems_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsTooFewItems_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2,]);

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
					IEnumerable<int> subject = ToEnumerable([1, 2, 3, 4, 5, 6, 7,]);

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
					IEnumerable<int>? subject = null;

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
