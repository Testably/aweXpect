using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	private static ConstructorInfo? ExpectedParameterlessConstructorInfo()
		=> typeof(ClassWithMultipleConstructors)
			.GetConstructor(
				BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
				null, [], null);

	private static ConstructorInfo? ExpectedSingleParameterConstructorInfo()
		=> typeof(ClassWithMultipleConstructors)
			.GetConstructor(
				BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
				null, [typeof(int),], null);

	private static ConstructorInfo? ExpectedMultipleParameterConstructorInfo()
		=> typeof(ClassWithMultipleConstructors)
			.GetConstructor(
				BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
				null, [typeof(string), typeof(int),], null);

	public sealed class WithParameterCount
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForConstructorsWithMultipleParameters()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParameterCount(2);

				await That(constructors).Contains(ExpectedMultipleParameterConstructorInfo());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with 2 parameters in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForConstructorsWithSpecifiedParameterCount()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParameterCount(1);

				await That(constructors).Contains(ExpectedSingleParameterConstructorInfo());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors with one parameter in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterForConstructorsWithZeroParameters()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParameterCount(0);

				await That(constructors).Contains(ExpectedParameterlessConstructorInfo());
				await That(constructors.GetDescription())
					.IsEqualTo("constructors without parameters in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldNotIncludeConstructorsWithDifferentParameterCount()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WithParameterCount(1);

				await That(constructors).DoesNotContain(ExpectedParameterlessConstructorInfo());
				await That(constructors).DoesNotContain(ExpectedMultipleParameterConstructorInfo());
			}
		}
	}

	// ReSharper disable UnusedMember.Local
	// ReSharper disable UnusedParameter.Local
	private class ClassWithMultipleConstructors
	{
		public ClassWithMultipleConstructors()
		{
		}

		public ClassWithMultipleConstructors(int value)
		{
		}

		public ClassWithMultipleConstructors(string name, int value)
		{
		}
	}
	// ReSharper restore UnusedParameter.Local
	// ReSharper restore UnusedMember.Local
}
