using System.Text.Json.Nodes;

namespace aweXpect.Json.Tests;

public sealed partial class ThatJsonNode
{
	public static JsonNode? FromString(string value)
		=> JsonNode.Parse(value);
}
