using System;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Xml;

namespace aweXpect;

public static partial class ThatXElement
{
	/// <summary>
	///     Verifies that the subject has an attribute with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The attribute is looked up like by <see cref="XElement.Attribute(XName)" />, so that its namespace must be
	///     specified, e.g. <c>XNamespace.Xml + "lang"</c>, but not its prefix.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement?, IThat<XElement?>> HasAttribute(
		this IThat<XElement?> subject,
		XName name)
		=> HasNodeConstraint.HasAttribute(subject, name, false, static element => element);

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
	public static AndOrResult<XElement?, IThat<XElement?>> HasAttribute(
		this IThat<XElement?> subject,
		XName name,
		Action<IThat<string?>> expectations)
		=> HasNodeConstraint.HasAttribute(subject, name, expectations, static element => element);

	/// <summary>
	///     Verifies that the subject does not have an attribute with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The attribute is looked up like by <see cref="XElement.Attribute(XName)" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement?, IThat<XElement?>> DoesNotHaveAttribute(
		this IThat<XElement?> subject,
		XName name)
		=> HasNodeConstraint.HasAttribute(subject, name, true, static element => element);
}
