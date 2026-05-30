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
		}

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

			public void MethodWithoutModifiers(int value)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
