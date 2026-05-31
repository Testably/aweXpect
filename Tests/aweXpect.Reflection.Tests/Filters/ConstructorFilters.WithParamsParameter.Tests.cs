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

			[Fact]
			public async Task WithType_ShouldFilterForConstructorsWithParamsParameterOfSpecificType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameter(typeof(int[]));

				await That(constructors).Contains(ParamsParameterConstructor());
			}

			[Fact]
			public async Task WithType_ShouldNotIncludeConstructorsWithParamsParameterOfOtherType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameter(typeof(string[]));

				await That(constructors).DoesNotContain(ParamsParameterConstructor());
			}

			[Fact]
			public async Task WithType_WithName_ShouldFilterForConstructorsWithParamsParameterOfSpecificTypeAndName()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameter(typeof(int[]), "values");

				await That(constructors).Contains(ParamsParameterConstructor());
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
