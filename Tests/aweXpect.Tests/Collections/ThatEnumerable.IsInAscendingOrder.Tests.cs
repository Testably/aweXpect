using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsInAscendingOrder
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenComparerIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder().Using(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("comparer").And
					.WithMessage("The 'comparer' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3, 1,]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had 3 before 1, which is not in ascending order

					             Collection:
					             [
					               1,
					               1,
					               2,
					               3,
					               1
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenComparerIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder().Using(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("comparer").And
					.WithMessage("The 'comparer' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 1, 2, 3, 1,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInAscendingOrder());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInAscendingOrder());

				await That(Act).Throws<XunitException>()
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
			[Fact]
			public async Task ShouldNotIgnoreCasing()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "A",]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had "a" before "A", which is not in ascending order

					             Collection:
					             [
					               "a",
					               "A"
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "A",]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder().Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenComparerThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("comparer failed");
				IEnumerable<string> subject = ToEnumerable(["a", "b",]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder().Using(new ThrowingComparer(exception));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order using ThatEnumerable.IsInAscendingOrder.ThrowingComparer,
					             but the comparer did throw an InvalidOperationException:
					               comparer failed

					             Collection:
					             [
					               "a",
					               "b"
					             ]
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a comparer that throws fails the expectation instead of aborting its evaluation");
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c", "a",]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had "c" before "a", which is not in ascending order

					             Collection:
					             [
					               "a",
					               "b",
					               "c",
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it was <null>
					             """);
			}
		}

		public sealed class MemberTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 1, 2, 3, 1,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).Throws<XunitException>()
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
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMemberAccessorIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder((Func<MyIntClass, int>)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("memberAccessor").And
					.WithMessage("The 'memberAccessor' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenMemberSelectorThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("selector failed");
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value == 2 ? throw exception : x.Value);

				await That(Act).Throws<XunitException>()
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
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 3
					               }
					             ]
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a member selector that throws fails the expectation instead of aborting its evaluation");
			}
		}

		public sealed class NegatedMemberTests
		{
			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldSucceed()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 1, 2, 3, 1,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInAscendingOrder(x => x.Value));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreSortedCorrectly_ShouldFail()
			{
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsInAscendingOrder(x => x.Value));

				await That(Act).Throws<XunitException>()
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

			[Fact]
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

			[Fact]
			public async Task WhenMemberSelectorThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("selector failed");
				IEnumerable<MyIntClass> subject = ToEnumerable([1, 2, 3,], x => new MyIntClass(x));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.IsInAscendingOrder(x => x.Value == 2 ? throw exception : x.Value));

				await That(Act).Throws<XunitException>()
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
					               ThatEnumerable.IsInAscendingOrder.MyIntClass {
					                 Value = 3
					               }
					             ]
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a member selector that threw answered nothing, so the negation fails as well");
			}
		}

		public sealed class StringMemberTests
		{
			[Fact]
			public async Task ShouldNotIgnoreCasing()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["a", "A",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).Throws<XunitException>()
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
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldUseCustomComparer()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["a", "A",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value)
						.Using(StringComparer.OrdinalIgnoreCase);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenItemsAreNotSortedCorrectly_ShouldFail()
			{
				IEnumerable<MyStringClass> subject = ToEnumerable(["a", "b", "c", "a",], x => new MyStringClass(x));

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x.Value);

				await That(Act).Throws<XunitException>()
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
					               }
					             ]
					             """);
			}

			[Fact]
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
