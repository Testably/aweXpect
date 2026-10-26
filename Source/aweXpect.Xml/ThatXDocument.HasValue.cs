using System;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Xml;
using aweXpect.Xml.Results;

namespace aweXpect;

public static partial class ThatXDocument
{
	/// <summary>
	///     Verifies that the root element of the subject has a value, which can be compared on the result.
	/// </summary>
	/// <remarks>
	///     The value is taken from the root element like by <see cref="ThatXElement.HasValue(IThat{XElement})" />. A
	///     document without a root element fails the expectation, also when it is negated.
	/// </remarks>
	[GuaranteesNotNull]
	public static XmlValueResult<XDocument, IThat<XDocument?>> HasValue(
		this IThat<XDocument?> subject)
		=> HasValueConstraint.HasValue(subject, static document => document.Root, "value of the root element");

	/// <summary>
	///     Verifies that the value of the root element of the subject satisfies the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The value is taken from the root element like by
	///     <see cref="ThatXElement.HasValue(IThat{XElement}, Action{IThatSubject{string}})" />, i.e. the concatenated text of all
	///     its descendant text nodes. A document without a root element fails the expectation, also when it is negated.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<XDocument, IThat<XDocument?>> HasValue(
		this IThat<XDocument?> subject,
		Action<IThatSubject<string>> expectations)
		=> HasValueConstraint.HasValue(subject, expectations, static document => document.Root,
			"value of the root element");
}
