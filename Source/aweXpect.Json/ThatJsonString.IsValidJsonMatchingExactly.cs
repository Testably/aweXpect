using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Json;
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
	public static AndOrResult<string?, IThat<string?>> IsValidJsonMatchingExactly(
		this IThat<string?> source,
		object? expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		JsonOptions defaultOptions = new();
		if (options != null)
		{
			defaultOptions = options(defaultOptions);
		}

		return new AndOrResult<string?, IThat<string?>>(
			source.ThatIs().ExpectationBuilder.AddConstraint((it, grammar) =>
				new MatchesJsonConstraint(it, expected, doNotPopulateThisValue, defaultOptions)),
			source);
	}

	/// <summary>
	///     Verifies that the subject is a valid JSON string which matches the <paramref name="expected" /> array exactly.
	/// </summary>
	public static AndOrResult<string?, IThat<string?>> IsValidJsonMatchingExactly<T>(
		this IThat<string?> source,
		IEnumerable<T> expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		JsonOptions defaultOptions = new();
		if (options != null)
		{
			defaultOptions = options(defaultOptions);
		}

		return new AndOrResult<string?, IThat<string?>>(
			source.ThatIs().ExpectationBuilder.AddConstraint((it, grammar) =>
				new MatchesJsonConstraint(it, expected, doNotPopulateThisValue, defaultOptions)),
			source);
	}
}
