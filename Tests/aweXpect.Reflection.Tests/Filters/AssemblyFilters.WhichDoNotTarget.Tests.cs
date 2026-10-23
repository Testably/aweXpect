using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class WhichDoNotTarget
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
			[Test]
			public async Task ShouldDescribeTheFilter()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WhichDoNotTarget(CurrentTarget);

				await That(assemblies.GetDescription())
					.IsEqualTo($"in all loaded assemblies not targeting equal to \"{CurrentTarget}\"");
			}

			[Test]
			public async Task ShouldFilterOutAssembliesTargetingTheFramework()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WhichDoNotTarget(CurrentTarget);

				await That(assemblies).DoesNotContain(typeof(AssemblyFilters).Assembly);
			}

			[Test]
			public async Task ShouldKeepAssembliesWhenNoneTargetsTheFramework()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<AssemblyFilters>()
					.WhichDoNotTarget("net99.0");

				await That(assemblies).Contains(typeof(AssemblyFilters).Assembly);
			}

			[Test]
			public async Task ShouldSupportIgnoringCase()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.WhichDoNotTarget(CurrentTarget.ToUpperInvariant()).IgnoringCase();

				await That(assemblies).DoesNotContain(typeof(AssemblyFilters).Assembly);
				await That(assemblies.GetDescription())
					.IsEqualTo(
						$"in all loaded assemblies not targeting equal to \"{CurrentTarget.ToUpperInvariant()}\" ignoring case");
			}
		}
	}
}
