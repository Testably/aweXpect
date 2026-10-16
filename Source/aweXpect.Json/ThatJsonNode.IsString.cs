using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonNode
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is a <see cref="JsonValueKind.String" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNode?, IThat<TNode?>> IsString<TNode>(this IThat<TNode?> source)
		where TNode : JsonNode
		=> new(
			source.Get().ExpectationBuilder.AddConstraint(static (it, grammar) =>
				new JsonNodeConstraint<TNode>(
					new ThatJsonElement.IsValueKindConstraint(it, grammar, JsonValueKind.String))),
			source);
}
