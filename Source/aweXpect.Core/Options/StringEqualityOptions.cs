using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
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
/// <remarks>
///     The options are an immutable value: every option method returns new options and leaves these unchanged, so a
///     value can be stored and shared, also across threads. Start from <see cref="Default" />.<br />
///     The options do not know the name of the expected value; the members that can reject it, e.g.
///     <see cref="ValidateExpected(string?, string)" />, receive it as <c>parameterName</c>.
/// </remarks>
[ImmutableOptions]
public sealed partial class StringEqualityOptions : IEqualityOptions<string?>, IEqualityEvaluator<string?>
{
	private const int DefaultMaxLength = 30;
	private const string ExpectedParameterName = "expected";

	private const int IgnoreCaseFlag = 1 << 0;
	private const int IgnoreIndentationFlag = 1 << 1;
	private const int IgnoreNewlineStyleFlag = 1 << 2;
	private const int IgnoreLeadingWhiteSpaceFlag = 1 << 3;
	private const int IgnoreTrailingWhiteSpaceFlag = 1 << 4;
	private const int NormalizeWhiteSpaceFlag = 1 << 5;
	private const int MatchTypeShift = 6;
	private const int OptionFlagsMask = (1 << MatchTypeShift) - 1;

	private const int ExactMatchTypeIndex = 0;
	private const int PrefixMatchTypeIndex = 1;
	private const int SuffixMatchTypeIndex = 2;
	private const int ContainingMatchTypeIndex = 3;
	private const int BlockMatchTypeIndex = 4;
	private const int WildcardMatchTypeIndex = 5;
	private const int RegexMatchTypeIndex = 6;
	private const int PrivateMatchTypeIndex = 7;

	private readonly IEqualityComparer<string>? _comparer;

	/// <summary>
	///     The enabled options (bits 0-5, in the order of the option methods) and the index of the match type (bits 6-8),
	///     which is <see cref="PrivateMatchTypeIndex" /> for a match type that is not shared.
	/// </summary>
	private readonly int _flags;

	private readonly StringMatchType _matchType;
	private readonly string? _matchTypeOption;

	/// <summary>
	///     The matcher of the evaluation for options returned by <see cref="ForEvaluation" />, otherwise the matcher for
	///     the comparisons outside an evaluation.
	/// </summary>
	/// <remarks>
	///     The options serve every evaluation, so they never keep the matcher of an evaluation, which may hold its state,
	///     its context and its cancellation. A match type without state per evaluation returns this matcher for every
	///     evaluation, so the options are only copied for an evaluation that gets a matcher of its own.<br />
	///     Shared options are created with their matcher, so that they are never written after they are published;
	///     other options create it when it is first needed.
	/// </remarks>
	private StringMatcher? _matcher;

	private StringEqualityOptions(StringMatchType matchType, string? matchTypeOption,
		IEqualityComparer<string>? comparer, int flags, StringMatcher? matcher)
	{
		_matchType = matchType;
		_matchTypeOption = matchTypeOption;
		_comparer = comparer;
		_flags = flags;
		_matcher = matcher;
	}

	/// <summary>
	///     The options without any option set, which compare two <see langword="string" />s with ordinal equality.
	/// </summary>
	/// <remarks>
	///     Options without a comparer and with a built-in match type that takes no arguments are shared values, so that
	///     setting such an option allocates nothing.
	/// </remarks>
	public static StringEqualityOptions Default => SharedValues.ExactOptions;

	/// <summary>
	///     The settings that the matcher of an evaluation is created with.
	/// </summary>
	private StringMatchSettings Settings => new(IgnoresCase, _comparer);

	private bool IgnoresCase => (_flags & IgnoreCaseFlag) != 0;

	private bool IgnoresIndentation => (_flags & IgnoreIndentationFlag) != 0;

	private bool IgnoresNewlineStyle => (_flags & IgnoreNewlineStyleFlag) != 0;

	private bool IgnoresLeadingWhiteSpace => (_flags & IgnoreLeadingWhiteSpaceFlag) != 0;

	private bool IgnoresTrailingWhiteSpace => (_flags & IgnoreTrailingWhiteSpaceFlag) != 0;

	private bool NormalizesWhiteSpace => (_flags & NormalizeWhiteSpaceFlag) != 0;

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
	public bool ComparesByOrdinalEquality => _flags == 0 && _comparer is null;

	/// <inheritdoc />
	/// <remarks>
	///     An unusable <paramref name="expected" /> value is reported against the parameter <c>expected</c>; use
	///     <see cref="AreConsideredEqual(string?, string?, string)" /> to name it.
	/// </remarks>
	ValueTask<bool> IEqualityEvaluator<string?>.AreConsideredEqual<TExpected>(string? actual, TExpected expected)
		=> AreConsideredEqual(actual, expected as string, ExpectedParameterName);

	/// <summary>
	///     Compares the <paramref name="actual" /> with the <paramref name="expected" /> value, which the caller received
	///     as <paramref name="parameterName" />.
	/// </summary>
	/// <remarks>
	///     The pattern is validated outside the asynchronous part, so that an unusable pattern throws at the call
	///     instead of only when the returned task is awaited.
	/// </remarks>
	/// <exception cref="ArgumentException">
	///     The match type cannot use the <paramref name="expected" /> value, e.g. an empty prefix or an invalid regex
	///     pattern.
	/// </exception>
	public ValueTask<bool> AreConsideredEqual(string? actual, string? expected, string parameterName)
	{
		// Without a pattern or an option, nothing is validated or normalized, and every item of a string collection
		// is compared this way.
		if (ComparesByOrdinalEquality)
		{
			return new ValueTask<bool>(string.Equals(actual, expected));
		}

		ValueTask<MatchResult> result = Compare(actual, expected, parameterName);
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
	///     <see cref="AreConsideredEqual(string?, string?, string)" />, and returns the result, from which
	///     <see cref="GetExtendedFailure(string, ExpectationGrammars, string?, string?, MatchResult)" /> explains a
	///     failure.
	/// </summary>
	/// <remarks>
	///     A constraint keeps the result for its failure message, so that the options keep nothing of a comparison.<br />
	///     A subject that the match type cannot compare throws, so that the expectation fails in both polarities.<br />
	///     An unusable <paramref name="expected" /> value is reported against the <paramref name="parameterName" />.
	/// </remarks>
	public ValueTask<MatchResult> MatchWithExplanation(string? actual, string? expected, string parameterName)
	{
		if (ComparesByOrdinalEquality)
		{
			return new ValueTask<MatchResult>(MatchResult.From(string.Equals(actual, expected)));
		}

		return Compare(actual, expected, parameterName);
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
	///     The options return themselves, unless the evaluation gets a matcher of its own, so also for the context of
	///     a comparison outside an evaluation, see <see cref="MatchEnvironment.Context" />.
	/// </remarks>
	public StringEqualityOptions ForEvaluation(IEvaluationContext context, CancellationToken cancellationToken)
	{
		MatchEnvironment environment = new(context, cancellationToken);
		if (ComparesByOrdinalEquality || environment.Evaluation is null)
		{
			return this;
		}

		StringMatcher matcher = _matchType.ForEvaluation(environment, Settings);
		return ReferenceEquals(matcher, GetMatcher())
			? this
			: new StringEqualityOptions(_matchType, _matchTypeOption, _comparer, _flags, matcher);
	}

	/// <inheritdoc cref="IEqualityOptions{TElement}.ForEvaluation(IEvaluationContext, CancellationToken)" />
	IEqualityEvaluator<string?> IEqualityOptions<string?>.ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
		=> ForEvaluation(context, cancellationToken);

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
	private ValueTask<MatchResult> Compare(string? actual, string? expected, string parameterName)
	{
		if (expected is null)
		{
			ValidatePattern(null, parameterName);
			return CompareWithMatchType(actual, null, parameterName);
		}

		(bool IsStartAnchored, bool IsEndAnchored) anchoredEdges = GetAnchoredEdges();
		expected = NormalizeExpected(expected, anchoredEdges);
		ValidatePattern(expected, parameterName);
		return CompareWithMatchType(PadWithInnerWhiteSpace(Normalize(actual), expected, anchoredEdges), expected,
			parameterName);
	}

	/// <summary>
	///     Verifies that the <paramref name="expected" /> value, which the caller received as
	///     <paramref name="parameterName" />, is usable for the current match type, exactly like
	///     <see cref="AreConsideredEqual(string?, string?, string)" /> does when a value is compared with it.
	/// </summary>
	/// <remarks>
	///     An expectation calls this for every expected value before it looks at the subject, so that an unusable
	///     value is rejected whichever subject it is verified for, also when no value is compared with it.<br />
	///     The match type rejects the value in its <see cref="StringMatchType.ValidateExpected(string?, string)" />, whose
	///     exception is thrown unchanged.
	/// </remarks>
	/// <exception cref="ArgumentNullException">
	///     The match type takes a pattern, e.g. a prefix, and the <paramref name="expected" /> value is
	///     <see langword="null" />.
	/// </exception>
	/// <exception cref="ArgumentException">
	///     The match type cannot use the <paramref name="expected" /> value, e.g. an empty prefix or an invalid regex
	///     pattern.
	/// </exception>
	public void ValidateExpected(string? expected, string parameterName)
		=> ValidatePattern(NormalizeExpected(expected, GetAnchoredEdges()), parameterName);

	/// <summary>
	///     Compares the <paramref name="actual" /> value with the <paramref name="expected" /> value by the matcher.
	/// </summary>
	/// <remarks>
	///     A subject that the matcher cannot compare is reported like an exception of the caller's code, so that every
	///     expectation and every collection fails for it in both polarities, without having to check for it.<br />
	///     A pattern that does not complete in time is only reported when the returned task is awaited, like every
	///     other answer about the subject.
	/// </remarks>
	private ValueTask<MatchResult> CompareWithMatchType(string? actual, string? expected, string parameterName)
	{
		ValueTask<MatchResult> result;
		try
		{
			result = GetMatcher().Match(actual, expected);
		}
		catch (Exception exception) when (IsRegexTimeout(exception))
		{
			return new ValueTask<MatchResult>(
				Task.FromException<MatchResult>(CreateTimeoutException(expected, exception, parameterName)));
		}

		if (!result.IsCompletedSuccessfully)
		{
			return AwaitTheComparison(result, expected, parameterName);
		}

		return new ValueTask<MatchResult>(ComparableOrThrow(result.Result));
	}

	private async ValueTask<MatchResult> AwaitTheComparison(ValueTask<MatchResult> result, string? expected,
		string parameterName)
	{
		try
		{
			return ComparableOrThrow(await result);
		}
		catch (Exception exception) when (IsRegexTimeout(exception))
		{
			throw CreateTimeoutException(expected, exception, parameterName);
		}
	}

	private static MatchResult ComparableOrThrow(MatchResult result)
		=> result.Outcome != MatchOutcome.NotComparable
			? result
			: throw new UserCodeException(new NotComparableException(result.NotComparableReason!, result.Cause));

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
	///     instead of only when the returned task is awaited. It is reported against the
	///     <paramref name="parameterName" />.
	/// </remarks>
	/// <exception cref="ArgumentException">
	///     The <paramref name="expected" /> <see langword="string" /> is empty after the normalization.
	/// </exception>
	public ValueTask<int> CountOccurrences(string actual, string expected, string parameterName)
		=> CountOccurrences(actual, expected, int.MaxValue, parameterName);

	/// <summary>
	///     Counts how often the <paramref name="expected" /> <see langword="string" /> occurs
	///     in the <paramref name="actual" /> <see langword="string" />, but at most <paramref name="maximum" /> times.
	/// </summary>
	/// <remarks>
	///     The search stops at the <paramref name="maximum" />, so that an expectation which is decided by a number of
	///     occurrences, e.g. at least one, does not search the rest of a long <paramref name="actual" /> string. The
	///     occurrences are counted like by <see cref="CountOccurrences(string, string, string)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">
	///     The <paramref name="expected" /> <see langword="string" /> is empty after the normalization.
	/// </exception>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="maximum" /> is not positive.</exception>
	public ValueTask<int> CountOccurrences(string actual, string expected, int maximum, string parameterName)
	{
		if (maximum <= 0)
		{
			throw Tracing.WriteException(new ArgumentOutOfRangeException(nameof(maximum),
				"The maximum must be greater than zero."));
		}

		expected = NormalizeExpected(expected, (false, false));
		ValidatePattern(expected, parameterName);
		if (expected.Length == 0)
		{
			throw CreateEmptyPatternException(parameterName, GetPatternKind() ?? "string");
		}

		actual = PadWithInnerWhiteSpace(Normalize(actual), expected, (false, false));

		int? count;
		try
		{
			count = GetMatcher().CountOccurrences(actual, expected, maximum);
		}
		catch (Exception exception) when (IsRegexTimeout(exception))
		{
			throw CreateTimeoutException(expected, exception, parameterName);
		}

		if (count is not null)
		{
			return new ValueTask<int>(count.Value);
		}

		return CountOccurrencesWithWindow(actual, expected, maximum, parameterName);
	}

	/// <summary>
	///     Describes where the first <paramref name="count" /> occurrences of the <paramref name="expected" />
	///     <see langword="string" /> are in the <paramref name="actual" /> <see langword="string" />, or without an
	///     occurrence, how it is missed, to extend a failure that reports the <paramref name="count" /> from
	///     <see cref="CountOccurrences(string, string, int, string)" />.
	/// </summary>
	/// <remarks>
	///     The text starts with its own separator, e.g. <c>" at lines 3-4"</c> or <c>", and the closest match at …"</c>,
	///     so it is appended directly after the formatted <paramref name="actual" /> <see langword="string" />.<br />
	///     The matcher describes the occurrences, see <see cref="StringMatcher.GetOccurrenceDetails" />, which only a block
	///     via <see cref="AsBlock()" /> does among the built-in match types. The text is empty when a custom comparer
	///     throws, because the failure has to be reported anyway, or when a positive <paramref name="count" /> is given
	///     for an <paramref name="actual" /> <see langword="string" /> without an occurrence.
	/// </remarks>
	public string GetOccurrenceDetails(string actual, string expected, int count)
	{
		if (expected.Length == 0)
		{
			return "";
		}

		try
		{
			return GetMatcher().GetOccurrenceDetails(actual, expected, count);
		}
		catch (Exception) when (_comparer is not null)
		{
			return "";
		}
	}

	/// <summary>
	///     Counts the occurrences of the already normalized and validated <paramref name="expected" /> string by
	///     comparing it with a window of the same length.
	/// </summary>
	private async ValueTask<int> CountOccurrencesWithWindow(string actual, string expected, int maximum,
		string parameterName)
	{
		int count = 0;
		int index = 0;
		while (index < actual.Length && count < maximum)
		{
			if ((await CompareWithMatchType(
				    actual.Substring(index, Math.Min(expected.Length, actual.Length - index)), expected,
				    parameterName)).IsMatch)
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
	///     A match type with the same <paramref name="optionName" /> replaces the earlier one.<br />
	///     The settings that are already specified and that the <paramref name="matchType" /> does not support, see
	///     <see cref="StringMatchType.SupportedSettings" />, are rejected together in one exception, like a setting that
	///     is specified afterwards.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     A match type with another name is already specified, or a specified setting is not supported by the
	///     <paramref name="matchType" />.
	/// </exception>
	public StringEqualityOptions SetMatchType(StringMatchType matchType, string optionName)
	{
		matchType.ThrowIfNull();
		return WithMatchType(matchType, optionName, PrivateMatchTypeIndex);
	}

	/// <summary>
	///     Returns options with the <paramref name="matchType" /> of the <paramref name="optionName" />, from the shared
	///     values when the <paramref name="matchTypeIndex" /> names a shared match type and no comparer is set.
	/// </summary>
	private StringEqualityOptions WithMatchType(StringMatchType matchType, string optionName, int matchTypeIndex)
	{
		ThrowHelper.ThrowIfCompetingOption(_matchTypeOption, optionName);
		ThrowIfSpecifiedSettingsAreNotSupported(matchType.SupportedSettings, optionName);
		return With((_flags & OptionFlagsMask) | (matchTypeIndex << MatchTypeShift), matchType, optionName);
	}

	/// <summary>
	///     Returns options in which the option of the <paramref name="flag" /> is <paramref name="isEnabled" />.
	/// </summary>
	private StringEqualityOptions WithFlag(int flag, bool isEnabled)
	{
		int flags = isEnabled ? _flags | flag : _flags & ~flag;
		return flags == _flags ? this : With(flags, _matchType, _matchTypeOption);
	}

	/// <summary>
	///     Returns the shared options of the <paramref name="flags" /> when they name a shared match type and no comparer
	///     is set, otherwise new options with the <paramref name="matchType" /> of the <paramref name="matchTypeOption" />.
	/// </summary>
	private StringEqualityOptions With(int flags, StringMatchType matchType, string? matchTypeOption)
		=> _comparer is null && flags >> MatchTypeShift != PrivateMatchTypeIndex
			? SharedValues.Get(flags)
			: new StringEqualityOptions(matchType, matchTypeOption, _comparer, flags, null);

	/// <summary>
	///     Rejects every specified setting that the match type of the <paramref name="matchTypeOption" /> does not
	///     support, named in the order of the option methods, e.g.
	///     <c>IgnoringIndentation and IgnoringNewlineStyle cannot be combined with AsBlock.</c>
	/// </summary>
	private void ThrowIfSpecifiedSettingsAreNotSupported(StringMatchSettingKinds supportedSettings,
		string matchTypeOption)
	{
		if (GetUnsupportedSettings(supportedSettings) is { } options)
		{
			string names = options.Count == 1
				? options[0]
				: string.Join(", ", options.Take(options.Count - 1)) + " and " + options[^1];
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new InvalidOperationException(
				$"{names} cannot be combined with {matchTypeOption}."));
		}
	}

	/// <summary>
	///     Returns the names of the specified settings that the <paramref name="supportedSettings" /> do not contain, in
	///     the order of the option methods, or <see langword="null" /> when they contain all of them.
	/// </summary>
	private List<string>? GetUnsupportedSettings(StringMatchSettingKinds supportedSettings)
	{
		List<string>? options = null;
		if (IgnoresCase && (supportedSettings & StringMatchSettingKinds.IgnoreCase) == 0)
		{
			AddOption(ref options, nameof(IgnoringCase));
		}

		if ((supportedSettings & StringMatchSettingKinds.WhiteSpace) == 0)
		{
			AddSpecifiedWhiteSpaceSettings(ref options);
		}

		if (_comparer is not null && (supportedSettings & StringMatchSettingKinds.Comparer) == 0)
		{
			AddOption(ref options, nameof(Using));
		}

		return options;
	}

	private void AddSpecifiedWhiteSpaceSettings(ref List<string>? options)
	{
		if (IgnoresIndentation)
		{
			AddOption(ref options, nameof(IgnoringIndentation));
		}

		if (IgnoresNewlineStyle)
		{
			AddOption(ref options, nameof(IgnoringNewlineStyle));
		}

		if (IgnoresLeadingWhiteSpace)
		{
			AddOption(ref options, nameof(IgnoringLeadingWhiteSpace));
		}

		if (IgnoresTrailingWhiteSpace)
		{
			AddOption(ref options, nameof(IgnoringTrailingWhiteSpace));
		}

		if (NormalizesWhiteSpace)
		{
			AddOption(ref options, nameof(NormalizingWhiteSpace));
		}
	}

	private static void AddOption(ref List<string>? options, string option)
		=> (options ??= []).Add(option);

	/// <summary>
	///     Rejects the specified <paramref name="option" /> of the <paramref name="setting" /> when the match type of
	///     the <paramref name="matchTypeOption" /> does not support it, with the same message in either order.
	/// </summary>
	private static void ThrowIfNotSupported(StringMatchSettingKinds supportedSettings,
		StringMatchSettingKinds setting, string? option, string? matchTypeOption)
	{
		if (option is not null && matchTypeOption is not null && (supportedSettings & setting) == 0)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new InvalidOperationException(
				$"{option} cannot be combined with {matchTypeOption}."));
		}
	}

	/// <summary>
	///     Adds the contexts of the matcher to the <paramref name="contexts" />, e.g. the options of a custom match
	///     type.
	/// </summary>
	public void AppendContexts(ResultContextCollector contexts)
		=> GetMatcher().AppendContexts(contexts);

	/// <summary>
	///     Rejects the line <paramref name="option" /> when it is enabled in combination with a match type that compares
	///     the lines on its own, e.g. <see cref="AsBlock()" />.
	/// </summary>
	private void ThrowIfLineOptionIsNotSupported(bool isEnabled, string option)
		=> ThrowIfNotSupported(_matchType.SupportedSettings, StringMatchSettingKinds.WhiteSpace,
			isEnabled ? option : null, _matchTypeOption);

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
	///     <see cref="MatchWithExplanation(string?, string?, string)" />, as a match type may explain the failure with what the
	///     comparison found.
	/// </remarks>
	public string GetExtendedFailure(string it, ExpectationGrammars grammars, string? actual, string? expected,
		MatchResult result)
	{
		if (grammars.HasFlag(ExpectationGrammars.Negated))
		{
			return $"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual)}";
		}

		int[]? ignoredColumnsPerLine = GetIgnoredColumnsPerLine(actual);
		actual = NormalizeLines(actual);

		int ignoredLineCount = 0;
		if (IgnoresLeadingWhiteSpace && actual is not null && ignoredColumnsPerLine is not null)
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
			return
				$"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual.TruncateWithEllipsisOnWord(DefaultMaxLength))}";
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
	///     Only the match types that show this difference get the hint, see
	///     <see cref="StringMatchType.SuggestsIgnoringNewlineStyle" />: a pattern matches newlines on its own terms, and
	///     a block already ignores the newline style. A value without a <c>\r</c> is not changed by the normalization, so
	///     this also gives no hint when the newline style is already ignored.<br />
	///     The comparer of the caller may throw for the normalized values, which then get no hint.
	/// </remarks>
	private string GetNewlineStyleHint(string? actual, string? expected)
	{
		if (!_matchType.SuggestsIgnoringNewlineStyle || actual is null || expected is null)
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
	///     Only a match type that compares for equality lists them, see
	///     <see cref="StringMatchType.ListsLineDifferences" />. A comparer of the caller defines when whole values are
	///     equal, which need not hold for their lines, so the lines are then not listed. Neither are they when the
	///     whitespace is normalized, which joins all lines.
	/// </remarks>
	internal string? GetLineDifferences(string actual, string expected)
	{
		if (!_matchType.ListsLineDifferences || _comparer is not null || NormalizesWhiteSpace ||
		    (!IsMultiLine(actual) && !IsMultiLine(expected)))
		{
			return null;
		}

		StringComparer comparer = UseDefaultComparer(IgnoresCase);
		bool ignoreLineBreaks = IgnoresNewlineStyle || IgnoresIndentation;
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
		if (IgnoresIndentation)
		{
			lines.ForEach(line => line.ComparedText = line.ComparedText.TrimStart());
		}

		if (IgnoresLeadingWhiteSpace)
		{
			lines.RemoveRange(0, lines.TakeWhile(line => string.IsNullOrWhiteSpace(line.ComparedText)).Count());
			if (lines.Count > 0)
			{
				lines[0].ComparedText = lines[0].ComparedText.TrimStart();
			}
		}

		if (IgnoresTrailingWhiteSpace)
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
	///     match type, is returned unchanged.<br />
	///     The <paramref name="member" /> holds a single value, also when <paramref name="it" /> is plural.
	/// </remarks>
	public string GetExtendedMemberFailure(string it, string member, ExpectationGrammars grammars,
		string? actual, string? expected, MatchResult result)
	{
		string failure = GetExtendedFailure(member, grammars & ~ExpectationGrammars.Plural, actual, expected, result);
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
	/// <exception cref="InvalidOperationException">
	///     A custom comparer is already set via <see cref="Using(IEqualityComparer{string})" />, or the match type does
	///     not support the casing, see <see cref="StringMatchType.SupportedSettings" />.
	/// </exception>
	public StringEqualityOptions IgnoringCase(bool ignoreCase = true)
	{
		if (ignoreCase && _comparer is not null)
		{
			throw CaseAndComparerConflict();
		}

		ThrowIfNotSupported(_matchType.SupportedSettings, StringMatchSettingKinds.IgnoreCase,
			ignoreCase ? nameof(IgnoringCase) : null, _matchTypeOption);
		return WithFlag(IgnoreCaseFlag, ignoreCase);
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
	///     The indentation is ignored and the expected value is matched as a block via <see cref="AsBlock()" />.
	/// </exception>
	public StringEqualityOptions IgnoringIndentation(bool ignoreIndentation = true)
	{
		ThrowIfLineOptionIsNotSupported(ignoreIndentation, nameof(IgnoringIndentation));
		return WithFlag(IgnoreIndentationFlag, ignoreIndentation);
	}

	/// <summary>
	///     Ignores the newline style when comparing <see langword="string" />s.
	/// </summary>
	/// <remarks>
	///     Enabling this option will replace all occurrences of <c>\r\n</c> and <c>\r</c> with <c>\n</c> in the strings before
	///     comparing them.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     The newline style is ignored and the expected value is matched as a block via <see cref="AsBlock()" />.
	/// </exception>
	public StringEqualityOptions IgnoringNewlineStyle(bool ignoreNewlineStyle = true)
	{
		ThrowIfLineOptionIsNotSupported(ignoreNewlineStyle, nameof(IgnoringNewlineStyle));
		return WithFlag(IgnoreNewlineStyleFlag, ignoreNewlineStyle);
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
	///     The leading whitespace is ignored and the expected value is matched as a block via <see cref="AsBlock()" />.
	/// </exception>
	public StringEqualityOptions IgnoringLeadingWhiteSpace(bool ignoreLeadingWhiteSpace = true)
	{
		ThrowIfLineOptionIsNotSupported(ignoreLeadingWhiteSpace, nameof(IgnoringLeadingWhiteSpace));
		return WithFlag(IgnoreLeadingWhiteSpaceFlag, ignoreLeadingWhiteSpace);
	}

	/// <summary>
	///     Ignores trailing whitespace when comparing <see langword="string" />s,
	///     according to the <paramref name="ignoreTrailingWhiteSpace" /> parameter.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///     The trailing whitespace is ignored and the expected value is matched as a block via <see cref="AsBlock()" />.
	/// </exception>
	public StringEqualityOptions IgnoringTrailingWhiteSpace(bool ignoreTrailingWhiteSpace = true)
	{
		ThrowIfLineOptionIsNotSupported(ignoreTrailingWhiteSpace, nameof(IgnoringTrailingWhiteSpace));
		return WithFlag(IgnoreTrailingWhiteSpaceFlag, ignoreTrailingWhiteSpace);
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
	///     The whitespace is normalized and the expected value is matched as a block via <see cref="AsBlock()" />.
	/// </exception>
	public StringEqualityOptions NormalizingWhiteSpace(bool normalizeWhiteSpace = true)
	{
		ThrowIfLineOptionIsNotSupported(normalizeWhiteSpace, nameof(NormalizingWhiteSpace));
		return WithFlag(NormalizeWhiteSpaceFlag, normalizeWhiteSpace);
	}

	/// <inheritdoc />
	public override string ToString() => _matchType.Phrase.Name + GetOptionString();

	/// <summary>
	///     Specifies a specific <see cref="IEqualityComparer{T}" /> to use for comparing <see langword="string" />s.
	/// </summary>
	/// <remarks>
	///     Without a comparer, the <see cref="StringComparer.Ordinal" /> or <see cref="StringComparer.OrdinalIgnoreCase" />
	///     is used depending on whether the casing is ignored.<br />
	///     A later comparer replaces the earlier one.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     The casing is already ignored via <see cref="IgnoringCase(bool)" />, or the match type does not support a
	///     comparer, see <see cref="StringMatchType.SupportedSettings" />, e.g. a regex or wildcard pattern, which the
	///     regex engine matches without a comparer.
	/// </exception>
	public StringEqualityOptions Using(IEqualityComparer<string> comparer)
	{
		comparer.ThrowIfNull();
		if (IgnoresCase)
		{
			throw CaseAndComparerConflict();
		}

		ThrowIfNotSupported(_matchType.SupportedSettings, StringMatchSettingKinds.Comparer, nameof(Using),
			_matchTypeOption);
		return new StringEqualityOptions(_matchType, _matchTypeOption, comparer, _flags, null);
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
	///     Whether the <paramref name="exception" /> is the timeout of a regex.
	/// </summary>
	/// <remarks>
	///     A catch clause of the regex timeout loads the regex assembly when the method is compiled, also for the
	///     match types that use no regex, so the type is only checked here, when an exception occurs.
	/// </remarks>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static bool IsRegexTimeout(Exception exception)
		=> exception is RegexMatchTimeoutException;

	/// <summary>
	///     Creates the exception for an <paramref name="expected" /> pattern that did not finish matching within the
	///     timeout.
	/// </summary>
	/// <remarks>
	///     An <see cref="ArgumentException" /> is not wrapped by the expectation node, so that the pattern which has
	///     to be simplified stays visible instead of being hidden behind a generic evaluation error.<br />
	///     The timeout is taken from the <paramref name="innerException" />, because it is the one that applied, also
	///     for a custom match type with a regex of its own, whose expected value is then called a pattern. It is a
	///     <see cref="RegexMatchTimeoutException" />, see <see cref="IsRegexTimeout(Exception)" />.
	/// </remarks>
	private ArgumentException CreateTimeoutException(string? expected, Exception innerException,
		string parameterName)
		// ReSharper disable once LocalizableElement
		=> Tracing.WriteException(new ArgumentException(
			$"The {_matchType.Phrase.PatternNoun ?? "pattern"} {Formatter.Format(expected)} did not complete within {Formatter.Format(((RegexMatchTimeoutException)innerException).MatchTimeout)}. Simplify the pattern to avoid catastrophic backtracking.",
			parameterName, innerException));

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
	///     See <see cref="StringMatchType.Anchors" />, e.g. a regex may match any part of the subject, like a substring.
	/// </remarks>
	private (bool IsStartAnchored, bool IsEndAnchored) GetAnchoredEdges()
	{
		StringMatchAnchors anchors = _matchType.Anchors;
		return ((anchors & StringMatchAnchors.Start) != 0, (anchors & StringMatchAnchors.End) != 0);
	}

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
		if (NormalizesWhiteSpace)
		{
			return value.CollapseWhiteSpace();
		}

		if (IgnoresIndentation)
		{
			return value.RemoveIndentation();
		}

		return IgnoresNewlineStyle ? value.RemoveNewlineStyle() : value;
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

	private bool IsLeadingWhiteSpaceIgnored => IgnoresLeadingWhiteSpace || NormalizesWhiteSpace;

	private bool IsTrailingWhiteSpaceIgnored => IgnoresTrailingWhiteSpace || NormalizesWhiteSpace;

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
		if (value is null || NormalizesWhiteSpace || (!IgnoresIndentation && !IgnoresLeadingWhiteSpace))
		{
			return null;
		}

		// When the indentation is ignored, the normalized value has the same lines as the newline normalized value,
		// but still contains the indentation whose width is measured below.
		string[] lines = (IgnoresIndentation ? value.RemoveNewlineStyle() : NormalizeLines(value)).Split('\n');
		int[] result = new int[lines.Length];
		if (IgnoresIndentation)
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
		string initialString = GetComparisonOptionString();
		if (!IgnoresLeadingWhiteSpace && !IgnoresTrailingWhiteSpace && !IgnoresNewlineStyle &&
		    !IgnoresIndentation)
		{
			return WithNormalizedWhiteSpace(initialString);
		}

		List<string> tokens = GetIgnoredWhiteSpaceTokens();
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

	private string GetComparisonOptionString()
	{
		if (_comparer is not null)
		{
			return $" using {Formatter.Format(_comparer.GetType())}";
		}

		return IgnoresCase ? " ignoring case" : "";
	}

	private List<string> GetIgnoredWhiteSpaceTokens()
	{
		List<string> tokens = [];
		switch (IgnoresLeadingWhiteSpace, IgnoresTrailingWhiteSpace)
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

		if (IgnoresNewlineStyle)
		{
			tokens.Add("newline style");
		}

		if (IgnoresIndentation)
		{
			tokens.Add("indentation");
		}

		return tokens;
	}

	private string WithNormalizedWhiteSpace(string optionString)
	{
		if (!NormalizesWhiteSpace)
		{
			return optionString;
		}

		return optionString.Length == 0
			? " normalizing whitespace"
			: optionString + " and normalizing whitespace";
	}

	/// <summary>
	///     Creates the exception for an expected value of the <paramref name="kind" /> that is empty, for the caller's
	///     <paramref name="parameterName" />.
	/// </summary>
	private static ArgumentException CreateEmptyPatternException(string parameterName, string kind)
		// ReSharper disable once LocalizableElement
		=> Tracing.WriteException(new ArgumentException($"The '{parameterName}' {kind} cannot be empty.",
			parameterName));

	/// <summary>
	///     Names the kind of pattern of the current match type, or returns <see langword="null" /> when the expected
	///     value is not a pattern.
	/// </summary>
	private string? GetPatternKind()
		=> _matchType.Phrase.PatternNoun;

	/// <summary>
	///     Verifies that the <paramref name="expected" /> value, which the caller received as
	///     <paramref name="parameterName" />, is usable for the current match type.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> pattern describes nothing to look for, so such an expectation says nothing about the
	///     subject. The match type rejects any other value it cannot use, e.g. an empty prefix, which matches every
	///     value. This can only be detected while the expectation is verified, because the match type can also be set
	///     after the pattern.
	/// </remarks>
	private void ValidatePattern(string? expected, string parameterName)
	{
		string? patternKind = GetPatternKind();
		if (patternKind is not null && expected is null)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentNullException(parameterName,
				$"The '{parameterName}' {patternKind} cannot be null."));
		}

		_matchType.ValidateExpected(expected, parameterName);
	}

	/// <summary>
	///     The shared options: one for every combination of the options without a comparer and of the built-in match
	///     types that take no arguments, created when first needed.
	/// </summary>
	/// <remarks>
	///     A value is created with its matcher and published with a compare-exchange, so that it is never written
	///     afterwards; a race creates a second value with the same options, which is dropped.<br />
	///     A nested class, so that the match types of the partial files are initialized before the table.
	/// </remarks>
	private static class SharedValues
	{
		private static readonly StringMatchType[] MatchTypes =
		[
			ExactMatch, PrefixMatch, SuffixMatch, ContainingMatch, BlockMatch, WildcardMatch, RegexMatch,
		];

		private static readonly string?[] MatchTypeOptions =
		[
			null, nameof(AsPrefix), nameof(AsSuffix), nameof(Containing), nameof(AsBlock), nameof(AsWildcard),
			nameof(AsRegex),
		];

		private static readonly StringEqualityOptions?[] Values =
			new StringEqualityOptions?[MatchTypes.Length << MatchTypeShift];

		public static StringEqualityOptions ExactOptions { get; } = Get(ExactMatchTypeIndex << MatchTypeShift);

		/// <summary>
		///     Returns the shared options of the <paramref name="flags" />, which name a built-in match type.
		/// </summary>
		public static StringEqualityOptions Get(int flags)
			=> Volatile.Read(ref Values[flags]) ?? Publish(flags);

		private static StringEqualityOptions Publish(int flags)
		{
			StringMatchType matchType = MatchTypes[flags >> MatchTypeShift];
			StringMatcher matcher = matchType.ForEvaluation(MatchEnvironment.WithoutEvaluation,
				new StringMatchSettings((flags & IgnoreCaseFlag) != 0, null));
			StringEqualityOptions value = new(matchType, MatchTypeOptions[flags >> MatchTypeShift], null, flags,
				matcher);
			return Interlocked.CompareExchange(ref Values[flags], value, null) ?? value;
		}
	}
}
