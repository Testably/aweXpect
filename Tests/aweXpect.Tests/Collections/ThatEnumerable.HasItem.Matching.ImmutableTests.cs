#if NET8_0_OR_GREATER
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasItem
	{
		public sealed partial class Matching
		{
			public sealed class ImmutablePredicateTests
			{
				[Fact]
				public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
				{
					ImmutableArray<int> subject = [0, 1, 2,];

					async Task Act()
						=> await That(subject).HasItem().Matching(_ => false).AtIndex(2);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item matching _ => false at index 2,
						              but it had item 2 at index 2

						              Collection:
						              {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
				{
					ImmutableArray<int> subject = [0, 1, 2,];

					async Task Act()
						=> await That(subject).HasItem().Matching(_ => true).AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
				{
					ImmutableArray<int> subject = [0, 1, 2,];

					async Task Act()
						=> await That(subject).HasItem().Matching(_ => true).AtIndex(3);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item matching _ => true at index 3,
						              but it did not contain any item at index 3

						              Collection:
						              {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					ImmutableArray<int> subject = [];

					async Task Act()
						=> await That(subject).HasItem().Matching(_ => true);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item matching _ => true,
						             but it did not contain any item

						             Collection:
						             []
						             """);
				}

				[Fact]
				public async Task WithInvalidMatch_ShouldNotMatch()
				{
					ImmutableArray<int> subject = [0, 1, 2, 3, 4,];

					async Task Act()
						=> await That(subject).HasItem().Matching(_ => true).WithInvalidMatch();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item matching _ => true with invalid match,
						             but it did not contain any item with invalid match

						             Collection:
						             [0, 1, 2, 3, 4]
						             """);
				}

				[Fact]
				public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
				{
					ImmutableArray<string> subject = ["a", "b", "c",];

					async Task Act()
						=> await That(subject)
							.HasItem().Matching(_ => false).AtIndex(0).And
							.HasItem().Matching(_ => false).AtIndex(1).And
							.HasItem().Matching(_ => false)
					;

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item matching _ => false at index 0 and has an item matching _ => false at index 1 and has an item matching _ => false,
						             but it had item "a" at index 0 and it had item "b" at index 1 and it did not match at any index

						             Collection:
						             [
						               "a",
						               "b",
						               "c"
						             ]
						             """);
				}
			}

			public sealed class ImmutableGenericPredicateTests
			{
				[Fact]
				public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
				{
					ImmutableArray<MyClass> subject = [..ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x)),];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>(_ => false).AtIndex(2);

					await That(Act).Throws<XunitException>()
						.WithMessage($$"""
						               Expected that subject
						               has an item of type MyBaseClass matching _ => false at index 2,
						               but it had item MyClass {
						                 StringValue = "",
						                 Value = 2
						               } at index 2

						               Collection:
						               {{Formatter.Format(subject, FormattingOptions.MultipleLines)}}
						               """);
				}

				[Fact]
				public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
				{
					ImmutableArray<MyClass> subject = [..ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x)),];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>(_ => true).AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
				{
					ImmutableArray<MyClass> subject = [..ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x)),];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>(_ => true).AtIndex(3);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item of type MyBaseClass matching _ => true at index 3,
						              but it did not contain any item at index 3

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					ImmutableArray<MyClass> subject = [];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>(_ => true);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item of type MyBaseClass matching _ => true,
						             but it did not contain any item

						             Collection:
						             []
						             """);
				}

				[Fact]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					ImmutableArray<int> subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasItem().Matching<uint>(_ => true);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item of type uint matching _ => true,
						              but it did not match at any index

						              Collection:
						              {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenTypeIsSubtype_ShouldSucceed()
				{
					ImmutableArray<MyClass> subject = [..ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x)),];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>(_ => true);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTypeIsSupertype_ShouldFail()
				{
					ImmutableArray<MyBaseClass> subject =
						[..ToEnumerable<MyBaseClass>([0, 1, 2,], x => new MyBaseClass(x)),];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyClass>(_ => true);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item of type MyClass matching _ => true,
						              but it did not match at any index

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WithInvalidMatch_ShouldNotMatch()
				{
					ImmutableArray<MyClass> subject = [..ToEnumerable<MyClass>([0, 1, 2, 3, 4,], x => new MyClass(x)),];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>(_ => true).WithInvalidMatch();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item of type MyBaseClass matching _ => true with invalid match,
						             but it did not contain any item with invalid match

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
						               MyClass {
						                 StringValue = "",
						                 Value = 3
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 4
						               }
						             ]
						             """);
				}
			}

			public sealed class ImmutableGenericTests
			{
				[Fact]
				public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
				{
					ImmutableArray<MyBaseClass> subject =
						[..ToEnumerable<MyBaseClass>([0, 1, 2,], x => new MyBaseClass(x)),];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyClass>().AtIndex(2);

					await That(Act).Throws<XunitException>()
						.WithMessage($$"""
						               Expected that subject
						               has an item of type MyClass at index 2,
						               but it had item MyBaseClass {
						                 Value = 2
						               } at index 2

						               Collection:
						               {{Formatter.Format(subject, FormattingOptions.MultipleLines)}}
						               """);
				}

				[Fact]
				public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
				{
					ImmutableArray<MyClass> subject = [..ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x)),];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>().AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
				{
					ImmutableArray<MyClass> subject = [..ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x)),];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>().AtIndex(3);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item of type MyBaseClass at index 3,
						              but it did not contain any item at index 3

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					ImmutableArray<MyClass> subject = [];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item of type MyBaseClass,
						             but it did not contain any item

						             Collection:
						             []
						             """);
				}

				[Fact]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					ImmutableArray<int> subject = [1, 2, 3,];

					async Task Act()
						=> await That(subject).HasItem().Matching<uint>();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item of type uint,
						              but it did not match at any index

						              Collection:
						              {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenTypeIsSubtype_ShouldSucceed()
				{
					ImmutableArray<MyClass> subject = [..ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x)),];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenTypeIsSupertype_ShouldFail()
				{
					ImmutableArray<MyBaseClass> subject =
						[..ToEnumerable<MyBaseClass>([0, 1, 2,], x => new MyBaseClass(x)),];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyClass>();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item of type MyClass,
						              but it did not match at any index

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WithInvalidMatch_ShouldNotMatch()
				{
					ImmutableArray<MyClass> subject = [..ToEnumerable<MyClass>([0, 1, 2, 3, 4,], x => new MyClass(x)),];

					async Task Act()
						=> await That(subject).HasItem().Matching<MyBaseClass>().WithInvalidMatch();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item of type MyBaseClass with invalid match,
						             but it did not contain any item with invalid match

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
						               MyClass {
						                 StringValue = "",
						                 Value = 3
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 4
						               }
						             ]
						             """);
				}
			}
		}
	}
}
#endif
