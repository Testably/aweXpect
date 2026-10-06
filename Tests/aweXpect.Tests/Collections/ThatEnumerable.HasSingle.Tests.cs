using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasSingle
	{
		public sealed class Tests
		{
			[Test]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasSingle();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item,
					             but it had more than one item

					             Collection:
					             [1, 1, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task ShouldReturnSingleItem()
			{
				IEnumerable<int> subject = ToEnumerable([42,]);

				int result = await That(subject).HasSingle();

				await That(result).IsEqualTo(42);
			}

			[Test]
			public async Task WhenAnEarlierAttemptHadSeveralItems_ShouldDescribeTheLastAttempt()
			{
				int calls = 0;
				Func<int[]> subject = () => calls++ == 0 ? [1, 2,] : [];

				async Task Act()
					=> await That(subject).Eventually().WithinTwoAttempts(5.Seconds())
						.HasSingle();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             eventually has a single item within 0:05,
					             but it was empty
					             """);
			}

			[Test]
			public async Task WhenCombinedAfterContains_AndItemTypeIsObject_ShouldReturnSingleItem()
			{
				object item = new();
				IEnumerable<object> subject = ToEnumerable(item);

				object result = await That(subject).Contains(item).And.HasSingle();

				await That(result).IsSameAs(item)
					.Because("the collection itself is also an object");
			}

			[Test]
			public async Task WhenCombinedAfterIsNotEmpty_AndItemTypeIsEnumerable_ShouldReturnSingleItem()
			{
				IEnumerable<object> item = ToEnumerable<object>(1, 2);
				IEnumerable<IEnumerable<object>> subject = ToEnumerable(item);

				IEnumerable<object> result = await That(subject).IsNotEmpty().And.HasSingle();

				await That(result).IsSameAs(item)
					.Because("the collection itself is also an IEnumerable<object>");
			}

			[Test]
			public async Task WhenCombinedAfterIsNotEmpty_AndItemTypeIsObject_ShouldReturnSingleItem()
			{
				object item = new();
				IEnumerable<object> subject = ToEnumerable(item);

				object result = await That(subject).IsNotEmpty().And.HasSingle();

				await That(result).IsSameAs(item)
					.Because("the collection itself is also an object");
			}

			[Test]
			public async Task WhenCombinedAfterOr_AndItemTypeIsObject_ShouldReturnSingleItem()
			{
				object item = new();
				IEnumerable<object> subject = ToEnumerable(item);

#pragma warning disable aweXpect0008
				object result = await That(subject).IsEmpty().Or.HasSingle();
#pragma warning restore aweXpect0008

				await That(result).IsSameAs(item)
					.Because("the collection itself is also an object");
			}

			[Test]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).HasSingle();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item,
					             but it had more than one item

					             Collection:
					             [1, 2, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsSingleElement_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1,]);

				int result = await That(subject).HasSingle();

				await That(result).IsEqualTo(1);
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasSingle();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item,
					             but it was empty
					             """);
			}

			[Test]
			public async Task WhenItemTypeIsEnumerable_ShouldReturnSingleItem()
			{
				IEnumerable<object> item = ToEnumerable<object>(1, 2);
				IEnumerable<IEnumerable<object>> subject = ToEnumerable(item);

				IEnumerable<object> result = await That(subject).HasSingle();

				await That(result).IsSameAs(item)
					.Because("the collection itself is also an IEnumerable<object>");
			}

			[Test]
			public async Task WhenItemTypeIsObject_ShouldReturnSingleItem()
			{
				IEnumerable<object> subject = ToEnumerable<object>(1);

				object result = await That(subject).HasSingle();

				await That(result).IsEqualTo(1);
			}

			[Test]
			public async Task WhenSourceIsEndless_ShouldNotReadFurtherItemsForTheFailureMessage()
			{
				int readItems = 0;

				IEnumerable<int> Source()
				{
					while (true)
					{
						readItems++;
						yield return 5;
					}
				}

				async Task Act()
					=> await That(Source()).HasSingle();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that Source()
					             has a single item,
					             but it had more than one item

					             Collection:
					             [5, 5, (… and maybe more)]
					             """);
				await That(readItems).IsEqualTo(2)
					.Because("the failure message must not read the source beyond the items that the evaluation needed");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).HasSingle();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item,
					             but it was <null>
					             """);
			}
		}

		public sealed class MatchingExactlyTypeTests
		{
			[Test]
			public async Task ShouldReturnSingleItem()
			{
				IEnumerable<MyBaseClass> subject =
					ToEnumerable(new MyClass(1), new MyOtherClass(2), new MyBaseClass(3));

				MyBaseClass result = await That(subject).HasSingle().MatchingExactly<MyBaseClass>();

				await That(result.Value).IsEqualTo(3);
			}

			[Test]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IEnumerable<MyBaseClass> subject = ToEnumerable(
					new MyBaseClass(1), new MyClass(2), new MyBaseClass(3));

				async Task Act()
					=> await That(subject).HasSingle().MatchingExactly<MyBaseClass>();

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenEnumerableContainsOnlySubtypes_ShouldFail()
			{
				IEnumerable<MyBaseClass> subject = ToEnumerable<MyBaseClass>(new MyClass(1), new MyOtherClass(2));

				async Task Act()
					=> await That(subject).HasSingle().MatchingExactly<MyBaseClass>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item exactly of type MyBaseClass,
					             but it had no matching item
					             """);
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable<MyBaseClass> subject = ToEnumerable<MyBaseClass>();

				async Task Act()
					=> await That(subject).HasSingle().MatchingExactly<MyClass>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item exactly of type MyClass,
					             but it was empty
					             """);
			}

			[Test]
			public async Task WhenItemHasAValueAndTypeIsNullable_ShouldReturnIt()
			{
				IEnumerable<int?> subject = ToEnumerable<int?>(null, 2);

				int? result = await That(subject).HasSingle().MatchingExactly<int?>();

				await That(result).IsEqualTo(2)
					.Because("a boxed nullable value has the underlying type, so the nullable type is compared by it");
			}
		}

		public sealed class MatchingExactlyTypePredicateTests
		{
			[Test]
			public async Task ShouldReturnSingleItem()
			{
				IEnumerable<MyBaseClass> subject =
					ToEnumerable(new MyBaseClass(1), new MyClass(2), new MyBaseClass(3));

				MyBaseClass result = await That(subject).HasSingle().MatchingExactly<MyBaseClass>(x => x.Value > 1);

				await That(result.Value).IsEqualTo(3);
			}

			[Test]
			public async Task WhenEnumerableContainsNoMatchingElements_ShouldFail()
			{
				IEnumerable<MyBaseClass> subject = ToEnumerable<MyBaseClass>(new MyClass(1), new MyClass(2));

				async Task Act()
					=> await That(subject).HasSingle().MatchingExactly<MyBaseClass>(x => x.Value > 1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item exactly of type MyBaseClass matching x => x.Value > 1,
					             but it had no matching item
					             """);
			}

			[Test]
			public async Task WhenItemHasAValueAndTypeIsNullable_ShouldReturnIt()
			{
				IEnumerable<int?> subject = ToEnumerable<int?>(1, 2);

				int? result = await That(subject).HasSingle().MatchingExactly<int?>(x => x > 1);

				await That(result).IsEqualTo(2)
					.Because("a boxed nullable value has the underlying type, so the nullable type is compared by it");
			}

			[Test]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<MyBaseClass> subject = ToEnumerable([1, 2, 3,], x => new MyBaseClass(x));

				async Task Act()
					=> await That(subject).HasSingle().MatchingExactly<MyClass>(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}
		}

		public sealed class MatchingPredicateTests
		{
			[Test]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasSingle().Matching(x => x > 1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item matching x => x > 1,
					             but it had more than one matching item

					             Collection:
					             [1, 1, 2, 3, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task ShouldReturnSingleItem()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				int result = await That(subject).HasSingle().Matching(x => x == 2);

				await That(result).IsEqualTo(2);
			}

			[Test]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).HasSingle().Matching(x => x > 1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item matching x => x > 1,
					             but it had more than one matching item

					             Collection:
					             [1, 2, 3, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsSingleElement_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				int result = await That(subject).HasSingle().Matching(x => x > 2);

				await That(result).IsEqualTo(3);
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasSingle().Matching(_ => true);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item matching _ => true,
					             but it was empty
					             """);
			}

			[Test]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<int> subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).HasSingle().Matching(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<string>? subject = null;

				async Task Act()
					=> await That(subject).HasSingle().Matching(_ => false);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item matching _ => false,
					             but it was <null>
					             """);
			}
		}

		public sealed class MatchingResultTests
		{
			[Test]
			public async Task ShouldNotOfferAnotherMatching()
			{
				MethodInfo[] matchingMethods = typeof(SingleItemResult<,>).GetMethods()
					.Where(method => method.Name.StartsWith("Matching", StringComparison.Ordinal))
					.ToArray();

				await That(matchingMethods).HasCount(5);
				await That(matchingMethods).All().Satisfy(method
					=> method.ReturnType.GetGenericTypeDefinition() == typeof(SingleMatchingItemResult<,>));
				await That(typeof(SingleMatchingItemResult<,>).GetMethods()
						.Where(method => method.Name.StartsWith("Matching", StringComparison.Ordinal)))
					.IsEmpty().Because("a second predicate would replace the first one");
			}

			[Test]
			public async Task WithBecause_WhenNoItemMatches_ShouldIncludeTheReason()
			{
				IEnumerable<int> subject = ToEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).HasSingle().Matching(x => x > 5).Because("we need a large item");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item matching x => x > 5, because we need a large item,
					             but it had no matching item
					             """);
			}

			[Test]
			public async Task WithExactType_ShouldOfferOptionsAndWhichOnTheTypedItem()
			{
				IEnumerable<MyBaseClass> subject =
					ToEnumerable(new MyClass(1), new MyOtherClass(2), new MyBaseClass(3));

				MyBaseClass result = await That(subject).HasSingle().MatchingExactly<MyBaseClass>()
					.Because("there is one").WithTimeout(30.Seconds()).WithCancellation(CancellationToken.None)
					.Which.Satisfies(item => item.Value == 3);

				await That(result.Value).IsEqualTo(3);
			}

			[Test]
			public async Task WithExactTypeAndPredicate_ShouldOfferOptionsAndWhichOnTheTypedItem()
			{
				IEnumerable<MyBaseClass> subject =
					ToEnumerable<MyBaseClass>(new MyClass(1), new MyOtherClass(2), new MyOtherClass(3));

				MyOtherClass result = await That(subject).HasSingle().MatchingExactly<MyOtherClass>(x => x.Value > 2)
					.Because("there is one").WithTimeout(30.Seconds()).WithCancellation(CancellationToken.None)
					.Which.Satisfies(item => item.Value == 3);

				await That(result.Value).IsEqualTo(3);
			}

			[Test]
			public async Task WithPredicate_ShouldOfferOptionsAndWhich()
			{
				IEnumerable<int> subject = ToEnumerable(1, 2, 3);

				int result = await That(subject).HasSingle().Matching(x => x > 2)
					.Because("there is one").WithTimeout(30.Seconds()).WithCancellation(CancellationToken.None)
					.Which.IsGreaterThan(2).And.IsLessThan(4);

				await That(result).IsEqualTo(3);
			}

			[Test]
			public async Task WithType_ShouldOfferOptionsAndWhichOnTheTypedItem()
			{
				IEnumerable<MyBaseClass> subject =
					ToEnumerable(new MyClass(1), new MyOtherClass(2), new MyBaseClass(3));

				MyOtherClass result = await That(subject).HasSingle().Matching<MyOtherClass>()
					.Because("there is one").WithTimeout(30.Seconds()).WithCancellation(CancellationToken.None)
					.Which.Satisfies(item => item.Value == 2);

				await That(result.Value).IsEqualTo(2);
			}

			[Test]
			public async Task WithType_WhenWhichIsNotMet_ShouldFail()
			{
				IEnumerable<MyBaseClass> subject =
					ToEnumerable(new MyClass(1), new MyOtherClass(2), new MyBaseClass(3));

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyOtherClass>()
						.Which.Satisfies(item => item.Value == 5);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item of type MyOtherClass that satisfies item => item.Value == 5,
					             but it was MyOtherClass {
					                 Value = 2
					               }
					             """);
			}

			[Test]
			public async Task WithTypeAndPredicate_ShouldOfferOptionsAndWhichOnTheTypedItem()
			{
				IEnumerable<MyBaseClass> subject =
					ToEnumerable<MyBaseClass>(new MyClass(1), new MyOtherClass(2), new MyOtherClass(3));

				MyOtherClass result = await That(subject).HasSingle().Matching<MyOtherClass>(x => x.Value > 2)
					.Because("there is one").WithTimeout(30.Seconds()).WithCancellation(CancellationToken.None)
					.Which.Satisfies(item => item.Value == 3);

				await That(result.Value).IsEqualTo(3);
			}

			[Test]
			public async Task WithTypeAndPredicate_WhenTheItemOfTheTypeDoesNotMatch_ShouldFail()
			{
				IEnumerable<MyBaseClass> subject = ToEnumerable<MyBaseClass>(new MyClass(1), new MyOtherClass(2));

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyOtherClass>(x => x.Value == 5);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item of type MyOtherClass matching x => x.Value == 5,
					             but it had no matching item
					             """);
			}
		}

		public sealed class MatchingTypeTests
		{
			[Test]
			public async Task ShouldReturnSingleItem()
			{
				IEnumerable<MyBaseClass> subject =
					ToEnumerable(new MyClass(1), new MyOtherClass(2), new MyBaseClass(3));

				MyBaseClass result = await That(subject).HasSingle().Matching<MyOtherClass>();

				await That(result.Value).IsEqualTo(2);
			}

			[Test]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IEnumerable<MyBaseClass> subject = ToEnumerable<MyBaseClass>(
					new MyClass(1), new MyOtherClass(2), new MyClass(3));

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyClass>();

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenEnumerableContainsSingleElement_ShouldSucceed()
			{
				IEnumerable<MyBaseClass> subject = ToEnumerable(
					new MyClass(1), new MyOtherClass(2), new MyBaseClass(3));

				MyBaseClass result = await That(subject).HasSingle().Matching<MyClass>();

				await That(result.Value).IsEqualTo(1);
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable<MyBaseClass> subject = ToEnumerable<MyBaseClass>();

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyClass>();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item of type MyClass,
					             but it was empty
					             """);
			}
		}

		public sealed class MatchingTypePredicateTests
		{
			[Test]
			public async Task ShouldReturnSingleItem()
			{
				IEnumerable<MyClass> subject = ToEnumerable([1, 2, 3,], x => new MyClass(x));

				MyBaseClass result = await That(subject).HasSingle().Matching<MyBaseClass>(x => x.Value == 2);

				await That(result.Value).IsEqualTo(2);
			}

			[Test]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IEnumerable<MyClass> subject = ToEnumerable([1, 2, 3,], x => new MyClass(x));

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyBaseClass>(x => x.Value > 1);

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenEnumerableContainsNoMatchingElements_ShouldFail()
			{
				IEnumerable<MyBaseClass> subject = ToEnumerable([1, 2, 3,], x => new MyBaseClass(x));

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyClass>(x => x.Value > 1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item of type MyClass matching x => x.Value > 1,
					             but it had no matching item
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsSingleElement_ShouldSucceed()
			{
				IEnumerable<MyClass> subject = ToEnumerable([1, 2, 3,], x => new MyClass(x));

				MyBaseClass result = await That(subject).HasSingle().Matching<MyBaseClass>(x => x.Value > 2);

				await That(result.Value).IsEqualTo(3);
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable<MyClass> subject = ToEnumerable<MyClass>();

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyBaseClass>(_ => true);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item of type MyBaseClass matching _ => true,
					             but it was empty
					             """);
			}

			[Test]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				IEnumerable<MyBaseClass> subject = ToEnumerable([1, 2, 3,], x => new MyBaseClass(x));

				async Task Act()
					=> await That(subject).HasSingle().Matching<MyClass>(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsSingleElement_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a single item,
					             but it had the single item 1

					             Collection:
					             [1]
					             """);
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMoreThanOneElementMatchesTheExactType_ShouldSucceed()
			{
				IEnumerable<MyBaseClass> subject = ToEnumerable(new MyBaseClass(1), new MyBaseClass(2));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle().MatchingExactly<MyBaseClass>());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenOnlyOneElementMatchesPredicate_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle().Matching(x => x > 2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a single item matching x => x > 2,
					             but it had the single matching item 3

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenOnlyOneElementMatchesTheExactType_ShouldFail()
			{
				IEnumerable<MyBaseClass> subject = ToEnumerable(new MyClass(1), new MyBaseClass(2));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle().MatchingExactly<MyBaseClass>());

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a single item,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_WithPredicate_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle().Matching(_ => true));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a single item matching _ => true,
					             but it was <null>
					             """);
			}
		}

		public sealed class WhichTests
		{
			[Test]
			public async Task ShouldReturnSingleItem()
			{
				IEnumerable<int> subject = ToEnumerable([42,]);

				int result = await That(subject).HasSingle().Which.IsGreaterThan(41).And
					.IsLessThan(43);

				await That(result).IsEqualTo(42);
			}

			[Test]
			public async Task WhenAfterAnd_AndOnlyTheLeftOperandFails_ShouldReportBoth()
			{
				IEnumerable<int> subject = ToEnumerable([3,]);

				async Task Act()
					=> await That(subject).Contains(1).And.HasSingle().Which.IsEqualTo(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains an item equal to 1 at least once and has a single item that is equal to 2,
					             but it did not contain it and it was 3, which differs by 1

					             Collection:
					             [3]
					             """);
			}

			[Test]
			public async Task WhenAfterAnd_AndItemTypeIsObject_ShouldContinueFromTheSingleItem()
			{
				object item = new();
				IEnumerable<object> subject = ToEnumerable(item);

				object? result = await That(subject).IsNotEmpty().And.HasSingle().Which.IsSameAs(item);

				await That(result).IsSameAs(item)
					.Because("the collection itself is also an object");
			}

			[Test]
			public async Task WhenAfterOr_AndNoOperandIsMet_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([2,]);

				async Task Act()
					=> await That(subject).IsEmpty().Or.HasSingle().Which.IsEqualTo(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is empty or has a single item that is equal to 1,
					             but it was [
					               2
					             ]
					             and it was 2, which differs by 1
					             """);
			}

			[Test]
			public async Task WhenAfterOr_AndTheLeftOperandIsMet_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).IsEmpty().Or.HasSingle().Which.IsEqualTo(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenAfterOr_AndTheRightOperandIsMet_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([1,]);

				async Task Act()
					=> await That(subject).IsEmpty().Or.HasSingle().Which.IsEqualTo(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsMoreThanOneElement_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).HasSingle().Which.IsGreaterThan(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item that is greater than 2,
					             but it had more than one item

					             Collection:
					             [1, 2, (… and maybe more)]
					             """);
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasSingle().Which.IsGreaterThan(4);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item that is greater than 4,
					             but it was empty
					             """);
			}

			[Test]
			public async Task WhenEnumerableIsEmpty_ShouldNotEvaluateTheItemExpectation()
			{
				IEnumerable<int> subject = ToEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).HasSingle().Which.Satisfies(i => 10 / i == 1);

				FailException exception = await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item that satisfies i => 10 / i == 1,
					             but it was empty
					             """);
				await That(exception.InnerException).IsNull()
					.Because("the predicate must not run on a default item that does not exist");
			}

			[Test]
			public async Task WhenItemTypeIsObject_ShouldReturnSingleItem()
			{
				IEnumerable<object> subject = ToEnumerable<object>(1);

				object? result = await That(subject).HasSingle().Which.IsEqualTo(1);

				await That(result).IsEqualTo(1);
			}

			[Test]
			public async Task WhenMatchingPredicate_ShouldCallThePredicateOncePerItem()
			{
				IEnumerable<int> subject = ToEnumerable([1, 2, 3,]);
				int calls = 0;

				await That(subject).HasSingle().Matching(x =>
				{
					calls++;
					return x == 2;
				}).Which.IsEqualTo(2);

				await That(calls).IsEqualTo(3)
					.Because("the single item is not searched again for the expectations on it");
			}

			[Test]
			public async Task WhenMemberOfWhose_AndNegated_ShouldDescribeTheCollection()
			{
				ItemsClass subject = new(1);

				async Task Act()
					=> await That(subject).Whose(o => o.Items,
						v => v.DoesNotComplyWith(i => i.HasSingle().Which.IsEqualTo(1)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             whose Items do not have a single item that is equal to 1,
					             but it had the single item 1

					             Collection (Items):
					             [1]
					             """);
			}

			[Test]
			public async Task WhenNegatedAfterOr_AndTheLeftOperandIsMet_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable(Array.Empty<int>());

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEmpty().Or.HasSingle().Which.IsEqualTo(1));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not empty and does not have a single item that is equal to 1,
					             but it was empty
					             """);
			}

			[Test]
			public async Task WhenNegated_AndEnumerableContainsMoreThanOneElement_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([3, 3,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle().Which.IsEqualTo(3));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegated_AndSingleItemDoesNotSatisfyExpectation_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([4,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle().Which.IsEqualTo(3));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNegated_AndSingleItemSatisfiesExpectation_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([3,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle().Which.IsEqualTo(3));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a single item that is equal to 3,
					             but it had the single item 3

					             Collection:
					             [3]
					             """);
			}

			[Test]
			public async Task WhenNegated_AndSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasSingle().Which.IsEqualTo(3));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a single item that is equal to 3,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSingleItemDoesNotSatisfyExpectation_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([3,]);

				async Task Act()
					=> await That(subject).HasSingle().Which.IsGreaterThan(4);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item that is greater than 4,
					             but it was 3, which differs by -1
					             """);
			}

			[Test]
			public async Task WhenSingleItemSatisfiesExpectation_ShouldSucceed()
			{
				IEnumerable<int> subject = ToEnumerable([3,]);

				async Task Act()
					=> await That(subject).HasSingle().Which.IsGreaterThan(2);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectCanOnlyBeEnumeratedOnce_ShouldUseMaterializedItem()
			{
				ThrowWhenIteratingTwiceEnumerable subject = new();

				async Task Act()
					=> await That(subject).HasSingle().Which.IsGreaterThan(4);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item that is greater than 4,
					             but it was 1, which differs by -3
					             """);
			}

			[Test]
			public async Task WhenSubjectCanOnlyBeEnumeratedOnce_WithPredicate_ShouldUseMaterializedItem()
			{
				ThrowWhenIteratingTwiceEnumerable subject = new();

				async Task Act()
					=> await That(subject).HasSingle().Matching(x => x > 0).Which.IsGreaterThan(4);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item matching x => x > 0 that is greater than 4,
					             but it was 1, which differs by -3
					             """);
			}

			[Test]
			public async Task WithWhose_ShouldNotRepeatConnector()
			{
				IEnumerable<string> subject = ToEnumerable(["foo",]);

				async Task Act()
					=> await That(subject).HasSingle().Which.Whose(x => x.Length, l => l.IsEqualTo(4));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a single item whose Length is equal to 4,
					             but Length was 3, which differs by -1
					             """);
			}

			private sealed class ItemsClass(params int[] items)
			{
				public List<int> Items { get; } = [..items,];
			}
		}
	}
}
