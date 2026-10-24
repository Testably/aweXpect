using System;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     The operators of an <see cref="IStringPropertyResult{TType, TThat}" />.
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
	/// </remarks>
	public static StringEqualityResult<TType, TThat> Containing<TType, TThat>(
		this IStringPropertyResult<TType, TThat> result,
		string expected)
	{
		ThrowIfNullOrEmpty(expected, nameof(expected));
		result.Validation?.Invoke(expected, nameof(expected));
		StringEqualityOptions options = new(nameof(expected));
		options.Containing();
		return result.Compare(expected, options, false, CreateResult<TType, TThat>);
	}

	/// <summary>
	///     …ends with the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> or empty <paramref name="expected" /> throws, because neither is a substring
	///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
	///     string always does.
	/// </remarks>
	public static StringEqualityResult<TType, TThat> EndingWith<TType, TThat>(
		this IStringPropertyResult<TType, TThat> result,
		string expected)
	{
		ThrowIfNullOrEmpty(expected, nameof(expected));
		result.Validation?.Invoke(expected, nameof(expected));
		StringEqualityOptions options = new(nameof(expected));
		options.AsSuffix();
		return result.Compare(expected, options, false, CreateResult<TType, TThat>);
	}

	/// <summary>
	///     …is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static StringEqualityTypeResult<TType, TThat> EqualTo<TType, TThat>(
		this IStringPropertyResult<TType, TThat> result,
		string? expected)
	{
		result.Validation?.Invoke(expected, nameof(expected));
		StringEqualityOptions options = new(nameof(expected));
		return result.Compare(expected, options, false, CreateTypeResult<TType, TThat>);
	}

	/// <summary>
	///     …does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> or empty <paramref name="unexpected" /> throws, because neither is a substring
	///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
	///     string always does.
	/// </remarks>
	public static StringEqualityResult<TType, TThat> NotContaining<TType, TThat>(
		this IStringPropertyResult<TType, TThat> result,
		string unexpected)
	{
		ThrowIfNullOrEmpty(unexpected, nameof(unexpected));
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		StringEqualityOptions options = new(nameof(unexpected));
		options.Containing();
		return result.Compare(unexpected, options, true, CreateResult<TType, TThat>);
	}

	/// <summary>
	///     …does not end with the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> or empty <paramref name="unexpected" /> throws, because neither is a substring
	///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
	///     string always does.
	/// </remarks>
	public static StringEqualityResult<TType, TThat> NotEndingWith<TType, TThat>(
		this IStringPropertyResult<TType, TThat> result,
		string unexpected)
	{
		ThrowIfNullOrEmpty(unexpected, nameof(unexpected));
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		StringEqualityOptions options = new(nameof(unexpected));
		options.AsSuffix();
		return result.Compare(unexpected, options, true, CreateResult<TType, TThat>);
	}

	/// <summary>
	///     …is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static StringEqualityTypeResult<TType, TThat> NotEqualTo<TType, TThat>(
		this IStringPropertyResult<TType, TThat> result,
		string? unexpected)
	{
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		StringEqualityOptions options = new(nameof(unexpected));
		return result.Compare(unexpected, options, true, CreateTypeResult<TType, TThat>);
	}

	/// <summary>
	///     …does not start with the <paramref name="unexpected" /> value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> or empty <paramref name="unexpected" /> throws, because neither is a substring
	///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
	///     string always does.
	/// </remarks>
	public static StringEqualityResult<TType, TThat> NotStartingWith<TType, TThat>(
		this IStringPropertyResult<TType, TThat> result,
		string unexpected)
	{
		ThrowIfNullOrEmpty(unexpected, nameof(unexpected));
		result.Validation?.Invoke(unexpected, nameof(unexpected));
		StringEqualityOptions options = new(nameof(unexpected));
		options.AsPrefix();
		return result.Compare(unexpected, options, true, CreateResult<TType, TThat>);
	}

	/// <summary>
	///     …starts with the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> or empty <paramref name="expected" /> throws, because neither is a substring
	///     anything could meaningfully be checked against: <see langword="null" /> never matches and the empty
	///     string always does.
	/// </remarks>
	public static StringEqualityResult<TType, TThat> StartingWith<TType, TThat>(
		this IStringPropertyResult<TType, TThat> result,
		string expected)
	{
		ThrowIfNullOrEmpty(expected, nameof(expected));
		result.Validation?.Invoke(expected, nameof(expected));
		StringEqualityOptions options = new(nameof(expected));
		options.AsPrefix();
		return result.Compare(expected, options, false, CreateResult<TType, TThat>);
	}

	private static StringEqualityResult<TType, TThat> CreateResult<TType, TThat>(
		ExpectationBuilder expectationBuilder, TThat subject, StringEqualityOptions options)
		=> new(expectationBuilder, subject, options);

	private static StringEqualityTypeResult<TType, TThat> CreateTypeResult<TType, TThat>(
		ExpectationBuilder expectationBuilder, TThat subject, StringEqualityOptions options)
		=> new(expectationBuilder, subject, options);

	private static void ThrowIfNullOrEmpty(string value, string paramName)
	{
#pragma warning disable S3236 // The caller information would name the local parameter instead of the public one it forwards
		value.ThrowIfNull(paramName);
#pragma warning restore S3236
		if (value.Length == 0)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException($"The '{paramName}' string cannot be empty.", paramName));
		}
	}
}
