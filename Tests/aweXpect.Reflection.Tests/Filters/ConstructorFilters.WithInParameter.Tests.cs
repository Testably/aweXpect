using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithInParameter
	{
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

		private static ConstructorInfo? InParameterConstructor()
			=> typeof(ClassWithInParameterConstructor).GetConstructors().Single();

		private static ConstructorInfo? PlainConstructor()
			=> typeof(ClassWithPlainConstructor).GetConstructors().Single();

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
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
