using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class Which
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForConstructorsWhichSatisfyThePredicate()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().Which(it
						=> it.DeclaringType == typeof(SomeClassToVerifyTheConstructorNameOfIt));

				await That(constructors).HasSingle().Which.IsEqualTo(ExpectedConstructorInfo());
				await That(constructors.GetDescription())
					.IsEqualTo(
						"constructors matching it => it.DeclaringType == typeof(SomeClassToVerifyTheConstructorNameOfIt) in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>().Constructors()
					.Which(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(constructors).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that constructors matching _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}
		}
	}
}
