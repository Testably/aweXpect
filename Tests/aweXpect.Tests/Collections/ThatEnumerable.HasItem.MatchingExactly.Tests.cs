using System.Collections.Generic;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasItem
	{
		public sealed partial class MatchingExactly
		{
			public sealed class GenericPredicateTests
			{
				[Test]
				public async Task DoesNotMaterializeEnumerable()
				{
					IEnumerable<MyClass> subject = Factory.GetFibonacciNumbers<MyClass>(x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(a => a.Value == 5);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldFail()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(_ => false).AtIndex(2);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyClass matching _ => false at index 2,
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
						=> await That(subject).HasItem().MatchingExactly<MyClass>(_ => true).AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(_ => true).AtIndex(3);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type MyClass matching _ => true at index 3,
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
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>(_ => true);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyBaseClass matching _ => true,
						             but it had no item

						             Collection:
						             []
						             """);
				}

				[Test]
				public async Task WhenItemHasAValueAndTypeIsNullable_ShouldSucceed()
				{
					IEnumerable<int?> subject = [null, 2,];

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<int?>(x => x == 2);

					await That(Act).DoesNotThrow()
						.Because("a boxed nullable value has the underlying type, so the nullable type is compared by it");
				}

				[Test]
				public async Task WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));
					HasItemWithConditionResult<IEnumerable<MyClass>, MyClass> result = That(subject).HasItem();
					_ = result.Matching(_ => true);

					void Act()
						=> _ = result.MatchingExactly<MyClass>(_ => true);

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Matching cannot be specified more than once.")
						.Because("a second filter would silently replace the first one");
				}

				[Test]
				public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
				{
					IEnumerable<MyClass>? subject = null;

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>(_ => true);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyBaseClass matching _ => true,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
				{
					IEnumerable<MyClass>? subject = null;

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>(_ => true).AtIndex(0);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyBaseClass matching _ => true at index 0,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IEnumerable<int> subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<uint>(_ => true);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type uint matching _ => true,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenTypeIsSubtype_ShouldFail()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>(_ => true);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type MyBaseClass matching _ => true,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenTypeIsSupertype_ShouldFail()
				{
					IEnumerable<MyBaseClass> subject = ToEnumerable<MyBaseClass>([0, 1, 2,], x => new MyBaseClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(_ => true);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type MyClass matching _ => true,
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
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>(_ => true).WithInvalidMatch();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyBaseClass matching _ => true with invalid match,
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
						=> await That(subject).HasItem().MatchingExactly<MyClass>();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldFail()
				{
					IEnumerable<MyBaseClass> subject = ToEnumerable<MyBaseClass>([0, 1, 2,], x => new MyBaseClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>().AtIndex(2);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyClass at index 2,
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
						=> await That(subject).HasItem().MatchingExactly<MyClass>().AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>().AtIndex(3);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type MyBaseClass at index 3,
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
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyBaseClass,
						             but it had no item

						             Collection:
						             []
						             """);
				}

				[Test]
				public async Task WhenItemHasAValueAndTypeIsNullable_ShouldSucceed()
				{
					IEnumerable<int?> subject = [null, 2,];

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<int?>().AtIndex(1);

					await That(Act).DoesNotThrow()
						.Because("a boxed nullable value has the underlying type, so the nullable type is compared by it");
				}

				[Test]
				public async Task WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));
					HasItemWithConditionResult<IEnumerable<MyClass>, MyClass> result = That(subject).HasItem();
					_ = result.Matching(_ => true);

					void Act()
						=> _ = result.MatchingExactly<MyClass>();

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Matching cannot be specified more than once.")
						.Because("a second filter would silently replace the first one");
				}

				[Test]
				public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
				{
					IEnumerable<MyClass>? subject = null;

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyBaseClass,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
				{
					IEnumerable<MyClass>? subject = null;

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>().AtIndex(0);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyBaseClass at index 0,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IEnumerable<int> subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<uint>();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type uint,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenTypeIsSubtype_ShouldFail()
				{
					IEnumerable<MyClass> subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type MyBaseClass,
						              but it had no matching item

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Test]
				public async Task WhenTypeIsSupertype_ShouldFail()
				{
					IEnumerable<MyBaseClass> subject = ToEnumerable<MyBaseClass>([0, 1, 2,], x => new MyBaseClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type MyClass,
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
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>().WithInvalidMatch();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyBaseClass with invalid match,
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
