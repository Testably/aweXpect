using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class Without
	{
		public sealed class AttributeTests
		{
			[Test]
			public async Task ShouldFilterForAssembliesWithoutAttribute()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.Without<AssemblyTitleAttribute>();

				await That(assemblies).DoesNotContain(typeof(AssemblyFilters).Assembly);
				await That(assemblies.GetDescription())
					.IsEqualTo("in all loaded assemblies without AssemblyTitleAttribute").AsPrefix();
			}

			[Test]
			public async Task WhenAttributeIsNeverApplied_ShouldKeepAllAssemblies()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.Without<NeverAppliedAssemblyAttribute>();

				await That(assemblies).Contains(typeof(AssemblyFilters).Assembly);
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies without AssemblyFilters.Without.AttributeTests.NeverAppliedAssemblyAttribute")
					.AsPrefix();
			}

			[AttributeUsage(AttributeTargets.Assembly)]
			private sealed class NeverAppliedAssemblyAttribute : Attribute;
		}
	}
}
