using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class WhichTarget
	{
#if NET8_0
		private const string CurrentTarget = "net8.0";
#elif NET10_0
		private const string CurrentTarget = "net10.0";
#else
		private const string CurrentTarget = "net48";
#endif

		public sealed class Tests
		{
			[Fact]
			public async Task ShouldDescribeTheFilter()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WhichTarget(CurrentTarget);

				await That(assemblies.GetDescription())
					.IsEqualTo($"in all loaded assemblies targeting equal to \"{CurrentTarget}\"");
			}

			[Fact]
			public async Task ShouldFilterForAssembliesTargetingTheFramework()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WhichTarget(CurrentTarget);

				await That(assemblies).Contains(typeof(AssemblyFilters).Assembly);
			}

			[Fact]
			public async Task ShouldNotMatchWhenNoAssemblyTargetsTheFramework()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WhichTarget("net99.0");

				await That(assemblies).IsEmpty();
			}

			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WhichTarget(CurrentTarget.ToUpperInvariant()).IgnoringCase();

				await That(assemblies).Contains(typeof(AssemblyFilters).Assembly);
			}
		}
	}
}
