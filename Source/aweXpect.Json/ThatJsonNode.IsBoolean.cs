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
	///     Verifies that the subject <see cref="JsonNode" /> is a boolean, i.e. <see cref="JsonValueKind.True" /> or
	///     <see cref="JsonValueKind.False" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNode, IThat<TNode?>> IsBoolean<TNode>(this IThat<TNode?> subject)
		where TNode : JsonNode
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(static (it, grammar) =>
				new JsonNodeConstraint<TNode>(
					new ThatJsonElement.IsValueKindConstraint(it, grammar, JsonValueKind.True))),
			subject);

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is a boolean, i.e. <see cref="JsonValueKind.True" /> or
	///     <see cref="JsonValueKind.False" />, whose value satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNode, IThat<TNode?>> IsBoolean<TNode>(this IThat<TNode?> subject,
		Action<IThatSubject<bool>> expectations)
		where TNode : JsonNode
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return IsValue(subject, JsonValueReaders.Boolean, expectations);
	}
}
