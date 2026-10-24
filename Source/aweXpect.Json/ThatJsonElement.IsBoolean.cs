using System.Text.Json;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatJsonElement
{
	[GenerateExpectation("IsBoolean", "is {not} a boolean",
		NullableOverload = false,
		Result = nameof(DescribeValueKind),
		Summary = """
		          Verifies that the subject <see cref="JsonElement" /> is a boolean, i.e. <see cref="JsonValueKind.True" /> or
		          <see cref="JsonValueKind.False" />.
		          """)]
	private static bool IsBooleanKind(JsonElement element)
		=> element.ValueKind is JsonValueKind.True or JsonValueKind.False;
}
