using System.Text;
using System.Xml;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Formatting;
using aweXpect.Helpers;
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
				_document = SecureXml.Load(actual);
			}
			catch (XmlException xmlException)
			{
				_containsDtd = SecureXml.IsCausedByDtd(actual, xmlException);
				_parseError = xmlException.Message;
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
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
