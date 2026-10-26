using System;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Xml;
using aweXpect.Xml.Options;

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
	///     sections are compared as text. Like <see cref="ThatXmlString.IsValidXml(IThat{string})" />, a document type
	///     definition (DTD) is prohibited.
	///     <para />
	///     The active <paramref name="options" /> are listed in the "XML options" context of a failed comparison.
	/// </remarks>
	public static TSelf AsXml<TSelf>(
		this TSelf result,
		Func<XmlOptions, XmlOptions>? options = null)
		where TSelf : IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions
	{
		XmlOptions xmlOptions = options?.Invoke(new XmlOptions()) ?? new XmlOptions();
		XmlMatchType matchType = new(xmlOptions);
		if (result is ICopyOnOption<StringEqualityOptions, TSelf> copyOnOption)
		{
			return copyOnOption.WithOptions(result.Options.Value.SetMatchType(matchType, nameof(AsXml)));
		}

		result.Options.SetMatchType(matchType, nameof(AsXml));
		return result;
	}
}
