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
