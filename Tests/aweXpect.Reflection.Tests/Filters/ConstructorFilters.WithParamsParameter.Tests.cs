using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithParamsParameter
	{
		private static ConstructorInfo? ParamsParameterConstructor()
			=> typeof(ClassWithParamsParameterConstructor).GetConstructors().Single();

		private static ConstructorInfo? PlainConstructor()
			=> typeof(ClassWithPlainConstructor).GetConstructors().Single();

		private static ConstructorInfo? MixedConstructor()
			=> typeof(ClassWithMixedConstructor).GetConstructors().Single();

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

		public sealed class DescriptionTests
		{
			[Fact]
			public async Task ByName_ShouldDescribeWithParamsModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameter("values");

				await That(constructors.GetDescription()).Contains("with params modifier");
			}

			[Fact]
			public async Task ExactlyOfType_ShouldDescribeWithParamsModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameterExactly<int[]>();

				await That(constructors.GetDescription()).Contains("with params modifier");
			}

			[Fact]
			public async Task ExactlyOfTypeWithName_ShouldDescribeWithParamsModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameterExactly<int[]>("values");

				await That(constructors.GetDescription()).Contains("with params modifier");
			}

			[Fact]
			public async Task OfType_ShouldDescribeWithParamsModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameter<int[]>();

				await That(constructors.GetDescription()).Contains("with params modifier");
			}

			[Fact]
			public async Task OfTypeWithName_ShouldDescribeWithParamsModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameter<int[]>("values");

				await That(constructors.GetDescription()).Contains("with params modifier");
			}

			[Fact]
			public async Task ParameterlessFilter_ShouldMatchConstructorWithOnlySomeParamsParameters()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameter();

				await That(constructors).Contains(MixedConstructor());
			}

#pragma warning disable CA2263
			[Fact]
			public async Task OfType_UsingType_ShouldDescribeWithParamsModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameter(typeof(int[]));

				await That(constructors.GetDescription()).Contains("with params modifier");
			}

			[Fact]
			public async Task OfTypeWithName_UsingType_ShouldDescribeWithParamsModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameter(typeof(int[]), "values");

				await That(constructors.GetDescription()).Contains("with params modifier");
			}

			[Fact]
			public async Task ExactlyOfType_UsingType_ShouldDescribeWithParamsModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameterExactly(typeof(int[]));

				await That(constructors.GetDescription()).Contains("with params modifier");
			}

			[Fact]
			public async Task ExactlyOfTypeWithName_UsingType_ShouldDescribeWithParamsModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParamsParameterExactly(typeof(int[]), "values");

				await That(constructors.GetDescription()).Contains("with params modifier");
			}
#pragma warning restore CA2263
		}

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

		private class ClassWithMixedConstructor
		{
			public ClassWithMixedConstructor(string other, params int[] values)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
