using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Json.Results;

namespace aweXpect;

public static partial class ThatJsonNode
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is an <see cref="JsonValueKind.Object" />.
	/// </summary>
	[GuaranteesNotNull]
	public static IsObjectResult<TNode, IThat<TNode?>, KeyValuePair<string, JsonNode?>> IsObject<TNode>(
		this IThat<TNode?> subject)
		where TNode : JsonNode
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(static (it, grammar) =>
				new JsonNodeConstraint<TNode>(
					new ThatJsonElement.IsValueKindConstraint(it, grammar, JsonValueKind.Object))),
			subject,
			GetProperties);

	/// <remarks>
	///     A <see cref="JsonValue" /> can also represent an object, e.g. one that holds a .NET object, whose values are no
	///     nodes of their own, so that they are parsed as new nodes then. So are the values of a parsed
	///     <see cref="JsonObject" /> that contains a name more than once, which it throws for when it is enumerated,
	///     while its <see cref="JsonElement" /> keeps every property.
	/// </remarks>
	private static IEnumerable<KeyValuePair<string, JsonNode?>> GetProperties(JsonNode node)
		=> node is JsonObject jsonObject && TryGetProperties(jsonObject, out KeyValuePair<string, JsonNode?>[]? properties)
			? properties
			: ToJsonElement(node).EnumerateObject()
				.Select(static property => new KeyValuePair<string, JsonNode?>(property.Name, ToJsonNode(property.Value)))
				.ToArray();

	private static bool TryGetProperties(JsonObject jsonObject,
		[NotNullWhen(true)] out KeyValuePair<string, JsonNode?>[]? properties)
	{
		try
		{
			properties = jsonObject.ToArray();
			return true;
		}
		catch (ArgumentException)
		{
			properties = null;
			return false;
		}
	}
}
