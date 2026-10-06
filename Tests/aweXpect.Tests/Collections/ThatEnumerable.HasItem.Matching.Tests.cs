using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasItem
	{
		public sealed partial class Matching
		{
			public sealed class PredicateTests
			{
				[Test]
				public async Task DoesNotEnumerateTwice()
				{
					ThrowWhenIteratingTwiceEnumerable subject = new();

					async Task Act()
						=> await That(subject).HasItem().Matching(_ => true)
							.And.HasItem().Matching(_ => true).AtIndex(0);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task DoesNotMaterializeEnumerable()
				{
					IEnumerable<int> subject = Factory.GetFibonacciNumbers();

					async Task Act()
						=> await That(subject).HasItem().Matching(a => a == 5);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldFail()
				{
					int[] subject = [0, 1, 2,];

					async Task Act()
						=> await That(subject).HasItem().Matching(_ => false).AtIndex(2);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item matching _ => false at index 2,
						              but it had item 2 at index 2

						              Collection:
						              {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
				{
					int[] subject = [0, 1, 2,];

					async Task Act()
						=> await That(subject).HasItem().Matching(_ => true).AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
				{
					List<int> subject =
					[
						0,
						1,
						2,
					];

					async Task Act()
						=> await That(subject).HasItem().Matching(_ => true).AtIndex(3);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item matching _ => true at index 3,
						              but it had no item at index 3

						              Collection:
						              {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					List<int> subject = [];

					async Task Act()
						=> await That(subject).HasItem().Matching(_ => true);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item matching _ => true,
						             but it had no item

						             Collection:
						             []
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).HasItem().Matching(_ => true);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item matching _ => true,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
				{
					IEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).HasItem().Matching(_ => true).AtIndex(0);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item matching _ => true at index 0,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WithInvalidMatch_ShouldNotMatch()
				{
					IEnumerable<int> subject = [0, 1, 2, 3, 4,];

					async Task Act()
						=> await That(subject).HasItem().Matching(_ => true).WithInvalidMatch();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item matching _ => true with invalid match,
						             but it had no item with invalid match

						             Collection:
						             [0, 1, 2, 3, 4]
						             """);
				}

				[Test]
				public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
				{
					IEnumerable<string> subject = ToEnumerable(["a", "b", "c",]);

					async Task Act()
						=> await That(subject)
							.HasItem().Matching(_ => false).AtIndex(0).And
							.HasItem().Matching(_ => false).AtIndex(1).And
							.HasItem().Matching(_ => false)
					;

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item matching _ => false at index 0 and has an item matching _ => false at index 1 and has an item matching _ => false,
						             but it had item "a" at index 0 and had item "b" at index 1 and had no matching item

						             Collection:
						             [
						               "a",
						               "b",
						               "c"
						             ]
						             """);
				}
			}

			public sealed class GenericPredicateTests
			{
				[Test]
				public async Task DoesNotMaterializeEnumerable()
				{
					IEnumerable<MyClass> subject = Factory.GetFibonacciNumbers<MyClass>(x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>(a => a.Value == 5);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldFail()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>(_ => false).AtIndex(2);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item of type MyBaseClass matching _ => false at index 2,
						             but it had item MyClass {
						               StringValue = "",
						               Value = 2
						             } at index 2

						             Collection:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 0
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 2
						               },
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>(_ => true).AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>(_ => true).AtIndex(3);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item of type MyBaseClass matching _ => true at index 3,
						              but it had no item at index 3

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					IEnumerable<MyClass> subject = [];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>(_ => true);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item of type MyBaseClass matching _ => true,
						             but it had no item

						             Collection:
						             []
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
				{
					IEnumerable<MyClass>? subject = null;

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>(_ => true);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item of type MyBaseClass matching _ => true,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
				{
					IEnumerable<MyClass>? subject = null;

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>(_ => true).AtIndex(0);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item of type MyBaseClass matching _ => true at index 0,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IEnumerable<int> subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasItem().Matching<uint>(_ => true);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item of type uint matching _ => true,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenTypeIsSubtype_ShouldSucceed()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>(_ => true);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTypeIsSupertype_ShouldFail()
				{
					IEnumerable<MyBaseClass> subject = ToEnumerable<MyBaseClass>([0, 1, 2,], x => new MyBaseClass(x));

					async Task Act()
						=> await That(subject).HasItem().Matching<MyClass>(_ => true);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item of type MyClass matching _ => true,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WithInvalidMatch_ShouldNotMatch()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2, 3, 4,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>(_ => true).WithInvalidMatch();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item of type MyBaseClass matching _ => true with invalid match,
						             but it had no item with invalid match

						             Collection:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 0
						               },
						               (… and maybe more)
						             ]
						             """);
				}
			}

			public sealed class GenericTests
			{
				[Test]
				public async Task DoesNotMaterializeEnumerable()
				{
					IEnumerable<MyClass> subject = Factory.GetFibonacciNumbers<MyClass>(x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldFail()
				{
					IEnumerable<MyBaseClass> subject = ToEnumerable<MyBaseClass>([0, 1, 2,], x => new MyBaseClass(x));

					async Task Act()
						=> await That(subject).HasItem().Matching<MyClass>().AtIndex(2);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item of type MyClass at index 2,
						             but it had item MyBaseClass {
						               Value = 2
						             } at index 2

						             Collection:
						             [
						               MyBaseClass {
						                 Value = 0
						               },
						               MyBaseClass {
						                 Value = 1
						               },
						               MyBaseClass {
						                 Value = 2
						               },
						               (… and maybe more)
						             ]
						             """);
				}

				[Test]
				public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>().AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>().AtIndex(3);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item of type MyBaseClass at index 3,
						              but it had no item at index 3

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					IEnumerable<MyClass> subject = [];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item of type MyBaseClass,
						             but it had no item

						             Collection:
						             []
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
				{
					IEnumerable<MyClass>? subject = null;

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item of type MyBaseClass,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
				{
					IEnumerable<MyClass>? subject = null;

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>().AtIndex(0);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item of type MyBaseClass at index 0,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IEnumerable<int> subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasItem().Matching<uint>();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item of type uint,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenTypeIsSubtype_ShouldSucceed()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenTypeIsSupertype_ShouldFail()
				{
					IEnumerable<MyBaseClass> subject = ToEnumerable<MyBaseClass>([0, 1, 2,], x => new MyBaseClass(x));

					async Task Act()
						=> await That(subject).HasItem().Matching<MyClass>();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item of type MyClass,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WithInvalidMatch_ShouldNotMatch()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2, 3, 4,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>().WithInvalidMatch();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item of type MyBaseClass with invalid match,
						             but it had no item with invalid match

						             Collection:
						             [
						               MyClass {
						                 StringValue = "",
						                 Value = 0
						               },
						               (… and maybe more)
						             ]
						             """);
				}
			}
		}
	}
}
