using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithOutParameter
	{
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

		private static ConstructorInfo? OutParameterConstructor()
			=> typeof(ClassWithOutParameterConstructor).GetConstructors().Single();

		private static ConstructorInfo? PlainConstructor()
			=> typeof(ClassWithPlainConstructor).GetConstructors().Single();

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
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
