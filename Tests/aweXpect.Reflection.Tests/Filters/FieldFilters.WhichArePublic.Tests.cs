using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichArePublic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPublicFieldsWithExplicitMethod()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichArePublic();

				await That(fields).ArePublic().And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("public fields in assembly").AsPrefix();
			}
		}
	}
}
