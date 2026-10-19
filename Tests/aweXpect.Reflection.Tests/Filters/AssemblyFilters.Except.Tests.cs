using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class Except
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterOutAssembliesThatSatisfyThePredicate()
			{
				Filtered.Assemblies assemblies = In.AllLoadedAssemblies()
					.Except(assembly => assembly.GetName().Name == "aweXpect.Reflection.Tests");

				await That(assemblies).All().Satisfy(a => a!.GetName().Name != "aweXpect.Reflection.Tests")
					.And.IsNotEmpty();
				await That(assemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies except assembly => assembly.GetName().Name == \"aweXpect.Reflection.Tests\"");
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<AssemblyFilters>()
					.Except(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(assemblies).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that in assembly containing type AssemblyFilters except _ => throw new InvalidOperationException("boom")
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}
		}
	}
}
