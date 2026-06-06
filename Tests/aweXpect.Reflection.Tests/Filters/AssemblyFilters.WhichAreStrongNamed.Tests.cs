using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class WhichAreStrongNamed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldDescribeTheFilter()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WhichAreStrongNamed();

				await That(assemblies.GetDescription())
					.IsEqualTo("in all loaded assemblies which are strong named");
			}

			[Fact]
			public async Task ShouldFilterForStrongNamedAssemblies()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WhichAreStrongNamed();

				await That(assemblies).All().Satisfy(assembly => assembly.IsStrongNamed()).And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldNotMatchAssembliesWithoutStrongName()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichAreStrongNamed();

				await That(assemblies).IsEmpty();
			}
		}
	}
}
