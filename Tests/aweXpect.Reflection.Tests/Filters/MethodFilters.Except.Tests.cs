using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class Except
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterOutMethodsThatSatisfyThePredicate()
			{
				Filtered.Methods methods = In.Type<ConcreteMethodClass>()
					.Methods().Except(method => method.Name == "ConcreteMethod");

				await That(methods).All().Satisfy(m => m!.Name != "ConcreteMethod").And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("methods except method => method.Name == \"ConcreteMethod\" in").AsPrefix();
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>().Methods()
					.Except(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(methods).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that methods except _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}
		}
	}
}
