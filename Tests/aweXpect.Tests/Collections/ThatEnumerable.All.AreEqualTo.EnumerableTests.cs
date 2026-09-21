using System.Collections;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreEqualTo
		{
			public sealed class EnumerableTests
			{
				[Fact]
				public async Task DoesNotEnumerateTwice()
				{
					IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

					async Task Act()
						=> await That(subject).All().AreEqualTo(1)
							.And.All().AreEqualTo(1);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task DoesNotMaterializeEnumerable()
				{
					IEnumerable subject = Factory.GetFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().AreEqualTo(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for all items,
						             but only 2 of at least 3 were

						             Not matching items:
						             [2, (… and maybe more)]

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
						               (… and maybe more)
						             ]
						             """);
				}

				[Theory]
				[InlineData(double.NaN, false)]
				[InlineData(1.0, true)]
				public async Task DoubleNaNValues_ShouldBeConsideredEqual(double additionalValue, bool expectFailure)
				{
					IEnumerable subject = new[]
					{
						double.NaN, double.NaN, additionalValue,
					};

					async Task Act()
						=> await That(subject).All().AreEqualTo(double.NaN);

					await That(Act).Throws<XunitException>().OnlyIf(expectFailure)
						.WithMessage("""
						             Expected that subject
						             is equal to NaN for all items,
						             but only 2 of 3 were

						             Not matching items:
						             [1.0]

						             Collection:
						             [NaN, NaN, 1.0]
						             """);
				}

				[Theory]
				[InlineData(float.NaN, false)]
				[InlineData(1.0F, true)]
				public async Task FloatNaNValues_ShouldBeConsideredEqual(float additionalValue, bool expectFailure)
				{
					IEnumerable subject = new[]
					{
						float.NaN, float.NaN, additionalValue,
					};

					async Task Act()
						=> await That(subject).All().AreEqualTo(float.NaN);

					await That(Act).Throws<XunitException>().OnlyIf(expectFailure)
						.WithMessage("""
						             Expected that subject
						             is equal to NaN for all items,
						             but only 2 of 3 were

						             Not matching items:
						             [1.0]

						             Collection:
						             [NaN, NaN, 1.0]
						             """);
				}
			}

			public sealed class EnumerableItemTests
			{
				[Fact]
				public async Task ShouldSupportNullableValues()
				{
					IEnumerable subject = Factory.GetConstantValueEnumerable<int?>(null, 20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(null);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable subject = Factory.GetFibonacciNumbers(20).ToArray();

					async Task Act()
						=> await That(subject).All().AreEqualTo(5).Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					IEnumerable subject = Factory.GetFibonacciNumbers(20).ToArray();

					async Task Act()
						=> await That(subject).All().AreEqualTo(5);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 5 for all items,
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
						             """);
				}

				[Fact]
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					int constantValue = 42;
					IEnumerable subject = Factory.GetConstantValueEnumerable(constantValue, 20).ToArray();

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					int constantValue = 42;
					IEnumerable subject = null!;

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 42 for all items,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
