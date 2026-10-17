using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Xml;

namespace aweXpect;

/// <summary>
///     Extension methods for working with XML strings.
/// </summary>
public static class XmlExtensions
{
	/// <summary>
	///     Interpret the <see cref="string" /> as XML.
	/// </summary>
	/// <remarks>
	///     The documents are compared by their content instead of their formatting: elements and attributes by their
	///     namespace and local name instead of their prefix, and attributes regardless of their order. Whitespace-only
	///     text between elements, comments, processing instructions and the XML declaration are ignored, and CDATA
	///     sections are compared as text. Like <see cref="ThatXmlString.IsValidXml" />, a document type definition (DTD)
	///     is prohibited.
	/// </remarks>
	public static TResult AsXml<TResult>(this TResult result)
		where TResult : IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions
	{
		result.Options.SetMatchType(new XmlMatchType(), nameof(AsXml));
		return result;
	}
}
