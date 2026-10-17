using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Formatting;
using aweXpect.Helpers;
using static aweXpect.Formatting.Format;

namespace aweXpect.Xml;

internal sealed class XmlMatchType(XmlOptions options) : IStringMatchType
{
	private XmlComparer.XmlComparisonResult? _comparisonResult;
	private XDocument? _expectedDocument;
	private string? _expectedXml;

	/// <inheritdoc cref="IStringMatchType.InspectsSubject" />
	public bool InspectsSubject => true;

	/// <inheritdoc
	///     cref="IStringMatchType.GetExtendedFailure(string, string?, string?, bool, IEqualityComparer{string}, StringDifferenceSettings?)" />
	public string GetExtendedFailure(
		string it,
		string? actual,
		string? expected,
		bool ignoreCase,
		IEqualityComparer<string> comparer,
		StringDifferenceSettings? settings)
	{
		if (actual is null)
		{
			return $"{it} was <null>";
		}

		string? result = _comparisonResult?.ToString();
		if (!string.IsNullOrEmpty(result))
		{
			return $"{it} differed as{result}";
		}

		return "";
	}

	/// <inheritdoc cref="IStringMatchType.AreConsideredEqual(string?, string?, bool, IEqualityComparer{string})" />
	/// <remarks>
	///     The state of the previous comparison is reset first, because the same match type compares every value the
	///     expectation is evaluated for.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="expected" /> value is <see langword="null" />.</exception>
	/// <exception cref="ArgumentException">The <paramref name="expected" /> value is no valid XML.</exception>
	public ValueTask<StringMatchResult> AreConsideredEqual(
		string? actual,
		string? expected,
		bool ignoreCase,
		IEqualityComparer<string>? comparer)
	{
		_comparisonResult = null;
		XDocument expectedDocument = GetExpectedDocument(expected);
		if (actual is null)
		{
			return new ValueTask<StringMatchResult>(false);
		}

		XDocument actualDocument;
		try
		{
			actualDocument = SecureXml.Load(actual);
		}
		catch (XmlException e)
		{
			return new ValueTask<StringMatchResult>(StringMatchResult.NotComparable(
				$"it was {Formatter.Format(actual)}, which {SecureXml.DescribeLoadError(actual, e)}", e));
		}

		_comparisonResult = XmlComparer.Compare(actualDocument.Root!, expectedDocument.Root!, options, false);
		return new ValueTask<StringMatchResult>(!_comparisonResult.HasError);
	}

	/// <inheritdoc cref="IStringMatchType.GetExpectation(string?, ExpectationGrammars)" />
	public string GetExpectation(string? expected, ExpectationGrammars grammars)
		=> (grammars.HasFlag(ExpectationGrammars.Active), grammars.HasFlag(ExpectationGrammars.Negated)) switch
		{
			(true, false) => $"{grammars.Verb("is", "are")} XML equivalent to {expected}",
			(false, false) => $"XML equivalent to {expected}",
			(true, true) => $"{grammars.Verb("is not", "are not")} XML equivalent to {expected}",
			(false, true) => $"not XML equivalent to {expected}",
		};

	/// <inheritdoc cref="IStringMatchType.GetTypeString()" />
	public string GetTypeString()
		=> " as XML";

	/// <inheritdoc cref="IStringMatchType.GetOptionString(bool, IEqualityComparer{string})" />
	/// <remarks>
	///     The casing and a comparer are rejected when they are specified, so there is no option to describe.
	/// </remarks>
	public string GetOptionString(bool ignoreCase, IEqualityComparer<string>? comparer)
		=> "";

	/// <inheritdoc cref="IStringMatchType.ValidateOptions(bool, IEqualityComparer{string})" />
	/// <exception cref="InvalidOperationException">
	///     The casing is ignored or a <paramref name="comparer" /> is specified, which an XML comparison cannot honour.
	/// </exception>
	public void ValidateOptions(bool ignoreCase, IEqualityComparer<string>? comparer)
	{
		if (ignoreCase)
		{
			throw Tracing.WriteException(
				new InvalidOperationException("IgnoringCase cannot be combined with AsXml."));
		}

		if (comparer is not null)
		{
			throw Tracing.WriteException(
				new InvalidOperationException("Using cannot be combined with AsXml."));
		}
	}

	/// <inheritdoc cref="IStringMatchType.ValidateExpected(string?)" />
	/// <exception cref="ArgumentNullException">The <paramref name="expected" /> value is <see langword="null" />.</exception>
	/// <exception cref="ArgumentException">The <paramref name="expected" /> value is no valid XML.</exception>
	public void ValidateExpected(string? expected)
		=> GetExpectedDocument(expected);

	/// <remarks>
	///     The last parsed document is kept, because the expected value is validated and compared again for every item of
	///     a collection.
	/// </remarks>
	private XDocument GetExpectedDocument(string? expected)
	{
		if (_expectedDocument is null || !string.Equals(expected, _expectedXml, StringComparison.Ordinal))
		{
			_expectedDocument = SecureXml.LoadExpected(expected);
			_expectedXml = expected;
		}

		return _expectedDocument;
	}
}
