using System;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Xml;

namespace aweXpect;

public static partial class ThatXElement
{
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
		Action<IThatSubject<string?>> expectations)
		=> HasValueConstraint.HasValue(subject, expectations, static element => element, "value of the element");
}
