using System.Reflection;
using aweXpect.Reflection.Formatting;

namespace aweXpect;

/// <summary>
///     Expectations on a collection of <see cref="FieldInfo" />.
/// </summary>
public static partial class ThatFields
{
	/// <summary>
	///     Lets the formatter of this package format the <paramref name="field" /> in a failure message.
	/// </summary>
	private static object? Formattable(FieldInfo? field) => FormattableMember.From(field);
}
