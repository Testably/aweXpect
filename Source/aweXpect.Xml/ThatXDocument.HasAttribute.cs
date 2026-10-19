using System;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Xml;

namespace aweXpect;

public static partial class ThatXDocument
{
	/// <summary>
	///     Verifies that the root element of the subject has an attribute with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The attribute is looked up on the root element like by
	///     <see cref="ThatXElement.HasAttribute(IThat{XElement}, XName)" />. A document without a root element fails the
	///     expectation, also when it is negated.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XDocument?, IThat<XDocument?>> HasAttribute(
		this IThat<XDocument?> subject,
		XName name)
		=> HasNodeConstraint.HasAttribute(subject, name, false, static document => document.Root);

	/// <summary>
	///     Verifies that the root element of the subject has an attribute with the <paramref name="name" /> whose value
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The attribute is looked up on the root element like by
	///     <see cref="ThatXElement.HasAttribute(IThat{XElement}, XName, Action{IThat{string}})" />. A document without a root
	///     element fails the expectation, also when it is negated.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XDocument?, IThat<XDocument?>> HasAttribute(
		this IThat<XDocument?> subject,
		XName name,
		Action<IThat<string?>> expectations)
		=> HasNodeConstraint.HasAttribute(subject, name, expectations, static document => document.Root);

	/// <summary>
	///     Verifies that the root element of the subject does not have an attribute with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The attribute is looked up on the root element like by
	///     <see cref="ThatXElement.DoesNotHaveAttribute(IThat{XElement}, XName)" />. A document without a root element
	///     fails the expectation.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XDocument?, IThat<XDocument?>> DoesNotHaveAttribute(
		this IThat<XDocument?> subject,
		XName name)
		=> HasNodeConstraint.HasAttribute(subject, name, true, static document => document.Root);
}
