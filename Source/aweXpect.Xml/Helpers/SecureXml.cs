using System.IO;
using System.Xml;
using System.Xml.Linq;

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
