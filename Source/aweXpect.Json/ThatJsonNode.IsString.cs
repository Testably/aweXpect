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
	///     Verifies that the subject <see cref="JsonNode" /> is a <see cref="JsonValueKind.String" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNode, IThat<TNode?>> IsString<TNode>(this IThat<TNode?> subject)
		where TNode : JsonNode
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(static (it, grammar) =>
				new JsonNodeConstraint<TNode>(
					new ThatJsonElement.IsValueKindConstraint(it, grammar, JsonValueKind.String))),
			subject);

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is a <see cref="JsonValueKind.String" /> whose value
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNode, IThat<TNode?>> IsString<TNode>(this IThat<TNode?> subject,
		Action<IThatSubject<string>> expectations)
		where TNode : JsonNode
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return IsValue(subject, JsonValueReaders.String, expectations);
	}
}
