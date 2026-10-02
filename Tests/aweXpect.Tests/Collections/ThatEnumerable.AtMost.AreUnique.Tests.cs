using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class AtMost
	{
		public sealed class AreUnique
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenFewEnoughItemsAreUnique_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 2, 3,]);

					async Task Act()
						=> await That(subject).AtMost(1).AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).AtMost(1).AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for at most one item,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenTooManyItemsAreUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3,]);

					async Task Act()
						=> await That(subject).AtMost(1).AreUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for at most one item,
						             but 2 of 4 were

						             Matching items:
						             [2, 3]

						             Collection:
						             [1, 1, 2, 3]
						             """);
				}
			}

			public sealed class AreNotUniqueTests
			{
				[Fact]
				public async Task WhenFewEnoughItemsAreNotUnique_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3,]);

					async Task Act()
						=> await That(subject).AtMost(2).AreNotUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTooManyItemsAreNotUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 2, 3,]);

					async Task Act()
						=> await That(subject).AtMost(2).AreNotUnique();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not unique for at most 2 items,
						             but at least 4 of at least 4 were

						             Matching items:
						             [1, 1, 2, 2, (… and maybe more)]

						             Collection:
						             [1, 1, 2, 2, (… and maybe more)]
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenFewEnoughItemsAreUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.AtMost(1).AreUnique());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is unique for more than one item,
						             but 1 of 5 were

						             Not matching items:
						             [1, 1, 2, 2]

						             Collection:
						             [1, 1, 2, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenTooManyItemsAreUnique_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.AtMost(1).AreUnique());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
