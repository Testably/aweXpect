using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableJsonElement
{
	/// <inheritdoc cref="ThatJsonElement.IsString(IThat{JsonElement}, Action{IThatSubject{string}})" />
	[GuaranteesNotNull]
	public static AndOrResult<JsonElement, IThat<JsonElement?>> IsString(this IThat<JsonElement?> subject,
		Action<IThatSubject<string>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return IsValue(subject, JsonValueReaders.String, expectations);
	}

	/// <inheritdoc cref="ThatJsonElement.IsString(IThat{JsonElement}, string)" />
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<JsonElement, IThat<JsonElement?>> IsString(
		this IThat<JsonElement?> subject, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		StringEqualityOptions options = new(nameof(expected));
		return new StringEqualityTypeResult<JsonElement, IThat<JsonElement?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options),
				static IAsyncContextConstraint<JsonElement?> (state, it, grammars)
					=> new IsJsonStringEqualToConstraint(it, grammars, state.Expected, state.Options)),
			subject,
			options);
	}

	[GenerateExpectation("IsString", "is {not} a string",
		Result = nameof(DescribeValueKind),
		Summary = "Verifies that the subject <see cref=\"JsonElement\" /> is a <see cref=\"JsonValueKind.String\" />.")]
	private static bool IsStringKind([DisallowNull] JsonElement? element)
		=> element.Value.ValueKind == JsonValueKind.String;
}
