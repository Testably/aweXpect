using System;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Xml;

namespace aweXpect;

/// <summary>
///     XML expectations on <see cref="XDocument" /> values.
/// </summary>
public static partial class ThatXDocument
{
	/// <summary>
	///     Verifies that the root element of the subject matches the <paramref name="expected" /> XML, ignoring additional
	///     attributes and child nodes of the subject.
	/// </summary>
	/// <remarks>
	///     The root elements are compared like by <see cref="ThatXElement.Matches" />. A document without a root element
	///     fails the expectation, also when it is negated.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XDocument?, IThat<XDocument?>> Matches(
		this IThat<XDocument?> source,
		string expected,
		Func<XmlOptions, XmlOptions>? options = null)
		=> MatchesXmlConstraint<XDocument>.Add(source, expected, false, document => document.Root, options);
}
