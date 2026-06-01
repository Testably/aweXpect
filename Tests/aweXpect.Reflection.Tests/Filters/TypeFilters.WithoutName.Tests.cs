using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WithoutName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutTypesWithName()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithoutName(nameof(TypeToExcludeByName));

				await That(types).All().Satisfy(t => t!.Name != nameof(TypeToExcludeByName))
					.And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("types without name equal to \"TypeToExcludeByName\" in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsSuffix()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithoutName("ToExcludeByName").AsSuffix();

				await That(types).All().Satisfy(t => !t!.Name.EndsWith("ToExcludeByName"))
					.And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("types without name ending with \"ToExcludeByName\" in assembly")
					.AsPrefix();
			}

			private class TypeToExcludeByName;
		}
	}
}
