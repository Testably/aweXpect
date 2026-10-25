using System.Text.Json;
using System.Text.Json.Nodes;

namespace aweXpect.Json;

/// <summary>
///     Projects the value that a constraint found in the <see cref="JsonElement" /> of a <see cref="JsonNode" /> back to
///     the node, so that the nested expectations receive the node itself.
/// </summary>
internal interface IJsonNodeProjection
{
	bool TryProject<TValue>(JsonNode? node, out TValue? value);
}
