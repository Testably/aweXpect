using System;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Helpers;

namespace aweXpect.Results;

/// <summary>
///     An <see cref="AndOrResult{TType,TThat}" /> for XML strings.
/// </summary>
public class XmlWhichResult(
	ExpectationBuilder expectationBuilder,
	IThat<string?> returnValue) : AndOrResult<string?, IThat<string?>>(expectationBuilder, returnValue)
{
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
