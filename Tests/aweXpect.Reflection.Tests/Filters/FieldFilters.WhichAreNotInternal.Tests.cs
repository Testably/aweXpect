using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreNotInternal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonInternalFieldsWithExplicitMethod()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreNotInternal();

				await That(fields).AreNotInternal();
				await That(fields.GetDescription())
					.IsEqualTo("non-internal fields in assembly").AsPrefix();
			}
		}
	}
}
