using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonNode
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> matches the <paramref name="expected" /> value.
	/// </summary>
	public static AndOrResult<TNode?, IThat<TNode?>> Matches<TNode>(
		this IThat<TNode?> subject,
		object? expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where TNode : JsonNode
		=> AddMatchesConstraint(subject, expected, doNotPopulateThisValue, new JsonOptions
		{
			IgnoreAdditionalProperties = true,
		}, options);

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> matches the <paramref name="expected" /> array.
	/// </summary>
	public static AndOrResult<TNode?, IThat<TNode?>> Matches<TNode, T>(
		this IThat<TNode?> subject,
		IEnumerable<T> expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where TNode : JsonNode
		=> AddMatchesConstraint(subject, expected, doNotPopulateThisValue, new JsonOptions
		{
			IgnoreAdditionalProperties = true,
		}, options);

	private static AndOrResult<TNode?, IThat<TNode?>> AddMatchesConstraint<TNode>(
		IThat<TNode?> source,
		object? expected,
		string expectedExpression,
		JsonOptions defaultOptions,
		Func<JsonOptions, JsonOptions>? options)
		where TNode : JsonNode
	{
		JsonOptions jsonOptions = options == null ? defaultOptions : options(defaultOptions);
		return new AndOrResult<TNode?, IThat<TNode?>>(
			source.Get().ExpectationBuilder.AddConstraint(
				(Expected: expected, Expression: expectedExpression, Options: jsonOptions),
				static (s, it, grammar) => new JsonNodeConstraint<TNode>(
					new ThatJsonElement.MatchesConstraint(it, grammar, s.Expected, s.Expression, s.Options))),
			source);
	}
}
