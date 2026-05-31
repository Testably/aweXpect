using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithRefParameter
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForConstructorsWithRefParameter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter();

				await That(constructors).Contains(RefParameterConstructor());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with ref parameter in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldNotIncludeConstructorsWithoutRefParameter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter();

				await That(constructors).DoesNotContain(PlainConstructor());
			}

			[Fact]
			public async Task WithType_ShouldFilterForConstructorsWithRefParameterOfSpecificType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter(typeof(int));

				await That(constructors).Contains(RefParameterConstructor());
			}

			[Fact]
			public async Task WithType_ShouldNotIncludeConstructorsWithRefParameterOfOtherType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter(typeof(string));

				await That(constructors).DoesNotContain(RefParameterConstructor());
			}

			[Fact]
			public async Task WithType_WithName_ShouldFilterForConstructorsWithRefParameterOfSpecificTypeAndName()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter(typeof(int), "value");

				await That(constructors).Contains(RefParameterConstructor());
			}
		}

		public sealed class DescriptionTests
		{
			[Fact]
			public async Task ParameterlessFilter_ShouldMatchConstructorWithOnlySomeRefParameters()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter();

				await That(constructors).Contains(MixedConstructor());
			}

			[Fact]
			public async Task OfType_ShouldDescribeWithRefModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter<int>();

				await That(constructors.GetDescription()).Contains("with ref modifier");
			}

			[Fact]
			public async Task OfTypeWithName_ShouldDescribeWithRefModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter<int>("value");

				await That(constructors.GetDescription()).Contains("with ref modifier");
			}

			[Fact]
			public async Task ByName_ShouldDescribeWithRefModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter("value");

				await That(constructors.GetDescription()).Contains("with ref modifier");
			}

			[Fact]
			public async Task ExactlyOfType_ShouldDescribeWithRefModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameterExactly<int>();

				await That(constructors.GetDescription()).Contains("with ref modifier");
			}

			[Fact]
			public async Task ExactlyOfTypeWithName_ShouldDescribeWithRefModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameterExactly<int>("value");

				await That(constructors.GetDescription()).Contains("with ref modifier");
			}

#pragma warning disable CA2263
			[Fact]
			public async Task OfType_UsingType_ShouldDescribeWithRefModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter(typeof(int));

				await That(constructors.GetDescription()).Contains("with ref modifier");
			}

			[Fact]
			public async Task OfTypeWithName_UsingType_ShouldDescribeWithRefModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameter(typeof(int), "value");

				await That(constructors.GetDescription()).Contains("with ref modifier");
			}

			[Fact]
			public async Task ExactlyOfType_UsingType_ShouldDescribeWithRefModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameterExactly(typeof(int));

				await That(constructors.GetDescription()).Contains("with ref modifier");
			}

			[Fact]
			public async Task ExactlyOfTypeWithName_UsingType_ShouldDescribeWithRefModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithRefParameterExactly(typeof(int), "value");

				await That(constructors.GetDescription()).Contains("with ref modifier");
			}
#pragma warning restore CA2263
		}

		private static ConstructorInfo? RefParameterConstructor()
			=> typeof(ClassWithRefParameterConstructor).GetConstructors().Single();

		private static ConstructorInfo? PlainConstructor()
			=> typeof(ClassWithPlainConstructor).GetConstructors().Single();

		private static ConstructorInfo? MixedConstructor()
			=> typeof(ClassWithMixedConstructor).GetConstructors().Single();

		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithRefParameterConstructor
		{
			public ClassWithRefParameterConstructor(ref int value)
			{
				value = 0;
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
			public ClassWithMixedConstructor(ref int value, string other)
			{
				value = 0;
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
