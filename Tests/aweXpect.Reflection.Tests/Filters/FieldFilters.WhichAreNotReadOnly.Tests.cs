using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreNotReadOnly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonReadOnlyFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreNotReadOnly();

				await That(fields).All().Satisfy(f => !f.IsInitOnly).And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("non-read-only fields in assembly").AsPrefix();
			}
		}
	}
}
