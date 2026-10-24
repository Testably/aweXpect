using System.Text.Json;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatJsonElement
{
	[GenerateExpectation("IsString", "is {not} a string",
		NullableOverload = false,
		Result = nameof(DescribeValueKind),
		Summary = "Verifies that the subject <see cref=\"JsonElement\" /> is a <see cref=\"JsonValueKind.String\" />.")]
	private static bool IsStringKind(JsonElement element)
		=> element.ValueKind == JsonValueKind.String;
}
