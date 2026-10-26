using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Formatting;
using aweXpect.Helpers;
using aweXpect.Xml.Options;
using static aweXpect.Formatting.Format;

namespace aweXpect.Xml;

internal sealed class XmlMatchType(XmlOptions options) : StringMatchType
{
	private static readonly MatchPhrase SharedPhrase =
		MatchPhrase.Being("XML equivalent to").WithName(" as XML").WithUnformattedExpected();

	private ParsedExpected? _parsedExpected;

	/// <inheritdoc />
	public override bool InspectsSubject => true;

	/// <inheritdoc />
	public override StringMatcher ForEvaluation(MatchEnvironment environment, StringMatchSettings settings)
		=> new XmlMatcher(this, options);

	/// <inheritdoc />
	public override MatchPhrase Phrase => SharedPhrase;

	/// <inheritdoc />
	/// <exception cref="ArgumentNullException">The <paramref name="expected" /> value is <see langword="null" />.</exception>
	/// <exception cref="ArgumentException">The <paramref name="expected" /> value is no valid XML.</exception>
	public override void ValidateExpected(string? expected, string parameterName)
		=> GetExpectedDocument(expected, parameterName);

	/// <remarks>
	///     The last parsed document is kept, because the expected value is validated and compared again for every item of
	///     a collection, and in every evaluation.
	/// </remarks>
	private XDocument GetExpectedDocument(string? expected, string parameterName)
	{
		ParsedExpected? parsedExpected = _parsedExpected;
		if (parsedExpected is null || !string.Equals(expected, parsedExpected.Xml, StringComparison.Ordinal))
		{
			parsedExpected = new ParsedExpected(expected, SecureXml.LoadExpected(expected, parameterName));
			_parsedExpected = parsedExpected;
		}

		return parsedExpected.Document;
	}

	/// <remarks>
	///     Immutable, so that it can be replaced as a whole while another evaluation reads it.
	/// </remarks>
	private sealed class ParsedExpected(string? xml, XDocument document)
	{
		public string? Xml { get; } = xml;
		public XDocument Document { get; } = document;
	}

	/// <summary>
	///     Compares the XML strings of one evaluation and keeps the members to ignore that matched no member.
	/// </summary>
	private sealed class XmlMatcher(XmlMatchType matchType, XmlOptions options) : StringMatcher
	{
		private Dictionary<string, string?>? _unmatchedMembersToIgnore;

		/// <inheritdoc />
		/// <exception cref="ArgumentNullException">The <paramref name="expected" /> value is <see langword="null" />.</exception>
		/// <exception cref="ArgumentException">The <paramref name="expected" /> value is no valid XML.</exception>
		public override ValueTask<MatchResult> Match(string? actual, string? expected)
		{
			XDocument expectedDocument = matchType.GetExpectedDocument(expected, nameof(expected));
			if (actual is null)
			{
				return new ValueTask<MatchResult>(MatchResult.NoMatch);
			}

			XDocument actualDocument;
			try
			{
				actualDocument = SecureXml.Load(actual);
			}
			catch (XmlException e)
			{
				return new ValueTask<MatchResult>(MatchResult.NotComparable(
					$"was {Formatter.Format(actual)}, which {SecureXml.DescribeLoadError(actual, e)}", e));
			}

			XmlComparer.XmlComparisonResult comparisonResult =
				XmlComparer.Compare(actualDocument.Root!, expectedDocument.Root!, options);
			_unmatchedMembersToIgnore = MembersToIgnoreTracker.Keep(_unmatchedMembersToIgnore,
				comparisonResult.GetUnmatchedMembersToIgnore());
			return new ValueTask<MatchResult>(comparisonResult.HasError
				? MatchResult.NoMatchBecause(new DifferencesExplanation(comparisonResult))
				: MatchResult.Match);
		}

		/// <inheritdoc />
		public override string GetFailure(string it, ExpectationGrammars grammars, string? actual, string? expected,
			MatchResult result, StringDifferenceSettings? differenceSettings)
		{
			if (actual is null)
			{
				return $"{it}{grammars.SubjectVerb(it, " was ", " were ")}<null>";
			}

			return result.Explanation?.GetFailure(it, grammars, null) ?? "";
		}

		/// <summary>
		///     Adds the "XML options" context with the members to ignore by path that matched no member in a failed
		///     comparison.
		/// </summary>
		/// <remarks>
		///     The items of a collection are compared one after the other with the same matcher, so the members to ignore
		///     of every failed comparison are kept, and a suggestion found for an item is not lost to a later item.
		/// </remarks>
		public override void AppendContexts(ResultContextCollector contexts)
			=> options.AddContext(contexts, _unmatchedMembersToIgnore);
	}

	/// <summary>
	///     The differences that an XML comparison found.
	/// </summary>
	private sealed class DifferencesExplanation(XmlComparer.XmlComparisonResult comparisonResult)
		: MatchExplanation
	{
		/// <inheritdoc />
		public override string GetFailure(string it, ExpectationGrammars grammars, string? indentation)
		{
			string result = comparisonResult.ToString();
			return string.IsNullOrEmpty(result) ? "" : $"{it} differed as{result}";
		}
	}
}
