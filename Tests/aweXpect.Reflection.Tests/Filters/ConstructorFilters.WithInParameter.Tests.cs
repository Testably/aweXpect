using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithInParameter
	{
		private static ConstructorInfo? InParameterConstructor()
			=> typeof(ClassWithInParameterConstructor).GetConstructors().Single();

		private static ConstructorInfo? PlainConstructor()
			=> typeof(ClassWithPlainConstructor).GetConstructors().Single();

		private static ConstructorInfo? MixedConstructor()
			=> typeof(ClassWithMixedConstructor).GetConstructors().Single();

		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForConstructorsWithInParameter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameter();

				await That(constructors).Contains(InParameterConstructor());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with in parameter in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldNotIncludeConstructorsWithoutInParameter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameter();

				await That(constructors).DoesNotContain(PlainConstructor());
			}

			[Fact]
			public async Task WithType_ShouldFilterForConstructorsWithInParameterOfSpecificType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameter(typeof(int));

				await That(constructors).Contains(InParameterConstructor());
			}

			[Fact]
			public async Task WithType_ShouldNotIncludeConstructorsWithInParameterOfOtherType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameter(typeof(string));

				await That(constructors).DoesNotContain(InParameterConstructor());
			}

			[Fact]
			public async Task WithType_WithName_ShouldFilterForConstructorsWithInParameterOfSpecificTypeAndName()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameter(typeof(int), "value");

				await That(constructors).Contains(InParameterConstructor());
			}
		}

		public sealed class DescriptionTests
		{
			[Fact]
			public async Task ByName_ShouldDescribeWithInModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameter("value");

				await That(constructors.GetDescription()).Contains("with in modifier");
			}

			[Fact]
			public async Task ExactlyOfType_ShouldDescribeWithInModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameterExactly<int>();

				await That(constructors.GetDescription()).Contains("with in modifier");
			}

			[Fact]
			public async Task ExactlyOfTypeWithName_ShouldDescribeWithInModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameterExactly<int>("value");

				await That(constructors.GetDescription()).Contains("with in modifier");
			}

			[Fact]
			public async Task OfType_ShouldDescribeWithInModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameter<int>();

				await That(constructors.GetDescription()).Contains("with in modifier");
			}

			[Fact]
			public async Task OfTypeWithName_ShouldDescribeWithInModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameter<int>("value");

				await That(constructors.GetDescription()).Contains("with in modifier");
			}

			[Fact]
			public async Task ParameterlessFilter_ShouldMatchConstructorWithOnlySomeInParameters()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameter();

				await That(constructors).Contains(MixedConstructor());
			}

#pragma warning disable CA2263
			[Fact]
			public async Task OfType_UsingType_ShouldDescribeWithInModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameter(typeof(int));

				await That(constructors.GetDescription()).Contains("with in modifier");
			}

			[Fact]
			public async Task OfTypeWithName_UsingType_ShouldDescribeWithInModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameter(typeof(int), "value");

				await That(constructors.GetDescription()).Contains("with in modifier");
			}

			[Fact]
			public async Task ExactlyOfType_UsingType_ShouldDescribeWithInModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameterExactly(typeof(int));

				await That(constructors.GetDescription()).Contains("with in modifier");
			}

			[Fact]
			public async Task ExactlyOfTypeWithName_UsingType_ShouldDescribeWithInModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithInParameterExactly(typeof(int), "value");

				await That(constructors.GetDescription()).Contains("with in modifier");
			}
#pragma warning restore CA2263
		}

		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithInParameterConstructor
		{
			public ClassWithInParameterConstructor(in int value)
			{
			}
		}

		private class ClassWithPlainConstructor
		{
			public ClassWithPlainConstructor(int value)
			{
			}
		}

		private class ClassWithMixedConstructor
		{
			public ClassWithMixedConstructor(in int value, string other)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
