using System;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Xml;

namespace aweXpect;

public static partial class ThatXDocument
{
	/// <summary>
	///     Verifies that the root element of the subject matches the <paramref name="expected" /> XML exactly.
	/// </summary>
	/// <remarks>
	///     The root elements are compared like by <see cref="ThatXElement.MatchesExactly" />. A document without a root
	///     element fails the expectation, also when it is negated.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XDocument?, IThat<XDocument?>> MatchesExactly(
		this IThat<XDocument?> source,
		string expected,
		Func<XmlOptions, XmlOptions>? options = null)
		=> MatchesXmlConstraint<XDocument>.Add(source, expected, true, document => document.Root, options);
}
