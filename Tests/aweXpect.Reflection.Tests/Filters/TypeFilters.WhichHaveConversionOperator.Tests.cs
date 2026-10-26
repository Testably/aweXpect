using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichHaveImplicitConversionOperator
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForTypesWithTheConversion()
			{
				Filtered.Types types = In.Types<Money, OperatorBaseClass, ClassWithoutOperators>()
					.WhichHaveImplicitConversionOperator<Money, decimal>();

				await That(types).IsEqualTo([typeof(Money),]).InAnyOrder();
			}

			[Test]
			public async Task ShouldIncludeDescriptionInFailureMessage()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<Money>().Types().WhichHaveImplicitConversionOperator<Money, decimal>())
						.DoesNotContain(typeof(Money));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types which have an implicit conversion operator from Money to decimal in assembly containing type Money
					             does not contain an item equal to Money,
					             but it contained Money at least once
					             """).AsPrefix();
			}

			[Test]
			public async Task WhenConversionIsExplicit_ShouldFilterOutType()
			{
				Filtered.Types types = In.Type<Money>()
					.WhichHaveImplicitConversionOperator<Money, int>();

				await That(types).IsEmpty();
			}

			[Test]
			public async Task WhenConversionIsOnlyInherited_Directly_ShouldFilterOutType()
			{
				Filtered.Types types = In.Types<OperatorBaseClass, OperatorDerivedClass>()
					.WhichHaveImplicitConversionOperator<OperatorBaseClass, int>().Directly();

				await That(types).IsEqualTo([typeof(OperatorBaseClass),]).InAnyOrder();
				await That(types.GetDescription())
					.IsEqualTo("which declare an implicit conversion operator from OperatorBaseClass to int ").AsSuffix();
			}

			[Test]
			public async Task WhenConversionIsOnlyInherited_ShouldIncludeType()
			{
				Filtered.Types types = In.Types<OperatorBaseClass, OperatorDerivedClass>()
					.WhichHaveImplicitConversionOperator(typeof(OperatorBaseClass), typeof(int));

				await That(types).IsEqualTo([typeof(OperatorBaseClass), typeof(OperatorDerivedClass),])
					.InAnyOrder();
			}
		}
	}

	public sealed class WhichDoNotHaveImplicitConversionOperator
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForTypesWithoutTheConversion()
			{
				Filtered.Types types = In.Types<Money, OperatorBaseClass, ClassWithoutOperators>()
					.WhichDoNotHaveImplicitConversionOperator<Money, decimal>();

				await That(types).IsEqualTo([typeof(OperatorBaseClass), typeof(ClassWithoutOperators),])
					.InAnyOrder();
			}

			[Test]
			public async Task ShouldIncludeDescriptionInFailureMessage()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<Money>().Types().WhichDoNotHaveImplicitConversionOperator<Money, decimal>())
						.DoesNotContain(typeof(ClassWithoutOperators));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types which do not have an implicit conversion operator from Money to decimal in assembly containing type Money
					             does not contain an item equal to ClassWithoutOperators,
					             but it contained ClassWithoutOperators at least once
					             """).AsPrefix();
			}

			[Test]
			public async Task WhenConversionIsOnlyInherited_Directly_ShouldKeepType()
			{
				Filtered.Types types = In.Types<OperatorBaseClass, OperatorDerivedClass>()
					.WhichDoNotHaveImplicitConversionOperator<OperatorBaseClass, int>().Directly();

				await That(types).IsEqualTo([typeof(OperatorDerivedClass),]).InAnyOrder();
				await That(types.GetDescription())
					.IsEqualTo("which do not declare an implicit conversion operator from OperatorBaseClass to int ").AsSuffix();
			}

			[Test]
			public async Task WhenConversionIsOnlyInherited_ShouldFilterOutType()
			{
				Filtered.Types types = In.Types<OperatorDerivedClass, ClassWithoutOperators>()
					.WhichDoNotHaveImplicitConversionOperator(typeof(OperatorBaseClass), typeof(int));

				await That(types).IsEqualTo([typeof(ClassWithoutOperators),]).InAnyOrder();
			}
		}
	}

	public sealed class WhichHaveExplicitConversionOperator
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForTypesWithTheConversion()
			{
				Filtered.Types types = In.Types<Money, OperatorBaseClass, ClassWithoutOperators>()
					.WhichHaveExplicitConversionOperator<Money, int>();

				await That(types).IsEqualTo([typeof(Money),]).InAnyOrder();
			}

			[Test]
			public async Task ShouldIncludeDescriptionInFailureMessage()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<Money>().Types().WhichHaveExplicitConversionOperator<Money, int>())
						.DoesNotContain(typeof(Money));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types which have an explicit conversion operator from Money to int in assembly containing type Money
					             does not contain an item equal to Money,
					             but it contained Money at least once
					             """).AsPrefix();
			}

			[Test]
			public async Task WhenConversionIsImplicit_ShouldFilterOutType()
			{
				Filtered.Types types = In.Type<Money>()
					.WhichHaveExplicitConversionOperator(typeof(Money), typeof(decimal));

				await That(types).IsEmpty();
			}

			[Test]
			public async Task WhenDirectly_ShouldIncludeTypeDeclaringTheConversion()
			{
				Filtered.Types types = In.Types<Money, ClassWithoutOperators>()
					.WhichHaveExplicitConversionOperator<Money, int>().Directly();

				await That(types).IsEqualTo([typeof(Money),]).InAnyOrder();
				await That(types.GetDescription())
					.IsEqualTo("which declare an explicit conversion operator from Money to int ").AsSuffix();
			}
		}
	}

	public sealed class WhichDoNotHaveExplicitConversionOperator
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForTypesWithoutTheConversion()
			{
				Filtered.Types types = In.Types<Money, OperatorBaseClass, ClassWithoutOperators>()
					.WhichDoNotHaveExplicitConversionOperator<Money, int>();

				await That(types).IsEqualTo([typeof(OperatorBaseClass), typeof(ClassWithoutOperators),])
					.InAnyOrder();
			}

			[Test]
			public async Task ShouldIncludeDescriptionInFailureMessage()
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<Money>().Types().WhichDoNotHaveExplicitConversionOperator<Money, int>())
						.DoesNotContain(typeof(ClassWithoutOperators));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types which do not have an explicit conversion operator from Money to int in assembly containing type Money
					             does not contain an item equal to ClassWithoutOperators,
					             but it contained ClassWithoutOperators at least once
					             """).AsPrefix();
			}

			[Test]
			public async Task WhenConversionIsImplicit_ShouldKeepType()
			{
				Filtered.Types types = In.Type<Money>()
					.WhichDoNotHaveExplicitConversionOperator(typeof(Money), typeof(decimal));

				await That(types).IsEqualTo([typeof(Money),]).InAnyOrder();
			}

			[Test]
			public async Task WhenDirectly_ShouldFilterOutTypeDeclaringTheConversion()
			{
				Filtered.Types types = In.Types<Money, ClassWithoutOperators>()
					.WhichDoNotHaveExplicitConversionOperator<Money, int>().Directly();

				await That(types).IsEqualTo([typeof(ClassWithoutOperators),]).InAnyOrder();
				await That(types.GetDescription())
					.IsEqualTo("which do not declare an explicit conversion operator from Money to int ").AsSuffix();
			}
		}
	}
}
