using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasCount
	{
		public sealed class NotGreaterThan
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenArrayContainsMatchingItems_ShouldSucceed()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().NotGreaterThan(3);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenArrayContainsTooFewItems_ShouldSucceed()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().NotGreaterThan(4);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenArrayContainsTooManyItems_ShouldFail()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().NotGreaterThan(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have more than 2 items,
						             but it had 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsMatchingItems_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).HasCount().NotGreaterThan(3);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsTooFewItems_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).HasCount().NotGreaterThan(4);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsTooManyItems_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).HasCount().NotGreaterThan(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have more than 2 items,
						             but it had at least 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).HasCount().NotGreaterThan(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have more than 2 items,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WithNamedArgument_ShouldSucceed()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().NotGreaterThan(expected: 3);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenArrayContainsMatchingItems_ShouldFail()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().NotGreaterThan(3));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has more than 3 items,
						             but it had only 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenArrayContainsTooManyItems_ShouldSucceed()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().NotGreaterThan(2));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsMatchingItems_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().NotGreaterThan(3));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has more than 3 items,
						             but it had only 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsTooManyItems_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().NotGreaterThan(2));

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
