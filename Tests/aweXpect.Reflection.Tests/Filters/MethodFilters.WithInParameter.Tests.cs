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
		}

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

			public void MethodWithoutModifiers(int value)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
#pragma warning restore CA1822
	}
}
