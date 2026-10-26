using System;
using System.Collections;
using System.Linq;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

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
				[Test]
				public async Task DoesNotEnumerateTwice()
				{
					IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

					async Task Act()
						=> await That(subject).All().AreEqualTo(1)
							.And.All().AreEqualTo(1);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task DoesNotMaterializeEnumerable()
				{
					IEnumerable subject = Factory.GetFibonacciNumbers();

					async Task Act()
						=> await That(subject).All().AreEqualTo(1);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for all items,
						             but only 2 of at least 3 were

						             Not matching items:
						             [2, (… and maybe more)]

						             Collection:
						             [1, 1, 2, (… and maybe more)]
						             """);
				}

				[Test]
				[Arguments(double.NaN, false)]
				[Arguments(1.0, true)]
				public async Task DoubleNaNValues_ShouldBeConsideredEqual(double additionalValue, bool expectFailure)
				{
					IEnumerable subject = new[]
					{
						double.NaN, double.NaN, additionalValue,
					};

					async Task Act()
						=> await That(subject).All().AreEqualTo(double.NaN);

					await That(Act).Throws<FailException>().OnlyIf(expectFailure)
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

				[Test]
				[Arguments(float.NaN, false)]
				[Arguments(1.0F, true)]
				public async Task FloatNaNValues_ShouldBeConsideredEqual(float additionalValue, bool expectFailure)
				{
					IEnumerable subject = new[]
					{
						float.NaN, float.NaN, additionalValue,
					};

					async Task Act()
						=> await That(subject).All().AreEqualTo(float.NaN);

					await That(Act).Throws<FailException>().OnlyIf(expectFailure)
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
				[Test]
				public async Task ShouldSupportNullableValues()
				{
					IEnumerable subject = Factory.GetConstantValueEnumerable<int?>(null, 20);

					async Task Act()
						=> await That(subject).All().AreEqualTo(null);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task ShouldUseCustomComparer()
				{
					IEnumerable subject = Factory.GetFibonacciNumbers(20).ToArray();

					async Task Act()
						=> await That(subject).All().AreEqualTo(5).Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenItemsDiffer_ShouldFailAndDisplayNotMatchingItems()
				{
					IEnumerable subject = Factory.GetFibonacciNumbers(20).ToArray();

					async Task Act()
						=> await That(subject).All().AreEqualTo(5);

					await That(Act).Throws<FailException>()
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

				[Test]
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					int constantValue = 42;
					IEnumerable subject = Factory.GetConstantValueEnumerable(constantValue, 20).ToArray();

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					int constantValue = 42;
					IEnumerable subject = null!;

					async Task Act()
						=> await That(subject).All().AreEqualTo(constantValue);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 42 for all items,
						             but it was <null>
						             """);
				}
			}

			public sealed class CustomCollectionTests
			{
				[Test]
				public async Task ShouldUseCustomComparer()
				{
					CustomCollection? subject = new(1, 2, 3);

					async Task Act()
						=> await new CustomItems(That(subject), EnumerableQuantifier.All())
							.AreEqualTo(5).Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenItemsDiffer_ShouldFail()
				{
					CustomCollection? subject = new(1, 2, 1);

					async Task Act()
						=> await new CustomItems(That(subject), EnumerableQuantifier.All())
							.AreEqualTo(1);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for all items,
						             but only 1 of at least 2 were

						             Not matching items:
						             [2, (… and maybe more)]

						             Collection:
						             [1, 2, (… and maybe more)]
						             """);
				}

				[Test]
				public async Task WhenNegated_ShouldFailWhenAllItemsAreEqual()
				{
					CustomCollection? subject = new(1, 1, 1);

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => new CustomItems(it, EnumerableQuantifier.All())
							.AreEqualTo(1));

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 not for all items,
						             but all 3 were

						             Collection:
						             [1, 1, 1]
						             """);
				}

				[Test]
				public async Task WhenNoItemsDiffer_ShouldSucceed()
				{
					CustomCollection? subject = new(1, 1, 1);

					async Task Act()
						=> await new CustomItems(That(subject), EnumerableQuantifier.All())
							.AreEqualTo(1);

					await That(Act).DoesNotThrow();
				}

				private sealed class CustomCollection(params int[] values) : IEnumerable
				{
					public IEnumerator GetEnumerator() => values.GetEnumerator();
				}

				private sealed class CustomItems(IThat<CustomCollection?> subject, EnumerableQuantifier quantifier)
					: INonGenericEnumerableItems<CustomCollection>
				{
					public EnumerableQuantifier Quantifier => quantifier;
					public IThat<CustomCollection?> Subject => subject;

					public AndOrResult<CustomCollection, IThat<CustomCollection?>> Are<TType>()
						=> throw new NotSupportedException();

					public AndOrResult<CustomCollection, IThat<CustomCollection?>> Are(Type type)
						=> throw new NotSupportedException();

					public AndOrResult<CustomCollection, IThat<CustomCollection?>> AreExactly<TType>()
						=> throw new NotSupportedException();

					public AndOrResult<CustomCollection, IThat<CustomCollection?>> AreExactly(Type type)
						=> throw new NotSupportedException();
				}
			}
		}
	}
}
