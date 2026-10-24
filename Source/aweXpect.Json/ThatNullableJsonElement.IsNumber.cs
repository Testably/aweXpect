using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableJsonElement
{
	[GenerateExpectation("IsNumber", "is {not} a number",
		Result = nameof(DescribeValueKind),
		Summary = "Verifies that the subject <see cref=\"JsonElement\" /> is a <see cref=\"JsonValueKind.Number\" />.")]
	private static bool IsNumberKind([DisallowNull] JsonElement? element)
		=> element.Value.ValueKind == JsonValueKind.Number;
}
