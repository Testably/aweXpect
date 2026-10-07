#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class All
	{
		public sealed class Satisfy
		{
			public sealed class ItemsTests
			{
				[Test]
				public async Task DoesNotEnumerateTwice()
				{
					ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

					async Task Act()
						=> await That(subject).All().Satisfy(x => x > 0)
							.And.All().Satisfy(x => x < 2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task DoesNotMaterializeAsyncEnumerable()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().Satisfy(x => x <= 1);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x <= 1 for all items,
						             but only 2 of at least 3 did

						             Not matching items:
						             [2, (… and maybe more)]

						             Collection:
						             [1, 1, 2, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x is > 4 and < 6);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x is > 4 and < 6 for all items,
						             but none of at least 1 did

						             Not matching items:
						             [1, (… and maybe more)]

						             Collection:
						             [1, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					int constantValue = 42;
					IAsyncEnumerable<int> subject = Factory.GetConstantValueAsyncEnumerable(constantValue, 20);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == constantValue);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().Satisfy(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("predicate").And
						.WithMessage("The 'predicate' cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenSubjectIsNull_AfterAnEarlierAttempt_ShouldNotShowItsItems()
				{
					int calls = 0;
					Func<IAsyncEnumerable<int>?> subject = () => calls++ == 0 ? ToAsyncEnumerable(1, 2) : null;

					async Task Act()
						=> await That(subject).Eventually().WithinTwoAttempts(5.Seconds())
							.All().Satisfy(x => x > 5).WithTimeSystem(new VirtualTimeSystem());

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
					IAsyncEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).All().Satisfy(_ => true);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies _ => true for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class StringTests
			{
				[Test]
				public async Task WhenEnumerableContainsDifferentValues_ShouldFail()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x?.StartsWith("ba") == true);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x?.StartsWith("ba") == true for all items,
						             but none of at least 1 did

						             Not matching items:
						             [
						               "foo",
						               (… and maybe more)
						             ]

						             Collection:
						             [
						               "foo",
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenEnumerableIsEmpty_ShouldSucceed()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable((string[])[]);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == "");

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableOnlyContainsMatchingValues_ShouldSucceed()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).All().Satisfy(x => x?.Length == 3);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).All().Satisfy(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("predicate").And
						.WithMessage("The 'predicate' cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<string>? subject = null;

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

			public sealed class NegatedItemsTests
			{
				[Test]
				public async Task WhenItemsDiffer_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x is > 4 and < 6));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenNoItemsDiffer_ShouldFail()
				{
					int constantValue = 42;
					IAsyncEnumerable<int> subject = Factory.GetConstantValueAsyncEnumerable(constantValue, 20);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x == constantValue));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == constantValue not for all items,
						             but all 20 did

						             Collection:
						             [
						               42,
						               42,
						               42,
						               42,
						               42,
						               42,
						               42,
						               42,
						               42,
						               42,
						               (… and 10 more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(null!));

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("predicate").And
						.WithMessage("The 'predicate' cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<string>? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(_ => true));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             satisfies _ => true not for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedStringTests
			{
				[Test]
				public async Task WhenEnumerableContainsDifferentValues_ShouldSucceed()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(x => x?.StartsWith("ba") == true));

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable((string[])[]);

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
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

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
				public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
				{
					IAsyncEnumerable<string> subject = ToAsyncEnumerable(["foo", "bar", "baz",]);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it =>
							it.All().Satisfy(null!));

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("predicate").And
						.WithMessage("The 'predicate' cannot be null.").AsPrefix();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<string>? subject = null;

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
#endif
