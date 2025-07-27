using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filter for events with the <paramref name="expected" /> name.
	/// </summary>
	public static Filtered.Events.StringEqualityResultType WithName(this Filtered.Events @this, string expected)
	{
		StringEqualityOptions options = new();
		return new Filtered.Events.StringEqualityResultType(@this.Which(Filter.Suffix<EventInfo>(
				eventInfo => options.AreConsideredEqual(eventInfo.Name, expected),
				() => $"with name {options.GetExpectation(expected, ExpectationGrammars.None)} ")),
			options);
	}
}
