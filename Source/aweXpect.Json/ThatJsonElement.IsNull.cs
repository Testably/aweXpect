using System.Text.Json;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatJsonElement
{
	[GenerateExpectation("IsNull", "is {not} null",
		NullableOverload = false,
		Result = nameof(DescribeValueKind),
		Summary = "Verifies that the subject <see cref=\"JsonElement\" /> is <see cref=\"JsonValueKind.Null\" />.")]
	private static bool IsNullKind(JsonElement element)
		=> element.ValueKind == JsonValueKind.Null;
}
