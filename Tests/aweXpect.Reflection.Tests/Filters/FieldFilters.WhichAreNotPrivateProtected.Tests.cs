using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreNotPrivateProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPrivateProtectedFieldsWithExplicitMethod()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreNotPrivateProtected();

				await That(fields).AreNotPrivateProtected().And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("non-private protected fields in assembly").AsPrefix();
			}
		}
	}
}
