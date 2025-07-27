using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNot
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalTypes()
			{
				Filtered.Methods types = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreNot(AccessModifiers.Internal);

				await That(types).AreNotInternal();
				await That(types.GetDescription())
					.IsEqualTo("non-internal methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPrivateTypes()
			{
				Filtered.Methods types = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreNot(AccessModifiers.Private);

				await That(types).AreNotPrivate();
				await That(types.GetDescription())
					.IsEqualTo("non-private methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForProtectedTypes()
			{
				Filtered.Methods types = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreNot(AccessModifiers.Protected);

				await That(types).AreNotProtected();
				await That(types.GetDescription())
					.IsEqualTo("non-protected methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPublicTypes()
			{
				Filtered.Methods types = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreNot(AccessModifiers.Public);

				await That(types).AreNotPublic();
				await That(types.GetDescription())
					.IsEqualTo("non-public methods in assembly").AsPrefix();
			}
		}
	}
}
