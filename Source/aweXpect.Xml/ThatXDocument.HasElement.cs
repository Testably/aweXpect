using System;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Xml;

namespace aweXpect;

public static partial class ThatXDocument
{
	/// <summary>
	///     Verifies that the root element of the subject has a child element with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The child element is looked up in the root element like by
	///     <see cref="ThatXElement.HasElement(IThat{XElement}, XName)" />, so that the root element itself never matches,
	///     unlike by <see cref="XContainer.Element(XName)" /> on the document. A document without a root element fails the
	///     expectation, also when it is negated.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XDocument?, IThat<XDocument?>> HasElement(
		this IThat<XDocument?> subject,
		XName name)
		=> HasNodeConstraint.HasElement(subject, name, false, static document => document.Root);

	/// <summary>
	///     Verifies that the root element of the subject has a child element with the <paramref name="name" /> which
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The child element is looked up in the root element like by
	///     <see cref="ThatXElement.HasElement(IThat{XElement}, XName, Action{IThat{XElement}})" />, so that the root element
	///     itself never matches, unlike by <see cref="XContainer.Element(XName)" /> on the document. A document without a
	///     root element fails the expectation, also when it is negated.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XDocument?, IThat<XDocument?>> HasElement(
		this IThat<XDocument?> subject,
		XName name,
		Action<IThat<XElement?>> expectations)
		=> HasNodeConstraint.HasElement(subject, name, expectations, static document => document.Root);

	/// <summary>
	///     Verifies that the root element of the subject does not have a child element with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The child element is looked up in the root element like by
	///     <see cref="ThatXElement.DoesNotHaveElement(IThat{XElement}, XName)" />, so that the root element itself is not
	///     considered. A document without a root element fails the expectation.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XDocument?, IThat<XDocument?>> DoesNotHaveElement(
		this IThat<XDocument?> subject,
		XName name)
		=> HasNodeConstraint.HasElement(subject, name, true, static document => document.Root);
}
