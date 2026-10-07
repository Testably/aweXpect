#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Equivalency;

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
				[Test]
				public async Task Using_ShouldThrowInvalidOperationException()
				{
					ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(5).Using(new AllEqualComparer());

					await That(Act).Throws<InvalidOperationException>()
						.WithMessage("Using cannot be combined with Equivalent.");
				}

				[Test]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					ImmutableArray<int> subject = [..Factory.GetFibonacciNumbers(20),];

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(5);

					await That(Act).Throws<FailException>()
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

				[Test]
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					int constantValue = 42;
					ImmutableArray<int> subject = [..Factory.GetConstantValueEnumerable(constantValue, 20),];

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(constantValue);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WithOptions_WhenItemsDiffer_ShouldFail()
				{
					ImmutableArray<int> subject = [1, 2,];

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(1, o => o.IncludingFields());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to 1 for all items,
						             but only 1 of 2 were

						             Not matching items:
						             [2]

						             Collection:
						             [1, 2]

						             Equivalency options:
						              - include public fields and properties
						             """);
				}
			}
		}
	}
}
#endif
