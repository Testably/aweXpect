using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Json.Results;
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
		return new StringEqualityTypeResult<JsonElement, IThat<JsonElement?>>(
			subject.Get().ExpectationBuilder.AddConstraint(expected,
				static (expected, it, grammars) => new IsJsonStringEqualToConstraint(it, grammars, expected),
				out IsJsonStringEqualToConstraint constraint),
			subject,
			constraint);
	}

	/// <inheritdoc cref="ThatJsonElement.IsString(IThat{JsonElement})" />
	[GuaranteesNotNull]
	public static IsStringResult<JsonElement, IThat<JsonElement?>> IsString(this IThat<JsonElement?> subject)
	{
		JsonComparisonSlot<StringPropertyComparison> slot = new("IsString()");
		return new IsStringResult<JsonElement, IThat<JsonElement?>>(
			subject.Get().ExpectationBuilder.AddConstraint(slot,
				static IAsyncContextConstraint<JsonElement?> (slot, it, grammars)
					=> new IsJsonStringConstraint(it, grammars, slot)),
			subject,
			slot);
	}
}
