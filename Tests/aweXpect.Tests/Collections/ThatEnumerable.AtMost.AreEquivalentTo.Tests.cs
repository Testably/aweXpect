using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class AtMost
	{
		public sealed class AreEquivalentTo
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenFewEnoughItemsAreEquivalent_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3,]);

					async Task Act()
						=> await That(subject).AtMost(2).AreEquivalentTo(1);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).AtMost(2).AreEquivalentTo(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to 1 for at most 2 items,
						             but it was <null>

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Fact]
				public async Task WhenTooManyItemsAreEquivalent_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1, 2,]);

					async Task Act()
						=> await That(subject).AtMost(2).AreEquivalentTo(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to 1 for at most 2 items,
						             but at least 3 of at least 3 were

						             Matching items:
						             [1, 1, 1, (… and maybe more)]

						             Collection:
						             [1, 1, 1, (… and maybe more)]

						             Equivalency options:
						              - include public fields and properties
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenFewEnoughItemsAreEquivalent_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.AtMost(2).AreEquivalentTo(1));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to 1 for more than 2 items,
						             but 2 of 4 were

						             Not matching items:
						             [2, 3]

						             Collection:
						             [1, 1, 2, 3]

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Fact]
				public async Task WhenTooManyItemsAreEquivalent_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1, 2,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.AtMost(2).AreEquivalentTo(1));

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
