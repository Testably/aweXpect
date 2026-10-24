using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is a <see cref="JsonValueKind.String" /> whose value
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	public static AndOrResult<JsonElement, IThat<JsonElement>> IsString(this IThat<JsonElement> subject,
		Action<IThatSubject<string>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return IsValue(subject, JsonValueReaders.String, expectations);
	}

	[GenerateExpectation("IsString", "is {not} a string",
		NullableOverload = false,
		Result = nameof(DescribeValueKind),
		Summary = "Verifies that the subject <see cref=\"JsonElement\" /> is a <see cref=\"JsonValueKind.String\" />.")]
	private static bool IsStringKind(JsonElement element)
		=> element.ValueKind == JsonValueKind.String;
}
