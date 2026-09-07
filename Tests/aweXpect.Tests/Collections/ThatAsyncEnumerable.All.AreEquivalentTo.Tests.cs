#if NET8_0_OR_GREATER
using System.Collections.Generic;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreEquivalentTo
		{
			public sealed class ItemTests
			{
				[Fact]
				public async Task DoesNotEnumerateTwice()
				{
					ThrowWhenIteratingTwiceAsyncEnumerable subject = new();

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(1)
							.And.All().AreEquivalentTo(1);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task DoesNotMaterializeAsyncEnumerable()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to 1 for all items,
						             but not all were

						             Not matching items:
						             [2, 3, 5, 8, 13, 21, 34, 55, 89, (… and maybe others)]

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
						               (… and maybe others)
						             ]

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Fact]
				public async Task ShouldSupportNullableValues()
				{
					IAsyncEnumerable<int?> subject = Factory.GetConstantValueAsyncEnumerable<int?>(null, 20);

					async Task Act()
						=> await That(subject).All().AreEquivalentTo((int?)null);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(5).Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					IAsyncEnumerable<int> subject = Factory.GetAsyncFibonacciNumbers(20);

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(5, o => o.IncludingFields());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to 5 for all items,
						             but not all were

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
						               (… and maybe others)
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
						               (… and maybe others)
						             ]

						             Equivalency options:
						              - include public fields and properties
						             """);
				}

				[Fact]
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					int constantValue = 42;
					IAsyncEnumerable<int> subject = Factory.GetConstantValueAsyncEnumerable(constantValue, 20);

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(constantValue);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					int constantValue = 42;
					IAsyncEnumerable<int>? subject = null;

					async Task Act()
						=> await That(subject).All().AreEquivalentTo(constantValue);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equivalent to constantValue for all items,
						             but it was <null>

						             Equivalency options:
						              - include public fields and properties
						             """);
				}
			}
		}
	}
}
#endif
