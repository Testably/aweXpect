using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNotAsync
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonAsyncMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<ClassWithAsyncMembers>()
					.Types().Methods().WhichAreNotAsync();

				await That(methods).All().Satisfy(x => !x.IsReallyAsync()).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("non-async methods in types in assembly").AsPrefix();
			}
		}
	}
}
