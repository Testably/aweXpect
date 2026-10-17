using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using aweXpect.Core;

namespace aweXpect.Helpers;

/// <summary>
///     Parses XML with DTD processing prohibited and without resolving external resources, so that parsing is not
///     exposed to XML external entity (XXE) or entity expansion attacks.
/// </summary>
internal static class SecureXml
{
	private static readonly XmlReaderSettings DtdProhibitingSettings = new()
	{
		DtdProcessing = DtdProcessing.Prohibit,
		XmlResolver = null,
		IgnoreWhitespace = true,
	};

	private static readonly XmlReaderSettings DtdSkippingSettings = new()
	{
		DtdProcessing = DtdProcessing.Ignore,
		XmlResolver = null,
		IgnoreWhitespace = true,
	};

	/// <remarks>
	///     Insignificant whitespace is dropped, like by <see cref="XDocument.Parse(string)" />.
	/// </remarks>
	/// <exception cref="XmlException">The <paramref name="xml" /> is not a well-formed XML document.</exception>
	public static XDocument Load(string xml)
		=> Load(xml, DtdProhibitingSettings);

	/// <summary>
	///     Loads the <paramref name="expected" /> XML of an expectation.
	/// </summary>
	/// <remarks>
	///     An expected value that is no XML is rejected, as it would differ from every subject, so that a negated
	///     expectation could never fail.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="expected" /> value is <see langword="null" />.</exception>
	/// <exception cref="ArgumentException">The <paramref name="expected" /> value is no valid XML.</exception>
	public static XDocument LoadExpected(string? expected)
	{
		if (expected is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(nameof(expected), "The expected XML cannot be null."));
		}

		try
		{
			return Load(expected);
		}
		catch (XmlException e)
		{
			throw Tracing.WriteException(new ArgumentException(IsCausedByDtd(expected, e)
				? "The expected XML is invalid, because it contains a document type definition (DTD), which is prohibited for security reasons."
				: $"The expected XML is invalid: {e.Message}", e));
		}
	}

	/// <summary>
	///     Describes why the subject <paramref name="xml" /> could not be loaded, following <c>which</c>.
	/// </summary>
	public static string DescribeLoadError(string xml, XmlException exception)
		=> IsCausedByDtd(xml, exception)
			? "could not be parsed as XML, because it contained a document type definition (DTD), which is prohibited for security reasons"
			: $"could not be parsed as XML: {exception.Message}";

	/// <remarks>
	///     The <see cref="XmlException" /> for a prohibited DTD cannot be told apart from other errors and its message
	///     suggests enabling DTD processing. As everything but the DTD is read the same way whether the DTD is
	///     prohibited or skipped, the DTD caused the error when skipping it changes the outcome.
	/// </remarks>
	public static bool IsCausedByDtd(string xml, XmlException errorWhenProhibitingDtd)
	{
		try
		{
			Load(xml, DtdSkippingSettings);
			return true;
		}
		catch (XmlException xmlException)
		{
			return xmlException.Message != errorWhenProhibitingDtd.Message;
		}
	}

	private static XDocument Load(string xml, XmlReaderSettings settings)
	{
		using XmlReader reader = XmlReader.Create(new StringReader(xml), settings);
		return XDocument.Load(reader);
	}
}
