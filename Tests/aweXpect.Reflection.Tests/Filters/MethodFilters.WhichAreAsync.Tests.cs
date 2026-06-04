using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreAsync
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForAsyncMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<ClassWithAsyncMembers>()
					.Types().Methods().WhichAreAsync();

				await That(methods).All().Satisfy(x => x.IsReallyAsync()).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("async methods in types in assembly").AsPrefix();
			}
		}
	}
}
