using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNotPrivate
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonPrivateMethodsWithExplicitMethod()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreNotPrivate();

				await That(methods).AreNotPrivate();
				await That(methods.GetDescription())
					.IsEqualTo("non-private methods in assembly").AsPrefix();
			}
		}
	}
}
