using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableJsonElement
{
	/// <inheritdoc cref="ThatJsonElement.IsBoolean(IThat{JsonElement}, Action{IThatSubject{bool}})" />
	[GuaranteesNotNull]
	public static AndOrResult<JsonElement, IThat<JsonElement?>> IsBoolean(this IThat<JsonElement?> subject,
		Action<IThatSubject<bool>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return IsValue(subject, JsonValueReaders.Boolean, expectations);
	}

	[GenerateExpectation("IsBoolean", "is {not} a boolean",
		Result = nameof(DescribeValueKind),
		Summary = """
		          Verifies that the subject <see cref="JsonElement" /> is a boolean, i.e. <see cref="JsonValueKind.True" /> or
		          <see cref="JsonValueKind.False" />.
		          """)]
	private static bool IsBooleanKind([DisallowNull] JsonElement? element)
		=> element.Value.ValueKind is JsonValueKind.True or JsonValueKind.False;
}
