using System;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Xml;

namespace aweXpect;

public static partial class ThatXElement
{
	/// <summary>
	///     Verifies that the subject matches the <paramref name="expected" /> XML exactly.
	/// </summary>
	/// <remarks>
	///     The elements are compared by their content like by <see cref="XmlExtensions.AsXml{TResult}" />, so that
	///     additional attributes and child nodes of the subject fail the expectation.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement?, IThat<XElement?>> MatchesExactly(
		this IThat<XElement?> subject,
		string expected,
		Func<XmlOptions, XmlOptions>? options = null)
		=> MatchesXmlConstraint<XElement>.Add(subject, expected, true, element => element, options);
}
