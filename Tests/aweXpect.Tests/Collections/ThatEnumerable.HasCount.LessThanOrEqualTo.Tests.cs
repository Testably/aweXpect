using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasCount
	{
		public sealed class LessThanOrEqualTo
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
						=> await That(subject).HasCount().LessThanOrEqualTo(6)
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that subject
						             has at most 6 items,
						             but it could not be verified, because the evaluation was already canceled

						             Collection:
						             [0, 1, 2, 3, 4, (… and maybe more)]
						             """);
				}

				[Fact]
				public async Task WhenAnEarlierAttemptHadAnotherSubject_ShouldNotReuseItsTotalCount()
				{
					int calls = 0;
					Func<IEnumerable<int>> subject = () => ++calls == 1 ? [1, 2, 3, 4, 5,] : Lazy(4);

					async Task Act()
						=> await That(subject).Eventually().Within(200.Milliseconds()).CheckEvery(10.Milliseconds())
							.HasCount().LessThanOrEqualTo(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             eventually has at most 2 items within 0:00.200,
						             but it had at least 3 items

						             Collection:
						             [1, 2, 3, (… and maybe more)]
						             """);

					static IEnumerable<int> Lazy(int count)
					{
						for (int i = 1; i <= count; i++)
						{
							yield return i;
						}
					}
				}

				[Fact]
				public async Task WhenArrayContainsMatchingItems_ShouldSucceed()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(3);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenArrayContainsTooFewItems_ShouldSucceed()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(4);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenArrayContainsTooManyItems_ShouldFail()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has at most 2 items,
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
						=> await That(subject).HasCount().LessThanOrEqualTo(3);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsTooFewItems_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(4);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsTooManyItems_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has at most 2 items,
						             but it had at least 3 items

						             Collection:
						             [1, 2, 3, (… and maybe more)]
						             """);
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(null);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has at most <null> items,
						             but it had 3 items

						             Collection:
						             [1, 2, 3]
						             """)
						.Because("nothing can be ordered against null");
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has at most 2 items,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WithNamedArgument_ShouldSucceed()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().LessThanOrEqualTo(expected: 3);

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
							it.HasCount().LessThanOrEqualTo(3));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have at most 3 items,
						             but it had 3 items

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
							it.HasCount().LessThanOrEqualTo(2));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsMatchingItems_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().LessThanOrEqualTo(3));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have at most 3 items,
						             but it had 3 items

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
							it.HasCount().LessThanOrEqualTo(2));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().LessThanOrEqualTo(null));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have at most <null> items,
						             but it had 3 items

						             Collection:
						             [1, 2, 3]
						             """)
						.Because("nothing can be ordered against null, so the negation fails as well");
				}
			}
		}
	}
}
