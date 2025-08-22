using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichAreProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForProtectedInternalConstructorsWithExplicitMethod()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAreProtectedInternal();

				await That(constructors).AreProtectedInternal().And.IsNotEmpty();
				await That(constructors.GetDescription())
					.IsEqualTo("protected internal constructors in assembly").AsPrefix();
			}

#pragma warning disable CS0628 // New protected member declared in sealed type
			// ReSharper disable once UnusedType.Local
			private class WithProtectedInternalConstructor
			{
				protected internal WithProtectedInternalConstructor()
				{
				}
			}
#pragma warning restore CS0628 // New protected member declared in sealed type
		}
	}
}
