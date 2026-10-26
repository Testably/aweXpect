using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Json;
using aweXpect.Json.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     Json expectations on <see langword="string" /> values.
/// </summary>
public static partial class ThatJsonString
{
	/// <summary>
	///     Verifies that the subject is a valid JSON string which matches the <paramref name="expected" /> value exactly.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<string, IThat<string?>> IsValidJsonMatchingExactly(
		this IThat<string?> subject,
		object? expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		JsonOptions defaultOptions = new();
		if (options != null)
		{
			defaultOptions = options(defaultOptions);
		}

		return new AndOrResult<string, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(Expected: expected, Expression: doNotPopulateThisValue, Options: defaultOptions),
				static (s, it, grammar) => new MatchesJsonConstraint(it, grammar, s.Expected, s.Expression, s.Options)),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is a valid JSON string which matches the <paramref name="expected" /> JSON exactly.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expected" /> value is parsed as JSON text, and <see langword="null" /> is the JSON
	///     <c>null</c>. A JSON string is written with quotes, e.g. <c>"\"Let It Be\""</c>.
	/// </remarks>
	/// <exception cref="ArgumentException">The <paramref name="expected" /> value is no valid JSON.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<string, IThat<string?>> IsValidJsonMatchingExactly(
		this IThat<string?> subject,
		string? expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		JsonOptions defaultOptions = new();
		if (options != null)
		{
			defaultOptions = options(defaultOptions);
		}

		return new AndOrResult<string, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(Expected: ExpectationJsonConverter.FromJsonText(expected, defaultOptions.DocumentOptions),
					Expression: doNotPopulateThisValue, Options: defaultOptions),
				static (s, it, grammar) => new MatchesJsonConstraint(it, grammar, s.Expected, s.Expression, s.Options)),
			subject);
	}

	/// <summary>
	///     Verifies that the subject is a valid JSON string which matches the <paramref name="expected" /> array exactly.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<string, IThat<string?>> IsValidJsonMatchingExactly<T>(
		this IThat<string?> subject,
		IEnumerable<T> expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		JsonOptions defaultOptions = new();
		if (options != null)
		{
			defaultOptions = options(defaultOptions);
		}

		return new AndOrResult<string, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(Expected: expected, Expression: doNotPopulateThisValue, Options: defaultOptions),
				static (s, it, grammar) => new MatchesJsonConstraint(it, grammar, s.Expected, s.Expression, s.Options)),
			subject);
	}
}
