using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class WhichHaveADependencyOn
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForAssembliesWithDependency()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichHaveADependencyOn("aweXpect.Core");

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in assembly containing type PublicAbstractClass which have a dependency on assembly equal to \"aweXpect.Core\"");
			}

			[Fact]
			public async Task WhenAssemblyHasNoSuchDependency_ShouldBeFilteredOut()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichHaveADependencyOn("NonExistentAssembly");

				await That(assemblies).IsEmpty();
			}

			[Fact]
			public async Task ShouldSupportAsPrefix()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichHaveADependencyOn("aweXpect.C").AsPrefix();

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in assembly containing type PublicAbstractClass which have a dependency on assembly starting with \"aweXpect.C\"");
			}

			[Fact]
			public async Task ShouldSupportAsWildcard()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichHaveADependencyOn("aweXpect.*").AsWildcard();

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in assembly containing type PublicAbstractClass which have a dependency on assembly matching \"aweXpect.*\"");
			}

			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichHaveADependencyOn("awexpect.core").IgnoringCase();

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in assembly containing type PublicAbstractClass which have a dependency on assembly equal to \"awexpect.core\" ignoring case");
			}
		}
	}
}
