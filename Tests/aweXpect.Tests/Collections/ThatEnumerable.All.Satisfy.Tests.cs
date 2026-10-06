using System.Collections.Generic;
using System.Threading;
using aweXpect.Customization;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class Satisfy
		{
			public sealed class ItemTests
			{
				[Test]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x < 6).WithCancellation(token);

					await That(Act).Throws<InconclusiveTestException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x < 6 for all items,
						             but it could not be verified, because the evaluation was already canceled

						             Collection:
						             [0, 1, 2, 3, 4, 5, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task DoesNotEnumerateTwice()
				{
					ThrowWhenIteratingTwiceEnumerable subject = new();

					async Task Act()
						=> await That(subject).All().Satisfy(_ => true)
							.And.All().Satisfy(_ => true);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task DoesNotMaterializeEnumerable()
				{
					IEnumerable<int> subject = Factory.GetFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == 1);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == 1 for all items,
						             but only 2 of at least 3 did

						             Not matching items:
						             [2, (… and maybe more)]

						             Collection:
						             [1, 1, 2, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsDifferentValues_ShouldFail()
				{
					int[] subject = [1, 1, 1, 1, 2, 2, 3,];

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == 1);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == 1 for all items,
						             but only 4 of 7 did

						             Not matching items:
						             [2, 2, 3]

						             Collection:
						             [1, 1, 1, 1, 2, 2, 3]
						             """);
				}

				[Test]
				public async Task WhenEnumerableIsEmpty_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable((int[]) []);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == 0);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableOnlyContainsEqualValues_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1, 1, 1, 1, 1,]);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == 1);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumeratingTheSubjectThrows_ShouldFailWithTheExceptionAsInnerException()
				{
					InvalidOperationException exception = new("enumeration failed");
					IEnumerable<int> subject = ThrowAfter(exception, 1, 2);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x > 0);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x > 0 for all items,
						             but it did throw an InvalidOperationException:
						               enumeration failed
						             """).And
						.Whose(e => e.InnerException, i => i.IsSameAs(exception))
						.Because("a subject that cannot be enumerated fails the expectation instead of aborting its evaluation");
				}

				[Test]
				public async Task WhenMaximumNumberOfCollectionItemsIsIntMaxValue_ShouldFailNormally()
				{
					int[] subject = [1, 2, 3,];

					async Task Act()
					{
						using (Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(int.MaxValue))
						{
							await That(subject).All().Satisfy(x => x > 1);
						}
					}

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x > 1 for all items,
						             but only 2 of 3 did

						             Not matching items:
						             [1]

						             Collection:
						             [1, 2, 3]
						             """)
						.Because("int.MaxValue lists all items, so it must not overflow the limits derived from it");
				}

#if NET8_0_OR_GREATER
				[Test]
				public async Task WhenMaximumNumberOfCollectionItemsIsLarge_ShouldNotAllocateProportionally()
				{
					int[] subject = [1, 2, 3,];
					long allocated;

					using (Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Set(10_000_000))
					{
						long before = GC.GetTotalAllocatedBytes(true);
						await That(subject).All().Satisfy(x => x > 0);
						allocated = GC.GetTotalAllocatedBytes(true) - before;
					}

					await That(allocated).IsLessThan(16_000_000)
						.Because("the buffers grow with the items instead of being sized to the maximum, which took about 160 MB");
				}
#endif

				[Test]
				public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
				{
					IEnumerable<int> subject = Factory.GetFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().Satisfy(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("predicate").And
						.WithMessage("The 'predicate' cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenPredicateThrows_ShouldFailWithTheExceptionAsInnerException()
				{
					InvalidOperationException exception = new("predicate failed");
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).All().Satisfy(x => x < 3 ? true : throw exception);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x < 3 ? true : throw exception for all items,
						             but the predicate did throw an InvalidOperationException:
						               predicate failed
						             """).And
						.Whose(e => e.InnerException, i => i.IsSameAs(exception))
						.Because("a predicate that throws fails the expectation instead of aborting its evaluation");
				}

				[Test]
				public async Task WhenPredicateThrowsArgumentOutOfRangeException_ShouldFailWithTheExceptionAsInnerException()
				{
					List<int> values = [1, 2,];
					int[] subject = [0, 1, 2,];

					async Task Act()
						=> await That(subject).All().Satisfy(i => values[i] > 0);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies i => values[i] > 0 for all items,
						             but the predicate did throw an ArgumentOutOfRangeException:
						               *
						             """).AsWildcard().And
						.Whose(e => e.InnerException, i => i.Is<ArgumentOutOfRangeException>())
						.Because("an argument exception from the predicate is not a validation by aweXpect");
				}

				[Test]
				public async Task WhenSubjectIsNull_AfterAnEarlierAttempt_ShouldNotShowItsItems()
				{
					int calls = 0;
					Func<IEnumerable<int>?> subject = () => calls++ == 0 ? [1, 2,] : null;

					async Task Act()
						=> await That(subject).Eventually().WithinTwoAttempts(5.Seconds())
							.All().Satisfy(x => x > 5);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             eventually satisfies x => x > 5 for all items within 0:05,
						             but it was <null>
						             """)
						.Because("the items of an earlier attempt do not describe the last one");
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == 0);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == 0 for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class StringTests
			{
				[Test]
				public async Task WhenEnumerableContainsDifferentValues_ShouldFail()
				{
					string[] subject = ["foo", "bar", "baz",];

					async Task Act()
						=> await That(subject).All().Satisfy(x => x?.StartsWith("ba") == true);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x?.StartsWith("ba") == true for all items,
						             but only 2 of 3 did

						             Not matching items:
						             [
						               "foo"
						             ]

						             Collection:
						             [
						               "foo",
						               "bar",
						               "baz"
						             ]
						             """);
				}

				[Test]
				public async Task WhenEnumerableIsEmpty_ShouldSucceed()
				{
					IEnumerable<string> subject = ToEnumerable((string[]) []);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == "");

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableOnlyContainsMatchingValues_ShouldSucceed()
				{
					IEnumerable<string> subject = ToEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x?.Length == 3);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
				{
					string[] subject = ["foo", "bar", "baz",];

					async Task Act()
						=> await That(subject).All().Satisfy(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("predicate").And
						.WithMessage("The 'predicate' cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == "");

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == "" for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedItemTests
			{
				[Test]
				public async Task WhenEnumerableContainsDifferentValues_ShouldSucceed()
				{
					int[] subject = [1, 1, 1, 1, 2, 2, 3,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == 1));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable((int[]) []);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == 0));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == 0 not for all items,
						             but it was empty

						             Collection:
						             []
						             """);
				}

				[Test]
				public async Task WhenEnumerableOnlyContainsEqualValues_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1, 1, 1, 1, 1,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == 1));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == 1 not for all items,
						             but all 7 did

						             Collection:
						             [1, 1, 1, 1, 1, 1, 1]
						             """);
				}

				[Test]
				public async Task WhenPredicateThrows_ShouldFailWithTheExceptionAsInnerException()
				{
					InvalidOperationException exception = new("predicate failed");
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x < 3 ? true : throw exception));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x < 3 ? true : throw exception not for all items,
						             but the predicate did throw an InvalidOperationException:
						               predicate failed
						             """).And
						.Whose(e => e.InnerException, i => i.IsSameAs(exception))
						.Because("a predicate that threw answered nothing, so the negation fails as well");
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == 0));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == 0 not for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedStringTests
			{
				[Test]
				public async Task WhenEnumerableContainsDifferentValues_ShouldSucceed()
				{
					string[] subject = ["foo", "bar", "baz",];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x?.StartsWith("ba") == true));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable((string[]) []);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == ""));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == "" not for all items,
						             but it was empty

						             Collection:
						             []
						             """);
				}

				[Test]
				public async Task WhenEnumerableOnlyContainsMatchingValues_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x?.Length == 3));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x?.Length == 3 not for all items,
						             but all 3 did

						             Collection:
						             [
						               "foo",
						               "bar",
						               "baz"
						             ]
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == ""));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == "" not for all items,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
