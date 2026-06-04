using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithOutParameter
	{
		private static ConstructorInfo? OutParameterConstructor()
			=> typeof(ClassWithOutParameterConstructor).GetConstructors().Single();

		private static ConstructorInfo? PlainConstructor()
			=> typeof(ClassWithPlainConstructor).GetConstructors().Single();

		private static ConstructorInfo? MixedConstructor()
			=> typeof(ClassWithMixedConstructor).GetConstructors().Single();

		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForConstructorsWithOutParameter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameter();

				await That(constructors).Contains(OutParameterConstructor());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with out parameter in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldNotIncludeConstructorsWithoutOutParameter()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameter();

				await That(constructors).DoesNotContain(PlainConstructor());
			}

			[Fact]
			public async Task WithType_ShouldFilterForConstructorsWithOutParameterOfSpecificType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameter(typeof(int));

				await That(constructors).Contains(OutParameterConstructor());
			}

			[Fact]
			public async Task WithType_ShouldNotIncludeConstructorsWithOutParameterOfOtherType()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameter(typeof(string));

				await That(constructors).DoesNotContain(OutParameterConstructor());
			}

			[Fact]
			public async Task WithType_WithName_ShouldFilterForConstructorsWithOutParameterOfSpecificTypeAndName()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameter(typeof(int), "value");

				await That(constructors).Contains(OutParameterConstructor());
			}
		}

		public sealed class DescriptionTests
		{
			[Fact]
			public async Task ByName_ShouldDescribeWithOutModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameter("value");

				await That(constructors.GetDescription()).Contains("with out modifier");
			}

			[Fact]
			public async Task ExactlyOfType_ShouldDescribeWithOutModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameterExactly<int>();

				await That(constructors.GetDescription()).Contains("with out modifier");
			}

			[Fact]
			public async Task ExactlyOfTypeWithName_ShouldDescribeWithOutModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameterExactly<int>("value");

				await That(constructors.GetDescription()).Contains("with out modifier");
			}

			[Fact]
			public async Task OfType_ShouldDescribeWithOutModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameter<int>();

				await That(constructors.GetDescription()).Contains("with out modifier");
			}

			[Fact]
			public async Task OfTypeWithName_ShouldDescribeWithOutModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameter<int>("value");

				await That(constructors.GetDescription()).Contains("with out modifier");
			}

			[Fact]
			public async Task ParameterlessFilter_ShouldMatchConstructorWithOnlySomeOutParameters()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameter();

				await That(constructors).Contains(MixedConstructor());
			}

#pragma warning disable CA2263
			[Fact]
			public async Task OfType_UsingType_ShouldDescribeWithOutModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameter(typeof(int));

				await That(constructors.GetDescription()).Contains("with out modifier");
			}

			[Fact]
			public async Task OfTypeWithName_UsingType_ShouldDescribeWithOutModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameter(typeof(int), "value");

				await That(constructors.GetDescription()).Contains("with out modifier");
			}

			[Fact]
			public async Task ExactlyOfType_UsingType_ShouldDescribeWithOutModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameterExactly(typeof(int));

				await That(constructors.GetDescription()).Contains("with out modifier");
			}

			[Fact]
			public async Task ExactlyOfTypeWithName_UsingType_ShouldDescribeWithOutModifier()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithOutParameterExactly(typeof(int), "value");

				await That(constructors.GetDescription()).Contains("with out modifier");
			}
#pragma warning restore CA2263
		}

		// ReSharper disable UnusedMember.Local
		// ReSharper disable UnusedParameter.Local
		private class ClassWithOutParameterConstructor
		{
			public ClassWithOutParameterConstructor(out int value)
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
			public ClassWithMixedConstructor(out int value, string other)
			{
				value = 0;
			}
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
