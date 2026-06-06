using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class Except
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutAssembliesThatSatisfyThePredicate()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.Except(assembly => assembly.GetName().Name == "aweXpect.Reflection.Tests");

				await That(assemblies).All().Satisfy(a => a!.GetName().Name != "aweXpect.Reflection.Tests")
					.And.IsNotEmpty();
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies except assembly => assembly.GetName().Name == \"aweXpect.Reflection.Tests\"");
			}
		}
	}
}
