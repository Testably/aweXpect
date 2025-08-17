using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAre
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAre(AccessModifiers.Internal);

				await That(types).AreInternal().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("internal types in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPrivateTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAre(AccessModifiers.Private);

				await That(types).ArePrivate().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("private types in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForProtectedTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAre(AccessModifiers.Protected);

				await That(types).AreProtected().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("protected types in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPublicTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAre(AccessModifiers.Public);

				await That(types).ArePublic().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("public types in assembly").AsPrefix();
			}
		}
	}
}
