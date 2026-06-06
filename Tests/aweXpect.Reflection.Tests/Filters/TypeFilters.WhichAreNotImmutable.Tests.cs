using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotImmutable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForMutableTypes()
			{
				Filtered.Types types = In.AssemblyContaining<ClassWithMutableField>()
					.Types().WhichAreNotImmutable();

				await That(types).AreNotImmutable().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("mutable types in assembly").AsPrefix();
			}
		}
	}
}
