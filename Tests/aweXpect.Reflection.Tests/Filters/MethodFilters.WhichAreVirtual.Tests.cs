using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreVirtual
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForVirtualMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<SealedMethodClass>()
					.Types().Methods().WhichAreVirtual();

				await That(methods).All().Satisfy(x => x.IsVirtual).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("virtual methods in types in assembly").AsPrefix();
			}
		}
	}
}
