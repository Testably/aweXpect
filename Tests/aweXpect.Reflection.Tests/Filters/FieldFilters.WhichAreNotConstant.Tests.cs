using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreNotConstant
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonConstantFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreNotConstant();

				await That(fields).All().Satisfy(f => !f.IsLiteral).And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("non-constant fields in assembly").AsPrefix();
			}
		}
	}
}
