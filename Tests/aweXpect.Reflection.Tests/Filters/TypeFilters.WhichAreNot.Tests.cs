using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNot
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreNot(AccessModifiers.Internal);

				await That(types).AreNotInternal();
				await That(types.GetDescription())
					.IsEqualTo("non-internal types in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPrivateTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreNot(AccessModifiers.Private);

				await That(types).AreNotPrivate();
				await That(types.GetDescription())
					.IsEqualTo("non-private types in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForProtectedTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreNot(AccessModifiers.Protected);

				await That(types).AreNotProtected();
				await That(types.GetDescription())
					.IsEqualTo("non-protected types in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPublicTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreNot(AccessModifiers.Public);

				await That(types).AreNotPublic();
				await That(types.GetDescription())
					.IsEqualTo("non-public types in assembly").AsPrefix();
			}
		}
	}
}
