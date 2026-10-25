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
	public static Filtered.Events.StringEqualityResult WithName(this Filtered.Events @this, string expected)
	{
		return new Filtered.Events.StringEqualityResult(@this, options => Filter.Suffix<EventInfo>(
				eventInfo => options.AreConsideredEqual(eventInfo.Name, expected, nameof(expected)),
				() => $"with name {options.GetExpectation(expected, ExpectationGrammars.None)} "),
			new OptionsSlot<StringEqualityOptions>(StringEqualityOptions.Default));
	}
}
