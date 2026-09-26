#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class HasItem
	{
		public sealed class MatchingExactly
		{
			public sealed class GenericPredicateTests
			{
				[Fact]
				public async Task DoesNotMaterializeEnumerable()
				{
					IAsyncEnumerable<MyClass> subject = Factory.GetAsyncFibonacciNumbers<MyClass>(x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(a => a.Value == 5);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(_ => false).AtIndex(2);

					await That(Act).Throws<XunitException>()
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
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(_ => true).AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(_ => true).AtIndex(3);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyClass matching _ => true at index 3,
						             but it had no item at index 3

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
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable(Array.Empty<MyClass>());

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(_ => true);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyClass matching _ => true,
						             but it had no item

						             Collection:
						             []
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
				{
					IAsyncEnumerable<MyClass>? subject = null;

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(_ => true);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyClass matching _ => true,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
				{
					IAsyncEnumerable<MyClass>? subject = null;

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(_ => true).AtIndex(0);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyClass matching _ => true at index 0,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<uint>(_ => true);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type uint matching _ => true,
						             but it had no matching item

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenTypeIsSubtype_ShouldFail()
				{
					IAsyncEnumerable<MyClass> subject =
						ToAsyncEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>(_ => true);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyBaseClass matching _ => true,
						             but it had no matching item

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
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenTypeIsSupertype_ShouldFail()
				{
					IAsyncEnumerable<MyBaseClass> subject =
						ToAsyncEnumerable<MyBaseClass>([0, 1, 2,], x => new MyBaseClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(_ => true);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyClass matching _ => true,
						             but it had no matching item

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
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WithInvalidMatch_ShouldNotMatch()
				{
					IAsyncEnumerable<MyClass> subject =
						ToAsyncEnumerable<MyClass>([0, 1, 2, 3, 4,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>(_ => true).WithInvalidMatch();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyClass matching _ => true with invalid match,
						             but it had no item with invalid match

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

			public sealed class GenericTests
			{
				[Fact]
				public async Task DoesNotMaterializeEnumerable()
				{
					IAsyncEnumerable<MyClass> subject = Factory.GetAsyncFibonacciNumbers<MyClass>(x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
				{
					IAsyncEnumerable<MyClass> subject =
						ToAsyncEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>().AtIndex(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyBaseClass at index 2,
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
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>().AtIndex(2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>().AtIndex(3);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyClass at index 3,
						             but it had no item at index 3

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
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldFail()
				{
					IAsyncEnumerable<MyClass> subject = ToAsyncEnumerable(Array.Empty<MyClass>());

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyClass,
						             but it had no item

						             Collection:
						             []
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_WithAnyIndex_ShouldFail()
				{
					IAsyncEnumerable<MyClass>? subject = null;

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyClass,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_WithFixedIndex_ShouldFail()
				{
					IAsyncEnumerable<MyClass>? subject = null;

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>().AtIndex(0);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyClass at index 0,
						             but it was <null>
						             """);
				}

				[Fact]
				public async Task WhenTypeDoesNotMatch_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<uint>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type uint,
						             but it had no matching item

						             Collection:
						             [1, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenTypeIsSubtype_ShouldFail()
				{
					IAsyncEnumerable<MyClass> subject =
						ToAsyncEnumerable<MyClass>([0, 1, 2,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyBaseClass>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyBaseClass,
						             but it had no matching item

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
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenTypeIsSupertype_ShouldFail()
				{
					IAsyncEnumerable<MyBaseClass> subject =
						ToAsyncEnumerable<MyBaseClass>([0, 1, 2,], x => new MyBaseClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyClass,
						             but it had no matching item

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
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WithInvalidMatch_ShouldNotMatch()
				{
					IAsyncEnumerable<MyClass> subject =
						ToAsyncEnumerable<MyClass>([0, 1, 2, 3, 4,], x => new MyClass(x));

					async Task Act()
						=> await That(subject).HasItem().MatchingExactly<MyClass>().WithInvalidMatch();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             has an item exactly of type MyClass with invalid match,
						             but it had no item with invalid match

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
