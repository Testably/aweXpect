using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreProtectedInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForProtectedInternalFieldsWithExplicitMethod()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreProtectedInternal();

				await That(fields).AreProtectedInternal().And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("protected internal fields in assembly").AsPrefix();
			}
		}
	}
}
