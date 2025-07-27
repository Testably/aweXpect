using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAre
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalTypes()
			{
				Filtered.Methods types = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAre(AccessModifiers.Internal);

				await That(types).AreInternal();
				await That(types.GetDescription())
					.IsEqualTo("internal methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPrivateTypes()
			{
				Filtered.Methods types = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAre(AccessModifiers.Private);

				await That(types).ArePrivate();
				await That(types.GetDescription())
					.IsEqualTo("private methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForProtectedTypes()
			{
				Filtered.Methods types = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAre(AccessModifiers.Protected);

				await That(types).AreProtected();
				await That(types.GetDescription())
					.IsEqualTo("protected methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPublicTypes()
			{
				Filtered.Methods types = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAre(AccessModifiers.Public);

				await That(types).ArePublic();
				await That(types.GetDescription())
					.IsEqualTo("public methods in assembly").AsPrefix();
			}
		}
	}
}
