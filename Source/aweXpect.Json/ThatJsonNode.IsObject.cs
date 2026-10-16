using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonNode
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is an <see cref="JsonValueKind.Object" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNode?, IThat<TNode?>> IsObject<TNode>(
		this IThat<TNode?> source)
		where TNode : JsonNode
		=> new(
			source.Get().ExpectationBuilder.AddConstraint(static (it, grammar) =>
				new JsonNodeConstraint<TNode>(
					new ThatJsonElement.IsValueKindConstraint(it, grammar, JsonValueKind.Object))),
			source);

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is an <see cref="JsonValueKind.Object" />
	///     whose value satisfies the <paramref name="expectation" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNode?, IThat<TNode?>> IsObject<TNode>(
		this IThat<TNode?> source,
		Func<IJsonObjectResult, IJsonObjectResult> expectation,
		Func<JsonOptions, JsonOptions>? options = null)
		where TNode : JsonNode
	{
		ThrowHelper.ThrowIfNull(expectation, nameof(expectation));
		JsonOptions jsonOptions = new()
		{
			IgnoreAdditionalProperties = true,
		};
		if (options != null)
		{
			jsonOptions = options(jsonOptions);
		}

		return new AndOrResult<TNode?, IThat<TNode?>>(
			source.Get().ExpectationBuilder.AddConstraint((Expectation: expectation, Options: jsonOptions),
				static (s, it, grammar) => new JsonNodeConstraint<TNode>(
					new ThatJsonElement.IsObjectConstraint(it, grammar, s.Expectation, s.Options))),
			source);
	}
}
