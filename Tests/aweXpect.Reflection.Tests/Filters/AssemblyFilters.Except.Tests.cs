using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class Except
	{
		public sealed class Tests
		{
			[Fact]
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

			[Fact]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<AssemblyFilters>()
					.Except(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(assemblies).IsEmpty();
				}

				await That(Act).Throws<XunitException>()
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
