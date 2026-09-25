using System.Collections.Generic;
using System.Threading;

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
				[Fact]
				public async Task ConsidersCancellationToken()
				{
					using CancellationTokenSource cts = new();
					CancellationToken token = cts.Token;
					IEnumerable<int> subject = GetCancellingEnumerable(5, cts);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x < 6).WithCancellation(token);

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x < 6 for all items,
						             but it could not be verified, because it was already canceled

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
						               (… and maybe more)
						             ]
						             """);
				}

				[Fact]
				public async Task DoesNotEnumerateTwice()
				{
					ThrowWhenIteratingTwiceEnumerable subject = new();

					async Task Act()
						=> await That(subject).All().Satisfy(_ => true)
							.And.All().Satisfy(_ => true);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task DoesNotMaterializeEnumerable()
				{
					IEnumerable<int> subject = Factory.GetFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == 1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == 1 for all items,
						             but only 2 of at least 3 did

						             Not matching items:
						             [2, (… and maybe more)]

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
						               (… and maybe more)
						             ]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsDifferentValues_ShouldFail()
				{
					int[] subject = [1, 1, 1, 1, 2, 2, 3,];

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == 1);

					await That(Act).Throws<XunitException>()
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

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable((int[]) []);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == 0);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsEqualValues_ShouldSucceed()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1, 1, 1, 1, 1,]);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == 1);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumeratingTheSubjectThrows_ShouldFailWithTheExceptionAsInnerException()
				{
					InvalidOperationException exception = new("enumeration failed");
					IEnumerable<int> subject = ThrowAfter(exception, 1, 2);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x > 0);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x > 0 for all items,
						             but it did throw an InvalidOperationException:
						               enumeration failed
						             """).And
						.Whose(e => e.InnerException, i => i.IsSameAs(exception))
						.Because("a subject that cannot be enumerated fails the expectation instead of aborting its evaluation");
				}

				[Fact]
				public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
				{
					IEnumerable<int> subject = Factory.GetFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().Satisfy(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("predicate").And
						.WithMessage("The 'predicate' cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenPredicateThrowsArgumentOutOfRangeException_ShouldFailWithTheExceptionAsInnerException()
				{
					List<int> values = [1, 2,];
					int[] subject = [0, 1, 2,];

					async Task Act()
						=> await That(subject).All().Satisfy(i => values[i] > 0);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies i => values[i] > 0 for all items,
						             but the predicate did throw an ArgumentOutOfRangeException:
						               *
						             """).AsWildcard().And
						.Whose(e => e.InnerException, i => i.Is<ArgumentOutOfRangeException>())
						.Because("an argument exception from the predicate is not a validation by aweXpect");
				}

				[Fact]
				public async Task WhenPredicateThrows_ShouldFailWithTheExceptionAsInnerException()
				{
					InvalidOperationException exception = new("predicate failed");
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).All().Satisfy(x => x < 3 ? true : throw exception);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x < 3 ? true : throw exception for all items,
						             but the predicate did throw an InvalidOperationException:
						               predicate failed
						             """).And
						.Whose(e => e.InnerException, i => i.IsSameAs(exception))
						.Because("a predicate that throws fails the expectation instead of aborting its evaluation");
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == 0);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == 0 for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class StringTests
			{
				[Fact]
				public async Task WhenEnumerableContainsDifferentValues_ShouldFail()
				{
					string[] subject = ["foo", "bar", "baz",];

					async Task Act()
						=> await That(subject).All().Satisfy(x => x?.StartsWith("ba") == true);

					await That(Act).Throws<XunitException>()
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

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldSucceed()
				{
					IEnumerable<string> subject = ToEnumerable((string[]) []);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == "");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsMatchingValues_ShouldSucceed()
				{
					IEnumerable<string> subject = ToEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x?.Length == 3);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
				{
					string[] subject = ["foo", "bar", "baz",];

					async Task Act()
						=> await That(subject).All().Satisfy(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("predicate").And
						.WithMessage("The 'predicate' cannot be null.").AsPrefix();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == "");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == "" for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedItemTests
			{
				[Fact]
				public async Task WhenEnumerableContainsDifferentValues_ShouldSucceed()
				{
					int[] subject = [1, 1, 1, 1, 2, 2, 3,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == 1));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable((int[]) []);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == 0));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == 0 not for all items,
						             but it was empty

						             Collection:
						             []
						             """);
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsEqualValues_ShouldFail()
				{
					IEnumerable<int> subject = ToEnumerable([1, 1, 1, 1, 1, 1, 1,]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == 1));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == 1 not for all items,
						             but all 7 did

						             Collection:
						             [1, 1, 1, 1, 1, 1, 1]
						             """);
				}

				[Fact]
				public async Task WhenPredicateThrows_ShouldFailWithTheExceptionAsInnerException()
				{
					InvalidOperationException exception = new("predicate failed");
					int[] subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x < 3 ? true : throw exception));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x < 3 ? true : throw exception not for all items,
						             but the predicate did throw an InvalidOperationException:
						               predicate failed
						             """).And
						.Whose(e => e.InnerException, i => i.IsSameAs(exception))
						.Because("a predicate that threw answered nothing, so the negation fails as well");
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == 0));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == 0 not for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedStringTests
			{
				[Fact]
				public async Task WhenEnumerableContainsDifferentValues_ShouldSucceed()
				{
					string[] subject = ["foo", "bar", "baz",];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x?.StartsWith("ba") == true));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable((string[]) []);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == ""));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == "" not for all items,
						             but it was empty

						             Collection:
						             []
						             """);
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsMatchingValues_ShouldFail()
				{
					IEnumerable<string> subject = ToEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x?.Length == 3));

					await That(Act).Throws<XunitException>()
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

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == ""));

					await That(Act).Throws<XunitException>()
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
