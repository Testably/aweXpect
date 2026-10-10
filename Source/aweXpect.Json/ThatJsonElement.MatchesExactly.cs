using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> matches the <paramref name="expected" /> value exactly.
	/// </summary>
	public static AndOrResult<JsonElement, IThat<JsonElement>> MatchesExactly(
		this IThat<JsonElement> source,
		object? expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		JsonOptions jsonOptions = new();
		if (options != null)
		{
			jsonOptions = options(jsonOptions);
		}

		return new AndOrResult<JsonElement, IThat<JsonElement>>(
			source.Get().ExpectationBuilder.AddConstraint(
				(Expected: expected, Expression: doNotPopulateThisValue, Options: jsonOptions),
				static (s, it, grammar) => new MatchesConstraint(it, grammar, s.Expected, s.Expression, s.Options)),
			source);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> matches the <paramref name="expected" /> array exactly.
	/// </summary>
	public static AndOrResult<JsonElement, IThat<JsonElement>> MatchesExactly<T>(
		this IThat<JsonElement> source,
		IEnumerable<T> expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		JsonOptions jsonOptions = new();
		if (options != null)
		{
			jsonOptions = options(jsonOptions);
		}

		return new AndOrResult<JsonElement, IThat<JsonElement>>(
			source.Get().ExpectationBuilder.AddConstraint(
				(Expected: expected, Expression: doNotPopulateThisValue, Options: jsonOptions),
				static (s, it, grammar) => new MatchesConstraint(it, grammar, s.Expected, s.Expression, s.Options)),
			source);
	}
}
