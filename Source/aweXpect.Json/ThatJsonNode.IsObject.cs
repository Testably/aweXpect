using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Json.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonNode
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is an <see cref="JsonValueKind.Object" />.
	/// </summary>
	[GuaranteesNotNull]
	public static IsObjectResult<TNode, IThat<TNode?>> IsObject<TNode>(
		this IThat<TNode?> subject)
		where TNode : JsonNode
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(static (it, grammar) =>
				new JsonNodeConstraint<TNode>(
					new ThatJsonElement.IsValueKindConstraint(it, grammar, JsonValueKind.Object))),
			subject,
			ToJsonElement);

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is an <see cref="JsonValueKind.Object" />
	///     whose value satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static IsObjectResult<TNode, IThat<TNode?>> IsObject<TNode>(
		this IThat<TNode?> subject,
		Action<IJsonObjectResult> expectations,
		Func<JsonOptions, JsonOptions>? options = null)
		where TNode : JsonNode
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		JsonOptions jsonOptions = new()
		{
			IgnoreAdditionalProperties = true,
		};
		if (options != null)
		{
			jsonOptions = options(jsonOptions);
		}

		return new IsObjectResult<TNode, IThat<TNode?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expectations: expectations, Options: jsonOptions),
				static (s, it, grammar) => new JsonNodeConstraint<TNode>(
					new ThatJsonElement.IsObjectConstraint(it, grammar, s.Expectations, s.Options))),
			subject,
			ToJsonElement);
	}
}
