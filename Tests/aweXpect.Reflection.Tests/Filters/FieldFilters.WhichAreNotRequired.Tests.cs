using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreNotRequired
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonRequiredFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreNotRequired();

				await That(fields).All().Satisfy(x => !x.IsRequired()).And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("non-required fields in assembly").AsPrefix();
			}
		}
	}
}
