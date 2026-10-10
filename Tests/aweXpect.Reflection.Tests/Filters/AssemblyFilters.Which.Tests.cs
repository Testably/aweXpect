using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class Which
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForAssembliesWhichSatisfyThePredicate()
			{
				Filtered.Assemblies typeAssemblies = In.AllLoadedAssemblies()
					.Which(it => it.FullName!.Contains("aweXpect.Reflection.Tests"));

				await That(typeAssemblies).HasSingle().Which.IsEqualTo(typeof(AssemblyFilters).Assembly);
				await That(typeAssemblies.GetDescription())
					.IsEqualTo(
						"in all loaded assemblies matching it => it.FullName!.Contains(\"aweXpect.Reflection.Tests\")");
			}

			[Fact]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Assemblies assemblies = In.AssemblyContaining<AssemblyFilters>()
					.Which(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(assemblies).IsEmpty();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that in assembly containing type AssemblyFilters matching _ => throw new InvalidOperationException("boom")
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}
		}
	}
}
