using System.Reflection;
using aweXpect.Reflection.Formatting;

namespace aweXpect;

/// <summary>
///     Expectations on a collection of <see cref="MethodInfo" />.
/// </summary>
public static partial class ThatMethods
{
	/// <summary>
	///     Lets the formatter of this package format the <paramref name="method" /> in a failure message.
	/// </summary>
	private static object? Formattable(MethodInfo? method) => FormattableMember.From(method);
}
