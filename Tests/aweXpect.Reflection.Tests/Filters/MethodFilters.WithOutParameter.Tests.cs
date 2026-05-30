using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WithOutParameter
	{
		public sealed class Tests
		{
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
			public async Task ShouldNotIncludeMethodsWithoutOutParameter()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WithOutParameter();

				await That(methods).DoesNotContain(PlainMethod());
			}
		}

		private static MethodInfo? OutParameterMethod()
			=> typeof(ClassWithOutParameterMethod)
				.GetMethod(nameof(ClassWithOutParameterMethod.MethodWithOutParameter));

		private static MethodInfo? PlainMethod()
			=> typeof(ClassWithOutParameterMethod)
				.GetMethod(nameof(ClassWithOutParameterMethod.MethodWithoutModifiers));

#pragma warning disable CA1822
		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithOutParameterMethod
		{
			public void MethodWithOutParameter(out int value)
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
