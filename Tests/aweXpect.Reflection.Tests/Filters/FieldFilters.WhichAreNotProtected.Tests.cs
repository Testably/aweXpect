using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreNotProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonProtectedFieldsWithExplicitMethod()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreNotProtected();

				await That(fields).AreNotProtected().And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("non-protected fields in assembly").AsPrefix();
			}
		}
	}
}
