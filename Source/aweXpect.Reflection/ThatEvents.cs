using System.Reflection;
using aweXpect.Reflection.Formatting;

namespace aweXpect;

/// <summary>
///     Expectations on a collection of <see cref="EventInfo" />.
/// </summary>
public static partial class ThatEvents
{
	/// <summary>
	///     Lets the formatter of this package format the <paramref name="event" /> in a failure message.
	/// </summary>
	private static object? Formattable(EventInfo? @event) => FormattableMember.From(@event);
}
