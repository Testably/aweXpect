using System.Reflection;
using aweXpect.Reflection.Formatting;

namespace aweXpect;

/// <summary>
///     Expectations on a collection of <see cref="ConstructorInfo" />.
/// </summary>
public static partial class ThatConstructors
{
	/// <summary>
	///     Lets the formatter of this package format the <paramref name="constructor" /> in a failure message.
	/// </summary>
	private static object? Formattable(ConstructorInfo? constructor) => FormattableMember.From(constructor);
}
