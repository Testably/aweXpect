using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichArePrivate
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldAllowFilteringForPrivateFieldsWithExplicitMethod()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichArePrivate();

				await That(fields).ArePrivate().And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("private fields in assembly").AsPrefix();
			}
		}
	}
}
