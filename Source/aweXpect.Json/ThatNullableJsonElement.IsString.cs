using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableJsonElement
{
	[GenerateExpectation("IsString", "is {not} a string",
		Result = nameof(DescribeValueKind),
		Summary = "Verifies that the subject <see cref=\"JsonElement\" /> is a <see cref=\"JsonValueKind.String\" />.")]
	private static bool IsStringKind([DisallowNull] JsonElement? element)
		=> element.Value.ValueKind == JsonValueKind.String;
}
