using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonNode
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> matches the <paramref name="expected" /> value exactly.
	/// </summary>
	public static AndOrResult<TNode?, IThat<TNode?>> MatchesExactly<TNode>(
		this IThat<TNode?> subject,
		object? expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where TNode : JsonNode
		=> AddMatchesConstraint(subject, expected, doNotPopulateThisValue, new JsonOptions(), options);

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> matches the <paramref name="expected" /> array exactly.
	/// </summary>
	public static AndOrResult<TNode?, IThat<TNode?>> MatchesExactly<TNode, T>(
		this IThat<TNode?> subject,
		IEnumerable<T> expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where TNode : JsonNode
		=> AddMatchesConstraint(subject, expected, doNotPopulateThisValue, new JsonOptions(), options);
}
