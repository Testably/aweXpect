using System;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Xml;

namespace aweXpect;

public static partial class ThatXElement
{
	/// <summary>
	///     Verifies that the subject has a child element with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The child element is looked up like by <see cref="XContainer.Element(XName)" />, so that its namespace must be
	///     specified, e.g. <c>ns + "item"</c>, but not its prefix.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement?, IThat<XElement?>> HasElement(
		this IThat<XElement?> subject,
		XName name)
		=> HasNodeConstraint.HasElement(subject, name, false, static element => element);

	/// <summary>
	///     Verifies that the subject has a child element with the <paramref name="name" /> which satisfies the
	///     <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The child element is looked up like by <see cref="XContainer.Element(XName)" />, so that its namespace must be
	///     specified, e.g. <c>ns + "item"</c>, but not its prefix, and the <paramref name="expectations" /> are verified on
	///     the first child element with the <paramref name="name" />, when it exists.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement?, IThat<XElement?>> HasElement(
		this IThat<XElement?> subject,
		XName name,
		Action<IThat<XElement?>> expectations)
		=> HasNodeConstraint.HasElement(subject, name, expectations, static element => element);

	/// <summary>
	///     Verifies that the subject does not have a child element with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The child element is looked up like by <see cref="XContainer.Element(XName)" />.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XElement?, IThat<XElement?>> DoesNotHaveElement(
		this IThat<XElement?> subject,
		XName name)
		=> HasNodeConstraint.HasElement(subject, name, true, static element => element);
}
