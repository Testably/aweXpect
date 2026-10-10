using System.Reflection;
using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class With
	{
		/// <summary>
		///     A custom attribute that is never applied to any assembly, used to verify
		///     <c>OrWith</c> alternatives.
		/// </summary>
		[AttributeUsage(AttributeTargets.Assembly)]
		private sealed class NeverAppliedAssemblyAttribute : Attribute;

		public sealed class AttributeTests
		{
			[Fact]
			public async Task ShouldFilterForAssembliesWithAttribute()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.With<AssemblyTitleAttribute>();

				await That(assemblies).HasCount().GreaterThanOrEqualTo(2);
				await That(assemblies.GetDescription())
					.IsEqualTo("in all loaded assemblies with AssemblyTitleAttribute")
					.AsPrefix();
			}

			[Fact]
			public async Task WhenInheritIsSetToFalse_ShouldFilterForAssembliesWithAttributeDirectlySet()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.With<AssemblyTitleAttribute>(false);

				await That(assemblies).HasCount().GreaterThanOrEqualTo(2);
				await That(assemblies.GetDescription())
					.IsEqualTo("in all loaded assemblies with direct AssemblyTitleAttribute")
					.AsPrefix();
			}

			[Fact]
			public async Task WithPredicate_ShouldFilterForAssembliesWithMatchingAttribute()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.With<AssemblyTitleAttribute>(attribute => attribute.Title == "aweXpect.Reflection.Tests");

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies with AssemblyTitleAttribute matching attribute => attribute.Title == \"aweXpect.Reflection.Tests\"");
			}

			[Fact]
			public async Task WithPredicate_WhenInheritIsSetToFalse_ShouldDescribeAsDirect()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.With<AssemblyTitleAttribute>(attribute => attribute.Title == "aweXpect.Reflection.Tests",
						false);

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies with direct AssemblyTitleAttribute matching attribute => attribute.Title == \"aweXpect.Reflection.Tests\"");
			}

			[Fact]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<AssemblyFilters>()
					.With<AssemblyTitleAttribute>(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(assemblies).IsEmpty();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type AssemblyFilters with AssemblyTitleAttribute matching _ => throw new InvalidOperationException("boom")
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}
		}

		public sealed class OrWithAttributeTests
		{
			[Fact]
			public async Task OrWithoutPredicate_ShouldDescribeBothAttributes()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.With<AssemblyTitleAttribute>()
					.OrWith<NeverAppliedAssemblyAttribute>();

				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies with AssemblyTitleAttribute or with AssemblyFilters.With.NeverAppliedAssemblyAttribute")
					.AsPrefix();
			}

			[Fact]
			public async Task OrWithoutPredicate_ShouldKeepAssembliesMatchingOnlyTheFirstAttribute()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.With<AssemblyTitleAttribute>()
					.OrWith<NeverAppliedAssemblyAttribute>();

				// No assembly has the NeverAppliedAssemblyAttribute, so an "and" semantic would yield none.
				await That(assemblies).HasCount().GreaterThanOrEqualTo(2);
			}

			[Fact]
			public async Task OrWithoutPredicate_WhenInheritIsSetToFalse_ShouldDescribeAsDirect()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.With<AssemblyTitleAttribute>()
					.OrWith<NeverAppliedAssemblyAttribute>(false);

				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies with AssemblyTitleAttribute or with direct AssemblyFilters.With.NeverAppliedAssemblyAttribute")
					.AsPrefix();
			}

			[Fact]
			public async Task OrWithPredicate_WhenInheritIsSetToFalse_ShouldDescribeAsDirect()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.With<AssemblyTitleAttribute>(attribute => attribute.Title == "aweXpect.Reflection.Tests")
					.OrWith<AssemblyTitleAttribute>(attribute => attribute.Title == "aweXpect.Reflection", false);

				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies with AssemblyTitleAttribute matching attribute => attribute.Title == \"aweXpect.Reflection.Tests\" or with direct AssemblyTitleAttribute matching attribute => attribute.Title == \"aweXpect.Reflection\"");
			}

			[Fact]
			public async Task ShouldFilterForAssembliesWithEitherAttribute()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.With<AssemblyTitleAttribute>(attribute => attribute.Title == "aweXpect.Reflection.Tests")
					.OrWith<AssemblyTitleAttribute>(attribute => attribute.Title == "aweXpect.Reflection");

				await That(assemblies).IsEqualTo([
					typeof(AssemblyFilters).Assembly,
					typeof(Filtered.Assemblies).Assembly,
				]).InAnyOrder();
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies with AssemblyTitleAttribute matching attribute => attribute.Title == \"aweXpect.Reflection.Tests\" or with AssemblyTitleAttribute matching attribute => attribute.Title == \"aweXpect.Reflection\"");
			}
		}
	}
}
