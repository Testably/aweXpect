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

		private static ConstructorInfo? RefParameterConstructor()
			=> typeof(ClassWithRefParameterConstructor).GetConstructors().Single();

		private static ConstructorInfo? PlainConstructor()
			=> typeof(ClassWithPlainConstructor).GetConstructors().Single();

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
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
