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
				[Test]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IAsyncEnumerable<int> subject = GetCancellingAsyncEnumerable(6, cts, token);

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6)
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveTestException>()
						.WithMessage("""
						             Expected that subject
						             has between 3 and 6 items,
						             but it could not be verified, because the evaluation was already canceled

						             Collection:
						             [0, 1, 2, 3, 4, 5, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsMatchingItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsTooFewItems_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2);

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has between 3 and 6 items,
						             but it had only 2 items

						             Collection:
						             [1, 2]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsTooManyItems_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 4, 5, 6, 7);

					async Task Act()
						=> await That(subject).HasCount().Between(3).And(6);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has between 3 and 6 items,
						             but it had at least 7 items

						             Collection:
						             [1, 2, 3, 4, 5, 6, 7, (… and maybe more)]
						             """);
				}

				[Test]
				[Arguments(null, 3)]
				[Arguments(1, null)]
				public async Task WhenMinimumOrMaximumIsNull_ShouldFail(int? minimum, int? maximum)
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().Between(minimum).And(maximum);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} items,
						              but it had at least 1 item

						              Collection:
						              [1, (… and maybe more)]
						              """)
						.Because("nothing can be ordered against a null bound");
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).HasCount().Between(2).And(4);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has between 2 and 4 items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenEnumerableContainsMatchingItems_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().Between(3).And(6));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have between 3 and 6 items,
						             but it had 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsTooFewItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().Between(3).And(6));

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments(null, 3)]
				[Arguments(1, null)]
				public async Task WhenMinimumOrMaximumIsNull_ShouldFail(int? minimum, int? maximum)
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().Between(minimum).And(maximum));

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} items,
						              but it had at least 1 item

						              Collection:
						              [1, (… and maybe more)]
						              """)
						.Because("nothing can be ordered against a null bound, so the negation fails as well");
				}
			}
		}
	}
}
#endif
