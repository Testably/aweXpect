using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichAreNot
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalConstructors()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAreNot(AccessModifiers.Internal);

				await That(constructors).AreNotInternal();
				await That(constructors.GetDescription())
					.IsEqualTo("non-internal constructors in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPrivateConstructors()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAreNot(AccessModifiers.Private);

				await That(constructors).AreNotPrivate();
				await That(constructors.GetDescription())
					.IsEqualTo("non-private constructors in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForProtectedConstructors()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAreNot(AccessModifiers.Protected);

				await That(constructors).AreNotProtected();
				await That(constructors.GetDescription())
					.IsEqualTo("non-protected constructors in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPublicConstructors()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAreNot(AccessModifiers.Public);

				await That(constructors).AreNotPublic();
				await That(constructors.GetDescription())
					.IsEqualTo("non-public constructors in assembly").AsPrefix();
			}
		}
	}
}
