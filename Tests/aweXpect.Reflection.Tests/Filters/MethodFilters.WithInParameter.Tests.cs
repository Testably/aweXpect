using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WithInParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForMethodsWithInParameter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameter();

				await That(methods).Contains(InParameterMethod());
				await That(methods.GetDescription())
					.IsEqualTo("methods with in parameter in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldNotIncludeMethodsWithoutInParameter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameter();

				await That(methods).DoesNotContain(PlainMethod());
			}

			[Fact]
			public async Task ShouldIncludeMethodWhenOnlySomeButNotAllParametersAreInParameters()
			{
				Filtered.Methods methods = In.Type<ClassWithInParameterMethod>()
					.Methods().WithInParameter();

				await That(methods).Contains(MixedParameterMethod());
				await That(methods).DoesNotContain(PlainMethod());
			}

			[Fact]
			public async Task Generic_ShouldIncludeInModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameter<int>();

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with in modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task Generic_WithName_ShouldIncludeInModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameter<int>("value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of type int and name equal to \"value\" and with in modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ByName_ShouldIncludeInModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameter("value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter with name equal to \"value\" and with in modifier in assembly")
					.AsPrefix();
			}

#pragma warning disable CA2263
			[Fact]
			public async Task UsingType_ShouldIncludeInModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameter(typeof(int));

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with in modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task UsingType_WithName_ShouldIncludeInModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameter(typeof(int), "value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of type int and name equal to \"value\" and with in modifier in assembly")
					.AsPrefix();
			}
#pragma warning restore CA2263

			[Fact]
			public async Task GenericExactly_ShouldIncludeInModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameterExactly<int>();

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type int and with in modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task GenericExactly_WithName_ShouldIncludeInModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameterExactly<int>("value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of exact type int and name equal to \"value\" and with in modifier in assembly")
					.AsPrefix();
			}

#pragma warning disable CA2263
			[Fact]
			public async Task UsingTypeExactly_ShouldIncludeInModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameterExactly(typeof(int));

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type int and with in modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task UsingTypeExactly_WithName_ShouldIncludeInModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithInParameterExactly(typeof(int), "value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of exact type int and name equal to \"value\" and with in modifier in assembly")
					.AsPrefix();
			}
#pragma warning restore CA2263
		}

		private static MethodInfo? MixedParameterMethod()
			=> typeof(ClassWithInParameterMethod)
				.GetMethod(nameof(ClassWithInParameterMethod.MethodWithMixedParameters));

		private static MethodInfo? InParameterMethod()
			=> typeof(ClassWithInParameterMethod)
				.GetMethod(nameof(ClassWithInParameterMethod.MethodWithInParameter));

		private static MethodInfo? PlainMethod()
			=> typeof(ClassWithInParameterMethod)
				.GetMethod(nameof(ClassWithInParameterMethod.MethodWithoutModifiers));

#pragma warning disable CA1822
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithInParameterMethod
		{
			public void MethodWithInParameter(in int value)
			{
			}

			public void MethodWithMixedParameters(in int value, int other)
			{
			}

			public void MethodWithoutModifiers(int value)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
