using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WithParamsParameter
	{
		public sealed class Tests
		{
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
			public async Task ShouldFilterForMethodsWithParamsCollectionParameter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameter();

				await That(methods).Contains(ParamsCollectionParameterMethod());
			}

			[Fact]
			public async Task ShouldNotIncludeMethodsWithoutParamsParameter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithParamsParameter();

				await That(methods).DoesNotContain(PlainMethod());
			}
		}

		private static MethodInfo? ParamsParameterMethod()
			=> typeof(ClassWithParamsParameterMethod)
				.GetMethod(nameof(ClassWithParamsParameterMethod.MethodWithParamsParameter));

		private static MethodInfo? ParamsCollectionParameterMethod()
			=> typeof(ClassWithParamsParameterMethod)
				.GetMethod(nameof(ClassWithParamsParameterMethod.MethodWithParamsCollectionParameter));

		private static MethodInfo? PlainMethod()
			=> typeof(ClassWithParamsParameterMethod)
				.GetMethod(nameof(ClassWithParamsParameterMethod.MethodWithoutModifiers));

#pragma warning disable CA1822
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithParamsParameterMethod
		{
			public void MethodWithParamsParameter(params int[] values)
			{
			}

			public void MethodWithParamsCollectionParameter(params System.Collections.Generic.List<int> values)
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
