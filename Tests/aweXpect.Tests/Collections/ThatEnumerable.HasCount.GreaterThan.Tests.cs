using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasCount
	{
		public sealed class GreaterThan
		{
			public sealed class Tests
			{
				[Test]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IEnumerable<int> subject = GetCancellingEnumerable(4, cts);

					async Task Act()
						=> await That(subject).HasCount().GreaterThan(6)
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveTestException>()
						.WithMessage("""
						             Expected that subject
						             has more than 6 items,
						             but it could not be verified, because the evaluation was already canceled

						             Collection:
						             [0, 1, 2, 3, 4, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenArrayContainsMatchingItems_ShouldFail()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().GreaterThan(3);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has more than 3 items,
						             but it had only 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Test]
				public async Task WhenArrayContainsTooFewItems_ShouldFail()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().GreaterThan(4);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has more than 4 items,
						             but it had only 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Test]
				public async Task WhenArrayContainsTooManyItems_ShouldSucceed()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().GreaterThan(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsMatchingItems_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).HasCount().GreaterThan(3);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has more than 3 items,
						             but it had only 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsTooFewItems_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).HasCount().GreaterThan(4);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has more than 4 items,
						             but it had only 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsTooManyItems_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).HasCount().GreaterThan(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().GreaterThan(null);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has more than <null> items,
						             but it had 3 items

						             Collection:
						             [1, 2, 3]
						             """)
						.Because("nothing can be ordered against null");
				}

				[Test]
				public async Task WhenSubjectIsNull_AndExpectedIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).HasCount().GreaterThan(null);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has more than <null> items,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).HasCount().GreaterThan(2);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has more than 2 items,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WithNamedArgument_ShouldSucceed()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasCount().GreaterThan(2);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenArrayContainsMatchingItems_ShouldSucceed()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().GreaterThan(3));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenArrayContainsTooFewItems_ShouldSucceed()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().GreaterThan(4));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenArrayContainsTooManyItems_ShouldFail()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().GreaterThan(2));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have more than 2 items,
						             but it had 3 items

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsMatchingItems_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().GreaterThan(3));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsTooFewItems_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().GreaterThan(4));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsTooManyItems_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().GreaterThan(2));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have more than 2 items,
						             but it had at least 3 items

						             Collection:
						             [1, 2, 3, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.HasCount().GreaterThan(null));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             does not have more than <null> items,
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
