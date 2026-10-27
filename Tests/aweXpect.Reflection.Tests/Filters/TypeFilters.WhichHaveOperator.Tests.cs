using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichHaveOperator
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForTypesWithTheOperator()
			{
				Filtered.Types types = In.Types<Money, OperatorDerivedClass, ClassWithoutOperators>()
					.WhichHaveOperator(Operator.Addition);

				await That(types).IsEqualTo([typeof(Money),]).InAnyOrder();
			}

			[Test]
			public async Task ShouldIncludeDescriptionInFailureMessage()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<Money>().Types().WhichHaveOperator(Operator.Addition))
						.DoesNotContain(typeof(Money));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types which have the operator Addition in assembly containing type Money
					             does not contain an item equal to Money,
					             but it contained Money at least once
					             """).AsPrefix();
			}

			[Test]
			public async Task WhenDirectly_ShouldIgnoreInheritedOperators()
			{
				Filtered.Types types = In.Types<OperatorBaseClass, OperatorDerivedClass>()
					.WhichHaveOperator(Operator.Subtraction).Directly();

				await That(types).IsEqualTo([typeof(OperatorBaseClass),]).InAnyOrder();
				await That(types.GetDescription()).IsEqualTo("which declare the operator Subtraction ").AsSuffix();
			}

			[Test]
			public async Task WhenOperandDoesNotMatch_ShouldFilterOutType()
			{
				Filtered.Types types = In.Type<Money>()
					.WhichHaveOperator<decimal>(Operator.Addition);

				await That(types).IsEmpty();
			}

			[Test]
			public async Task WhenOperandIsSpecifiedAtRuntime_ShouldFilterForTypesWithTheOperand()
			{
				Type operand = typeof(int);

				Filtered.Types types = In.Types<Money, OperatorDerivedClass, ClassWithoutOperators>()
					.WhichHaveOperator(Operator.Addition, operand);

				await That(types).IsEqualTo([typeof(Money),]).InAnyOrder();
				await That(types.GetDescription())
					.IsEqualTo("which have the operator Addition with operand int ").AsSuffix();
			}

			[Test]
			public async Task WhenOperandMatches_ShouldFilterForTypesWithTheOperand()
			{
				Filtered.Types types = In.Types<Money, OperatorDerivedClass, ClassWithoutOperators>()
					.WhichHaveOperator<int>(Operator.Addition);

				await That(types).IsEqualTo([typeof(Money),]).InAnyOrder();
			}

			[Test]
			public async Task WhenOperatorIsOnlyInherited_ShouldIncludeType()
			{
				Filtered.Types types = In.Types<OperatorBaseClass, OperatorDerivedClass>()
					.WhichHaveOperator(Operator.Subtraction);

				await That(types).IsEqualTo([typeof(OperatorBaseClass), typeof(OperatorDerivedClass),])
					.InAnyOrder();
			}
		}
	}

	public sealed class WhichDoNotHaveOperator
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForTypesWithoutTheOperator()
			{
				Filtered.Types types = In.Types<Money, OperatorDerivedClass, ClassWithoutOperators>()
					.WhichDoNotHaveOperator(Operator.Addition);

				await That(types).IsEqualTo([typeof(OperatorDerivedClass), typeof(ClassWithoutOperators),])
					.InAnyOrder();
			}

			[Test]
			public async Task ShouldIncludeDescriptionInFailureMessage()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<Money>().Types().WhichDoNotHaveOperator(Operator.Addition))
						.DoesNotContain(typeof(ClassWithoutOperators));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types which do not have the operator Addition in assembly containing type Money
					             does not contain an item equal to ClassWithoutOperators,
					             but it contained ClassWithoutOperators at least once
					             """).AsPrefix();
			}

			[Test]
			public async Task WhenDirectly_ShouldKeepTypesWithOnlyInheritedOperators()
			{
				Filtered.Types types = In.Types<OperatorBaseClass, OperatorDerivedClass>()
					.WhichDoNotHaveOperator(Operator.Subtraction).Directly();

				await That(types).IsEqualTo([typeof(OperatorDerivedClass),]).InAnyOrder();
				await That(types.GetDescription()).IsEqualTo("which do not declare the operator Subtraction ").AsSuffix();
			}

			[Test]
			public async Task WhenOperandDoesNotMatch_ShouldKeepType()
			{
				Filtered.Types types = In.Type<Money>()
					.WhichDoNotHaveOperator<decimal>(Operator.Addition);

				await That(types).IsEqualTo([typeof(Money),]).InAnyOrder();
			}

			[Test]
			public async Task WhenOperandIsSpecifiedAtRuntime_ShouldFilterOutTypesWithTheOperand()
			{
				Type operand = typeof(int);

				Filtered.Types types = In.Types<Money, ClassWithoutOperators>()
					.WhichDoNotHaveOperator(Operator.Addition, operand);

				await That(types).IsEqualTo([typeof(ClassWithoutOperators),]).InAnyOrder();
				await That(types.GetDescription())
					.IsEqualTo("which do not have the operator Addition with operand int ").AsSuffix();
			}

			[Test]
			public async Task WhenOperandMatches_ShouldFilterOutType()
			{
				Filtered.Types types = In.Type<Money>()
					.WhichDoNotHaveOperator<int>(Operator.Addition);

				await That(types).IsEmpty();
			}

			[Test]
			public async Task WhenOperatorIsOnlyInherited_ShouldFilterOutType()
			{
				Filtered.Types types = In.Types<OperatorDerivedClass, ClassWithoutOperators>()
					.WhichDoNotHaveOperator(Operator.Subtraction);

				await That(types).IsEqualTo([typeof(ClassWithoutOperators),]).InAnyOrder();
			}
		}
	}
}
