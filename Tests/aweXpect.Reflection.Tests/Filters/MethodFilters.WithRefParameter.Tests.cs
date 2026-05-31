using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WithRefParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForMethodsWithRefParameter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter();

				await That(methods).Contains(RefParameterMethod());
				await That(methods.GetDescription())
					.IsEqualTo("methods with ref parameter in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldNotIncludeMethodsWithoutRefParameter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter();

				await That(methods).DoesNotContain(PlainMethod());
			}

			[Fact]
			public async Task ShouldIncludeMethodWhenOnlySomeButNotAllParametersAreRefParameters()
			{
				Filtered.Methods methods = In.Type<ClassWithRefParameterMethod>()
					.Methods().WithRefParameter();

				await That(methods).Contains(MixedParameterMethod());
				await That(methods).DoesNotContain(PlainMethod());
			}

			[Fact]
			public async Task Generic_ShouldIncludeRefModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter<int>();

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with ref modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task Generic_WithName_ShouldIncludeRefModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter<int>("value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of type int and name equal to \"value\" and with ref modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ByName_ShouldIncludeRefModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter("value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter with name equal to \"value\" and with ref modifier in assembly")
					.AsPrefix();
			}

#pragma warning disable CA2263
			[Fact]
			public async Task UsingType_ShouldIncludeRefModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter(typeof(int));

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with ref modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task UsingType_WithName_ShouldIncludeRefModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameter(typeof(int), "value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of type int and name equal to \"value\" and with ref modifier in assembly")
					.AsPrefix();
			}
#pragma warning restore CA2263

			[Fact]
			public async Task GenericExactly_ShouldIncludeRefModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameterExactly<int>();

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type int and with ref modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task GenericExactly_WithName_ShouldIncludeRefModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameterExactly<int>("value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of exact type int and name equal to \"value\" and with ref modifier in assembly")
					.AsPrefix();
			}

#pragma warning disable CA2263
			[Fact]
			public async Task UsingTypeExactly_ShouldIncludeRefModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameterExactly(typeof(int));

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type int and with ref modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task UsingTypeExactly_WithName_ShouldIncludeRefModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithRefParameterExactly(typeof(int), "value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of exact type int and name equal to \"value\" and with ref modifier in assembly")
					.AsPrefix();
			}
#pragma warning restore CA2263
		}

		private static MethodInfo? MixedParameterMethod()
			=> typeof(ClassWithRefParameterMethod)
				.GetMethod(nameof(ClassWithRefParameterMethod.MethodWithMixedParameters));

		private static MethodInfo? RefParameterMethod()
			=> typeof(ClassWithRefParameterMethod)
				.GetMethod(nameof(ClassWithRefParameterMethod.MethodWithRefParameter));

		private static MethodInfo? PlainMethod()
			=> typeof(ClassWithRefParameterMethod)
				.GetMethod(nameof(ClassWithRefParameterMethod.MethodWithoutModifiers));

#pragma warning disable CA1822
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithRefParameterMethod
		{
			public void MethodWithRefParameter(ref int value)
			{
				value = 0;
			}

			public void MethodWithMixedParameters(ref int value, int other)
			{
				value = 0;
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
