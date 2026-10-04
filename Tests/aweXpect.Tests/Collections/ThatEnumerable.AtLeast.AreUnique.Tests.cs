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
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).AtLeast(4).AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for at least 4 items,
						             but it was <null>
						             """);
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

						             Not matching items:
						             [3, 3, 4, 4]

						             Collection:
						             [1, 2, 3, 3, 4, 4]
						             """);
				}
			}

			public sealed class AreNotUniqueTests
			{
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
						             but none of 3 were

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenItContainsDuplicates_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3, 1,]);

					async Task Act()
						=> await That(subject).AtLeast(1).AreNotUnique();

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenEnoughItemsAreUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3, 4, 5, 5,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.AtLeast(4).AreUnique());

					await That(Act).Throws<XunitException>()
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

				[Fact]
				public async Task WhenTooFewItemsAreUnique_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3, 3, 4, 4,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.AtLeast(4).AreUnique());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
