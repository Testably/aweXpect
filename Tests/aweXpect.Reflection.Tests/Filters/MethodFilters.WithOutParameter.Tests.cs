using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WithOutParameter
	{
		private static MethodInfo? MixedParameterMethod()
			=> typeof(ClassWithOutParameterMethod)
				.GetMethod(nameof(ClassWithOutParameterMethod.MethodWithMixedParameters));

		private static MethodInfo? OutParameterMethod()
			=> typeof(ClassWithOutParameterMethod)
				.GetMethod(nameof(ClassWithOutParameterMethod.MethodWithOutParameter));

		private static MethodInfo? PlainMethod()
			=> typeof(ClassWithOutParameterMethod)
				.GetMethod(nameof(ClassWithOutParameterMethod.MethodWithoutModifiers));

		public sealed class Tests
		{
			[Fact]
			public async Task ByName_ShouldIncludeOutModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameter("value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter with name equal to \"value\" and with out modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task Generic_ShouldIncludeOutModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameter<int>();

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with out modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task Generic_WithName_ShouldIncludeOutModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameter<int>("value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of type int and name equal to \"value\" and with out modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task GenericExactly_ShouldIncludeOutModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameterExactly<int>();

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type int and with out modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task GenericExactly_WithName_ShouldIncludeOutModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameterExactly<int>("value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of exact type int and name equal to \"value\" and with out modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForMethodsWithOutParameter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameter();

				await That(methods).Contains(OutParameterMethod());
				await That(methods.GetDescription())
					.IsEqualTo("methods with out parameter in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldIncludeMethodWhenOnlySomeButNotAllParametersAreOutParameters()
			{
				Filtered.Methods methods = In.Type<ClassWithOutParameterMethod>()
					.Methods().WithOutParameter();

				await That(methods).Contains(MixedParameterMethod());
				await That(methods).DoesNotContain(PlainMethod());
			}

			[Fact]
			public async Task ShouldNotIncludeMethodsWithoutOutParameter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameter();

				await That(methods).DoesNotContain(PlainMethod());
			}

#pragma warning disable CA2263
			[Fact]
			public async Task UsingType_ShouldIncludeOutModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameter(typeof(int));

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int and with out modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task UsingType_WithName_ShouldIncludeOutModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameter(typeof(int), "value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of type int and name equal to \"value\" and with out modifier in assembly")
					.AsPrefix();
			}
#pragma warning restore CA2263

#pragma warning disable CA2263
			[Fact]
			public async Task UsingTypeExactly_ShouldIncludeOutModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameterExactly(typeof(int));

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type int and with out modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task UsingTypeExactly_WithName_ShouldIncludeOutModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameterExactly(typeof(int), "value");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of exact type int and name equal to \"value\" and with out modifier in assembly")
					.AsPrefix();
			}
#pragma warning restore CA2263
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithOutParameterMethod
		{
			public void MethodWithOutParameter(out int value) => value = 0;

			public void MethodWithMixedParameters(out int value, int other) => value = 0;

			public void MethodWithoutModifiers(int value)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
