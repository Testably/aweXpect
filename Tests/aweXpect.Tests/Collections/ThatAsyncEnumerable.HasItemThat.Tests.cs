#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class HasItemThat
	{
		public sealed class Tests
		{
			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(5));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(1)).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is equal to 1 at index 2,
					             but it had item 2 at index 2

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(2)).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(3)).AtIndex(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is equal to 3 at index 3,
					             but it did not contain any item at index 3

					             Collection:
					             [0, 1, 2]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotEqualTo(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is not equal to 0,
					             but it did not contain any item

					             Collection:
					             []
					             """);
			}

			[Fact]
			public async Task WhenItemsUseWhose_ShouldIncludeMemberInExpectation()
			{
				IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable(new MyClass(1));

				async Task Act()
					=> await That(subject).HasItemThat(it => it.Whose(o => o.Value, v => v.IsEqualTo(5)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that has Value which is equal to 5,
					             but it did not match at any index

					             Collection:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               }
					             ]
					             """)
					.Because("the member text must survive the node tree rendering");
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotEqualTo(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is not equal to 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotEqualTo(0)).AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is not equal to 0 at index 0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WithInvalidMatch_ShouldNotMatch()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2, 3, 4);

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(3)).WithInvalidMatch();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is equal to 3 with invalid match,
					             but it did not contain any item with invalid match

					             Collection:
					             [0, 1, 2, 3, 4]
					             """);
			}

			[Fact]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);

				async Task Act()
					=> await That(subject).HasItemThat(x => x.StartsWith("a").And.EndsWith("b")).AtIndex(0).And
						.HasItemThat(x => x.Contains("c").IgnoringCase()).AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that starts with "a" and ends with "b" at index 0 and has item that contains "c" at least once ignoring case at index 1,
					             but it had item "a" at index 0 and it had item "b" at index 1

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}
		}

		public sealed class FromEndTests
		{
			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed(int expected)
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, expected, 3, 4);

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(expected - 1)).AtIndex(2).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has item that is equal to {expected - 1} at index 2 from end,
					              but it had item {expected} at index 2 from end

					              Collection:
					              [0, 1, {expected}, 3, 4]
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed(int expected)
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, expected, 3, 4);

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(expected)).AtIndex(2).FromEnd();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail(int expected)
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(expected, 3, 4);

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(expected)).AtIndex(3).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has item that is equal to {expected} at index 3 from end,
					              but it did not contain any item at index 3 from end

					              Collection:
					              [{expected}, 3, 4]
					              """);
			}

			[Theory]
			[AutoData]
			public async Task WhenEnumerableIsEmpty_ShouldFail(int expected)
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(expected)).AtIndex(0).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has item that is equal to {expected} at index 0 from end,
					              but it did not contain any item at index 0 from end

					              Collection:
					              []
					              """);
			}

			[Theory]
			[InlineData(-1)]
			[InlineData(-10)]
			public async Task WhenIndexIsNegative_ShouldThrowArgumentOutOfRangeException(int index)
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(0)).AtIndex(index).FromEnd();

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("index").And
					.WithMessage("The index must be greater than or equal to 0.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				int expected = 42;
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject!).HasItemThat(it => it.IsEqualTo(expected)).AtIndex(0).FromEnd();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is equal to 42 at index 0 from end,
					             but it was <null>
					             """);
			}
		}
	}
}
#endif
