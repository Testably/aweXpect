using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichAreStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForStaticConstructors()
			{
				// Note: Static constructors exist but are rare
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAreStatic();

				await That(constructors).All().Satisfy(c => c.IsStatic).And.IsNotEmpty();
				await That(constructors.GetDescription())
					.IsEqualTo("static constructors in assembly").AsPrefix();
			}

			// ReSharper disable once UnusedType.Local
			private class ClassWithStaticConstructor
			{
				// ReSharper disable once EmptyConstructor
				static ClassWithStaticConstructor()
				{
				}
			}
		}
	}
}
