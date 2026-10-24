using System.Reflection;
using aweXpect.Reflection.Formatting;

namespace aweXpect;

/// <summary>
///     Expectations on a collection of <see cref="PropertyInfo" />.
/// </summary>
public static partial class ThatProperties
{
	/// <summary>
	///     Lets the formatter of this package format the <paramref name="property" /> in a failure message.
	/// </summary>
	private static object? Formattable(PropertyInfo? property) => FormattableMember.From(property);
}
