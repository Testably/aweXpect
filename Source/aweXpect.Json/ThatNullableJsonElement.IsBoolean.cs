using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using aweXpect.Core.Extending;

namespace aweXpect;

public static partial class ThatNullableJsonElement
{
	[GenerateExpectation("IsBoolean", "is {not} a boolean",
		Result = nameof(DescribeValueKind),
		Summary = """
		          Verifies that the subject <see cref="JsonElement" /> is a boolean, i.e. <see cref="JsonValueKind.True" /> or
		          <see cref="JsonValueKind.False" />.
		          """)]
	private static bool IsBooleanKind([DisallowNull] JsonElement? element)
		=> element.Value.ValueKind is JsonValueKind.True or JsonValueKind.False;
}
