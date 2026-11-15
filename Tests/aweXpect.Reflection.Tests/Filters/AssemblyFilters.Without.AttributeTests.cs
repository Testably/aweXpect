using System.Reflection;
using aweXpect.Reflection.Collections;

[assembly: AssemblyMetadata("AssemblyFilters.Without.Multi", "1")]
[assembly: AssemblyMetadata("AssemblyFilters.Without.Multi", "2")]

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

			[Test]
			public async Task WithPredicate_ShouldFilterForAssembliesWithoutMatchingAttribute()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.Without<AssemblyTitleAttribute>(attribute => attribute.Title == "aweXpect.Reflection.Tests");

				await That(assemblies).DoesNotContain(typeof(AssemblyFilters).Assembly)
					.And.Contains(typeof(Filtered).Assembly)
					.Because("an assembly with a not matching attribute is kept");
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies without AssemblyTitleAttribute matching attribute => attribute.Title == \"aweXpect.Reflection.Tests\"");
			}

			[Test]
			[Arguments("1")]
			[Arguments("2")]
			public async Task WithPredicate_WhenOneOfSeveralAttributesMatches_ShouldExcludeAssembly(string value)
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<AssemblyFilters>()
					.Without<AssemblyMetadataAttribute>(
						attribute => attribute.Key == "AssemblyFilters.Without.Multi" && attribute.Value == value);
				Filtered.Assemblies notMatchingAssemblies = In.AssemblyContaining<AssemblyFilters>()
					.Without<AssemblyMetadataAttribute>(
						attribute => attribute.Key == "AssemblyFilters.Without.Multi" && attribute.Value == "3");

				await That(assemblies).DoesNotContain(typeof(AssemblyFilters).Assembly)
					.Because("every attribute of the assembly counts, not only the first one");
				await That(notMatchingAssemblies).Contains(typeof(AssemblyFilters).Assembly)
					.Because("an assembly whose attributes all do not match the predicate is kept");
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<AssemblyFilters>()
					.Without<AssemblyTitleAttribute>(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(assemblies).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type AssemblyFilters without AssemblyTitleAttribute matching _ => throw new InvalidOperationException("boom")
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			[AttributeUsage(AttributeTargets.Assembly)]
			private sealed class NeverAppliedAssemblyAttribute : Attribute;
		}
	}
}
