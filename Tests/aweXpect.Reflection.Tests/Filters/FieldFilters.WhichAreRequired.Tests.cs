using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreRequired
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForRequiredFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreRequired();

				await That(fields).All().Satisfy(x => x.IsRequired()).And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("required fields in assembly").AsPrefix();
			}
		}
	}
}
