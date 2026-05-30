using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithParamsParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForConstructorsWithParamsParameter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameter();

				await That(constructors).Contains(ParamsParameterConstructor());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with params parameter in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldNotIncludeConstructorsWithoutParamsParameter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameter();

				await That(constructors).DoesNotContain(PlainConstructor());
			}
		}

		private static ConstructorInfo? ParamsParameterConstructor()
			=> typeof(ClassWithParamsParameterConstructor).GetConstructors().Single();

		private static ConstructorInfo? PlainConstructor()
			=> typeof(ClassWithPlainConstructor).GetConstructors().Single();

		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithParamsParameterConstructor
		{
			public ClassWithParamsParameterConstructor(params int[] values)
			{
			}
		}

		private class ClassWithPlainConstructor
		{
			public ClassWithPlainConstructor(int[] values)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
