using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Exactly
	{
		public sealed class AreUnique
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenExactlyTheExpectedNumberOfItemsIsUnique_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3,]);

					async Task Act()
						=> await That(subject).Exactly(2).AreUnique();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).Exactly(2).AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for exactly 2 items,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenTooFewItemsAreUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 2, 3,]);

					async Task Act()
						=> await That(subject).Exactly(2).AreUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for exactly 2 items,
						             but only 1 of 5 were

						             Collection:
						             [1, 1, 2, 2, 3]
						             """);
				}
			}

			public sealed class AreNotUniqueTests
			{
				[Test]
				public async Task WhenExactlyTheExpectedNumberOfItemsIsNotUnique_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3,]);

					async Task Act()
						=> await That(subject).Exactly(2).AreNotUnique();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTooManyItemsAreNotUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1, 2, 3,]);

					async Task Act()
						=> await That(subject).Exactly(2).AreNotUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not unique for exactly 2 items,
						             but at least 3 of at least 3 were

						             Collection:
						             [1, 1, 1, (… and maybe more)]
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenExactlyTheExpectedNumberOfItemsIsUnique_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Exactly(2).AreUnique());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is unique for not exactly 2 items,
						             but 2 of 4 were

						             Collection:
						             [1, 1, 2, 3]
						             """);
				}

				[Test]
				public async Task WhenTooFewItemsAreUnique_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 2, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.Exactly(2).AreUnique());

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
