using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WithParameterModifier
	{
		private static MethodInfo? RefIntMethod()
			=> typeof(ClassWithModifierParameters).GetMethod(nameof(ClassWithModifierParameters.RefInt));

		private static MethodInfo? OutIntMethod()
			=> typeof(ClassWithModifierParameters).GetMethod(nameof(ClassWithModifierParameters.OutInt));

		private static MethodInfo? InIntMethod()
			=> typeof(ClassWithModifierParameters).GetMethod(nameof(ClassWithModifierParameters.InInt));

		private static MethodInfo? OptionalIntMethod()
			=> typeof(ClassWithModifierParameters).GetMethod(nameof(ClassWithModifierParameters.OptionalInt));

		private static MethodInfo? ParamsIntMethod()
			=> typeof(ClassWithModifierParameters).GetMethod(nameof(ClassWithModifierParameters.ParamsInt));

		private static MethodInfo? PlainIntMethod()
			=> typeof(ClassWithModifierParameters).GetMethod(nameof(ClassWithModifierParameters.PlainInt));

		public sealed class Tests
		{
			[Test]
			public async Task WithInModifier_ShouldFilterForInParameterOfType()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParameter<int>().WithInModifier();

				await That(methods).Contains(InIntMethod());
				await That(methods).DoesNotContain(PlainIntMethod());
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with in modifier in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task WithOptionalModifier_ShouldFilterForOptionalParameterOfType()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParameter<int>().WithOptionalModifier();

				await That(methods).Contains(OptionalIntMethod());
				await That(methods).DoesNotContain(PlainIntMethod());
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with optional modifier in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task WithOutModifier_ShouldFilterForOutParameterOfType()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParameter<int>().WithOutModifier();

				await That(methods).Contains(OutIntMethod());
				await That(methods).DoesNotContain(PlainIntMethod());
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with out modifier in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task WithParamsModifier_ShouldFilterForParamsParameterOfType()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParameter<int[]>().WithParamsModifier();

				await That(methods).Contains(ParamsIntMethod());
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int[] and with params modifier in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task WithRefModifier_ShouldFilterForRefParameterOfType()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParameter<int>().WithRefModifier();

				await That(methods).Contains(RefIntMethod());
				await That(methods).DoesNotContain(PlainIntMethod());
				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with ref modifier in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task WithRefModifier_ShouldNotChangeTheFilterItWasCalledOn()
			{
				Reflection.MethodFilters.MethodsWithParameter<int> withInt = In.Type<ClassWithModifierParameters>()
					.Methods().WithParameter<int>();

				Filtered.Methods withRef = withInt.WithRefModifier();

				await That(withInt).Contains(PlainIntMethod())
					.Because("WithRefModifier must return a new filter instead of changing the one it was called on");
				await That(withInt.GetDescription())
					.IsEqualTo("methods with parameter of type int in type").AsPrefix();
				await That(withRef).Contains(RefIntMethod());
				await That(withRef).DoesNotContain(PlainIntMethod());
			}
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithModifierParameters
		{
			public void RefInt(ref int value) => value = 0;

			public void OutInt(out int value) => value = 0;

			public void InInt(in int value)
			{
			}

			public void OptionalInt(int value = 0)
			{
			}

			public void ParamsInt(params int[] values)
			{
			}

			public void PlainInt(int value)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
