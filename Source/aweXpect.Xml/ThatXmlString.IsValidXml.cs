using System;
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
	public static AndOrResult<string, IThat<string?>> IsValidXml(
		this IThat<string?> subject)
		=> new(AddIsValidXmlConstraint(subject), subject);

	/// <summary>
	///     Verifies that the subject is a well-formed XML document whose root <see cref="XElement" /> satisfies the
	///     <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expectations" /> are only verified, when the <see langword="string" /> is valid XML.
	///     A subject that is no valid XML fails also when negated, as the <paramref name="expectations" /> cannot be
	///     verified.
	///     Insignificant whitespace is dropped, like by <see cref="XDocument.Parse(string)" />.
	///     <para />
	///     A document with a document type definition (<c>&lt;!DOCTYPE ...&gt;</c>) is not valid, as DTD processing is
	///     prohibited and external resources are never resolved, so that parsing is not exposed to XML external entity
	///     (XXE) or entity expansion attacks.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<string, IThat<string?>> IsValidXml(
		this IThat<string?> subject,
		Action<IThatSubject<XElement>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return new AndOrResult<string, IThat<string?>>(AddIsValidXmlConstraint(subject, expectations), subject);
	}

	/// <summary>
	///     Verifies that the subject is not a well-formed XML document.
	/// </summary>
	/// <remarks>
	///     A document with a document type definition (<c>&lt;!DOCTYPE ...&gt;</c>) is not valid, as DTD processing is
	///     prohibited and external resources are never resolved, so that parsing is not exposed to XML external entity
	///     (XXE) or entity expansion attacks.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<string, IThat<string?>> IsNotValidXml(
		this IThat<string?> subject)
		=> new(subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
			=> new IsValidXmlConstraint(it, grammars).Invert()), subject);

	private static ExpectationBuilder AddIsValidXmlConstraint(IThat<string?> subject)
		=> subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
			=> new IsValidXmlConstraint(it, grammars));

	private static ExpectationBuilder AddIsValidXmlConstraint(IThat<string?> subject,
		Action<IThatSubject<XElement>> expectations)
		=> subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
				=> new IsValidXmlConstraint(it, grammars, true))
			.ForMember(MemberAccessor<XDocument, XElement>.FromFuncWithName(document => document.Root!, "it"),
				(_, stringBuilder) => stringBuilder.Append(" which "))
			.AddExpectations(expectations,
				static (expectations, e) => expectations(new ThatSubject<XElement>(e)));

	private sealed class IsValidXmlConstraint(string it, ExpectationGrammars grammars, bool hasExpectations = false)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IValueConstraint<string?>
	{
		private bool _containsDtd;
		private XDocument? _document;
		private string? _parseError;

		public ConstraintResult IsMetBy(string? actual)
		{
			Actual = actual;
			_containsDtd = false;
			_parseError = null;
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
				// The expectations on the root element cannot be verified, so their negation cannot be met either.
				Outcome = hasExpectations ? Outcome.FailureBothWays : Outcome.Failure;
				return this;
			}

			Outcome = Outcome.Success;
			return this;
		}

		/// <remarks>
		///     Stores the parsed document, so that the expectations on the root element do not parse the subject again and
		///     are not applied to a subject that could not be parsed.
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
			if (_parseError is not null)
			{
				AppendNormalResult(stringBuilder, indentation);
				return;
			}

			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
