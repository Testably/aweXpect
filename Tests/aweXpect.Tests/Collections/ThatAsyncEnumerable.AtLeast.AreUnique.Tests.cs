#if NET8_0_OR_GREATER
using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class AtLeast
	{
		public sealed class AreUnique
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenEnoughItemsAreUnique_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 4, 5, 5);

					async Task Act()
						=> await That(subject).AtLeast(4).AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).AtLeast(4).AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for at least 4 items,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenTooFewItemsAreUnique_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 3, 4, 4);

					async Task Act()
						=> await That(subject).AtLeast(4).AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for at least 4 items,
						             but only 2 of 6 were

						             Not matching items:
						             [3, 3, 4, 4]

						             Collection:
						             [1, 2, 3, 3, 4, 4]
						             """);
				}
			}

			public sealed class AreNotUniqueTests
			{
				[Test]
				public async Task WhenAllItemsAreUnique_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).AtLeast(1).AreNotUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not unique for at least one item,
						             but none of 3 were

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Test]
				public async Task WhenItContainsDuplicates_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 1);

					async Task Act()
						=> await That(subject).AtLeast(1).AreNotUnique();

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenEnoughItemsAreUnique_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 4, 5, 5);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.AtLeast(4).AreUnique());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for fewer than 4 items,
						             but 4 of 6 were

						             Matching items:
						             [1, 2, 3, 4]

						             Collection:
						             [1, 2, 3, 4, 5, 5]
						             """);
				}

				[Test]
				public async Task WhenTooFewItemsAreUnique_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3, 3, 4, 4);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.AtLeast(4).AreUnique());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
