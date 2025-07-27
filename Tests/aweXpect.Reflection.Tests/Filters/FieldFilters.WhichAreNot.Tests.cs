using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreNot
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreNot(AccessModifiers.Internal);

				await That(fields).AreNotInternal();
				await That(fields.GetDescription())
					.IsEqualTo("non-internal fields in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPrivateFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreNot(AccessModifiers.Private);

				await That(fields).AreNotPrivate();
				await That(fields.GetDescription())
					.IsEqualTo("non-private fields in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForProtectedFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreNot(AccessModifiers.Protected);

				await That(fields).AreNotProtected();
				await That(fields.GetDescription())
					.IsEqualTo("non-protected fields in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPublicFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreNot(AccessModifiers.Public);

				await That(fields).AreNotPublic();
				await That(fields.GetDescription())
					.IsEqualTo("non-public fields in assembly").AsPrefix();
			}
		}
	}
}
