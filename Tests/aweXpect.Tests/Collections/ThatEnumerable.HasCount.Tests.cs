using System.Collections;
using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasCount
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
					=> await That(subject).HasCount(6)
						.WithCancellation(token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 6 items,
					             but it could not be verified, because the evaluation was already canceled

					             Collection:
					             [0, 1, 2, 3, 4, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task WhenArrayContainsMatchingItems_ShouldSucceed()
			{
				int[] subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount(3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenArrayContainsTooFewItems_ShouldFail()
			{
				int[] subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount(4);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 4 items,
					             but it had only 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenArrayContainsTooManyItems_ShouldFail()
			{
				int[] subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
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
					=> await That(subject).HasCount(3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsTooFewItems_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).HasCount(4);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 4 items,
					             but it had only 3 items

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsTooManyItems_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but it had at least 3 items

					             Collection:
					             [1, 2, 3, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task WhenExpectedIsNull_ForArray_ShouldFail()
			{
				int[] subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly <null> items,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """)
					.Because("a count is never null");
			}

			[Test]
			public async Task WhenExpectedIsNull_ForEnumerable_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).HasCount(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly <null> items,
					             but it had at least 1 item

					             Collection:
					             [1, (… and maybe more)]
					             """)
					.Because("a count is never null, so the enumeration can stop at the first item");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).HasCount(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUntypedSetContainsTooManyItems_ShouldFail()
			{
				IEnumerable subject = new HashSet<int>
				{
					1,
					2,
					3,
				};

				async Task Act()
					=> await That(subject).HasCount(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has exactly one item,
					             but it had 3 items

					             Collection:
					             [1, 2, 3]
					             """)
					.Because("a set knows its count, also when it is read as a non-generic enumerable");
			}
		}
	}
}
