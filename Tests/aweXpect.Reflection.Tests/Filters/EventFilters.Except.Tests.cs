using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class Except
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterOutEventsThatSatisfyThePredicate()
			{
				Filtered.Events events = In.Type<ConcreteEventClass>()
					.Events().Except(e => e.Name == "ConcreteEvent");

				await That(events).All().Satisfy(e => e!.Name != "ConcreteEvent").And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("events except e => e.Name == \"ConcreteEvent\" in").AsPrefix();
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>().Events()
					.Except(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(events).IsEmpty();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that events except _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}
		}
	}
}
