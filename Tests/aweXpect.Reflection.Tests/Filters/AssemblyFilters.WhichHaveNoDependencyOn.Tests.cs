using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class WhichHaveNoDependencyOn
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForAssembliesWithoutDependency()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichHaveNoDependencyOn("NonExistentAssembly");

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in assembly containing type PublicAbstractClass which have no dependency on assembly equal to \"NonExistentAssembly\"");
			}

			[Fact]
			public async Task WhenAssemblyHasTheDependency_ShouldBeFilteredOut()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichHaveNoDependencyOn("aweXpect.Core");

				await That(assemblies).IsEmpty();
			}

			[Fact]
			public async Task ShouldSupportAsWildcard()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichHaveNoDependencyOn("aweXpect.*").AsWildcard();

				await That(assemblies).IsEmpty();
			}

			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichHaveNoDependencyOn("NONEXISTENT").IgnoringCase();

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in assembly containing type PublicAbstractClass which have no dependency on assembly equal to \"NONEXISTENT\" ignoring case");
			}
		}
	}
}
