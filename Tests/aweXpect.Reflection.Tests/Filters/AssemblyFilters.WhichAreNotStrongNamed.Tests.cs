using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class WhichAreNotStrongNamed
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldDescribeTheFilter()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WhichAreNotStrongNamed();

				await That(assemblies.GetDescription())
					.IsEqualTo("in all loaded assemblies which are not strong named");
			}

			[Fact]
			public async Task ShouldFilterForNotStrongNamedAssemblies()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichAreNotStrongNamed();

				await That(assemblies).All().Satisfy(assembly => !assembly.IsStrongNamed()).And.IsNotEmpty();
			}

			[Fact]
			public async Task ShouldNotMatchStrongNamedAssemblies()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining(typeof(In))
					.WhichAreNotStrongNamed();

				await That(assemblies).IsEmpty();
			}
		}
	}
}
