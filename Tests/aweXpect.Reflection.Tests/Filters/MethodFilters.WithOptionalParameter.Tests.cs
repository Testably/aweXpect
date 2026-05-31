using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WithOptionalParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForMethodsWithOptionalParameter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameter();

				await That(methods).Contains(OptionalParameterMethod());
				await That(methods.GetDescription())
					.IsEqualTo("methods with optional parameter in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldNotIncludeMethodsWithoutOptionalParameter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameter();

				await That(methods).DoesNotContain(PlainMethod());
			}

			[Fact]
			public async Task ShouldIncludeMethodWhenOnlySomeButNotAllParametersAreOptionalParameters()
			{
				Filtered.Methods methods = In.Type<ClassWithOptionalParameterMethod>()
					.Methods().WithOptionalParameter();

				await That(methods).Contains(MixedParameterMethod());
				await That(methods).DoesNotContain(PlainMethod());
			}

			[Fact]
			public async Task Generic_ShouldIncludeOptionalModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameter<int>();

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with optional modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task Generic_WithName_ShouldIncludeOptionalModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameter<int>("value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of type int and name equal to \"value\" and with optional modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ByName_ShouldIncludeOptionalModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameter("value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter with name equal to \"value\" and with optional modifier in assembly")
					.AsPrefix();
			}

#pragma warning disable CA2263
			[Fact]
			public async Task UsingType_ShouldIncludeOptionalModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameter(typeof(int));

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with optional modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task UsingType_WithName_ShouldIncludeOptionalModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameter(typeof(int), "value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of type int and name equal to \"value\" and with optional modifier in assembly")
					.AsPrefix();
			}
#pragma warning restore CA2263

			[Fact]
			public async Task GenericExactly_ShouldIncludeOptionalModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameterExactly<int>();

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type int and with optional modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task GenericExactly_WithName_ShouldIncludeOptionalModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameterExactly<int>("value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of exact type int and name equal to \"value\" and with optional modifier in assembly")
					.AsPrefix();
			}

#pragma warning disable CA2263
			[Fact]
			public async Task UsingTypeExactly_ShouldIncludeOptionalModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameterExactly(typeof(int));

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type int and with optional modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task UsingTypeExactly_WithName_ShouldIncludeOptionalModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOptionalParameterExactly(typeof(int), "value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of exact type int and name equal to \"value\" and with optional modifier in assembly")
					.AsPrefix();
			}
#pragma warning restore CA2263
		}

		private static MethodInfo? MixedParameterMethod()
			=> typeof(ClassWithOptionalParameterMethod)
				.GetMethod(nameof(ClassWithOptionalParameterMethod.MethodWithMixedParameters));

		private static MethodInfo? OptionalParameterMethod()
			=> typeof(ClassWithOptionalParameterMethod)
				.GetMethod(nameof(ClassWithOptionalParameterMethod.MethodWithOptionalParameter));

		private static MethodInfo? PlainMethod()
			=> typeof(ClassWithOptionalParameterMethod)
				.GetMethod(nameof(ClassWithOptionalParameterMethod.MethodWithoutModifiers));

#pragma warning disable CA1822
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithOptionalParameterMethod
		{
			public void MethodWithOptionalParameter(int value = 0)
			{
			}

			public void MethodWithMixedParameters(int other, int value = 0)
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
