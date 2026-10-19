using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class Which
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForMethodsWhichSatisfyThePredicate()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().Which(it => it.Name.Equals("SomeMethodToVerifyTheNameOfIt"));

				await That(methods).HasSingle().Which.IsEqualTo(ExpectedMethodInfo());
				await That(methods.GetDescription())
					.IsEqualTo(
						"methods matching it => it.Name.Equals(\"SomeMethodToVerifyTheNameOfIt\") in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>().Methods()
					.Which(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(methods).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods matching _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}
		}
	}
}
