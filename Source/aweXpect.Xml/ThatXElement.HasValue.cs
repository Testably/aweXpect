using System;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Xml;
using aweXpect.Xml.Results;

namespace aweXpect;

public static partial class ThatXElement
{
	/// <summary>
	///     Verifies that the subject has a value, which can be compared on the result.
	/// </summary>
	/// <remarks>
	///     The value is compared on the result in the same expectation, e.g. with <c>EqualTo("Let It Be")</c>,
	///     <c>StartingWith("Let").IgnoringCase()</c> or <c>Containing("e").AtLeast(2)</c>. It is
	///     <see cref="XElement.Value" />, like for <see cref="HasValue(IThat{XElement}, Action{IThatSubject{string}})" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static XmlValueResult<XElement, IThat<XElement?>> HasValue(
		this IThat<XElement?> subject)
		=> HasValueConstraint.HasValue(subject, static element => element, "value of the element");

	/// <summary>
	///     Verifies that the value of the subject satisfies the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The value is <see cref="XElement.Value" />, i.e. the concatenated text of all descendant text nodes, including
	///     CDATA sections. For mixed content, e.g. <c>&lt;p&gt;a&lt;b&gt;x&lt;/b&gt;c&lt;/p&gt;</c>, it is <c>axc</c>, and
	///     for an element without text it is empty.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement, IThat<XElement?>> HasValue(
		this IThat<XElement?> subject,
		Action<IThatSubject<string>> expectations)
		=> HasValueConstraint.HasValue(subject, expectations, static element => element, "value of the element");
}
