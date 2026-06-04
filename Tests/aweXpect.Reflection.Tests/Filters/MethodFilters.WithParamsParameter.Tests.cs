using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WithParamsParameter
	{
		private static MethodInfo? MixedParameterMethod()
			=> typeof(ClassWithParamsParameterMethod)
				.GetMethod(nameof(ClassWithParamsParameterMethod.MethodWithMixedParameters));

		private static MethodInfo? ParamsParameterMethod()
			=> typeof(ClassWithParamsParameterMethod)
				.GetMethod(nameof(ClassWithParamsParameterMethod.MethodWithParamsParameter));

		private static MethodInfo? ParamsCollectionParameterMethod()
			=> typeof(ClassWithParamsParameterMethod)
				.GetMethod(nameof(ClassWithParamsParameterMethod.MethodWithParamsCollectionParameter));

		private static MethodInfo? PlainMethod()
			=> typeof(ClassWithParamsParameterMethod)
				.GetMethod(nameof(ClassWithParamsParameterMethod.MethodWithoutModifiers));

		public sealed class Tests
		{
			[Fact]
			public async Task ByName_ShouldIncludeParamsModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameter("values");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter with name equal to \"values\" and with params modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task Generic_ShouldIncludeParamsModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameter<int[]>();

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int[] and with params modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task Generic_WithName_ShouldIncludeParamsModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameter<int[]>("values");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of type int[] and name equal to \"values\" and with params modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task GenericExactly_ShouldIncludeParamsModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameterExactly<int[]>();

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type int[] and with params modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task GenericExactly_WithName_ShouldIncludeParamsModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameterExactly<int[]>("values");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of exact type int[] and name equal to \"values\" and with params modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForMethodsWithParamsCollectionParameter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameter();

				await That(methods).Contains(ParamsCollectionParameterMethod());
			}

			[Fact]
			public async Task ShouldFilterForMethodsWithParamsParameter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameter();

				await That(methods).Contains(ParamsParameterMethod());
				await That(methods.GetDescription())
					.IsEqualTo("methods with params parameter in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldIncludeMethodWhenOnlySomeButNotAllParametersAreParamsParameters()
			{
				Filtered.Methods methods = In.Type<ClassWithParamsParameterMethod>()
					.Methods().WithParamsParameter();

				await That(methods).Contains(MixedParameterMethod());
				await That(methods).DoesNotContain(PlainMethod());
			}

			[Fact]
			public async Task ShouldNotIncludeMethodsWithoutParamsParameter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameter();

				await That(methods).DoesNotContain(PlainMethod());
			}

#pragma warning disable CA2263
			[Fact]
			public async Task UsingType_ShouldIncludeParamsModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameter(typeof(int[]));

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of type int[] and with params modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task UsingType_WithName_ShouldIncludeParamsModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameter(typeof(int[]), "values");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of type int[] and name equal to \"values\" and with params modifier in assembly")
					.AsPrefix();
			}
#pragma warning restore CA2263

#pragma warning disable CA2263
			[Fact]
			public async Task UsingTypeExactly_ShouldIncludeParamsModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameterExactly(typeof(int[]));

				await That(methods.GetDescription())
					.IsEqualTo("methods with parameter of exact type int[] and with params modifier in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task UsingTypeExactly_WithName_ShouldIncludeParamsModifierInDescription()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameterExactly(typeof(int[]), "values");

				await That(methods.GetDescription())
					.IsEqualTo(
						"methods with parameter of exact type int[] and name equal to \"values\" and with params modifier in assembly")
					.AsPrefix();
			}
#pragma warning restore CA2263
		}

#pragma warning disable CA1822
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithParamsParameterMethod
		{
			public void MethodWithParamsParameter(params int[] values)
			{
			}

			public void MethodWithParamsCollectionParameter(params List<int> values)
			{
			}

			public void MethodWithMixedParameters(int other, params int[] values)
			{
			}

			public void MethodWithoutModifiers(int[] values)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
