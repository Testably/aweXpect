using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class WhichSatisfy
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForAssembliesWhichSatisfyThePredicate()
			{
				Filtered.Assemblies typeAssemblies = In.AllLoadedAssemblies()
					.WhichSatisfy(it => it.FullName!.Contains("aweXpect.Reflection.Tests"));

				await That(typeAssemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(typeAssemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies matching it => it.FullName!.Contains(\"aweXpect.Reflection.Tests\")");
			}
		}
	}
}
