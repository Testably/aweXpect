using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreConstant
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForConstantFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreConstant();

				await That(fields).All().Satisfy(f => f.IsLiteral).And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("constant fields in assembly").AsPrefix();
			}
		}
	}
}
