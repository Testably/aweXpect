#if NET8_0_OR_GREATER
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreEquivalentTo
		{
			public sealed class ImmutableTests
			{
				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(5).Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(5);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to 5 for all items,
						             but only 1 of 20 were

						             Not matching items:
						             [
						               1,
						               1,
						               2,
						               3,
						               8,
						               13,
						               21,
						               34,
						               55,
						               89,
						               (… and 9 more)
						             ]

						             Collection:
						             [
						               1,
						               1,
						               2,
						               3,
						               5,
						               8,
						               13,
						               21,
						               34,
						               55,
						               (… and 10 more)
						             ]

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Fact]
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					int constantValue = 42;
					ImmutableArray<int> subject = [..Factory.GetConstantValueEnumerable(constantValue, 20),];

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(constantValue);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
