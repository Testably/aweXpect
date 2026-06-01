using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class WithoutName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutAssembliesWithName()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WithoutName("aweXpect.Reflection.Tests");

				await That(assemblies).All().Satisfy(a => a!.GetName().Name != "aweXpect.Reflection.Tests")
					.And.IsNotEmpty();
				await That(assemblies.GetDescription())
					.IsEqualTo("in all loaded assemblies without name equal to \"aweXpect.Reflection.Tests\"");
			}

			[Fact]
			public async Task ShouldSupportAsSuffix()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WithoutName(".Tests").AsSuffix();

				await That(assemblies).All().Satisfy(a => !a!.GetName().Name!.EndsWith(".Tests"))
					.And.IsNotEmpty();
				await That(assemblies.GetDescription())
					.IsEqualTo("in all loaded assemblies without name ending with \".Tests\"");
			}

			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WithoutName("awexpect.reflection.tests").IgnoringCase();

				await That(assemblies).All()
					.Satisfy(a => !string.Equals(a!.GetName().Name, "aweXpect.Reflection.Tests",
						System.StringComparison.OrdinalIgnoreCase))
					.And.IsNotEmpty();
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies without name equal to \"awexpect.reflection.tests\" ignoring case");
			}
		}
	}
}
