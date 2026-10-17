using System;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Xml;

namespace aweXpect;

/// <summary>
///     XML expectations on <see cref="XElement" /> values.
/// </summary>
public static partial class ThatXElement
{
	/// <summary>
	///     Verifies that the subject matches the <paramref name="expected" /> XML, ignoring additional attributes and
	///     child nodes of the subject.
	/// </summary>
	/// <remarks>
	///     The elements are compared by their content like by <see cref="XmlExtensions.AsXml{TResult}" />, but the
	///     attributes and child nodes of the subject that are not in the <paramref name="expected" /> XML are ignored.
	///     Missing nodes, different values and the order of the child nodes in the <paramref name="expected" /> XML fail
	///     the expectation, unless the order is ignored with <see cref="XmlOptionsExtensions.IgnoringElementOrder" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement?, IThat<XElement?>> Matches(
		this IThat<XElement?> source,
		string expected,
		Func<XmlOptions, XmlOptions>? options = null)
		=> MatchesXmlConstraint<XElement>.Add(source, expected, false, element => element, options);
}
