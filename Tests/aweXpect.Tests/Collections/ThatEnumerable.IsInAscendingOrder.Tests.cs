using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsInAscendingOrder
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenComparerIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder().Using(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("comparer").And
					.WithMessage("The 'comparer' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenComparerIsSpecifiedTwice_ShouldThrowInvalidOperationException()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);
				CollectionOrderResult<int, IEnumerable<int>, IThat<IEnumerable<int>?>> sut =
					That(subject).IsInAscendingOrder();
				_ = sut.Using(Comparer<int>.Default);

				void Act() => sut.Using(Comparer<int>.Default);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Using cannot be specified more than once.");
			}

			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3, 1,]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had 3 before 1, which is not in ascending order

					             Collection:
					             [1, 1, 2, 3, 1, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenComparerIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder().Using(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("comparer").And
					.WithMessage("The 'comparer' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3, 1,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInAscendingOrder());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInAscendingOrder());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order,
					             but it was

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}

		public sealed class StringTests
		{
			[Test]
			public async Task ShouldNotIgnoreCasing()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "A",]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had "a" before "A", which is not in ascending order

					             Collection:
					             [
					               "a",
					               "A",
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task ShouldUseCustomComparer()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "A",]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder().Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenComparerThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("comparer failed");
				IEnumerable<string> subject = ToEnumerable(["a", "b",]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder().Using(new ThrowingComparer(exception));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order using ThatEnumerable.IsInAscendingOrder.ThrowingComparer,
					             but the comparer did throw an InvalidOperationException:
					               comparer failed

					             Collection:
					             [
					               "a",
					               "b",
					               (… and maybe more)
					             ]
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a comparer that throws fails the expectation instead of aborting its evaluation");
			}

			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "a",]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had "c" before "a", which is not in ascending order

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "a",
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemsAreTypedAsObjects_ShouldCompareThemOrdinally()
			{
				IEnumerable<object> subject = ToEnumerable<object>("a", "B");

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had "a" before "B", which is not in ascending order

					             Collection:
					             [
					               "a",
					               "B",
					               (… and maybe more)
					             ]
					             """)
					.Because("strings are ordered ordinally, whatever the item type of the collection");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it was <null>
					             """);
			}
		}

		public sealed class MemberTests
		{
			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 1, 2, 3, 1,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order by x => x.Value,
					             but it had 3 before 1, which is not in ascending order

					             Collection:
					             [
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 2
					               },
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 3
					               },
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 1
					               },
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMemberAccessorIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder((Func<MyIntClass, int>)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("memberAccessor").And
					.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenMemberSelectorThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("selector failed");
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value == 2 ? throw exception : x.Value);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order by x => x.Value == 2 ? throw exception : x.Value,
					             but the member selector did throw an InvalidOperationException:
					               selector failed

					             Collection:
					             [
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 2
					               },
					               (… and maybe more)
					             ]
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a member selector that throws fails the expectation instead of aborting its evaluation");
			}
		}

		public sealed class NegatedMemberTests
		{
			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 1, 2, 3, 1,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInAscendingOrder(x => x.Value));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInAscendingOrder(x => x.Value));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order by x => x.Value,
					             but it was

					             Collection:
					             [
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 2
					               },
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 3
					               }
					             ]
					             """);
			}

			[Test]
			public async Task WhenMemberAccessorIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsInAscendingOrder((Func<MyIntClass, int>)null!));

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("memberAccessor").And
					.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenMemberSelectorThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("selector failed");
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsInAscendingOrder(x => x.Value == 2 ? throw exception : x.Value));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order by x => x.Value == 2 ? throw exception : x.Value,
					             but the member selector did throw an InvalidOperationException:
					               selector failed

					             Collection:
					             [
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 1
					               },
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 2
					               },
					               (… and maybe more)
					             ]
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a member selector that threw answered nothing, so the negation fails as well");
			}
		}

		public sealed class StringMemberTests
		{
			[Test]
			public async Task ShouldNotIgnoreCasing()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["a", "A",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order by x => x.Value,
					             but it had "a" before "A", which is not in ascending order

					             Collection:
					             [
					               ThatEnumerable.IsInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatEnumerable.IsInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "A"
					               },
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task ShouldUseCustomComparer()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["a", "A",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value)
						.Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["a", "b", "c", "a",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order by x => x.Value,
					             but it had "c" before "a", which is not in ascending order

					             Collection:
					             [
					               ThatEnumerable.IsInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               ThatEnumerable.IsInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "b"
					               },
					               ThatEnumerable.IsInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "c"
					               },
					               ThatEnumerable.IsInAscendingOrder.StringMemberTests.MyStringClass {
					                 Value = "a"
					               },
					               (… and maybe more)
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["a", "b", "c",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			private sealed class MyStringClass(string value)
			{
				public string Value { get; } = value;
			}
		}

		private sealed class MyIntClass(int value)
		{
			public int Value { get; } = value;
		}

		private sealed class ThrowingComparer(Exception exception) : IComparer<string>
		{
			public int Compare(string? x, string? y) => throw exception;
		}
	}
}
