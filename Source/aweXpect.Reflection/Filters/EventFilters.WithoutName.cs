using System.Reflection;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection;

public static partial class EventFilters
{
	/// <summary>
	///     Filter for events without the <paramref name="unexpected" /> name.
	/// </summary>
	public static Filtered.Events.StringEqualityResult WithoutName(this Filtered.Events @this, string unexpected)
	{
		return new Filtered.Events.StringEqualityResult(@this, options => Filter.Suffix<EventInfo>(
				async eventInfo => !await options.AreConsideredEqual(eventInfo.Name, unexpected),
				() => $"without name {options.GetExpectation(unexpected, ExpectationGrammars.None)} "),
			new StringEqualityOptions(nameof(unexpected)));
	}
}
