using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithParameterModifier
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WithRefModifier_ShouldFilterForRefParameterOfType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParameter<int>().WithRefModifier();

				await That(constructors).Contains(RefIntConstructor());
				await That(constructors).DoesNotContain(PlainIntConstructor());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of type int and with ref modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task WithOutModifier_ShouldFilterForOutParameterOfType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParameter<int>().WithOutModifier();

				await That(constructors).Contains(OutIntConstructor());
				await That(constructors).DoesNotContain(PlainIntConstructor());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of type int and with out modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task WithInModifier_ShouldFilterForInParameterOfType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParameter<int>().WithInModifier();

				await That(constructors).Contains(InIntConstructor());
				await That(constructors).DoesNotContain(PlainIntConstructor());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of type int and with in modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task WithOptionalModifier_ShouldFilterForOptionalParameterOfType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParameter<int>().WithOptionalModifier();

				await That(constructors).Contains(OptionalIntConstructor());
				await That(constructors).DoesNotContain(PlainIntConstructor());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of type int and with optional modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task WithParamsModifier_ShouldFilterForParamsParameterOfType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParameter<int[]>().WithParamsModifier();

				await That(constructors).Contains(ParamsIntConstructor());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with parameter of type int[] and with params modifier in assembly")
					.AsPrefix();
			}
		}

		private static ConstructorInfo? RefIntConstructor()
			=> typeof(ClassWithRefIntConstructor).GetConstructors().Single();

		private static ConstructorInfo? OutIntConstructor()
			=> typeof(ClassWithOutIntConstructor).GetConstructors().Single();

		private static ConstructorInfo? InIntConstructor()
			=> typeof(ClassWithInIntConstructor).GetConstructors().Single();

		private static ConstructorInfo? OptionalIntConstructor()
			=> typeof(ClassWithOptionalIntConstructor).GetConstructors().Single();

		private static ConstructorInfo? ParamsIntConstructor()
			=> typeof(ClassWithParamsIntConstructor).GetConstructors().Single();

		private static ConstructorInfo? PlainIntConstructor()
			=> typeof(ClassWithPlainIntConstructor).GetConstructors().Single();

		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithRefIntConstructor
		{
			public ClassWithRefIntConstructor(ref int value)
			{
				value = 0;
			}
		}

		private class ClassWithOutIntConstructor
		{
			public ClassWithOutIntConstructor(out int value)
			{
				value = 0;
			}
		}

		private class ClassWithInIntConstructor
		{
			public ClassWithInIntConstructor(in int value)
			{
			}
		}

		private class ClassWithOptionalIntConstructor
		{
			public ClassWithOptionalIntConstructor(int value = 0)
			{
			}
		}

		private class ClassWithParamsIntConstructor
		{
			public ClassWithParamsIntConstructor(params int[] values)
			{
			}
		}

		private class ClassWithPlainIntConstructor
		{
			public ClassWithPlainIntConstructor(int value)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
