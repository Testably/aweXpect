using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreNot
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreNot(AccessModifiers.Internal);

				await That(properties).AreNotInternal();
				await That(properties.GetDescription())
					.IsEqualTo("non-internal properties in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPrivateProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreNot(AccessModifiers.Private);

				await That(properties).AreNotPrivate();
				await That(properties.GetDescription())
					.IsEqualTo("non-private properties in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForProtectedProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreNot(AccessModifiers.Protected);

				await That(properties).AreNotProtected();
				await That(properties.GetDescription())
					.IsEqualTo("non-protected properties in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPublicProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreNot(AccessModifiers.Public);

				await That(properties).AreNotPublic();
				await That(properties.GetDescription())
					.IsEqualTo("non-public properties in assembly").AsPrefix();
			}
		}
	}
}
