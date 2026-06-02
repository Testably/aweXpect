using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNotVirtual
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonVirtualMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<ConcreteMethodClass>()
					.Types().Methods().WhichAreNotVirtual();

				await That(methods).All().Satisfy(x => !x.IsVirtual).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("non-virtual methods in types in assembly").AsPrefix();
			}
		}
	}
}
