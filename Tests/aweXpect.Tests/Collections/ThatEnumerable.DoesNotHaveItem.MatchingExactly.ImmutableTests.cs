#if NET8_0_OR_GREATER
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotHaveItem
	{
		public sealed partial class MatchingExactly
		{
			public sealed class ImmutableGenericTests
			{
				[Fact]
				public async Task WhenTypeMatchesExactlyAtGivenIndex_ShouldFail()
				{
					ImmutableArray<MyBaseClass> subject = [new MyBaseClass(0), new MyClass(1),];

					async Task Act()
						=> await That(subject).DoesNotHaveItem().MatchingExactly<MyClass>().AtIndex(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have an item exactly of type MyClass at index 1,
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
				public async Task WhenTypeIsSubtype_ShouldSucceed()
				{
					ImmutableArray<MyBaseClass> subject = [new MyBaseClass(0), new MyClass(1),];

					async Task Act()
						=> await That(subject).DoesNotHaveItem().MatchingExactly<MyBaseClass>().AtIndex(1);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ImmutableGenericPredicateTests
			{
				[Fact]
				public async Task WhenItemOfTypeMatchesAtGivenIndex_ShouldFail()
				{
					ImmutableArray<MyBaseClass> subject = [new MyBaseClass(0), new MyClass(1),];

					async Task Act()
						=> await That(subject).DoesNotHaveItem().MatchingExactly<MyClass>(x => x.Value == 1).AtIndex(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             does not have an item exactly of type MyClass matching x => x.Value == 1 at index 1,
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
					ImmutableArray<MyBaseClass> subject = [new MyBaseClass(0), new MyClass(1),];

					async Task Act()
						=> await That(subject).DoesNotHaveItem().MatchingExactly<MyClass>(x => x.Value == 2).AtIndex(1);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
