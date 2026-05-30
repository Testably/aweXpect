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
		}

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

			public void MethodWithoutModifiers(int value)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
