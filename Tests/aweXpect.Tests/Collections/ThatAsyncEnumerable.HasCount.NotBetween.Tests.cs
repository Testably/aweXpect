#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class HasCount
	{
		public sealed class NotBetween
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenEnumerableContainsMatchingItems_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().NotBetween(3).And(6);

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
						=> await That(subject).HasCount().NotBetween(3).And(6);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsTooManyItems_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 4, 5, 6, 7);

					async Task Act()
						=> await That(subject).HasCount().NotBetween(3).And(6);

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments(null, 3)]
				[Arguments(1, null)]
				public async Task WhenMinimumOrMaximumIsNull_ShouldFail(int? minimum, int? maximum)
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasCount().NotBetween(minimum).And(maximum);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              does not have between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} items,
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
						=> await That(subject).HasCount().NotBetween(2).And(4);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have between 2 and 4 items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenEnumerableContainsTooFewItems_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().NotBetween(3).And(6));

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
				[Arguments(null, 3)]
				[Arguments(1, null)]
				public async Task WhenMinimumOrMaximumIsNull_ShouldFail(int? minimum, int? maximum)
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().NotBetween(minimum).And(maximum));

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has between {Formatter.Format(minimum)} and {Formatter.Format(maximum)} items,
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
