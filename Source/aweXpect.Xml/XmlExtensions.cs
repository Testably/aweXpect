using System;
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
	///     <para />
	///     The active <paramref name="options" /> are listed in the "XML options" context of every failure on the
	///     subject, as the string comparison offers no way to add a context only to its own failure.
	/// </remarks>
	public static TResult AsXml<TResult>(
		this TResult result,
		Func<XmlOptions, XmlOptions>? options = null)
		where TResult : IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions
	{
		XmlOptions xmlOptions = options?.Invoke(new XmlOptions()) ?? new XmlOptions();
		XmlMatchType matchType = new(xmlOptions);
		result.Options.SetMatchType(matchType, nameof(AsXml));
		if (result is IOptionsProvider<ExpectationBuilder> expectationBuilder)
		{
			expectationBuilder.Options.AddSubjectContexts<object>((_, contexts) => matchType.AddContext(contexts));
		}

		return result;
	}
}
