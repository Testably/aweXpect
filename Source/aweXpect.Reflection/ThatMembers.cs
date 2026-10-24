using System.Reflection;
using aweXpect.Reflection.Formatting;

namespace aweXpect;

/// <summary>
///     Expectations on a collection of <see cref="MemberInfo" />.
/// </summary>
public static partial class ThatMembers
{
	/// <summary>
	///     Lets the formatter of this package format the <paramref name="member" /> in a failure message.
	/// </summary>
	private static object? Formattable(MemberInfo? member) => FormattableMember.From(member);
}
