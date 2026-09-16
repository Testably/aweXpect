using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class AtLeast
	{
		public sealed class AreUnique
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenEnoughItemsAreUnique_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3, 4, 5, 5,]);

					async Task Act()
						=> await That(subject).AtLeast(4).AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTooFewItemsAreUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3, 3, 4, 4,]);

					async Task Act()
						=> await That(subject).AtLeast(4).AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for at least 4 items,
						             but only 2 of 6 were

						             Collection:
						             [1, 2, 3, 3, 4, 4]
						             """);
				}
			}

			public sealed class AreNotUniqueTests
			{
				[Fact]
				public async Task WhenItContainsDuplicates_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3, 1,]);

					async Task Act()
						=> await That(subject).AtLeast(1).AreNotUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).AtLeast(1).AreNotUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not unique for at least one item,
						             but only 0 of 3 were

						             Collection:
						             [1, 2, 3]
						             """);
				}
			}
		}
	}
}
