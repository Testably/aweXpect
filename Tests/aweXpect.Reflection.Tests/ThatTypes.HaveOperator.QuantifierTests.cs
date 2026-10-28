using System.Collections.Generic;
using aweXpect.Reflection.Tests.TestHelpers.Types;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
using aweXpect.Reflection.Tests.TestHelpers;
#endif

namespace aweXpect.Reflection.Tests;

public sealed partial class ThatTypes
{
	public sealed partial class HaveOperator
	{
		public sealed class QuantifierTests
		{
			[Test]
			public async Task AtLeast_WhenTooFewItemsHaveTheOperator_ShouldFail()
			{
				IEnumerable<Type?> subject = [typeof(Money), typeof(ClassWithoutOperators),];

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveOperator(Operator.Addition);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has the operator Addition for at least 2 items,
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
			public async Task Exactly_Directly_ShouldOnlyCountDeclaringItems()
			{
				IEnumerable<Type?> subject = [typeof(OperatorBaseClass), typeof(OperatorDerivedClass),];

				async Task Act()
				{
					await That(subject).Exactly(2).HaveOperator(Operator.Subtraction).Directly();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             declares the operator Subtraction for exactly 2 items,
					             but only 1 of 2 did

					             Collection:
					             [
					               OperatorBaseClass,
					               OperatorDerivedClass
					             ]
					             """);
			}

			[Test]
			public async Task None_WithOperand_WhenNoItemHasTheOperator_ShouldSucceed()
			{
				IEnumerable<Type?> subject = [typeof(ClassWithoutOperators), typeof(OperatorBaseClass),];

				async Task Act()
				{
					await That(subject).None().HaveOperator<int>(Operator.Addition);
				}

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenItemIsNull_ShouldFailBothWays()
			{
				IEnumerable<Type?> subject = [null, typeof(Money),];

				Type operandType = typeof(int);

				async Task Act()
				{
					await That(subject).AtLeast(1).HaveOperator(Operator.Addition, operandType);
				}

				async Task ActNegated()
				{
					await That(subject).DoesNotComplyWith(they
						=> they.AtLeast(2).HaveOperator(Operator.Addition, operandType));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has the operator Addition with operand int for at least one item,
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
					             has the operator Addition with operand int for fewer than 2 items,
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
			public async Task AtLeast_WithOperand_WhenTooFewItemsHaveTheOperator_ShouldFail()
			{
				ImmutableArray<Type?> subject = [typeof(Money), typeof(ClassWithoutOperators),];

				async Task Act()
				{
					await That(subject).AtLeast(2).HaveOperator(Operator.Addition, typeof(int));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has the operator Addition with operand int for at least 2 items,
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
			public async Task Exactly_Directly_WhenTheNumberOfDeclaringItemsMatches_ShouldSucceed()
			{
				ImmutableArray<Type?> subject = [typeof(OperatorBaseClass), typeof(OperatorDerivedClass),];

				async Task Act()
				{
					await That(subject).Exactly(1).HaveOperator(Operator.Subtraction).Directly();
				}

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class QuantifiedAsyncEnumerableTests
		{
			[Test]
			public async Task AtMost_WhenTooManyItemsHaveTheOperator_ShouldFail()
			{
				IAsyncEnumerable<Type?> subject = new[]
				{
					typeof(OperatorBaseClass), typeof(OperatorDerivedClass),
				}.ToTestAsyncEnumerable<Type?>();

				async Task Act()
				{
					await That(subject).AtMost(1).HaveOperator<OperatorBaseClass>(Operator.Subtraction);
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has the operator Subtraction with operand OperatorBaseClass for at most one item,
					             but at least 2 of at least 2 did

					             Matching items:
					             [
					               OperatorBaseClass,
					               OperatorDerivedClass,
					               (… and maybe more)
					             ]

					             Collection:
					             [
					               OperatorBaseClass,
					               OperatorDerivedClass,
					               (… and maybe more)
					             ]
					             """);
			}
		}
#endif
	}
}
