using System.Text.Json;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatJsonElement
{
	[GenerateExpectation("IsNumber", "is {not} a number",
		NullableOverload = false,
		Result = nameof(DescribeValueKind),
		Summary = "Verifies that the subject <see cref=\"JsonElement\" /> is a <see cref=\"JsonValueKind.Number\" />.")]
	private static bool IsNumberKind(JsonElement element)
		=> element.ValueKind == JsonValueKind.Number;
}
