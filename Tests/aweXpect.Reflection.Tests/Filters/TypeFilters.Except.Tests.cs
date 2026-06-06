using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class Except
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutTheGenericType()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().Except<TypeToExclude>();

				await That(types).DoesNotContain(typeof(TypeToExclude)).And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("types except TypeFilters.Except.Tests.TypeToExclude in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterOutTypesThatSatisfyThePredicate()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().Except(type => type.Name == "TypeToExclude");

				await That(types).All().Satisfy(t => t!.Name != "TypeToExclude").And.IsNotEmpty();
				await That(types).DoesNotContain(typeof(TypeToExclude));
				await That(types.GetDescription())
					.IsEqualTo("types except type => type.Name == \"TypeToExclude\" in assembly").AsPrefix();
			}

			private class TypeToExclude;
		}
	}
}
