#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class HasSingle
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldReturnSingleItem()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(42);

				int result = await That(subject).HasSingle();

				await That(result).IsEqualTo(42);
			}

			[Fact]
			public async Task WhenAsyncEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).HasSingle();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item,
					             but it had more than one item

					             Collection:
					             [1, 2, (… and maybe more)]
					             """);
			}

			[Fact]
			public async Task WhenAsyncEnumerableContainsSingleElement_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1);

				int result = await That(subject).HasSingle();

				await That(result).IsEqualTo(1);
			}

			[Fact]
			public async Task WhenAsyncEnumerableIsEmpty_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasSingle();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item,
					             but it was empty
					             """);
			}

			[Fact]
			public async Task WhenItemTypeIsAsyncEnumerable_ShouldReturnSingleItem()
			{
				IAsyncEnumerable<object> item = ToAsyncEnumerable<object>(1, 2);
				IAsyncEnumerable<IAsyncEnumerable<object>> subject = ToAsyncEnumerable(item);

				IAsyncEnumerable<object> result = await That(subject).HasSingle();

				await That(result).IsSameAs(item)
					.Because("the collection itself is also an IAsyncEnumerable<object>");
			}

			[Fact]
			public async Task WhenItemTypeIsObject_ShouldReturnSingleItem()
			{
				IAsyncEnumerable<object> subject = ToAsyncEnumerable<object>(1);

				object result = await That(subject).HasSingle();

				await That(result).IsEqualTo(1);
			}

			[Fact]
			public async Task WhenSourceThrowsAfterTwoItems_ShouldReportMoreThanOneItem()
			{
				IAsyncEnumerable<int> subject = ThrowAfter(new InvalidOperationException("src"), 1, 2);

				async Task Act()
					=> await That(subject).HasSingle();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item,
					             but it had more than one item

					             Collection:
					             [1, 2, (… and maybe more)]
					             """)
					.Because("the second item already decides the result, so the exception of the source must not replace it");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).HasSingle();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item,
					             but it was <null>
					             """);
			}
		}

		public sealed class MatchingExactlyTypeTests
		{
			[Fact]
			public async Task ShouldReturnSingleItem()
			{
				IAsyncEnumerable<MyBaseClass> subject =
					ToAsyncEnumerable(new MyClass(1), new MyOtherClass(2), new MyBaseClass(3));

				MyBaseClass result = await That(subject).HasSingle().MatchingExactly<MyBaseClass>();

				await That(result.Value).IsEqualTo(3);
			}

			[Fact]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IAsyncEnumerable<MyBaseClass> subject = ToAsyncEnumerable(
					new MyBaseClass(1), new MyClass(2), new MyBaseClass(3));

				async Task Act()
					=> await That(subject).HasSingle().MatchingExactly<MyBaseClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item exactly of type MyBaseClass,
					             but it had more than one matching item

					             Collection:
					             [
					               MyBaseClass {
					                 Value = 1
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 2
					               },
					               MyBaseClass {
					                 Value = 3
					               },
					               (… and maybe more)
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsOnlySubtypes_ShouldFail()
			{
				IAsyncEnumerable<MyBaseClass> subject = ToAsyncEnumerable<MyBaseClass>(new MyClass(1), new MyOtherClass(2));

				async Task Act()
					=> await That(subject).HasSingle().MatchingExactly<MyBaseClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item exactly of type MyBaseClass,
					             but it had no matching item
					             """);
			}
		}

		public sealed class MatchingExactlyTypePredicateTests
		{
			[Fact]
			public async Task ShouldReturnSingleItem()
			{
				IAsyncEnumerable<MyBaseClass> subject =
					ToAsyncEnumerable(new MyBaseClass(1), new MyClass(2), new MyBaseClass(3));

				MyBaseClass result = await That(subject).HasSingle().MatchingExactly<MyBaseClass>(x => x.Value > 1);

				await That(result.Value).IsEqualTo(3);
			}

			[Fact]
			public async Task WhenEnumerableContainsNoMatchingElements_ShouldFail()
			{
				IAsyncEnumerable<MyBaseClass> subject = ToAsyncEnumerable<MyBaseClass>(new MyClass(1), new MyClass(2));

				async Task Act()
					=> await That(subject).HasSingle().MatchingExactly<MyBaseClass>(x => x.Value > 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item exactly of type MyBaseClass matching x => x.Value > 1,
					             but it had no matching item
					             """);
			}

			[Fact]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				IAsyncEnumerable<MyBaseClass> subject = ToAsyncEnumerable(new MyBaseClass(1));

				async Task Act()
					=> await That(subject).HasSingle().MatchingExactly<MyBaseClass>(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}
		}

		public sealed class MatchingPredicateTests
		{
			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasSingle().Matching(x => x > 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item matching x => x > 1,
					             but it had more than one matching item

					             Collection:
					             [1, 1, 2, 3, (… and maybe more)]
					             """);
			}

			[Fact]
			public async Task ShouldReturnSingleItem()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				int result = await That(subject).HasSingle().Matching(x => x == 2);

				await That(result).IsEqualTo(2);
			}

			[Fact]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).HasSingle().Matching(x => x > 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item matching x => x > 1,
					             but it had more than one matching item

					             Collection:
					             [1, 2, 3, (… and maybe more)]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsSingleElement_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				int result = await That(subject).HasSingle().Matching(x => x > 2);

				await That(result).IsEqualTo(3);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasSingle().Matching(_ => true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item matching _ => true,
					             but it was empty
					             """);
			}

			[Fact]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasSingle().Matching(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).HasSingle().Matching(_ => false);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item matching _ => false,
					             but it was <null>
					             """);
			}
		}

		public sealed class MatchingTypeTests
		{
			[Fact]
			public async Task ShouldReturnSingleItem()
			{
				IAsyncEnumerable<MyBaseClass> subject =
					ToAsyncEnumerable(new MyClass(1), new MyOtherClass(2), new MyBaseClass(3));

				MyBaseClass result = await That(subject).HasSingle().Matching<MyOtherClass>();

				await That(result.Value).IsEqualTo(2);
			}

			[Fact]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IAsyncEnumerable<MyBaseClass> subject = ToAsyncEnumerable<MyBaseClass>(
					new MyClass(1), new MyOtherClass(2), new MyClass(3));

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item of type MyClass,
					             but it had more than one matching item

					             Collection:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               },
					               MyOtherClass {
					                 Value = 2
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 3
					               },
					               (… and maybe more)
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsSingleElement_ShouldSucceed()
			{
				IAsyncEnumerable<MyBaseClass> subject = ToAsyncEnumerable(
					new MyClass(1), new MyOtherClass(2), new MyBaseClass(3));

				MyBaseClass result = await That(subject).HasSingle().Matching<MyClass>();

				await That(result.Value).IsEqualTo(1);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IAsyncEnumerable<MyBaseClass> subject = ToAsyncEnumerable<MyBaseClass>();

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyClass>();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item of type MyClass,
					             but it was empty
					             """);
			}
		}

		public sealed class MatchingTypePredicateTests
		{
			[Fact]
			public async Task ShouldReturnSingleItem()
			{
				IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([1, 2, 3,], x => new MyClass(x));

				MyBaseClass result = await That(subject).HasSingle().Matching<MyBaseClass>(x => x.Value == 2);

				await That(result.Value).IsEqualTo(2);
			}

			[Fact]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([1, 2, 3,], x => new MyClass(x));

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyBaseClass>(x => x.Value > 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item of type MyBaseClass matching x => x.Value > 1,
					             but it had more than one matching item

					             Collection:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 2
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 3
					               },
					               (… and maybe more)
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsNoMatchingElements_ShouldFail()
			{
				IAsyncEnumerable<MyBaseClass> subject = ToAsyncEnumerable([1, 2, 3,], x => new MyBaseClass(x));

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyClass>(x => x.Value > 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item of type MyClass matching x => x.Value > 1,
					             but it had no matching item
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsSingleElement_ShouldSucceed()
			{
				IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([1, 2, 3,], x => new MyClass(x));

				MyBaseClass result = await That(subject).HasSingle().Matching<MyBaseClass>(x => x.Value > 2);

				await That(result.Value).IsEqualTo(3);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([], x => new MyClass(x));

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyBaseClass>(_ => true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item of type MyBaseClass matching _ => true,
					             but it was empty
					             """);
			}

			[Fact]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable([1, 2, 3,], x => new MyClass(x));

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyBaseClass>(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsSingleElement_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a single item,
					             but it had the single item 1

					             Collection:
					             [1]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenOnlyOneElementMatchesPredicate_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle().Matching(x => x > 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a single item matching x => x > 2,
					             but it had the single matching item 3

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenOnlyOneElementMatchesTheExactType_ShouldFail()
			{
				IAsyncEnumerable<MyBaseClass> subject = ToAsyncEnumerable(new MyClass(1), new MyBaseClass(2));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle().MatchingExactly<MyBaseClass>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a single item exactly of type MyBaseClass,
					             but it had the single matching item MyBaseClass {
					               Value = 2
					             }

					             Collection:
					             [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               },
					               MyBaseClass {
					                 Value = 2
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a single item,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_WithPredicate_ShouldFail()
			{
				IAsyncEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle().Matching(_ => true));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a single item matching _ => true,
					             but it was <null>
					             """);
			}
		}

		public sealed class WhichTests
		{
			[Fact]
			public async Task ShouldReturnSingleItem()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(42);

				int result = await That(subject).HasSingle().Which.IsGreaterThan(41).And
					.IsLessThan(43);

				await That(result).IsEqualTo(42);
			}

			[Fact]
			public async Task WhenAfterOr_AndTheLeftOperandIsMet_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).IsEmpty().Or.HasSingle().Which.IsEqualTo(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenAsyncEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).HasSingle().Which.IsGreaterThan(4);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item that is greater than 4,
					             but it had more than one item

					             Collection:
					             [1, 2, (… and maybe more)]
					             """);
			}

			[Fact]
			public async Task WhenAsyncEnumerableIsEmpty_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasSingle().Which.IsGreaterThan(4);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item that is greater than 4,
					             but it was empty
					             """);
			}

			[Fact]
			public async Task WhenMemberOfWhose_AndNegated_ShouldDescribeTheCollection()
			{
				ItemsClass subject = new(1);

				async Task Act()
					=> await That(subject).Whose(o => o.Items,
						v => v.DoesNotComplyWith(i => i.HasSingle().Which.IsEqualTo(1)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Items do not have a single item that is equal to 1,
					             but it had the single item 1

					             Collection (Items):
					             [1]
					             """);
			}

			[Fact]
			public async Task WhenMemberOfWhose_AndSingleItemDoesNotSatisfyExpectation_ShouldUseSingularFormAndNameTheItemIt()
			{
				ItemsClass subject = new(1);

				async Task Act()
					=> await That(subject).Whose(o => o.Items, v => v.HasSingle().Which.IsEqualTo(2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             whose Items have a single item that is equal to 2,
					             but it was 1, which differs by -1
					             """)
					.Because("the single item, not the member Items, is the subject of the continued expectation");
			}

			[Fact]
			public async Task WhenSingleItemDoesNotSatisfyExpectation_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(3);

				async Task Act()
					=> await That(subject).HasSingle().Which.IsGreaterThan(4);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a single item that is greater than 4,
					             but it was 3, which differs by -1
					             """);
			}

			[Fact]
			public async Task WhenSingleItemSatisfiesExpectation_ShouldEnumerateTheSourceOnce()
			{
				int enumerations = 0;

				async IAsyncEnumerable<int> Numbers()
				{
					enumerations++;
					await Task.Yield();
					yield return 3;
				}

				IAsyncEnumerable<int> subject = Numbers();

				async Task Act()
					=> await That(subject).HasSingle().Which.IsGreaterThan(2);

				await That(Act).DoesNotThrow();
				await That(enumerations).IsEqualTo(1)
					.Because("the single item is taken from the materialized items instead of enumerating the source again");
			}

			[Fact]
			public async Task WhenSingleItemSatisfiesExpectation_ShouldSucceed()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(3);

				async Task Act()
					=> await That(subject).HasSingle().Which.IsGreaterThan(2);

				await That(Act).DoesNotThrow();
			}

			private sealed class ItemsClass(params int[] items)
			{
				public IAsyncEnumerable<int> Items { get; } = ToAsyncEnumerable(items);
			}
		}
	}
}
#endif
