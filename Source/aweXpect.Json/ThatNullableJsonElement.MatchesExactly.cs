using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Json;
using aweXpect.Json.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> matches the <paramref name="expected" /> value exactly.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<JsonElement, IThat<JsonElement?>> MatchesExactly(
		this IThat<JsonElement?> subject,
		object? expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		JsonOptions jsonOptions = new();
		if (options != null)
		{
			jsonOptions = options(jsonOptions);
		}

		return new AndOrResult<JsonElement, IThat<JsonElement?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(Expected: expected, Expression: doNotPopulateThisValue, Options: jsonOptions),
				static IAsyncContextConstraint<JsonElement?> (s, it, grammar) =>
					new ThatJsonElement.MatchesConstraint(it, grammar, s.Expected, s.Expression, s.Options)),
			subject);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> matches the <paramref name="expected" /> JSON exactly.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expected" /> value is parsed as JSON text, and <see langword="null" /> is the JSON
	///     <c>null</c>. A JSON string is written with quotes, e.g. <c>"\"Let It Be\""</c>.
	/// </remarks>
	/// <exception cref="ArgumentException">The <paramref name="expected" /> value is no valid JSON.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<JsonElement, IThat<JsonElement?>> MatchesExactly(
		this IThat<JsonElement?> subject,
		string? expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		JsonOptions jsonOptions = new();
		if (options != null)
		{
			jsonOptions = options(jsonOptions);
		}

		return new AndOrResult<JsonElement, IThat<JsonElement?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(Expected: ExpectationJsonConverter.FromJsonText(expected, jsonOptions.DocumentOptions),
					Expression: doNotPopulateThisValue, Options: jsonOptions),
				static IAsyncContextConstraint<JsonElement?> (s, it, grammar) =>
					new ThatJsonElement.MatchesConstraint(it, grammar, s.Expected, s.Expression, s.Options)),
			subject);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> matches the <paramref name="expected" /> array exactly.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<JsonElement, IThat<JsonElement?>> MatchesExactly<T>(
		this IThat<JsonElement?> subject,
		IEnumerable<T> expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		JsonOptions jsonOptions = new();
		if (options != null)
		{
			jsonOptions = options(jsonOptions);
		}

		return new AndOrResult<JsonElement, IThat<JsonElement?>>(
			subject.Get().ExpectationBuilder.AddConstraint(
				(Expected: expected, Expression: doNotPopulateThisValue, Options: jsonOptions),
				static IAsyncContextConstraint<JsonElement?> (s, it, grammar) =>
					new ThatJsonElement.MatchesConstraint(it, grammar, s.Expected, s.Expression, s.Options)),
			subject);
	}
}
