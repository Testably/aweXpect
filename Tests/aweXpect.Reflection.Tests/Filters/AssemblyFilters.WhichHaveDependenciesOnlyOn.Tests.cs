using aweXpect.Customization;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class WhichHaveDependenciesOnlyOn
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldDescribeTheFilterForASingleAllowedAssembly()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichHaveDependenciesOnlyOn("aweXpect.Core");

				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in assembly containing type PublicAbstractClass which have dependencies only on assemblies equal to \"aweXpect.Core\"");
			}

			[Fact]
			public async Task ShouldDescribeTheFilterForMultipleAllowedAssemblies()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichHaveDependenciesOnlyOn("aweXpect.Core", "aweXpect");

				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in assembly containing type PublicAbstractClass which have dependencies only on assemblies equal to \"aweXpect.Core\" or equal to \"aweXpect\"");
			}

			[Fact]
			public async Task ShouldFilterForAssembliesDependingOnlyOnAllowed()
			{
				Filtered.Assemblies assemblies = In.Assemblies(typeof(In).Assembly)
					.WhichHaveDependenciesOnlyOn("aweXpect.Core");

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(In).Assembly);
			}

			[Fact]
			public async Task ShouldSupportAsWildcard()
			{
				Filtered.Assemblies assemblies = In.Assemblies(typeof(In).Assembly)
					.WhichHaveDependenciesOnlyOn("aweXpect.*").AsWildcard();

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(In).Assembly);
			}

			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				Filtered.Assemblies assemblies = In.Assemblies(typeof(In).Assembly)
					.WhichHaveDependenciesOnlyOn("awexpect.core").IgnoringCase();

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(In).Assembly);
			}

			[Fact]
			public async Task WhenAssemblyDependsOnDisallowedAssembly_ShouldBeFilteredOut()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<PublicAbstractClass>()
					.WhichHaveDependenciesOnlyOn("aweXpect.Core");

				await That(assemblies).IsEmpty();
			}
		}
	}
}
