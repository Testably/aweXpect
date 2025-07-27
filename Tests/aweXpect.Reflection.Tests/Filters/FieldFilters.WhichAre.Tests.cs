using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAre
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAre(AccessModifiers.Internal);

				await That(fields).AreInternal();
				await That(fields.GetDescription())
					.IsEqualTo("internal fields in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPrivateFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAre(AccessModifiers.Private);

				await That(fields).ArePrivate();
				await That(fields.GetDescription())
					.IsEqualTo("private fields in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForProtectedFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAre(AccessModifiers.Protected);

				await That(fields).AreProtected();
				await That(fields.GetDescription())
					.IsEqualTo("protected fields in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPublicFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAre(AccessModifiers.Public);

				await That(fields).ArePublic();
				await That(fields.GetDescription())
					.IsEqualTo("public fields in assembly").AsPrefix();
			}
		}
	}
}
