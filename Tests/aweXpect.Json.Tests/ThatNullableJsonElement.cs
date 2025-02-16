using System.Text.Json;

namespace aweXpect.Tests;

public sealed partial class ThatNullableJsonElement
{
	public static JsonElement? FromString(string value)
	{
		using JsonDocument document = JsonDocument.Parse(value);
		return document.RootElement.Clone();
	}
}
