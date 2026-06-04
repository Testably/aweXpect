using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreReadOnly
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForReadOnlyFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreReadOnly();

				await That(fields).All().Satisfy(f => f.IsInitOnly).And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("read-only fields in assembly").AsPrefix();
			}
		}
	}
}
