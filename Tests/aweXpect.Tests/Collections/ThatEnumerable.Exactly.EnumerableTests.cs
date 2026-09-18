using System.Collections;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Exactly
	{
		public sealed class EnumerableTests
		{
			[Fact]
			public async Task ConsidersCancellationToken()
			{
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;
				IEnumerable subject =
					GetCancellingEnumerable(6, cts);

				async Task Act()
					=> await That(subject).Exactly(6).Satisfy(y => (int?)y < 6)
						.WithCancellation(token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that subject
					             satisfies y => (int?)y < 6 for exactly 6 items,
					             but it could not be verified, because it was already cancelled

					             Collection:
					             [
					               0,
					               1,
					               2,
					               3,
					               4,
					               5,
					               6,
					               7,
					               8,
					               9,
					               (… and maybe others)
					             ]
					             """);
			}

			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).Exactly(1).AreEqualTo(1)
						.And.Exactly(1).AreEqualTo(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).Exactly(1).AreEqualTo(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for exactly one item,
					             but at least 2 of at least 2 were

					             Collection:
					             [
					               1,
					               1,
					               2,
					               3,
					               5,
					               8,
					               13,
					               21,
					               34,
					               55,
					               (… and maybe others)
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedNumberOfEqualItems_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1, 1, 1, 1, 2, 2, 3,]);

				async Task Act()
					=> await That(subject).Exactly(4).AreEqualTo(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsTooFewEqualItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 1, 1, 1, 2, 2, 3,]);

				async Task Act()
					=> await That(subject).Exactly(4).AreEqualTo(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2 for exactly 4 items,
					             but only 2 of 7 were

					             Collection:
					             [1, 1, 1, 1, 2, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsTooManyEqualItems_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 1, 1, 1, 2, 2, 3,]);

				async Task Act()
					=> await That(subject).Exactly(3).AreEqualTo(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for exactly 3 items,
					             but at least 4 of at least 4 were

					             Collection:
					             [1, 1, 1, 1, 2, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject)!.Exactly(1).AreEqualTo(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 0 for exactly one item,
					             but it was <null>
					             """);
			}
		}
	}
}
