using System.Linq;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WithOptionalParameter
	{
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
		}

		private static ConstructorInfo? OptionalParameterConstructor()
			=> typeof(ClassWithOptionalParameterConstructor).GetConstructors().Single();

		private static ConstructorInfo? PlainConstructor()
			=> typeof(ClassWithPlainConstructor).GetConstructors().Single();

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
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore UnusedMember.Local
	}
}
