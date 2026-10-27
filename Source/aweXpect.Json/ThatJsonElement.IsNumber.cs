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

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is a <see cref="JsonValueKind.Number" /> whose value as
	///     <see langword="double" /> satisfies the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     A number that does not fit into a <see langword="double" /> fails the expectation. Use
	///     <see cref="IsNumber{TNumber}(IThat{JsonElement}, Action{IThatSubject{TNumber}})" /> for another number type.
	/// </remarks>
	public static AndOrResult<JsonElement, IThat<JsonElement>> IsNumber(this IThat<JsonElement> subject,
		Action<IThatSubject<double>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return IsValue(subject, JsonValueReaders.Number, expectations);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is a <see cref="JsonValueKind.Number" /> that fits into a
	///     <typeparamref name="TNumber" />, e.g. an integer within the range of an <see langword="int" />.
	/// </summary>
	/// <remarks>
	///     The supported number types are <see langword="byte" />, <see langword="sbyte" />, <see langword="short" />,
	///     <see langword="ushort" />, <see langword="int" />, <see langword="uint" />, <see langword="long" />,
	///     <see langword="ulong" />, <see cref="System.Numerics.BigInteger" />, <see langword="float" />,
	///     <see langword="double" /> and <see langword="decimal" />, and on .NET 8 or later also <see langword="nint" />,
	///     <see langword="nuint" />, <c>Int128</c>, <c>UInt128</c> and <c>Half</c>.
	/// </remarks>
	/// <exception cref="NotSupportedException">The <typeparamref name="TNumber" /> is not supported.</exception>
	public static AndOrResult<JsonElement, IThat<JsonElement>> IsNumber<TNumber>(this IThat<JsonElement> subject)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
		=> IsValue(subject, JsonValueReaders.GetNumber<TNumber>(), null);

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is a <see cref="JsonValueKind.Number" /> that fits into a
	///     <typeparamref name="TNumber" /> and whose value satisfies the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The supported number types are <see langword="byte" />, <see langword="sbyte" />, <see langword="short" />,
	///     <see langword="ushort" />, <see langword="int" />, <see langword="uint" />, <see langword="long" />,
	///     <see langword="ulong" />, <see cref="System.Numerics.BigInteger" />, <see langword="float" />,
	///     <see langword="double" /> and <see langword="decimal" />, and on .NET 8 or later also <see langword="nint" />,
	///     <see langword="nuint" />, <c>Int128</c>, <c>UInt128</c> and <c>Half</c>.
	/// </remarks>
	/// <exception cref="NotSupportedException">The <typeparamref name="TNumber" /> is not supported.</exception>
	public static AndOrResult<JsonElement, IThat<JsonElement>> IsNumber<TNumber>(this IThat<JsonElement> subject,
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

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is a <see cref="JsonValueKind.Number" /> that fits into a
	///     <typeparamref name="TNumber" /> and is equal to the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     The number type follows the <paramref name="expected" /> value, so <c>IsNumber(42)</c> reads the number as
	///     <see langword="int" /> and fails for <c>42.5</c> or <c>42.0</c>, while <c>IsNumber(42.0)</c> or
	///     <c>IsNumber&lt;double&gt;(42)</c> compare it as <see langword="double" />. A tolerance is specified on the
	///     result, e.g. <c>Within(0.5)</c>. A value of another kind, or a number that does not fit into a
	///     <typeparamref name="TNumber" />, fails the expectation, but satisfies its negation.
	///     <para />
	///     The supported number types are <see langword="byte" />, <see langword="sbyte" />, <see langword="short" />,
	///     <see langword="ushort" />, <see langword="int" />, <see langword="uint" />, <see langword="long" />,
	///     <see langword="ulong" />, <see cref="System.Numerics.BigInteger" />, <see langword="float" />,
	///     <see langword="double" /> and <see langword="decimal" />, and on .NET 8 or later also <see langword="nint" />,
	///     <see langword="nuint" />, <c>Int128</c>, <c>UInt128</c> and <c>Half</c>.
	/// </remarks>
	/// <exception cref="NotSupportedException">The <typeparamref name="TNumber" /> is not supported.</exception>
	public static NumberToleranceResult<JsonElement, IThat<JsonElement>, TNumber> IsNumber<TNumber>(
		this IThat<JsonElement> subject, TNumber expected)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		JsonValueReader<TNumber> reader = JsonValueReaders.GetNumber<TNumber>();
		return new NumberToleranceResult<JsonElement, IThat<JsonElement>, TNumber>(
			subject.Get().ExpectationBuilder.AddConstraint((Reader: reader, Expected: expected),
				static (state, it, grammars)
					=> new IsJsonNumberEqualToConstraint<TNumber>(it, grammars, state.Reader, state.Expected),
				out IsJsonNumberEqualToConstraint<TNumber> constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is a <see cref="JsonValueKind.Number" />.
	/// </summary>
	/// <remarks>
	///     The value can be compared as <see langword="double" /> on the result in the same expectation, e.g. with
	///     <c>GreaterThan(3)</c>, <c>Between(1).And(5)</c> or <c>EqualTo(4.5).Within(0.1)</c>; a number that does not fit
	///     into a <see langword="double" /> then fails. A value of another kind fails the expectation, but satisfies its
	///     negation.
	/// </remarks>
	public static IsNumberResult<JsonElement, IThat<JsonElement>> IsNumber(this IThat<JsonElement> subject)
	{
		JsonComparisonSlot<JsonNumberComparison> slot = new("IsNumber()");
		return new IsNumberResult<JsonElement, IThat<JsonElement>>(
			subject.Get().ExpectationBuilder.AddConstraint(slot,
				static IAsyncContextConstraint<JsonElement> (slot, it, grammars)
					=> new IsJsonNumberConstraint(it, grammars, slot)),
			subject,
			slot);
	}

	private static AndOrResult<JsonElement, IThat<JsonElement>> IsValue<TValue>(IThat<JsonElement> subject,
		JsonValueReader<TValue> reader, Action<IThatSubject<TValue>>? expectations)
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint(reader, static IValueConstraint<JsonElement> (reader, it, grammars)
			=> new IsJsonValueConstraint<TValue>(it, grammars, reader));
		if (expectations is not null)
		{
			JsonValueExpectations.AddOnValue(expectationBuilder, expectations);
		}

		return new AndOrResult<JsonElement, IThat<JsonElement>>(expectationBuilder, subject);
	}
}
