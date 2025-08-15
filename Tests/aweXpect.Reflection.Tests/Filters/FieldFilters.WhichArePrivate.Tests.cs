using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichArePrivate
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPrivateFieldsWithExplicitMethod()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichArePrivate();

				await That(fields).ArePrivate();
				await That(fields.GetDescription())
					.IsEqualTo("private fields in assembly").AsPrefix();
			}
		}
	}
}
