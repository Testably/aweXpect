using System;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     The operators of the <see langword="string" /> property results
///     <see cref="IStringEqualityPropertyResult{TResult}" />, <see cref="IStringAffixPropertyResult{TResult}" /> and
///     <see cref="IStringOccurrencePropertyResult{TResult}" />.
/// </summary>
public static class StringPropertyResultExtensions
{
	/// <summary>
	///     …contains the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> or empty <paramref name="expected" /> throws, because neither is a substring
	///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
	///     string always does.
	///     <para />
	///     Without a quantifier, the <paramref name="expected" /> value has to occur at least once; specify how often
	///     it has to occur, e.g. with <c>AtLeast(2)</c> or <c>Never()</c>, and how it is matched, e.g. with
	///     <c>AsWildcard()</c>, on the result.
	/// </remarks>
	public static TResult Containing<TResult>(this IStringOccurrencePropertyResult<TResult> result,
		string expected)
	{
		expected.ThrowIfNull();
		ThrowIfEmpty(expected, nameof(expected));
		result.Validation?.Invoke(expected, nameof(expected));
		return result.Compare(new StringPropertyComparison(expected,
			new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default),
			new OptionsHolder<Quantifier>(Quantifier.Default)));
	}

	/// <summary>
	///     …ends with the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> or empty <paramref name="expected" /> throws, because neither is a substring
	///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
	///     string always does.
	/// </remarks>
	public static TResult EndingWith<TResult>(this IStringAffixPropertyResult<TResult> result,
		string expected)
	{
		expected.ThrowIfNull();
		ThrowIfEmpty(expected, nameof(expected));
		result.Validation?.Invoke(expected, nameof(expected));
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default.AsSuffix());
		return result.Compare(new StringPropertyComparison(expected, options, false));
	}

	/// <summary>
	///     …is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static TResult EqualTo<TResult>(this IStringEqualityPropertyResult<TResult> result,
		string? expected)
	{
		result.Validation?.Invoke(expected, nameof(expected));
		return result.Compare(new StringPropertyComparison(expected, new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default),
			false));
	}

	/// <summary>
	///     …does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> or empty <paramref name="unexpected" /> throws, because neither is a substring
	///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
	///     string always does.
	/// </remarks>
	public static TResult NotContaining<TResult>(this IStringAffixPropertyResult<TResult> result,
		string unexpected)
	{
		unexpected.ThrowIfNull();
		ThrowIfEmpty(unexpected, nameof(unexpected));
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default.Containing());
		return result.Compare(new StringPropertyComparison(unexpected, options, true));
	}

	/// <summary>
	///     …does not end with the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> or empty <paramref name="unexpected" /> throws, because neither is a substring
	///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
	///     string always does.
	/// </remarks>
	public static TResult NotEndingWith<TResult>(this IStringAffixPropertyResult<TResult> result,
		string unexpected)
	{
		unexpected.ThrowIfNull();
		ThrowIfEmpty(unexpected, nameof(unexpected));
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default.AsSuffix());
		return result.Compare(new StringPropertyComparison(unexpected, options, true));
	}

	/// <summary>
	///     …is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static TResult NotEqualTo<TResult>(this IStringEqualityPropertyResult<TResult> result,
		string? unexpected)
	{
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		return result.Compare(new StringPropertyComparison(unexpected, new OptionsHolder<StringEqualityOptions>(StringEqualityOptions.Default),
			true));
	}

	/// <summary>
	///     …does not start with the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> or empty <paramref name="unexpected" /> throws, because neither is a substring
	///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
	///     string always does.
	/// </remarks>
	public static TResult NotStartingWith<TResult>(this IStringAffixPropertyResult<TResult> result,
		string unexpected)
	{
		unexpected.ThrowIfNull();
		ThrowIfEmpty(unexpected, nameof(unexpected));
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default.AsPrefix());
		return result.Compare(new StringPropertyComparison(unexpected, options, true));
	}

	/// <summary>
	///     …starts with the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> or empty <paramref name="expected" /> throws, because neither is a substring
	///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
	///     string always does.
	/// </remarks>
	public static TResult StartingWith<TResult>(this IStringAffixPropertyResult<TResult> result,
		string expected)
	{
		expected.ThrowIfNull();
		ThrowIfEmpty(expected, nameof(expected));
		result.Validation?.Invoke(expected, nameof(expected));
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default.AsPrefix());
		return result.Compare(new StringPropertyComparison(expected, options, false));
	}

	private static void ThrowIfEmpty(string value, string paramName)
	{
		if (value.Length == 0)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException($"The '{paramName}' string cannot be empty.", paramName));
		}
	}
}
