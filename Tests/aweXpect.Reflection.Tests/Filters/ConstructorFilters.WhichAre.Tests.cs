using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichAre
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalConstructors()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAre(AccessModifiers.Internal);

				await That(constructors).AreInternal();
				await That(constructors.GetDescription())
					.IsEqualTo("internal constructors in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPrivateConstructors()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAre(AccessModifiers.Private);

				await That(constructors).ArePrivate();
				await That(constructors.GetDescription())
					.IsEqualTo("private constructors in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForProtectedConstructors()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAre(AccessModifiers.Protected);

				await That(constructors).AreProtected();
				await That(constructors.GetDescription())
					.IsEqualTo("protected constructors in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPublicConstructors()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichAre(AccessModifiers.Public);

				await That(constructors).ArePublic();
				await That(constructors.GetDescription())
					.IsEqualTo("public constructors in assembly").AsPrefix();
			}
		}
	}
}
