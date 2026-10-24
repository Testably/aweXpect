using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableJsonElement
{
	/// <inheritdoc cref="ThatJsonElement.IsNumber(IThat{JsonElement}, Action{IThatSubject{double}})" />
	[GuaranteesNotNull]
	public static AndOrResult<JsonElement, IThat<JsonElement?>> IsNumber(this IThat<JsonElement?> subject,
		Action<IThatSubject<double>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return IsValue(subject, JsonValueReaders.Number, expectations);
	}

	/// <inheritdoc cref="ThatJsonElement.IsNumber{TNumber}(IThat{JsonElement})" />
	[GuaranteesNotNull]
	public static AndOrResult<JsonElement, IThat<JsonElement?>> IsNumber<TNumber>(this IThat<JsonElement?> subject)
		=> IsValue(subject, JsonValueReaders.GetNumber<TNumber>(), null);

	/// <inheritdoc cref="ThatJsonElement.IsNumber{TNumber}(IThat{JsonElement}, Action{IThatSubject{TNumber}})" />
	[GuaranteesNotNull]
	public static AndOrResult<JsonElement, IThat<JsonElement?>> IsNumber<TNumber>(this IThat<JsonElement?> subject,
		Action<IThatSubject<TNumber>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return IsValue(subject, JsonValueReaders.GetNumber<TNumber>(), expectations);
	}

	[GenerateExpectation("IsNumber", "is {not} a number",
		Result = nameof(DescribeValueKind),
		Summary = "Verifies that the subject <see cref=\"JsonElement\" /> is a <see cref=\"JsonValueKind.Number\" />.")]
	private static bool IsNumberKind([DisallowNull] JsonElement? element)
		=> element.Value.ValueKind == JsonValueKind.Number;

	private static AndOrResult<JsonElement, IThat<JsonElement?>> IsValue<TValue>(IThat<JsonElement?> subject,
		JsonValueReader<TValue> reader, Action<IThatSubject<TValue>>? expectations)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint(reader, static IValueConstraint<JsonElement?> (reader, it, grammars)
			=> new IsJsonValueConstraint<TValue>(it, grammars, reader));
		if (expectations is not null)
		{
			JsonValueExpectations.AddOnValue(expectationBuilder, expectations);
		}

		return new AndOrResult<JsonElement, IThat<JsonElement?>>(expectationBuilder, subject);
	}
}
