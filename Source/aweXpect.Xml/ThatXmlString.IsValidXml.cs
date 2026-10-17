using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Formatting;
using aweXpect.Results;
using static aweXpect.Formatting.Format;

namespace aweXpect;

/// <summary>
///     XML expectations on <see langword="string" /> values.
/// </summary>
public static partial class ThatXmlString
{
	/// <summary>
	///     Verifies that the subject is a well-formed XML document.
	/// </summary>
	/// <remarks>
	///     A document with a document type definition (<c>&lt;!DOCTYPE ...&gt;</c>) is not valid, as DTD processing is
	///     prohibited and external resources are never resolved, so that parsing is not exposed to XML external entity
	///     (XXE) or entity expansion attacks.
	/// </remarks>
	[GuaranteesNotNull]
	public static XmlWhichResult IsValidXml(
		this IThat<string?> source)
		=> new(source.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsValidXmlConstraint(it, grammars)),
			source);

	private sealed class IsValidXmlConstraint(string it, ExpectationGrammars grammars)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IValueConstraint<string?>
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

		private bool _containsDtd;
		private XDocument? _document;
		private string? _parseError;

		public ConstraintResult IsMetBy(string? actual)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			try
			{
				_document = Load(actual, DtdProhibitingSettings);
			}
			catch (XmlException xmlException)
			{
				_containsDtd = IsCausedByDtd(actual, xmlException.Message);
				_parseError = xmlException.Message;
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		/// <remarks>
		///     The <see cref="XmlException" /> for a prohibited DTD cannot be told apart from other errors and its message
		///     suggests enabling DTD processing. As everything but the DTD is read the same way whether the DTD is
		///     prohibited or skipped, the DTD caused the error when skipping it changes the outcome.
		/// </remarks>
		private static bool IsCausedByDtd(string xml, string errorWhenProhibitingDtd)
		{
			try
			{
				Load(xml, DtdSkippingSettings);
				return true;
			}
			catch (XmlException xmlException)
			{
				return xmlException.Message != errorWhenProhibitingDtd;
			}
		}

		/// <remarks>
		///     Insignificant whitespace is dropped, like by <see cref="XDocument.Parse(string)" />.
		/// </remarks>
		private static XDocument Load(string xml, XmlReaderSettings settings)
		{
			using XmlReader reader = XmlReader.Create(new StringReader(xml), settings);
			return XDocument.Load(reader);
		}

		/// <remarks>
		///     Stores the parsed document, so that <see cref="XmlWhichResult.Which" /> does not parse the subject again and
		///     is not applied to a subject that could not be parsed.
		/// </remarks>
		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (typeof(TValue) == typeof(XDocument) && _document is TValue document)
			{
				value = document;
				return true;
			}

			return base.TryGetStoredValue(out value);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is valid XML", "are valid XML"));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_containsDtd)
			{
				stringBuilder.Append(It).Append(" contained a document type definition (DTD), which is prohibited for security reasons");
				return;
			}

			stringBuilder.Append(It).Append(" could not be parsed: ").Append(_parseError);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is not valid XML", "are not valid XML"));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
