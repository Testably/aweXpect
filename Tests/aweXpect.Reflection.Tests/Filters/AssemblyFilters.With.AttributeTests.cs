using System.Reflection;
using aweXpect.Reflection.Collections;

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
			[Test]
			public async Task ShouldFilterForAssembliesWithAttribute()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.With<AssemblyTitleAttribute>();

				await That(assemblies).HasCount().GreaterThanOrEqualTo(2);
				await That(assemblies.GetDescription())
					.IsEqualTo("in all loaded assemblies with AssemblyTitleAttribute")
					.AsPrefix();
			}

			[Test]
			public async Task WithPredicate_ShouldFilterForAssembliesWithMatchingAttribute()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.With<AssemblyTitleAttribute>(attribute => attribute.Title == "aweXpect.Reflection.Tests");

				await That(assemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies with AssemblyTitleAttribute matching attribute => attribute.Title == \"aweXpect.Reflection.Tests\"");
			}

			[Test]
			public async Task WithPredicate_WhenPredicateThrows_ShouldFail()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<AssemblyFilters>()
					.With<AssemblyTitleAttribute>(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(assemblies).IsEmpty();
				}

				await That(Act).Throws<FailException>()
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
			[Test]
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

			[Test]
			public async Task OrWithoutPredicate_ShouldKeepAssembliesMatchingOnlyTheFirstAttribute()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.With<AssemblyTitleAttribute>()
					.OrWith<NeverAppliedAssemblyAttribute>();

				// No assembly has the NeverAppliedAssemblyAttribute, so an "and" semantic would yield none.
				await That(assemblies).HasCount().GreaterThanOrEqualTo(2);
			}

			[Test]
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
