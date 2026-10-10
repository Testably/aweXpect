using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     Json expectations on <see langword="string" /> values.
/// </summary>
public static partial class ThatJsonString
{
	/// <summary>
	///     Verifies that the subject is a valid JSON string which matches the <paramref name="expected" /> value.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<string?, IThat<string?>> IsValidJsonMatching(
		this IThat<string?> source,
		object? expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		JsonOptions defaultOptions = new()
		{
			IgnoreAdditionalProperties = true,
		};
		if (options != null)
		{
			defaultOptions = options(defaultOptions);
		}

		return new AndOrResult<string?, IThat<string?>>(
			source.Get().ExpectationBuilder.AddConstraint(
				(Expected: expected, Expression: doNotPopulateThisValue, Options: defaultOptions),
				static (s, it, grammar) => new MatchesJsonConstraint(it, grammar, s.Expected, s.Expression, s.Options)),
			source);
	}

	/// <summary>
	///     Verifies that the subject is a valid JSON string which matches the <paramref name="expected" /> array.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<string?, IThat<string?>> IsValidJsonMatching<T>(
		this IThat<string?> source,
		IEnumerable<T> expected,
		Func<JsonOptions, JsonOptions>? options = null,
		[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		JsonOptions defaultOptions = new()
		{
			IgnoreAdditionalProperties = true,
		};
		if (options != null)
		{
			defaultOptions = options(defaultOptions);
		}

		return new AndOrResult<string?, IThat<string?>>(
			source.Get().ExpectationBuilder.AddConstraint(
				(Expected: expected, Expression: doNotPopulateThisValue, Options: defaultOptions),
				static (s, it, grammar) => new MatchesJsonConstraint(it, grammar, s.Expected, s.Expression, s.Options)),
			source);
	}
}
