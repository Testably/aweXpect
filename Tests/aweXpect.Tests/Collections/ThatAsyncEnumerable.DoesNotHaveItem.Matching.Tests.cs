#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class DoesNotHaveItem
	{
		public sealed class Matching
		{
			public sealed class PredicateTests
			{
				[Fact]
				public async Task WhenEnumerableContainsMatchingItemAtGivenIndex_ShouldFail()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching(_ => true).AtIndex(2);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have an item matching _ => true at index 2,
						             but it had item 2 at index 2

						             Collection:
						             [0, 1, 2]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
				{
					IAsyncEnumerable<int> subject = ToAsyncEnumerable(0, 1, 2);

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching(_ => true).AtIndex(3);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching(_ => true).AtIndex(0);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have an item matching _ => true at index 0,
						             but it was <null>
						             """);
				}
			}

			public sealed class GenericTests
			{
				[Fact]
				public async Task WhenTypeMatchesAtGivenIndex_ShouldFail()
				{
					IAsyncEnumerable<MyBaseClass> subject =
						ToAsyncEnumerable<MyBaseClass>(new MyBaseClass(0), new MyClass(1));

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching<MyClass>().AtIndex(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have an item of type MyClass at index 1,
						             but it had item MyClass {
						               StringValue = "",
						               Value = 1
						             } at index 1

						             Collection:
						             [
						               MyBaseClass {
						                 Value = 0
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenTypeIsSupertype_ShouldSucceed()
				{
					IAsyncEnumerable<MyBaseClass> subject =
						ToAsyncEnumerable<MyBaseClass>(new MyBaseClass(0), new MyBaseClass(1));

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching<MyClass>().AtIndex(1);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class GenericPredicateTests
			{
				[Fact]
				public async Task WhenItemOfTypeMatchesAtGivenIndex_ShouldFail()
				{
					IAsyncEnumerable<MyBaseClass> subject =
						ToAsyncEnumerable<MyBaseClass>(new MyBaseClass(0), new MyClass(1));

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching<MyClass>(x => x.Value == 1).AtIndex(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have an item of type MyClass matching x => x.Value == 1 at index 1,
						             but it had item MyClass {
						               StringValue = "",
						               Value = 1
						             } at index 1

						             Collection:
						             [
						               MyBaseClass {
						                 Value = 0
						               },
						               MyClass {
						                 StringValue = "",
						                 Value = 1
						               }
						             ]
						             """);
				}

				[Fact]
				public async Task WhenPredicateDoesNotMatch_ShouldSucceed()
				{
					IAsyncEnumerable<MyBaseClass> subject =
						ToAsyncEnumerable<MyBaseClass>(new MyBaseClass(0), new MyClass(1));

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching<MyClass>(x => x.Value == 2).AtIndex(1);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
