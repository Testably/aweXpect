using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichSatisfy
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForEventsWhichSatisfyThePredicate()
			{
				Filtered.Events events = In.AssemblyContaining<AssemblyFilters>()
					.Events().WhichSatisfy(it => it.Name.Equals("SomeEventToVerifyTheNameOfIt"));

				await That(events).HasSingle().Which.IsEqualTo(ExpectedEventInfo());
				await That(events.GetDescription())
					.IsEqualTo(
						"events matching it => it.Name.Equals(\"SomeEventToVerifyTheNameOfIt\") in assembly")
					.AsPrefix();
			}
		}
	}
}
