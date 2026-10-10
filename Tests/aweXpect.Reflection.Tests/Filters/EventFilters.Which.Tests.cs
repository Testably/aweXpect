using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class Which
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForEventsWhichSatisfyThePredicate()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().Which(it => it.Name.Equals("SomeEventToVerifyTheNameOfIt"));

				await That(events).HasSingle().Which.IsEqualTo(ExpectedEventInfo());
				await That(events.GetDescription())
					.IsEqualTo(
						"events matching it => it.Name.Equals(\"SomeEventToVerifyTheNameOfIt\") in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>().Events()
					.Which(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(events).IsEmpty();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that events matching _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}
		}
	}
}
