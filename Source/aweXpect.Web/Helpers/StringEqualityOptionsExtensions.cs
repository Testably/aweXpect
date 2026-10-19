using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Helpers;

internal static class StringEqualityOptionsExtensions
{
	/// <summary>
	///     Gets the extended failure text that states which value <paramref name="it" /> had in its
	///     <paramref name="member" />.
	/// </summary>
	/// <remarks>
	///     Mirrors the member failure of aweXpect.Core, which is not public: the exact match type phrases its failure as
	///     "<c>{member} was …</c>", so it is rephrased here.
	/// </remarks>
	public static string GetExtendedMemberFailure(this StringEqualityOptions options, string it, string member,
		ExpectationGrammars grammars, string? actual, string? expected)
	{
		string wasPrefix = $"{member} was ";
		string failure = options.GetExtendedFailure(member, grammars, actual, expected);
		return $"{it} had {member} {failure.Substring(wasPrefix.Length)}";
	}
}
