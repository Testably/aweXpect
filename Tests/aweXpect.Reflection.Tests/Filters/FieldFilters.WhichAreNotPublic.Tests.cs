using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreNotPublic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPublicFieldsWithExplicitMethod()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreNotPublic();

				await That(fields).AreNotPublic();
				await That(fields.GetDescription())
					.IsEqualTo("non-public fields in assembly").AsPrefix();
			}
		}
	}
}
