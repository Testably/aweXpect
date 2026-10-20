using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class Which
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForPropertiesWhichSatisfyThePredicate()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().Which(it => it.Name.Equals("SomePropertyToVerifyTheNameOfIt"));

				await That(properties).HasSingle().Which.IsEqualTo(ExpectedPropertyInfo());
				await That(properties.GetDescription())
					.IsEqualTo(
						"properties matching it => it.Name.Equals(\"SomePropertyToVerifyTheNameOfIt\") in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>().Properties()
					.Which(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(properties).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that properties matching _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}
		}
	}
}
