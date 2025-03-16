using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> matches the <paramref name="expected" /> value exactly.
	/// </summary>
	public static AndOrResult<JsonElement?, IThat<JsonElement?>> MatchesExactly(
		this IThat<JsonElement?> source,
		object? expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		JsonOptions jsonOptions = new();
		if (options != null)
		{
			jsonOptions = options(jsonOptions);
		}

		return new AndOrResult<JsonElement?, IThat<JsonElement?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammar) =>
				new MatchesConstraint(it, grammar, expected, doNotPopulateThisValue, jsonOptions)),
			source);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> matches the <paramref name="expected" /> array exactly.
	/// </summary>
	public static AndOrResult<JsonElement?, IThat<JsonElement?>> MatchesExactly<T>(
		this IThat<JsonElement?> source,
		IEnumerable<T> expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		JsonOptions jsonOptions = new();
		if (options != null)
		{
			jsonOptions = options(jsonOptions);
		}

		return new AndOrResult<JsonElement?, IThat<JsonElement?>>(
			source.Get().ExpectationBuilder.AddConstraint((it, grammar) =>
				new MatchesConstraint(it, grammar, expected, doNotPopulateThisValue, jsonOptions)),
			source);
	}
}
