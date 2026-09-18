using System.Collections.Generic;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class None
	{
		public sealed class Satisfy
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IEnumerable<int> subject = GetCancellingEnumerable(6, cts);

					async Task Act()
						=> await That(subject).None().Satisfy(item => item < 0)
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that subject
						             satisfies item => item < 0 for no items,
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
					ThrowWhenIteratingTwiceEnumerable subject = new();

					async Task Act()
						=> await That(subject).None().Satisfy(item => item == 15)
							.And.None().Satisfy(item => item == 81);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task DoesNotMaterializeEnumerable()
				{
					IEnumerable<int> subject = Factory.GetFibonacciNumbers();

					async Task Act()
						=> await That(subject).None().Satisfy(item => item == 5);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies item => item == 5 for no items,
						             but at least 1 of at least 5 did

						             Matching items:
						             [5, (… and maybe others)]

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
				public async Task WhenEnumerableContainsEqualValues_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1, 1, 2, 2, 3,]);

					async Task Act()
						=> await That(subject).None().Satisfy(item => item == 1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies item => item == 1 for no items,
						             but at least 1 of at least 1 did

						             Matching items:
						             [1, (… and maybe others)]

						             Collection:
						             [1, 1, 1, 1, 2, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable((int[]) []);

					async Task Act()
						=> await That(subject).None().Satisfy(item => item == 0);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsDifferentValues_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1, 1, 2, 2, 3,]);

					async Task Act()
						=> await That(subject).None().Satisfy(item => item == 42);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).None().Satisfy(item => item == 0);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies item => item == 0 for no items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Fact]
				public async Task WhenNoItemSatisfiesThePredicate_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.None().Satisfy(x => x > 5));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x > 5 for at least one item,
						             *
						             """).AsWildcard();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.None().Satisfy(x => x > 5));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x > 5 for at least one item,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenOneItemSatisfiesThePredicate_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 6, 3,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.None().Satisfy(x => x > 5));

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
