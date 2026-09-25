using System.Collections;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasItem
	{
		public sealed partial class MatchingExactly
		{
			public sealed class EnumerableGenericPredicateTests
			{
				[Fact]
				public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
				{
					IEnumerable subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>(_ => false).AtIndex(2);

					await That(Act).Throws<XunitException>()
						.WithMessage($$"""
						               Expected that subject
						               has an item exactly of type MyBaseClass matching _ => false at index 2,
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
					IEnumerable subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(_ => true).AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
				{
					IEnumerable subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(_ => true).AtIndex(3);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type MyClass matching _ => true at index 3,
						              but it did not contain any item at index 3

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					IEnumerable subject = Array.Empty<MyClass>();

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>(_ => true);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyBaseClass matching _ => true,
						             but it did not contain any item

						             Collection:
						             []
						             """);
				}

				[Fact]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IEnumerable subject = ToEnumerable<int>(1, 2, 3);

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<uint>(_ => true);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type uint matching _ => true,
						              but it did not match at any index

						              Collection:
						              {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenTypeIsSubtype_ShouldFail()
				{
					IEnumerable subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>(_ => true);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type MyBaseClass matching _ => true,
						              but it did not match at any index

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenTypeIsSupertype_ShouldFail()
				{
					IEnumerable subject = ToEnumerable<MyBaseClass>([0, 1, 2,], x => new MyBaseClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(_ => true);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type MyClass matching _ => true,
						              but it did not match at any index

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WithInvalidMatch_ShouldNotMatch()
				{
					IEnumerable subject = ToEnumerable<MyClass>([0, 1, 2, 3, 4,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>(_ => true).WithInvalidMatch();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyBaseClass matching _ => true with invalid match,
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

			public sealed class EnumerableGenericTests
			{
				[Fact]
				public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
				{
					IEnumerable subject = ToEnumerable<MyBaseClass>([0, 1, 2,], x => new MyBaseClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>().AtIndex(2);

					await That(Act).Throws<XunitException>()
						.WithMessage($$"""
						               Expected that subject
						               has an item exactly of type MyClass at index 2,
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
					IEnumerable subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>().AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
				{
					IEnumerable subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>().AtIndex(3);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type MyClass at index 3,
						              but it did not contain any item at index 3

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					IEnumerable subject = Array.Empty<MyClass>();

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyBaseClass,
						             but it did not contain any item

						             Collection:
						             []
						             """);
				}

				[Fact]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IEnumerable subject = ToEnumerable<int>(1, 2, 3);

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<uint>();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type uint,
						              but it did not match at any index

						              Collection:
						              {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenTypeIsSubtype_ShouldFail()
				{
					IEnumerable subject = ToEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type MyBaseClass,
						              but it did not match at any index

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WhenTypeIsSupertype_ShouldFail()
				{
					IEnumerable subject = ToEnumerable<MyBaseClass>([0, 1, 2,], x => new MyBaseClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              has an item exactly of type MyClass,
						              but it did not match at any index

						              Collection:
						              {Formatter.Format(subject, FormattingOptions.MultipleLines)}
						              """);
				}

				[Fact]
				public async Task WithInvalidMatch_ShouldNotMatch()
				{
					IEnumerable subject = ToEnumerable<MyClass>([0, 1, 2, 3, 4,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>().WithInvalidMatch();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyBaseClass with invalid match,
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
