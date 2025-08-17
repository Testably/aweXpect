using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreSealed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForSealedMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<SealedMethodClass>()
					.Types().Methods().WhichAreSealed();

				await That(methods).All().Satisfy(x => x.IsFinal).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("sealed methods in types in assembly").AsPrefix();
			}
		}
	}
}
