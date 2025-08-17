using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreNotProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonProtectedInternalFieldsWithExplicitMethod()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreNotProtectedInternal();

				await That(fields).AreNotProtectedInternal().And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("non-protected internal fields in assembly").AsPrefix();
			}
		}
	}
}
