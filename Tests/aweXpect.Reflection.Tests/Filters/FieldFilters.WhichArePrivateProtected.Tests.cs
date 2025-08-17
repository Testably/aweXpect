using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichArePrivateProtected
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPrivateProtectedFieldsWithExplicitMethod()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichArePrivateProtected();

				await That(fields).ArePrivateProtected().And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("private protected fields in assembly").AsPrefix();
			}
		}
	}
}
