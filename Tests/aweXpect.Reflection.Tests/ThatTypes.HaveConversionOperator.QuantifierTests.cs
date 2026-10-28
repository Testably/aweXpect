using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class HaveConversionOperator
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_Explicit_WhenTooFewItemsHaveTheConversion_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(Money), typeof(ClassWithoutOperators),];

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an explicit conversion operator from Money to int for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ClassWithoutOperators
					             ]

					             Collection:
					             [
					               Money,
					               ClassWithoutOperators
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_Implicit_Directly_ShouldOnlyCountDeclaringItems()
			{
				IEnumerable<Type?> subject = [typeof(OperatorBaseClass), typeof(OperatorDerivedClass),];

				async Task Act()
				{
					await That(subject).Exactly(1).HaveImplicitConversionOperator<OperatorBaseClass, int>().Directly();
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task None_Implicit_WhenAnItemHasTheConversion_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(ClassWithoutOperators), typeof(Money),];

				Type sourceType = typeof(Money);
				Type targetType = typeof(decimal);

				async Task Act()
				{
					await That(subject).None().HaveImplicitConversionOperator(sourceType, targetType);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an implicit conversion operator from Money to decimal for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               Money
					             ]

					             Collection:
					             [
					               ClassWithoutOperators,
					               Money
					             ]
					             """);
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [null, typeof(Money),];

				Type sourceType = typeof(Money);
				Type targetType = typeof(int);

				async Task Act()
				{
					await That(subject).AtLeast(1).HaveExplicitConversionOperator(sourceType, targetType);
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.AtLeast(1).HaveExplicitConversionOperator(sourceType, targetType));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an explicit conversion operator from Money to int for at least one item,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               Money
					             ]
					             """);
				await That(ActNegated).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an explicit conversion operator from Money to int for no items,
					             but for the item at index 0, it was <null>

					             Collection:
					             [
					               <null>,
					               Money
					             ]
					             """).Because("a null item cannot be inspected, like a null subject");
			}
		}

#if NET8_0_OR_GREATER
		public sealed class StructCollectionTests
		{
			[Test]
			public async Task AtLeast_Implicit_Directly_WhenTooFewItemsDeclareTheConversion_ShouldFail()
			{
				ImmutableArray<Type?> subject = [typeof(OperatorBaseClass), typeof(OperatorDerivedClass),];

				async Task Act()
				{
					await That(subject).AtLeast(2)
						.HaveImplicitConversionOperator(typeof(OperatorBaseClass), typeof(int)).Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             declares an implicit conversion operator from OperatorBaseClass to int for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               OperatorDerivedClass
					             ]

					             Collection:
					             [
					               OperatorBaseClass,
					               OperatorDerivedClass
					             ]
					             """);
			}

			[Test]
			public async Task Exactly_Explicit_WhenTheNumberOfItemsMatches_ShouldSucceed()
			{
				ImmutableArray<Type?> subject = [typeof(Money), typeof(ClassWithoutOperators),];

				async Task Act()
				{
					await That(subject).Exactly(1).HaveExplicitConversionOperator(typeof(Money), typeof(int));
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class QuantifiedAsyncEnumerableTests
		{
			[Test]
			public async Task AtLeast_Explicit_WhenTooFewItemsHaveTheConversion_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(Money), typeof(ClassWithoutOperators),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveExplicitConversionOperator<Money, int>();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an explicit conversion operator from Money to int for at least 2 items,
					             but only 1 of 2 did

					             Not matching items:
					             [
					               ClassWithoutOperators
					             ]

					             Collection:
					             [
					               Money,
					               ClassWithoutOperators
					             ]
					             """);
			}

			[Test]
			public async Task None_Implicit_WhenNoItemHasTheConversion_ShouldSucceed()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(ClassWithoutOperators),
				}.ToTestAsyncEnumerable<Type?>();

				Type sourceType = typeof(Money);
				Type targetType = typeof(decimal);

				async Task Act()
				{
					await That(subject).None().HaveImplicitConversionOperator(sourceType, targetType);
				}

				await That(Act).DoesNotThrow();
			}
		}
#endif
	}
}
