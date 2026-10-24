using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Customization;

namespace aweXpect.Options;

/// <summary>
///     Equality options for <see langword="string" />s.
/// </summary>
public partial class StringEqualityOptions : IOptionsEquality<string?>
{
	private const int DefaultMaxLength = 30;

	private readonly string _parameterName;
	private IEqualityComparer<string>? _comparer;
	private bool _ignoreCase;
	private bool _ignoreIndentation;
	private bool _ignoreLeadingWhiteSpace;
	private bool _ignoreNewlineStyle;
	private bool _ignoreTrailingWhiteSpace;
	private bool _isIgnoreCaseSpecified;
	private bool _isIgnoreIndentationSpecified;
	private bool _isIgnoreLeadingWhiteSpaceSpecified;
	private bool _isIgnoreNewlineStyleSpecified;
	private bool _isIgnoreTrailingWhiteSpaceSpecified;
	private bool _isNormalizeWhiteSpaceSpecified;
	private bool _normalizeWhiteSpace;

	/// <summary>
	///     The first specified option that changes the lines, which <see cref="AsBlock()" /> cannot honour.
	/// </summary>
	private string? _lineOption;

	/// <summary>
	///     The matcher that compares the strings, created for the first evaluation or comparison.
	/// </summary>
	/// <remarks>
	///     A match type without state per evaluation returns the same matcher for every evaluation, so the options are
	///     only copied for an evaluation that gets a matcher of its own.
	/// </remarks>
	private StringMatcher? _matcher;

	private StringMatchType _matchType = ExactMatch;
	private string? _matchTypeOption;

	/// <summary>
	///     The pattern that was parsed last, because a collection expectation compares every item with the same one.
	/// </summary>
	private ParsedPattern? _parsedPattern;

	/// <summary>
	///     Initializes the options for a pattern that the caller received as <paramref name="parameterName" />, which
	///     an unusable pattern is reported against.
	/// </summary>
	public StringEqualityOptions(string parameterName)
	{
		_parameterName = parameterName;
	}

	/// <summary>
	///     Copies the <paramref name="options" /> for an evaluation that compares with the <paramref name="matcher" />.
	/// </summary>
	private StringEqualityOptions(StringEqualityOptions options, StringMatcher matcher)
	{
		_parameterName = options._parameterName;
		_comparer = options._comparer;
		_ignoreCase = options._ignoreCase;
		_ignoreIndentation = options._ignoreIndentation;
		_ignoreLeadingWhiteSpace = options._ignoreLeadingWhiteSpace;
		_ignoreNewlineStyle = options._ignoreNewlineStyle;
		_ignoreTrailingWhiteSpace = options._ignoreTrailingWhiteSpace;
		_isIgnoreCaseSpecified = options._isIgnoreCaseSpecified;
		_isIgnoreIndentationSpecified = options._isIgnoreIndentationSpecified;
		_isIgnoreLeadingWhiteSpaceSpecified = options._isIgnoreLeadingWhiteSpaceSpecified;
		_isIgnoreNewlineStyleSpecified = options._isIgnoreNewlineStyleSpecified;
		_isIgnoreTrailingWhiteSpaceSpecified = options._isIgnoreTrailingWhiteSpaceSpecified;
		_isNormalizeWhiteSpaceSpecified = options._isNormalizeWhiteSpaceSpecified;
		_normalizeWhiteSpace = options._normalizeWhiteSpace;
		_lineOption = options._lineOption;
		_matchType = options._matchType;
		_matchTypeOption = options._matchTypeOption;
		_parsedPattern = options._parsedPattern;
		_matcher = matcher;
	}

	/// <summary>
	///     The settings that the matcher of an evaluation is created with.
	/// </summary>
	private StringMatchSettings Settings => new(_ignoreCase, _comparer);

	/// <summary>
	///     Indicates whether the current match type inspects the content of the subject instead of comparing it as a value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> subject has no content to inspect, so an inspecting match type must fail for it in both
	///     polarities, exactly like the dedicated <c>StartsWith</c> / <c>DoesNotStartWith</c> expectations do.
	/// </remarks>
	public bool InspectsSubject => _matchType.InspectsSubject;

	/// <summary>
	///     Indicates whether the options compare two <see langword="string" />s with plain ordinal equality, i.e. whether
	///     neither the match type nor any option that changes the comparison was set.
	/// </summary>
	/// <remarks>
	///     This lets an expectation hand the comparison to a collection that has its own comparer, as long as nothing
	///     was configured that the collection could not honour.
	/// </remarks>
	public bool ComparesByOrdinalEquality
		=> _matchType is ExactMatchType && _comparer is null && !_ignoreCase && !_ignoreLeadingWhiteSpace &&
		   !_ignoreTrailingWhiteSpace && !_ignoreNewlineStyle && !_ignoreIndentation && !_normalizeWhiteSpace;

	/// <inheritdoc />
	/// <remarks>
	///     The pattern is validated outside the asynchronous part, so that an unusable pattern throws at the call
	///     instead of only when the returned task is awaited.
	/// </remarks>
	public ValueTask<bool> AreConsideredEqual<TExpected>(string? actual, TExpected expected)
	{
		// Without a pattern or an option, nothing is validated or normalized, and every item of a string collection
		// is compared this way.
		if (ComparesByOrdinalEquality)
		{
			return new ValueTask<bool>(string.Equals(actual, expected as string));
		}

		ValueTask<MatchResult> result = Compare(actual, expected as string);
		if (!result.IsCompletedSuccessfully)
		{
			return AwaitIsMatch(result);
		}

		return new ValueTask<bool>(result.Result.IsMatch);

		static async ValueTask<bool> AwaitIsMatch(ValueTask<MatchResult> result)
			=> (await result).IsMatch;
	}

	/// <summary>
	///     Compares the <paramref name="actual" /> with the <paramref name="expected" /> value like
	///     <see cref="AreConsideredEqual{TExpected}(string?, TExpected)" />, and returns the result, from which
	///     <see cref="GetExtendedFailure(string, ExpectationGrammars, string?, string?, MatchResult)" /> explains a
	///     failure.
	/// </summary>
	/// <remarks>
	///     A constraint keeps the result for its failure message, so that the options keep nothing of a comparison.<br />
	///     A subject that the match type cannot compare throws, so that the expectation fails in both polarities.
	/// </remarks>
	public ValueTask<MatchResult> MatchWithExplanation(string? actual, string? expected)
	{
		if (ComparesByOrdinalEquality)
		{
			return new ValueTask<MatchResult>(MatchResult.From(string.Equals(actual, expected)));
		}

		return Compare(actual, expected);
	}

	/// <summary>
	///     Returns the options to use for all comparisons of the evaluation in the <paramref name="context" />.
	/// </summary>
	/// <remarks>
	///     The match type creates the matcher of the evaluation with the settings that are specified at this point, so
	///     that it can keep state across the items of a collection and use the <paramref name="context" /> and the
	///     <paramref name="cancellationToken" />. A constraint compares with the returned options, and also appends its
	///     contexts and explains its failures with them, as only they know what the comparisons of the evaluation
	///     found.<br />
	///     The options return themselves, unless the evaluation gets a matcher of its own.
	/// </remarks>
	public StringEqualityOptions ForEvaluation(IEvaluationContext context, CancellationToken cancellationToken)
	{
		if (ComparesByOrdinalEquality)
		{
			return this;
		}

		StringMatcher matcher = _matchType.ForEvaluation(new MatchEnvironment(context, cancellationToken), Settings);
		StringMatcher? current = Interlocked.CompareExchange(ref _matcher, matcher, null);
		return current is null || ReferenceEquals(current, matcher)
			? this
			: new StringEqualityOptions(this, matcher);
	}

	/// <summary>
	///     Returns the matcher of the evaluation, or for a comparison outside an evaluation, a matcher without one.
	/// </summary>
	private StringMatcher GetMatcher()
	{
		StringMatcher? matcher = _matcher;
		if (matcher is null)
		{
			matcher = _matchType.ForEvaluation(MatchEnvironment.WithoutEvaluation, Settings);
			_matcher = matcher;
		}

		return matcher;
	}

	/// <summary>
	///     Validates and normalizes the <paramref name="expected" /> value and compares it with the normalized
	///     <paramref name="actual" /> value.
	/// </summary>
	private ValueTask<MatchResult> Compare(string? actual, string? expected)
	{
		if (expected is null)
		{
			ValidatePattern(null);
			return CompareWithMatchType(actual, null);
		}

		(bool IsStartAnchored, bool IsEndAnchored) anchoredEdges = GetAnchoredEdges();
		expected = NormalizeExpected(expected, anchoredEdges);
		Regex? regex = ValidatePattern(expected);
		return AreConsideredEqualToPattern(
			PadWithInnerWhiteSpace(Normalize(actual), expected, anchoredEdges), expected, regex);
	}

	/// <summary>
	///     Verifies that the <paramref name="expected" /> value is usable for the current match type, exactly like
	///     <see cref="AreConsideredEqual{TExpected}(string?, TExpected)" /> does when a value is compared with it.
	/// </summary>
	/// <remarks>
	///     An expectation calls this for every expected value before it looks at the subject, so that an unusable
	///     value is rejected whichever subject it is verified for, also when no value is compared with it.<br />
	///     A custom match type rejects the value in its <see cref="StringMatchType.ValidateExpected(string?)" />, whose
	///     exception is thrown unchanged.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	///     The match type takes a pattern, a prefix or a suffix and the <paramref name="expected" /> value is
	///     <see langword="null" />.
	/// </exception>
	/// <exception cref="ArgumentException">
	///     The <paramref name="expected" /> regex pattern, prefix or suffix is empty, or the regex pattern is invalid.
	/// </exception>
	public void ValidateExpected(string? expected)
		=> ValidatePattern(NormalizeExpected(expected, GetAnchoredEdges()));

	/// <summary>
	///     Compares the already normalized <paramref name="actual" /> value with the already normalized and validated
	///     <paramref name="expected" /> pattern, which was parsed as <paramref name="regex" /> for a regex match type.
	/// </summary>
	/// <remarks>
	///     Without a pattern, a comparison that completes synchronously is returned without a state machine, because
	///     every item of a string collection is compared this way.
	/// </remarks>
	private ValueTask<MatchResult> AreConsideredEqualToPattern(string? actual, string expected, Regex? regex)
	{
		if (regex is not null || _matchType is WildcardMatchType)
		{
			return AreConsideredEqualToPatternAsync(actual, expected, regex);
		}

		return CompareWithMatchType(actual, expected);
	}

	/// <summary>
	///     Compares the <paramref name="actual" /> value with the <paramref name="expected" /> value by the matcher.
	/// </summary>
	/// <remarks>
	///     A subject that the matcher cannot compare is reported like an exception of the caller's code, so that every
	///     expectation and every collection fails for it in both polarities, without having to check for it.
	/// </remarks>
	private ValueTask<MatchResult> CompareWithMatchType(string? actual, string? expected)
	{
		ValueTask<MatchResult> result;
		try
		{
			result = GetMatcher().Match(actual, expected);
		}
		catch (RegexMatchTimeoutException exception)
		{
			throw CreateTimeoutException(expected, exception);
		}

		if (!result.IsCompletedSuccessfully)
		{
			return AwaitTheComparison(result, expected);
		}

		return new ValueTask<MatchResult>(ComparableOrThrow(result.Result));
	}

	private async ValueTask<MatchResult> AwaitTheComparison(ValueTask<MatchResult> result, string? expected)
	{
		try
		{
			return ComparableOrThrow(await result);
		}
		catch (RegexMatchTimeoutException exception)
		{
			throw CreateTimeoutException(expected, exception);
		}
	}

	private static MatchResult ComparableOrThrow(MatchResult result)
		=> result.Outcome != MatchOutcome.NotComparable
			? result
			: throw new UserCodeException(new NotComparableException(result.NotComparableReason!, result.Cause));

	private async ValueTask<MatchResult> AreConsideredEqualToPatternAsync(string? actual, string expected,
		Regex? regex)
	{
		try
		{
			if (regex is not null)
			{
				return MatchResult.From(actual is not null && regex.IsMatch(actual));
			}

			if (_matchType is WildcardMatchType)
			{
				return MatchResult.From(actual is not null && WildcardMatchType.IsMatch(
					GetParsedPattern(expected, static (_, pattern, ignoreCase)
						=> WildcardMatchType.CreateRegex(pattern, ignoreCase)), actual, _ignoreCase));
			}

			return await CompareWithMatchType(actual, expected);
		}
		catch (RegexMatchTimeoutException exception)
		{
			throw CreateTimeoutException(expected, exception);
		}
	}

	/// <summary>
	///     Counts how often the <paramref name="expected" /> <see langword="string" /> occurs
	///     in the <paramref name="actual" /> <see langword="string" />.
	/// </summary>
	/// <remarks>
	///     Both strings are normalized once before the comparison, so that the options which change the length of the
	///     strings are applied to the complete strings and not to the individual substrings that are compared.<br />
	///     The leading or trailing whitespace of the <paramref name="expected" /> <see langword="string" /> is only ignored
	///     where an occurrence reaches the start or the end of the <paramref name="actual" /> <see langword="string" />, so
	///     that whitespace inside it still has to match.<br />
	///     An <paramref name="expected" /> <see langword="string" /> that is empty after the normalization is rejected
	///     for every match type, because it never occurs, so that a negated expectation could never fail.<br />
	///     The pattern is validated outside the asynchronous part, so that an unusable pattern throws at the call
	///     instead of only when the returned task is awaited.
	/// </remarks>
	/// <exception cref="ArgumentException">
	///     The <paramref name="expected" /> <see langword="string" /> is empty after the normalization.
	/// </exception>
	public ValueTask<int> CountOccurrences(string actual, string expected)
		=> CountOccurrences(actual, expected, int.MaxValue);

	/// <summary>
	///     Counts how often the <paramref name="expected" /> <see langword="string" /> occurs
	///     in the <paramref name="actual" /> <see langword="string" />, but at most <paramref name="maximum" /> times.
	/// </summary>
	/// <remarks>
	///     The search stops at the <paramref name="maximum" />, so that an expectation which is decided by a number of
	///     occurrences, e.g. at least one, does not search the rest of a long <paramref name="actual" /> string. The
	///     occurrences are counted like by <see cref="CountOccurrences(string, string)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">
	///     The <paramref name="expected" /> <see langword="string" /> is empty after the normalization.
	/// </exception>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="maximum" /> is not positive.</exception>
	public ValueTask<int> CountOccurrences(string actual, string expected, int maximum)
	{
		if (maximum <= 0)
		{
			throw Tracing.WriteException(new ArgumentOutOfRangeException(nameof(maximum),
				"The maximum must be greater than zero."));
		}

		expected = NormalizeExpected(expected, (false, false));
		Regex? regex = ValidatePattern(expected);
		if (expected.Length == 0)
		{
			throw CreateEmptyPatternException(GetPatternKind() ?? "string");
		}

		actual = PadWithInnerWhiteSpace(Normalize(actual), expected, (false, false));

		int? count = CountOccurrencesWithoutWindow(actual, expected, regex, maximum);
		if (count is not null)
		{
			return new ValueTask<int>(count.Value);
		}

		return CountOccurrencesWithWindow(actual, expected, maximum);
	}

	/// <summary>
	///     Describes where the first <paramref name="count" /> occurrences of the <paramref name="expected" />
	///     <see langword="string" /> are in the <paramref name="actual" /> <see langword="string" />, or without an
	///     occurrence, how it is missed, to extend a failure that reports the <paramref name="count" /> from
	///     <see cref="CountOccurrences(string, string, int)" />.
	/// </summary>
	/// <remarks>
	///     The text starts with its own separator, e.g. <c>" at lines 3-4"</c> or <c>", and the closest match at …"</c>,
	///     so it is appended directly after the formatted <paramref name="actual" /> <see langword="string" />.<br />
	///     Only a block via <see cref="AsBlock()" /> has details, so the text is empty for any other match type. It is
	///     also empty when a custom comparer throws, because the failure has to be reported anyway, or when a positive
	///     <paramref name="count" /> is given for an <paramref name="actual" /> <see langword="string" /> without an
	///     occurrence.
	/// </remarks>
	public string GetOccurrenceDetails(string actual, string expected, int count)
	{
		if (_matchType is not BlockMatchType || expected.Length == 0)
		{
			return "";
		}

		try
		{
			return BlockMatchType.GetOccurrenceDetails(actual, expected, count,
				_comparer ?? UseDefaultComparer(_ignoreCase));
		}
		catch (Exception) when (_comparer is not null)
		{
			return "";
		}
	}

	/// <summary>
	///     Counts the occurrences of the already normalized and validated <paramref name="expected" /> pattern for
	///     match types that cannot be counted with a window of the expected length, or returns <see langword="null" />.
	/// </summary>
	private int? CountOccurrencesWithoutWindow(string actual, string expected, Regex? regex, int maximum)
	{
		// A block spans whole lines, so its occurrences cannot be found with a window of the expected length.
		if (_matchType is BlockMatchType)
		{
			return BlockMatchType.CountOccurrences(actual, expected, _comparer ?? UseDefaultComparer(_ignoreCase),
				maximum);
		}

		try
		{
			// A pattern can match a different number of characters than it is long, so its occurrences cannot be found
			// with a window of the expected length.
			if (regex is not null)
			{
				return RegexMatchType.CountOccurrences(actual, regex, maximum);
			}

			if (_matchType is WildcardMatchType)
			{
				return WildcardMatchType.CountOccurrences(actual, expected, _ignoreCase, maximum);
			}
		}
		catch (RegexMatchTimeoutException exception)
		{
			throw CreateTimeoutException(expected, exception);
		}

		return null;
	}

	/// <summary>
	///     Counts the occurrences of the already normalized and validated <paramref name="expected" /> string by
	///     comparing it with a window of the same length.
	/// </summary>
	/// <remarks>
	///     For the default match type without a comparer, a window equals the <paramref name="expected" /> string
	///     exactly where an ordinal search finds it, so the windows are not copied.
	/// </remarks>
	private async ValueTask<int> CountOccurrencesWithWindow(string actual, string expected, int maximum)
	{
		if (_matchType is ExactMatchType && _comparer is null)
		{
			return CountOrdinalOccurrences(actual, expected,
				_ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal, maximum);
		}

		int count = 0;
		int index = 0;
		while (index < actual.Length && count < maximum)
		{
			if ((await CompareWithMatchType(
				    actual.Substring(index, Math.Min(expected.Length, actual.Length - index)), expected)).IsMatch)
			{
				count++;
				index += expected.Length;
			}
			else
			{
				index++;
			}
		}

		return count;
	}

	private static int CountOrdinalOccurrences(string actual, string expected, StringComparison comparison,
		int maximum)
	{
		int count = 0;
		int index = actual.IndexOf(expected, 0, comparison);
		while (index >= 0 && ++count < maximum)
		{
			index = actual.IndexOf(expected, index + expected.Length, comparison);
		}

		return count;
	}

	/// <summary>
	///     Specifies a new <see cref="StringMatchType" /> to use for matching two strings, named
	///     <paramref name="optionName" /> in the exception when another match type is already specified.
	/// </summary>
	/// <remarks>
	///     The <paramref name="matchType" /> can reject the casing and the comparer that are already specified in its
	///     <see cref="StringMatchType.ValidateOptions(bool, IEqualityComparer{string})" />, whose exception is thrown
	///     unchanged.
	/// </remarks>
	/// <exception cref="InvalidOperationException">A match type is already specified.</exception>
	public StringEqualityOptions SetMatchType(StringMatchType matchType, string optionName)
	{
		matchType.ThrowIfNull();
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_matchTypeOption, optionName);
		matchType.ValidateOptions(_ignoreCase, _comparer);
		_matchTypeOption = optionName;
		_matchType = matchType;
		_matcher = null;
		return this;
	}

	/// <summary>
	///     Adds the contexts of the matcher to the <paramref name="contexts" />, e.g. the options of a custom match
	///     type.
	/// </summary>
	public void AppendContexts(ResultContextCollector contexts)
		=> GetMatcher().AppendContexts(contexts);

	/// <summary>
	///     Rejects a second call of the line <paramref name="option" />, and any line option in combination with
	///     <see cref="AsBlock()" />, which compares the lines on its own.
	/// </summary>
	private void SpecifyLineOption(ref bool isSpecified, string option)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(isSpecified, option);
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_matchType is BlockMatchType ? nameof(AsBlock) : null, option);
		_lineOption ??= option;
		isSpecified = true;
	}

	/// <summary>
	///     Get the expectations text.
	/// </summary>
	/// <remarks>
	///     The text is written from the <see cref="StringMatchType.Phrase" /> of the match type for the
	///     <paramref name="grammars" />, followed by the options, e.g. <c>starts with "Let it" ignoring case</c>.
	/// </remarks>
	public string GetExpectation(string? expected, ExpectationGrammars grammars)
	{
		MatchPhrase phrase = _matchType.Phrase;
		string formattedExpected = phrase.FormatsExpected
			? Formatter.Format(expected.TruncateWithEllipsisOnWord(DefaultMaxLength))
			: expected ?? "";
		return phrase.GetExpectation(formattedExpected, grammars, false) + GetOptionString();
	}

	/// <summary>
	///     Formats the <paramref name="expected" /> item of an expected collection.
	/// </summary>
	/// <remarks>
	///     A pattern is preceded by its kind, e.g. <c>prefix "foo"</c>, so that it is not mistaken for the value the item
	///     had to be equal to.
	/// </remarks>
	public string FormatExpectedItem(string? expected)
		=> WithPatternKind(Formatter.Format(expected));

	/// <summary>
	///     Precedes the already <paramref name="formattedExpected" /> item with the kind of the pattern.
	/// </summary>
	internal string WithPatternKind(string formattedExpected)
		=> _matchType.Phrase.PatternItemPrefix + formattedExpected;

	/// <summary>
	///     Get an extended failure text for the comparison of the <paramref name="actual" /> with the
	///     <paramref name="expected" /> value that returned the <paramref name="result" />.
	/// </summary>
	/// <remarks>
	///     Pass the <paramref name="result" /> of
	///     <see cref="MatchWithExplanation(string?, string?)" />, as a match type may explain the failure with what the
	///     comparison found.
	/// </remarks>
	public string GetExtendedFailure(string it, ExpectationGrammars grammars, string? actual, string? expected,
		MatchResult result)
	{
		if (grammars.HasFlag(ExpectationGrammars.Negated))
		{
			return $"{it} was {Formatter.Format(actual)}";
		}

		int[]? ignoredColumnsPerLine = GetIgnoredColumnsPerLine(actual);
		actual = NormalizeLines(actual);

		int ignoredLineCount = 0;
		if (_ignoreLeadingWhiteSpace && actual is not null && ignoredColumnsPerLine is not null)
		{
			(ignoredLineCount, int ignoredColumnCount) = CountLeadingWhiteSpace(actual);
			ignoredColumnsPerLine[ignoredLineCount] += ignoredColumnCount;
		}

		StringDifferenceSettings? settings = ignoredColumnsPerLine is null
			? null
			: new StringDifferenceSettings(ignoredLineCount, 0, ignoredColumnsPerLine);

		actual = TrimWhiteSpace(actual);
		expected = NormalizeExpected(expected, GetAnchoredEdges());

		try
		{
			return GetMatcher().GetFailure(it, grammars, actual, expected, result, settings) +
			       GetNewlineStyleHint(actual, expected);
		}
		catch (Exception) when (_comparer is not null)
		{
			// The comparer of the caller answered for the values. To explain the failure it is called for parts of
			// them, for which it may throw, so the failure is then reported without the difference.
			return $"{it} was {Formatter.Format(actual.TruncateWithEllipsisOnWord(DefaultMaxLength))}";
		}
	}

	/// <summary>
	///     Returns a hint on a new line, when the already normalized <paramref name="actual" /> and
	///     <paramref name="expected" /> values would match with <see cref="IgnoringNewlineStyle(bool)" />, otherwise an empty
	///     string.
	/// </summary>
	/// <remarks>
	///     The difference that is shown for such values points at the first <c>\r</c>, which does not reveal that the
	///     newline style is the only difference.<br />
	///     Only the match types that show this difference get the hint: a pattern matches newlines on its own terms, and
	///     a block already ignores the newline style. A value without a <c>\r</c> is not changed by the normalization, so
	///     this also gives no hint when the newline style is already ignored.<br />
	///     The comparer of the caller may throw for the normalized values, which then get no hint.
	/// </remarks>
	private string GetNewlineStyleHint(string? actual, string? expected)
	{
		if (_matchType is not (ExactMatchType or PrefixMatchType or SuffixMatchType) ||
		    actual is null || expected is null)
		{
			return "";
		}

		string normalizedActual = actual.RemoveNewlineStyle();
		string normalizedExpected = expected.RemoveNewlineStyle();
		if (ReferenceEquals(normalizedActual, actual) && ReferenceEquals(normalizedExpected, expected))
		{
			return "";
		}

		try
		{
			ValueTask<MatchResult> result = GetMatcher().Match(
				PadWithInnerWhiteSpace(normalizedActual, normalizedExpected, GetAnchoredEdges()), normalizedExpected);
			if (!result.IsCompletedSuccessfully)
			{
				return "";
			}

			return result.Result.IsMatch
				? Environment.NewLine + "(but the values match when ignoring the newline style)"
				: "";
		}
		catch (UserCodeException)
		{
			return "";
		}
	}

	/// <summary>
	///     Lists the lines that differ between the <paramref name="actual" /> and the <paramref name="expected" /> value,
	///     when they are compared for equality, one of them has several lines and they differ in at least two places,
	///     otherwise returns <see langword="null" />.
	/// </summary>
	/// <remarks>
	///     The lines are compared with the options, so that lines which only differ in what the options ignore are
	///     equal, but are shown as they are in the original values.<br />
	///     A comparer of the caller defines when whole values are equal, which need not hold for their lines, so the
	///     lines are then not listed. Neither are they when the whitespace is normalized, which joins all lines.
	/// </remarks>
	internal string? GetLineDifferences(string actual, string expected)
	{
		if (_matchType is not ExactMatchType || _comparer is not null || _normalizeWhiteSpace ||
		    (!IsMultiLine(actual) && !IsMultiLine(expected)))
		{
			return null;
		}

		StringComparer comparer = UseDefaultComparer(_ignoreCase);
		bool ignoreLineBreaks = _ignoreNewlineStyle || _ignoreIndentation;
		return LineDifferences.Format(GetComparedLines(actual), GetComparedLines(expected),
			(actualLine, expectedLine) => comparer.Equals(actualLine.ComparedText, expectedLine.ComparedText) &&
			                              (ignoreLineBreaks ||
			                               HaveSameLineBreak(actualLine, expectedLine)),
			Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get());

		static bool IsMultiLine(string value)
			=> value.IndexOfAny(['\r', '\n',]) >= 0;

		// The last line has no line break, so it equals a line with the same text whichever line break that has.
		static bool HaveSameLineBreak(LineDifferences.Line actualLine, LineDifferences.Line expectedLine)
			=> actualLine.LineBreak.Length == 0 || expectedLine.LineBreak.Length == 0 ||
			   actualLine.LineBreak == expectedLine.LineBreak;
	}

	/// <summary>
	///     Splits the <paramref name="value" /> into its lines and applies the options to each of them, like
	///     <see cref="Normalize(string?)" /> applies them to the whole value.
	/// </summary>
	/// <remarks>
	///     Removing the whitespace at the start or the end of the whole value removes the lines that consist only of
	///     whitespace there, and the whitespace at the start of the first or at the end of the last remaining line.
	/// </remarks>
	private List<LineDifferences.Line> GetComparedLines(string value)
	{
		List<LineDifferences.Line> lines = LineDifferences.Split(value);
		if (_ignoreIndentation)
		{
			lines.ForEach(line => line.ComparedText = line.ComparedText.TrimStart());
		}

		if (_ignoreLeadingWhiteSpace)
		{
			lines.RemoveRange(0, lines.TakeWhile(line => string.IsNullOrWhiteSpace(line.ComparedText)).Count());
			if (lines.Count > 0)
			{
				lines[0].ComparedText = lines[0].ComparedText.TrimStart();
			}
		}

		if (_ignoreTrailingWhiteSpace)
		{
			int count = Enumerable.Reverse(lines).TakeWhile(line => string.IsNullOrWhiteSpace(line.ComparedText))
				.Count();
			lines.RemoveRange(lines.Count - count, count);
			if (lines.Count > 0)
			{
				lines[^1].ComparedText = lines[^1].ComparedText.TrimEnd();
				lines[^1].LineBreak = "";
			}
		}

		return lines;
	}

	/// <summary>
	///     Get an extended failure text that states which value <paramref name="it" /> had in its
	///     <paramref name="member" />, e.g. <c>it had message "foo", which differs …</c>.
	/// </summary>
	/// <remarks>
	///     The text of <see cref="GetExtendedFailure(string, ExpectationGrammars, string?, string?, MatchResult)" /> for
	///     the <paramref name="member" /> is rephrased when it starts with <c>&lt;member&gt; was</c> or
	///     <c>&lt;member&gt; did not match</c>, as for every built-in match type; any other text, e.g. of a custom
	///     match type, is returned unchanged.
	/// </remarks>
	public string GetExtendedMemberFailure(string it, string member, ExpectationGrammars grammars,
		string? actual, string? expected, MatchResult result)
	{
		string failure = GetExtendedFailure(member, grammars, actual, expected, result);
		string wasPrefix = member + " was ";
		if (failure.StartsWith(wasPrefix, StringComparison.Ordinal))
		{
			return $"{it} had {member} {failure.Substring(wasPrefix.Length)}";
		}

		string didNotMatchPrefix = member + " did not match";
		if (failure.StartsWith(didNotMatchPrefix, StringComparison.Ordinal))
		{
			return
				$"{it} had {member} {Formatter.Format(actual.TruncateWithEllipsisOnWord(DefaultMaxLength))}, which {failure.Substring(member.Length + 1)}";
		}

		return failure;
	}

	/// <summary>
	///     Ignores casing when comparing the <see langword="string" />s.
	/// </summary>
	/// <remarks>
	///     A custom match type can reject the casing in its
	///     <see cref="StringMatchType.ValidateOptions(bool, IEqualityComparer{string})" />, whose exception is thrown
	///     unchanged.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     The casing is already specified, or a custom comparer is already set via
	///     <see cref="Using(IEqualityComparer{string})" />.
	/// </exception>
	public StringEqualityOptions IgnoringCase(bool ignoreCase = true)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_isIgnoreCaseSpecified, nameof(IgnoringCase));
		if (ignoreCase && _comparer is not null)
		{
			throw CaseAndComparerConflict();
		}

		_matchType.ValidateOptions(ignoreCase, _comparer);
		_isIgnoreCaseSpecified = true;
		_ignoreCase = ignoreCase;
		_matcher = null;
		return this;
	}

	/// <summary>
	///     Ignores the indentation when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreIndentation" /> parameter.
	/// </summary>
	/// <remarks>
	///     Enabling this option will remove the leading whitespace from every line and replace all occurrences of
	///     <c>\r\n</c> and <c>\r</c> with <c>\n</c> in the strings before comparing them, which makes
	///     <see cref="IgnoringNewlineStyle(bool)" /> redundant.<br />
	///     Any Unicode whitespace counts as indentation, e.g. also a non-breaking space.<br />
	///     Trailing whitespace within a line is kept, but a line that consists only of whitespace becomes empty.<br />
	///     The expected value is transformed as well, which also applies to wildcard and regex patterns.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     The indentation is already specified, or the expected value is matched as a block via <see cref="AsBlock()" />.
	/// </exception>
	public StringEqualityOptions IgnoringIndentation(bool ignoreIndentation = true)
	{
		SpecifyLineOption(ref _isIgnoreIndentationSpecified, nameof(IgnoringIndentation));
		_ignoreIndentation = ignoreIndentation;
		return this;
	}

	/// <summary>
	///     Ignores the newline style when comparing <see langword="string" />s.
	/// </summary>
	/// <remarks>
	///     Enabling this option will replace all occurrences of <c>\r\n</c> and <c>\r</c> with <c>\n</c> in the strings before
	///     comparing them.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     The newline style is already specified, or the expected value is matched as a block via
	///     <see cref="AsBlock()" />.
	/// </exception>
	public StringEqualityOptions IgnoringNewlineStyle(bool ignoreNewlineStyle = true)
	{
		SpecifyLineOption(ref _isIgnoreNewlineStyleSpecified, nameof(IgnoringNewlineStyle));
		_ignoreNewlineStyle = ignoreNewlineStyle;
		return this;
	}

	/// <summary>
	///     Ignores leading whitespace when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreLeadingWhiteSpace" /> parameter.
	/// </summary>
	/// <remarks>
	///     Note:<br />
	///     The position of the first mismatch in the failure message (its index, or its line and column) refers to the
	///     original subject, so it also counts the removed whitespace.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     The leading whitespace is already specified, or the expected value is matched as a block via
	///     <see cref="AsBlock()" />.
	/// </exception>
	public StringEqualityOptions IgnoringLeadingWhiteSpace(bool ignoreLeadingWhiteSpace = true)
	{
		SpecifyLineOption(ref _isIgnoreLeadingWhiteSpaceSpecified, nameof(IgnoringLeadingWhiteSpace));
		_ignoreLeadingWhiteSpace = ignoreLeadingWhiteSpace;
		return this;
	}

	/// <summary>
	///     Ignores trailing whitespace when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreTrailingWhiteSpace" /> parameter.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///     The trailing whitespace is already specified, or the expected value is matched as a block via
	///     <see cref="AsBlock()" />.
	/// </exception>
	public StringEqualityOptions IgnoringTrailingWhiteSpace(bool ignoreTrailingWhiteSpace = true)
	{
		SpecifyLineOption(ref _isIgnoreTrailingWhiteSpaceSpecified, nameof(IgnoringTrailingWhiteSpace));
		_ignoreTrailingWhiteSpace = ignoreTrailingWhiteSpace;
		return this;
	}

	/// <summary>
	///     Normalizes the whitespace when comparing <see langword="string" />s,
	///     according to the <paramref name="normalizeWhiteSpace" /> parameter.
	/// </summary>
	/// <remarks>
	///     Enabling this option will replace every run of whitespace, including line breaks, with a single space and
	///     remove the whitespace at the start and the end of the strings before comparing them, which makes
	///     <see cref="IgnoringIndentation(bool)" />, <see cref="IgnoringNewlineStyle(bool)" />,
	///     <see cref="IgnoringLeadingWhiteSpace(bool)" /> and <see cref="IgnoringTrailingWhiteSpace(bool)" /> redundant.<br />
	///     Any Unicode whitespace counts, e.g. also a tab or a non-breaking space.<br />
	///     The expected value is transformed as well, which also applies to wildcard and regex patterns. A prefix, suffix
	///     or substring keeps the whitespace at its inner end, which then has to match whitespace in the subject, unless
	///     that end reaches the corresponding end of the subject.<br />
	///     Note:<br />
	///     The failure message shows the strings with normalized whitespace, and the position of the first mismatch
	///     refers to them.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     The whitespace normalization is already specified, or the expected value is matched as a block via
	///     <see cref="AsBlock()" />.
	/// </exception>
	public StringEqualityOptions NormalizingWhiteSpace(bool normalizeWhiteSpace = true)
	{
		SpecifyLineOption(ref _isNormalizeWhiteSpaceSpecified, nameof(NormalizingWhiteSpace));
		_normalizeWhiteSpace = normalizeWhiteSpace;
		return this;
	}

	/// <inheritdoc />
	public override string ToString() => _matchType.Phrase.Name + GetOptionString();

	/// <summary>
	///     Specifies a specific <see cref="IEqualityComparer{T}" /> to use for comparing <see langword="string" />s.
	/// </summary>
	/// <remarks>
	///     Without a comparer, the <see cref="StringComparer.Ordinal" /> or <see cref="StringComparer.OrdinalIgnoreCase" />
	///     is used depending on whether the casing is ignored.<br />
	///     A custom match type can reject the <paramref name="comparer" /> in its
	///     <see cref="StringMatchType.ValidateOptions(bool, IEqualityComparer{string})" />, whose exception is thrown
	///     unchanged.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     A comparer is already set, the casing is already ignored via <see cref="IgnoringCase(bool)" />, or the
	///     expected value is matched as a regex or wildcard pattern.
	/// </exception>
	public StringEqualityOptions Using(IEqualityComparer<string> comparer)
	{
		comparer.ThrowIfNull();
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_comparer is not null, nameof(Using));
		if (_ignoreCase)
		{
			throw CaseAndComparerConflict();
		}

		if (_matchType is RegexMatchType or WildcardMatchType)
		{
			throw ComparerAndPatternConflict();
		}

		_matchType.ValidateOptions(_ignoreCase, comparer);
		_comparer = comparer;
		_matcher = null;
		return this;
	}

	/// <summary>
	///     Creates the exception for a custom comparer that is combined with <see cref="IgnoringCase(bool)" />.
	/// </summary>
	/// <remarks>
	///     Only one of the two can be honoured, so the combination is rejected instead of silently dropping the
	///     casing option, which would also disappear from the expectation text.
	/// </remarks>
	private static InvalidOperationException CaseAndComparerConflict()
		// ReSharper disable once LocalizableElement
		=> Tracing.WriteException(new InvalidOperationException(
			"IgnoringCase cannot be combined with a custom comparer; use a case-insensitive comparer instead."));

	/// <summary>
	///     Creates the exception for a custom comparer that is combined with a regex or wildcard pattern.
	/// </summary>
	/// <remarks>
	///     A pattern is matched by the regex engine, which has no way to consult a comparer, so the combination is
	///     rejected instead of silently ignoring the comparer.
	/// </remarks>
	private static InvalidOperationException ComparerAndPatternConflict()
		// ReSharper disable once LocalizableElement
		=> Tracing.WriteException(new InvalidOperationException(
			"A custom comparer is not supported for regex or wildcard matching."));

	/// <summary>
	///     Creates the exception for an <paramref name="expected" /> pattern that did not finish matching within the
	///     timeout.
	/// </summary>
	/// <remarks>
	///     An <see cref="ArgumentException" /> is not wrapped by the expectation node, so that the pattern which has
	///     to be simplified stays visible instead of being hidden behind a generic evaluation error.<br />
	///     The timeout is taken from the <paramref name="innerException" />, because it is the one that applied, also
	///     for a custom match type with a regex of its own.
	/// </remarks>
	private ArgumentException CreateTimeoutException(string? expected, RegexMatchTimeoutException innerException)
		// ReSharper disable once LocalizableElement
		=> Tracing.WriteException(new ArgumentException(
			$"The {(_matchType is RegexMatchType ? "regex" : "wildcard pattern")} {Formatter.Format(expected)} did not complete within {Formatter.Format(innerException.MatchTimeout)}. Simplify the pattern to avoid catastrophic backtracking.",
			_parameterName, innerException));

	private static StringComparer UseDefaultComparer(bool ignoreCase)
		=> ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

	private static bool AreEqualByComparer(IEqualityComparer<string> comparer, string actual, string expected)
		=> UserCode.Invoke(static values => values.Comparer.Equals(values.Actual, values.Expected),
			(Comparer: comparer, Actual: actual, Expected: expected), "the comparer");

	/// <summary>
	///     Applies all options that transform the <paramref name="value" /> as a whole.
	/// </summary>
	/// <remarks>
	///     All these options change the length of the <paramref name="value" />, so they must never be applied to an
	///     individual substring of an already normalized value.
	/// </remarks>
	[return: NotNullIfNotNull(nameof(value))]
	private string? Normalize(string? value)
		=> TrimWhiteSpace(NormalizeLines(value));

	/// <summary>
	///     Applies all options that transform the <paramref name="expected" /> value as a whole, but removes its whitespace
	///     only at the <paramref name="anchoredEdges" />, which line up with the corresponding edge of the subject.
	/// </summary>
	[return: NotNullIfNotNull(nameof(expected))]
	private string? NormalizeExpected(string? expected, (bool IsStartAnchored, bool IsEndAnchored) anchoredEdges)
	{
		expected = NormalizeLines(expected);
		if (IsLeadingWhiteSpaceIgnored && anchoredEdges.IsStartAnchored)
		{
			expected = expected?.TrimStart();
		}

		if (IsTrailingWhiteSpaceIgnored && anchoredEdges.IsEndAnchored)
		{
			expected = expected?.TrimEnd();
		}

		return expected;
	}

	/// <summary>
	///     Returns which edges of the expected value line up with the corresponding edge of the subject for the current
	///     match type.
	/// </summary>
	/// <remarks>
	///     A regex may match any part of the subject, like a substring. A custom match type keeps the behaviour of an exact
	///     match, whose expected value covers the whole subject.
	/// </remarks>
	private (bool IsStartAnchored, bool IsEndAnchored) GetAnchoredEdges()
		=> _matchType switch
		{
			PrefixMatchType => (true, false),
			SuffixMatchType => (false, true),
			ContainingMatchType or RegexMatchType => (false, false),
			_ => (true, true),
		};

	/// <summary>
	///     Surrounds the already normalized <paramref name="actual" /> value with the whitespace that the
	///     <paramref name="expected" /> value has at an edge that is not anchored, so that this whitespace is optional
	///     where the expected value reaches the edge of the subject, but still has to match inside the subject.
	/// </summary>
	/// <remarks>
	///     An <paramref name="expected" /> value that consists only of whitespace is empty at an edge of the subject, and
	///     an empty value never occurs, so a substring that consists only of whitespace can only occur inside the subject.
	/// </remarks>
	[return: NotNullIfNotNull(nameof(actual))]
	private string? PadWithInnerWhiteSpace(string? actual, string expected,
		(bool IsStartAnchored, bool IsEndAnchored) anchoredEdges)
	{
		int leading = IsLeadingWhiteSpaceIgnored && !anchoredEdges.IsStartAnchored
			? expected.Length - expected.TrimStart().Length
			: 0;
		int trailing = IsTrailingWhiteSpaceIgnored && !anchoredEdges.IsEndAnchored
			? expected.Length - expected.TrimEnd().Length
			: 0;
		if (actual is null || leading + trailing == 0 ||
		    (!anchoredEdges.IsStartAnchored && !anchoredEdges.IsEndAnchored &&
		     Math.Max(leading, trailing) == expected.Length))
		{
			return actual;
		}

#if NET8_0_OR_GREATER
		return string.Concat(expected.AsSpan(0, leading), actual, expected.AsSpan(expected.Length - trailing));
#else
		return expected.Substring(0, leading) + actual + expected.Substring(expected.Length - trailing);
#endif
	}

	/// <summary>
	///     Applies the options that transform the lines of the <paramref name="value" />.
	/// </summary>
	/// <remarks>
	///     Removing the indentation also normalizes the newline style, so it supersedes
	///     <see cref="IgnoringNewlineStyle(bool)" />. Collapsing the whitespace replaces every line break and the
	///     indentation, so it supersedes both.
	/// </remarks>
	[return: NotNullIfNotNull(nameof(value))]
	private string? NormalizeLines(string? value)
	{
		if (_normalizeWhiteSpace)
		{
			return value.CollapseWhiteSpace();
		}

		if (_ignoreIndentation)
		{
			return value.RemoveIndentation();
		}

		return _ignoreNewlineStyle ? value.RemoveNewlineStyle() : value;
	}

	/// <summary>
	///     Removes the whitespace at the start and the end of the <paramref name="value" />.
	/// </summary>
	[return: NotNullIfNotNull(nameof(value))]
	private string? TrimWhiteSpace(string? value)
	{
		if (IsLeadingWhiteSpaceIgnored)
		{
			value = value?.TrimStart();
		}

		if (IsTrailingWhiteSpaceIgnored)
		{
			value = value?.TrimEnd();
		}

		return value;
	}

	private bool IsLeadingWhiteSpaceIgnored => _ignoreLeadingWhiteSpace || _normalizeWhiteSpace;

	private bool IsTrailingWhiteSpaceIgnored => _ignoreTrailingWhiteSpace || _normalizeWhiteSpace;

	/// <summary>
	///     Counts the lines and the columns in the last of these lines that are covered by the leading whitespace
	///     of the <paramref name="value" />.
	/// </summary>
	private static (int Lines, int Columns) CountLeadingWhiteSpace(string value)
	{
		int lines = 0;
		int columns = 0;
		foreach (char c in value)
		{
			if (c == '\n')
			{
				lines++;
				columns = 0;
			}
			else if (char.IsWhiteSpace(c))
			{
				columns++;
			}
			else
			{
				break;
			}
		}

		return (lines, columns);
	}

	/// <summary>
	///     Creates the table of ignored columns per line for the <paramref name="value" />, pre-filled with the width
	///     of the ignored indentation, or <see langword="null" /> when no option shifts the reported positions.
	/// </summary>
	/// <remarks>
	///     The table is indexed by the line number of the value returned by <see cref="NormalizeLines" />, so that a
	///     position in the normalized value can be mapped back to the position in the original
	///     <paramref name="value" />.<br />
	///     Collapsed whitespace cannot be mapped back by lines and columns, so with normalized whitespace the reported
	///     positions refer to the normalized value.
	/// </remarks>
	private int[]? GetIgnoredColumnsPerLine(string? value)
	{
		if (value is null || _normalizeWhiteSpace || (!_ignoreIndentation && !_ignoreLeadingWhiteSpace))
		{
			return null;
		}

		// When the indentation is ignored, the normalized value has the same lines as the newline normalized value,
		// but still contains the indentation whose width is measured below.
		string[] lines = (_ignoreIndentation ? value.RemoveNewlineStyle() : NormalizeLines(value)).Split('\n');
		int[] result = new int[lines.Length];
		if (_ignoreIndentation)
		{
			for (int i = 0; i < lines.Length; i++)
			{
				result[i] = lines[i].Length - lines[i].TrimStart().Length;
			}
		}

		return result;
	}

	private string GetOptionString()
	{
		string initialString = _comparer is not null
			? $" using {Formatter.Format(_comparer.GetType())}"
			: _ignoreCase
				? " ignoring case"
				: "";
		if (!_ignoreLeadingWhiteSpace && !_ignoreTrailingWhiteSpace && !_ignoreNewlineStyle &&
		    !_ignoreIndentation)
		{
			return WithNormalizedWhiteSpace(initialString);
		}

		List<string> tokens = [];
		switch (_ignoreLeadingWhiteSpace, _ignoreTrailingWhiteSpace)
		{
			case (true, true):
				tokens.Add("whitespace");
				break;
			case (true, false):
				tokens.Add("leading whitespace");
				break;
			case (false, true):
				tokens.Add("trailing whitespace");
				break;
		}

		if (_ignoreNewlineStyle)
		{
			tokens.Add("newline style");
		}

		if (_ignoreIndentation)
		{
			tokens.Add("indentation");
		}

		StringBuilder sb = new();
		sb.Append(initialString);
		if (!initialString.Contains("ignoring"))
		{
			sb.Append(" ignoring");
		}
		else
		{
			sb.Append(tokens.Count == 1 ? " and" : ",");
		}

		for (int i = 0; i < tokens.Count; i++)
		{
			if (i > 0)
			{
				sb.Append(i == tokens.Count - 1 ? " and" : ",");
			}

			sb.Append(' ').Append(tokens[i]);
		}

		return WithNormalizedWhiteSpace(sb.ToString());
	}

	private string WithNormalizedWhiteSpace(string optionString)
	{
		if (!_normalizeWhiteSpace)
		{
			return optionString;
		}

		return optionString.Length == 0
			? " normalizing whitespace"
			: optionString + " and normalizing whitespace";
	}

	/// <summary>
	///     Creates the exception for an expected value of the <paramref name="kind" /> that is empty.
	/// </summary>
	private ArgumentException CreateEmptyPatternException(string kind)
		// ReSharper disable once LocalizableElement
		=> Tracing.WriteException(new ArgumentException($"The '{_parameterName}' {kind} cannot be empty.",
			_parameterName));

	/// <summary>
	///     Names the kind of pattern of the current match type, or returns <see langword="null" /> when the expected
	///     value is not a pattern.
	/// </summary>
	private string? GetPatternKind()
		=> _matchType.Phrase.PatternNoun;

	/// <summary>
	///     Verifies that the <paramref name="expected" /> value is usable for the current match type and returns the
	///     parsed <see cref="Regex" /> for a regex pattern.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> pattern describes nothing to look for and an empty regex, prefix or suffix pattern
	///     matches every value, so such an expectation says nothing about the subject. This can only be detected while
	///     the expectation is verified, because the match type can also be set after the pattern.
	/// </remarks>
	private Regex? ValidatePattern(string? expected)
	{
		_matchType.ValidateExpected(expected);
		string? patternKind = GetPatternKind();
		if (patternKind is null)
		{
			return null;
		}

		if (expected is null)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentNullException(_parameterName,
				$"The '{_parameterName}' {patternKind} cannot be null."));
		}

		if (expected.Length == 0 && _matchType is not WildcardMatchType)
		{
			throw CreateEmptyPatternException(patternKind);
		}

		if (_matchType is not RegexMatchType)
		{
			return null;
		}

		try
		{
			return GetParsedPattern(expected, static (matchType, pattern, ignoreCase)
				=> ((RegexMatchType)matchType).CreateRegex(pattern, ignoreCase));
		}
		catch (ArgumentException exception) when (exception is not ArgumentOutOfRangeException)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException(
				$"The '{_parameterName}' regex pattern is invalid: {exception.Message}", _parameterName, exception));
		}
	}

	/// <summary>
	///     Returns the regex for the <paramref name="expected" /> pattern of the current match type, and parses it with
	///     <paramref name="parse" /> only when the last parsed pattern was a different one or has another timeout.
	/// </summary>
	private Regex GetParsedPattern(string expected, Func<StringMatchType, string, bool, Regex> parse)
	{
		ParsedPattern? parsedPattern = _parsedPattern;
		if (parsedPattern is not null && parsedPattern.IsFor(_matchType, expected, _ignoreCase) &&
		    parsedPattern.Regex.MatchTimeout == GetRegexTimeout())
		{
			return parsedPattern.Regex;
		}

		Regex regex = parse(_matchType, expected, _ignoreCase);
		_parsedPattern = new ParsedPattern(_matchType, expected, _ignoreCase, regex);
		return regex;
	}

	/// <remarks>
	///     Immutable, so that it can be replaced as a whole while another evaluation reads it.
	/// </remarks>
	private sealed class ParsedPattern(StringMatchType matchType, string expected, bool ignoreCase, Regex regex)
	{
		public Regex Regex { get; } = regex;

		public bool IsFor(StringMatchType otherMatchType, string otherExpected, bool otherIgnoreCase)
			=> ReferenceEquals(matchType, otherMatchType) && ignoreCase == otherIgnoreCase &&
			   string.Equals(expected, otherExpected, StringComparison.Ordinal);
	}
}
