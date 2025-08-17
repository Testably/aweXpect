using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichArePrivate
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForPrivateMethodsWithExplicitMethod()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichArePrivate();

				await That(methods).ArePrivate().And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("private methods in assembly").AsPrefix();
			}
		}
	}
}
