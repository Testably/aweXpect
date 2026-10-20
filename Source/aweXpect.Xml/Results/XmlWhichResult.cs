using System;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect.Results;

/// <summary>
///     An <see cref="AndOrResult{TType,TThat}" /> for XML strings.
/// </summary>
/// <remarks>
///     <see cref="Which" /> verifies the document that <see cref="ThatXmlString.IsValidXml" /> parsed, so the result can
///     only be created by it.
/// </remarks>
public sealed class XmlWhichResult : AndOrResult<string?, IThat<string?>>
{
	internal XmlWhichResult(ExpectationBuilder expectationBuilder, IThat<string?> returnValue)
		: base(expectationBuilder, returnValue)
	{
	}

	/// <summary>
	///     Allows specifying <paramref name="expectations" /> on the root <see cref="XElement" />
	///     of the document represented by the <see langword="string" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expectations" /> are only verified, when the <see langword="string" /> is valid XML.
	///     Insignificant whitespace is dropped, like by <see cref="XDocument.Parse(string)" />.
	/// </remarks>
	public AndOrResult<string?, IThat<string?>> Which(Action<IThat<XElement?>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		ExpectationBuilder
			.ForMember(MemberAccessor<XDocument, XElement?>.FromFunc(document => document.Root, "it"),
				(_, stringBuilder) => stringBuilder.Append(" which "))
			.AddExpectations(e => expectations(new ThatSubject<XElement?>(e)));
		return this;
	}
}
