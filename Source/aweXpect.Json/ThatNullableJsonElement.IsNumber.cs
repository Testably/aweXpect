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
#if NET8_0_OR_GREATER
using System.Numerics;
#endif

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
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
		=> IsValue(subject, JsonValueReaders.GetNumber<TNumber>(), null);

	/// <inheritdoc cref="ThatJsonElement.IsNumber{TNumber}(IThat{JsonElement}, Action{IThatSubject{TNumber}})" />
	[GuaranteesNotNull]
	public static AndOrResult<JsonElement, IThat<JsonElement?>> IsNumber<TNumber>(this IThat<JsonElement?> subject,
		Action<IThatSubject<TNumber>> expectations)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return IsValue(subject, JsonValueReaders.GetNumber<TNumber>(), expectations);
	}

	/// <inheritdoc cref="ThatJsonElement.IsNumber{TNumber}(IThat{JsonElement}, TNumber)" />
	[GuaranteesNotNull]
	public static NumberToleranceResult<JsonElement, IThat<JsonElement?>, TNumber> IsNumber<TNumber>(
		this IThat<JsonElement?> subject, TNumber expected)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		JsonValueReader<TNumber> reader = JsonValueReaders.GetNumber<TNumber>();
		return new NumberToleranceResult<JsonElement, IThat<JsonElement?>, TNumber>(
			subject.Get().ExpectationBuilder.AddConstraint((Reader: reader, Expected: expected),
				static (state, it, grammars)
					=> new IsJsonNumberEqualToConstraint<TNumber>(it, grammars, state.Reader, state.Expected),
				out IsJsonNumberEqualToConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	/// <inheritdoc cref="ThatJsonElement.IsNumber(IThat{JsonElement})" />
	[GuaranteesNotNull]
	public static IsNumberResult<JsonElement, IThat<JsonElement?>> IsNumber(this IThat<JsonElement?> subject)
	{
		JsonComparisonSlot<JsonNumberComparison> slot = new("IsNumber()");
		return new IsNumberResult<JsonElement, IThat<JsonElement?>>(
			subject.Get().ExpectationBuilder.AddConstraint(slot,
				static IAsyncContextConstraint<JsonElement?> (slot, it, grammars)
					=> new IsJsonNumberConstraint(it, grammars, slot)),
			subject,
			slot);
	}

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
