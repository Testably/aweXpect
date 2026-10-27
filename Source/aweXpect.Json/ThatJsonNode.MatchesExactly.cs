using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Json.Options;
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
		=> AddMatchesConstraint(subject, expected, doNotPopulateThisValue, JsonOptions.Default, options);

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> matches the <paramref name="expected" /> JSON exactly.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expected" /> value is parsed as JSON text, and <see langword="null" /> is the JSON
	///     <c>null</c>. A JSON string is written with quotes, e.g. <c>"\"Let It Be\""</c>.
	/// </remarks>
	/// <exception cref="ArgumentException">The <paramref name="expected" /> value is no valid JSON.</exception>
	public static AndOrResult<TNode?, IThat<TNode?>> MatchesExactly<TNode>(
		this IThat<TNode?> subject,
		string? expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where TNode : JsonNode
		=> AddMatchesJsonTextConstraint(subject, expected, doNotPopulateThisValue, JsonOptions.Default, options);

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> matches the <paramref name="expected" /> array exactly.
	/// </summary>
	public static AndOrResult<TNode?, IThat<TNode?>> MatchesExactly<TNode, T>(
		this IThat<TNode?> subject,
		IEnumerable<T> expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
		where TNode : JsonNode
		=> AddMatchesConstraint(subject, expected, doNotPopulateThisValue, JsonOptions.Default, options);
}
