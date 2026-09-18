using System.Collections;
using System.Threading;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class ComplyWith
		{
			public sealed class EnumerableTests
			{
				[Fact]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IEnumerable subject = GetCancellingEnumerable(5, cts);

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.Satisfies(y => (int?)y < 6))
							.WithCancellation(token);

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that subject
						             satisfies y => (int?)y < 6 for all items,
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
						=> await That(subject).All().ComplyWith(x => x.Satisfies(_ => true))
							.And.All().ComplyWith(x => x.Satisfies(_ => true));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task DoesNotMaterializeEnumerable()
				{
					IEnumerable subject = Factory.GetFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(1));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for all items,
						             but not all were

						             Not matching items:
						             [2, (… and maybe others)]

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
				public async Task WhenEnumerableContainsDifferentValues_ShouldFail()
				{
					IEnumerable subject = new[]
					{
						1, 1, 1, 1, 2, 2, 3,
					};

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(1));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for all items,
						             but not all were

						             Not matching items:
						             [2, (… and maybe others)]

						             Collection:
						             [1, 1, 1, 1, 2, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldSucceed()
				{
					IEnumerable subject = ToEnumerable((int[]) []);

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(0));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsEqualValues_ShouldSucceed()
				{
					IEnumerable subject = ToEnumerable([1, 1, 1, 1, 1, 1, 1,]);

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(1));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable? subject = null;

					async Task Act()
						=> await That(subject)!.All().ComplyWith(x => x.IsEqualTo(0));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 0 for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class EnumerableNegatedTests
			{
				[Fact]
				public async Task WhenEnumerableOnlyContainsEqualValues_ShouldFail()
				{
					IEnumerable subject = ToEnumerable([1, 1, 1, 1, 1, 1, 1,]);

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.DoesNotComplyWith(it => it.IsEqualTo(1)));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to 1 for all items,
						             but not all were

						             Not matching items:
						             [1, (… and maybe others)]

						             Collection:
						             [1, 1, 1, 1, 1, 1, 1]
						             """);
				}

				[Fact]
				public async Task WhenAllItemsComply_ShouldFail()
				{
					IEnumerable subject = ToEnumerable([1, 1, 1,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it
							=> it.All().ComplyWith(x => x.IsEqualTo(1)));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for not all items,
						             but all 3 were

						             Collection:
						             [1, 1, 1]
						             """);
				}
			}
		}
	}
}
