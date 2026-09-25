#if NET8_0_OR_GREATER
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotHaveItem
	{
		public sealed partial class Matching
		{
			public sealed class ImmutablePredicateTests
			{
				[Fact]
				public async Task WhenEnumerableContainsMatchingItemAtGivenIndex_ShouldFail()
				{
					ImmutableArray<int> subject = [0, 1, 2,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching(_ => true).AtIndex(2);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              does not have an item matching _ => true at index 2,
						              but it had item 2 at index 2

						              Collection:
						              {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
				{
					ImmutableArray<int> subject = [0, 1, 2,];

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching(_ => true).AtIndex(3);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ImmutableGenericTests
			{
				[Fact]
				public async Task WhenTypeMatchesAtGivenIndex_ShouldFail()
				{
					ImmutableArray<MyBaseClass> subject = [new MyBaseClass(0), new MyClass(1),];

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
					ImmutableArray<MyBaseClass> subject = [new MyBaseClass(0), new MyBaseClass(1),];

					async Task Act()
						=> await That(subject).DoesNotHaveItem().Matching<MyClass>().AtIndex(1);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
