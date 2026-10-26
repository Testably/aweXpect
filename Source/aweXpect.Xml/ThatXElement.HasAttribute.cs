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
	///     Verifies that the subject has an attribute with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The attribute is looked up like by <see cref="XElement.Attribute(XName)" />, so that its namespace must be
	///     specified, e.g. <c>XNamespace.Xml + "lang"</c>, but not its prefix. Its value can be compared on the result in
	///     the same expectation, e.g. with <c>EqualTo("7")</c>, <c>StartingWith("Let").IgnoringCase()</c> or
	///     <c>Containing("e").AtLeast(2)</c>. A missing attribute fails the expectation, but satisfies its negation.
	/// </remarks>
	[GuaranteesNotNull]
	public static XmlValueResult<XElement, IThat<XElement?>> HasAttribute(
		this IThat<XElement?> subject,
		XName name)
		=> HasNodeConstraint.HasAttribute(subject, name, static element => element);

	/// <summary>
	///     Verifies that the subject has an attribute with the <paramref name="name" /> whose value satisfies the
	///     <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The attribute is looked up like by <see cref="XElement.Attribute(XName)" />, so that its namespace must be
	///     specified, e.g. <c>XNamespace.Xml + "lang"</c>, but not its prefix. The <paramref name="expectations" /> are only
	///     verified, when the attribute exists.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement, IThat<XElement?>> HasAttribute(
		this IThat<XElement?> subject,
		XName name,
		Action<IThatSubject<string>> expectations)
		=> HasNodeConstraint.HasAttribute(subject, name, expectations, static element => element);

	/// <summary>
	///     Verifies that the subject does not have an attribute with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The attribute is looked up like by <see cref="XElement.Attribute(XName)" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement, IThat<XElement?>> DoesNotHaveAttribute(
		this IThat<XElement?> subject,
		XName name)
		=> HasNodeConstraint.DoesNotHaveAttribute(subject, name, static element => element);
}
