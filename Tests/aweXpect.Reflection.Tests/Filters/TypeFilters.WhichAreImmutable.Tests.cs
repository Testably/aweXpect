using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreImmutable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForImmutableTypes()
			{
				Filtered.Types types = In.AssemblyContaining<ImmutableClass>()
					.Types().WhichAreImmutable();

				await That(types).AreImmutable().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("immutable types in assembly").AsPrefix();
			}
		}
	}
}
