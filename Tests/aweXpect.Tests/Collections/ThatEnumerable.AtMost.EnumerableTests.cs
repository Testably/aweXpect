using System.Collections;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class AtMost
	{
		public sealed class EnumerableTests
		{
			[Test]
			public async Task ConsidersCancellationToken()
			{
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;
				IEnumerable subject = GetCancellingEnumerable(6, cts);

				async Task Act()
					=> await That(subject).AtMost(8).Satisfy(y => (int?)y < 6)
						.WithCancellation(token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that subject
					             satisfies y => (int?)y < 6 for at most 8 items,
					             but it could not be verified, because the evaluation was already canceled

					             Collection:
					             [0, 1, 2, 3, 4, 5, 6, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).AtMost(3).AreEqualTo(1)
						.And.AtMost(3).AreEqualTo(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).AtMost(1).AreEqualTo(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for at most one item,
					             but at least 2 of at least 2 were

					             Matching items:
					             [1, 1, (… and maybe more)]

					             Collection:
					             [1, 1, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task WhenArrayContainsSufficientlyFewEqualItems_ShouldSucceed()
			{
				IEnumerable subject = new[]
				{
					1, 1, 1, 1, 2, 2, 3,
				};

				async Task Act()
					=> await That(subject).AtMost(3).AreEqualTo(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenArrayContainsTooManyEqualItems_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					1, 1, 1, 1, 2, 2, 3,
				};

				async Task Act()
					=> await That(subject).AtMost(3).AreEqualTo(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for at most 3 items,
					             but 4 of 7 were

					             Matching items:
					             [1, 1, 1, 1]

					             Collection:
					             [1, 1, 1, 1, 2, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsSufficientlyFewEqualItems_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1, 1, 1, 1, 2, 2, 3,]);

				async Task Act()
					=> await That(subject).AtMost(3).AreEqualTo(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsTooManyEqualItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 1, 1, 1, 2, 2, 3,]);

				async Task Act()
					=> await That(subject).AtMost(3).AreEqualTo(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for at most 3 items,
					             but at least 4 of at least 4 were

					             Matching items:
					             [1, 1, 1, 1, (… and maybe more)]

					             Collection:
					             [1, 1, 1, 1, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject)!.AtMost(1).AreEqualTo(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 0 for at most one item,
					             but it was <null>
					             """);
			}
		}
	}
}
