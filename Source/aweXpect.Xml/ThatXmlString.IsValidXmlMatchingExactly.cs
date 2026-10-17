using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatXmlString
{
	/// <summary>
	///     Verifies that the subject is a well-formed XML document whose root element matches the
	///     <paramref name="expected" /> XML exactly.
	/// </summary>
	/// <remarks>
	///     This is a shortcut for <c>IsValidXml().Which(root => root.MatchesExactly(expected))</c>.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<string?, IThat<string?>> IsValidXmlMatchingExactly(
		this IThat<string?> source,
		string expected)
		=> source.IsValidXml().Which(root => root.MatchesExactly(expected));
}
