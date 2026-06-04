using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithOptionalParameter
	{
		private static ConstructorInfo? OptionalParameterConstructor()
			=> typeof(ClassWithOptionalParameterConstructor).GetConstructors().Single();

		private static ConstructorInfo? PlainConstructor()
			=> typeof(ClassWithPlainConstructor).GetConstructors().Single();

		private static ConstructorInfo? MixedConstructor()
			=> typeof(ClassWithMixedConstructor).GetConstructors().Single();

		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForConstructorsWithOptionalParameter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameter();

				await That(constructors).Contains(OptionalParameterConstructor());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with optional parameter in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldNotIncludeConstructorsWithoutOptionalParameter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameter();

				await That(constructors).DoesNotContain(PlainConstructor());
			}

			[Fact]
			public async Task WithType_ShouldFilterForConstructorsWithOptionalParameterOfSpecificType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameter(typeof(int));

				await That(constructors).Contains(OptionalParameterConstructor());
			}

			[Fact]
			public async Task WithType_ShouldNotIncludeConstructorsWithOptionalParameterOfOtherType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameter(typeof(string));

				await That(constructors).DoesNotContain(OptionalParameterConstructor());
			}

			[Fact]
			public async Task WithType_WithName_ShouldFilterForConstructorsWithOptionalParameterOfSpecificTypeAndName()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameter(typeof(int), "value");

				await That(constructors).Contains(OptionalParameterConstructor());
			}
		}

		public sealed class DescriptionTests
		{
			[Fact]
			public async Task ByName_ShouldDescribeWithOptionalModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameter("value");

				await That(constructors.GetDescription()).Contains("with optional modifier");
			}

			[Fact]
			public async Task ExactlyOfType_ShouldDescribeWithOptionalModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameterExactly<int>();

				await That(constructors.GetDescription()).Contains("with optional modifier");
			}

			[Fact]
			public async Task ExactlyOfTypeWithName_ShouldDescribeWithOptionalModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameterExactly<int>("value");

				await That(constructors.GetDescription()).Contains("with optional modifier");
			}

			[Fact]
			public async Task OfType_ShouldDescribeWithOptionalModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameter<int>();

				await That(constructors.GetDescription()).Contains("with optional modifier");
			}

			[Fact]
			public async Task OfTypeWithName_ShouldDescribeWithOptionalModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameter<int>("value");

				await That(constructors.GetDescription()).Contains("with optional modifier");
			}

			[Fact]
			public async Task ParameterlessFilter_ShouldMatchConstructorWithOnlySomeOptionalParameters()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameter();

				await That(constructors).Contains(MixedConstructor());
			}

#pragma warning disable CA2263
			[Fact]
			public async Task OfType_UsingType_ShouldDescribeWithOptionalModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameter(typeof(int));

				await That(constructors.GetDescription()).Contains("with optional modifier");
			}

			[Fact]
			public async Task OfTypeWithName_UsingType_ShouldDescribeWithOptionalModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameter(typeof(int), "value");

				await That(constructors.GetDescription()).Contains("with optional modifier");
			}

			[Fact]
			public async Task ExactlyOfType_UsingType_ShouldDescribeWithOptionalModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameterExactly(typeof(int));

				await That(constructors.GetDescription()).Contains("with optional modifier");
			}

			[Fact]
			public async Task ExactlyOfTypeWithName_UsingType_ShouldDescribeWithOptionalModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOptionalParameterExactly(typeof(int), "value");

				await That(constructors.GetDescription()).Contains("with optional modifier");
			}
#pragma warning restore CA2263
		}

		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithOptionalParameterConstructor
		{
			public ClassWithOptionalParameterConstructor(int value = 0)
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
			public ClassWithMixedConstructor(string other, int value = 0)
			{
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
