using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Json.Results;

namespace aweXpect;

public static partial class ThatJsonNode
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is an <see cref="JsonValueKind.Array" />.
	/// </summary>
	[GuaranteesNotNull]
	public static IsArrayResult<TNode, IThat<TNode?>> IsArray<TNode>(
		this IThat<TNode?> subject)
		where TNode : JsonNode
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(static (it, grammar) =>
				new JsonNodeConstraint<TNode>(
					new ThatJsonElement.IsValueKindConstraint(it, grammar, JsonValueKind.Array))),
			subject,
			ToJsonElement);
}
