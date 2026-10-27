using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;
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

public static partial class ThatJsonNode
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is a <see cref="JsonValueKind.Number" />.
	/// </summary>
	/// <remarks>
	///     The value can be compared as <see langword="double" /> on the result in the same expectation, e.g. with
	///     <c>GreaterThan(3)</c>, <c>Between(1).And(5)</c> or <c>EqualTo(4.5).Within(0.1)</c>; a number that does not fit
	///     into a <see langword="double" /> then fails. A value of another kind fails the expectation, but satisfies its
	///     negation.
	/// </remarks>
	[GuaranteesNotNull]
	public static IsNumberResult<TNode, IThat<TNode?>> IsNumber<TNode>(this IThat<TNode?> subject)
		where TNode : JsonNode
	{
		JsonComparisonSlot<JsonNumberComparison> slot = new("IsNumber()");
		return new IsNumberResult<TNode, IThat<TNode?>>(
			subject.Get().ExpectationBuilder.AddConstraint(slot, static (slot, it, grammars) =>
				new JsonNodeConstraint<TNode>(new IsJsonNumberConstraint(it, grammars, slot))),
			subject,
			slot);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is a <see cref="JsonValueKind.Number" /> whose value as
	///     <see langword="double" /> satisfies the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     A number that does not fit into a <see langword="double" /> fails the expectation. Use
	///     <see cref="IsNumber{TNumber}(IThat{JsonNode}, Action{IThatSubject{TNumber}})" /> for another number type.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<TNode, IThat<TNode?>> IsNumber<TNode>(this IThat<TNode?> subject,
		Action<IThatSubject<double>> expectations)
		where TNode : JsonNode
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return IsValue(subject, JsonValueReaders.Number, expectations);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is a <see cref="JsonValueKind.Number" /> that fits into a
	///     <typeparamref name="TNumber" />, e.g. an integer within the range of an <see langword="int" />.
	/// </summary>
	/// <remarks>
	///     The supported number types are <see langword="byte" />, <see langword="sbyte" />, <see langword="short" />,
	///     <see langword="ushort" />, <see langword="int" />, <see langword="uint" />, <see langword="long" />,
	///     <see langword="ulong" />, <see cref="System.Numerics.BigInteger" />, <see langword="float" />,
	///     <see langword="double" /> and <see langword="decimal" />, and on .NET 8 or later also <see langword="nint" />,
	///     <see langword="nuint" />, <c>Int128</c>, <c>UInt128</c> and <c>Half</c>.
	///     <para />
	///     The subject is a <see cref="JsonNode" /> of any type, so that the number type is the only type argument; a
	///     <see cref="JsonValue" /> subject has an overload of its own, which keeps its type.
	/// </remarks>
	/// <exception cref="NotSupportedException">The <typeparamref name="TNumber" /> is not supported.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<JsonNode, IThat<JsonNode?>> IsNumber<TNumber>(this IThat<JsonNode?> subject)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
		=> IsValue(subject, JsonValueReaders.GetNumber<TNumber>(), null);

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is a <see cref="JsonValueKind.Number" /> that fits into a
	///     <typeparamref name="TNumber" /> and whose value satisfies the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The supported number types are <see langword="byte" />, <see langword="sbyte" />, <see langword="short" />,
	///     <see langword="ushort" />, <see langword="int" />, <see langword="uint" />, <see langword="long" />,
	///     <see langword="ulong" />, <see cref="System.Numerics.BigInteger" />, <see langword="float" />,
	///     <see langword="double" /> and <see langword="decimal" />, and on .NET 8 or later also <see langword="nint" />,
	///     <see langword="nuint" />, <c>Int128</c>, <c>UInt128</c> and <c>Half</c>.
	///     <para />
	///     The subject is a <see cref="JsonNode" /> of any type, so that the number type is the only type argument; a
	///     <see cref="JsonValue" /> subject has an overload of its own, which keeps its type.
	/// </remarks>
	/// <exception cref="NotSupportedException">The <typeparamref name="TNumber" /> is not supported.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<JsonNode, IThat<JsonNode?>> IsNumber<TNumber>(this IThat<JsonNode?> subject,
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
	///     Verifies that the subject <see cref="JsonNode" /> is a <see cref="JsonValueKind.Number" /> that fits into a
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
	///     <para />
	///     The subject is a <see cref="JsonNode" /> of any type, so that the number type is the only type argument; a
	///     <see cref="JsonValue" /> subject has an overload of its own, which keeps its type.
	/// </remarks>
	/// <exception cref="NotSupportedException">The <typeparamref name="TNumber" /> is not supported.</exception>
	[GuaranteesNotNull]
	public static NumberToleranceResult<JsonNode, IThat<JsonNode?>, TNumber> IsNumber<TNumber>(
		this IThat<JsonNode?> subject, TNumber expected)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
		=> IsNumberEqualTo(subject, expected);

	/// <inheritdoc cref="IsNumber{TNumber}(IThat{JsonNode}, TNumber)" />
	/// <remarks>
	///     The number type follows the <paramref name="expected" /> value, so <c>IsNumber(42)</c> reads the number as
	///     <see langword="int" />. This overload keeps the type of the subject, e.g. a <see cref="JsonObject" />, while
	///     <see cref="IsNumber{TNumber}(IThat{JsonNode}, TNumber)" /> applies when the number type is specified.
	/// </remarks>
	/// <exception cref="NotSupportedException">The <typeparamref name="TNumber" /> is not supported.</exception>
	[GuaranteesNotNull]
	public static NumberToleranceResult<TNode, IThat<TNode?>, TNumber> IsNumber<TNode, TNumber>(
		this IThat<TNode?> subject, TNumber expected)
		where TNode : JsonNode
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
		=> IsNumberEqualTo(subject, expected);

	/// <inheritdoc cref="IsNumber{TNumber}(IThat{JsonNode})" />
	/// <remarks>
	///     This overload keeps the type of a <see cref="JsonValue" /> subject, as the number type is the only type
	///     argument.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<JsonValue, IThat<JsonValue?>> IsNumber<TNumber>(this IThat<JsonValue?> subject)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
		=> IsValue(subject, JsonValueReaders.GetNumber<TNumber>(), null);

	/// <inheritdoc cref="IsNumber{TNumber}(IThat{JsonNode}, Action{IThatSubject{TNumber}})" />
	/// <remarks>
	///     This overload keeps the type of a <see cref="JsonValue" /> subject, as the number type is the only type
	///     argument.
	/// </remarks>
	[GuaranteesNotNull]
	public static AndOrResult<JsonValue, IThat<JsonValue?>> IsNumber<TNumber>(this IThat<JsonValue?> subject,
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

	/// <inheritdoc cref="IsNumber{TNumber}(IThat{JsonNode}, TNumber)" />
	/// <remarks>
	///     This overload keeps the type of a <see cref="JsonValue" /> subject, also when the number type is specified,
	///     e.g. <c>IsNumber&lt;double&gt;(42)</c>.
	/// </remarks>
	[GuaranteesNotNull]
	public static NumberToleranceResult<JsonValue, IThat<JsonValue?>, TNumber> IsNumber<TNumber>(
		this IThat<JsonValue?> subject, TNumber expected)
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
		=> IsNumberEqualTo(subject, expected);

	private static NumberToleranceResult<TNode, IThat<TNode?>, TNumber> IsNumberEqualTo<TNode, TNumber>(
		IThat<TNode?> subject, TNumber expected)
		where TNode : JsonNode
#if NET8_0_OR_GREATER
		where TNumber : struct, INumber<TNumber>
#else
		where TNumber : struct, IComparable<TNumber>
#endif
	{
		JsonValueReader<TNumber> reader = JsonValueReaders.GetNumber<TNumber>();
		OptionsHolder<NumberTolerance<TNumber>> options = new(JsonValueReaders.CreateNumberTolerance<TNumber>());
		return new NumberToleranceResult<TNode, IThat<TNode?>, TNumber>(
			subject.Get().ExpectationBuilder.AddConstraint((Reader: reader, Expected: expected, Options: options),
				static (state, it, grammars) => new JsonNodeConstraint<TNode>(
					new IsJsonNumberEqualToConstraint<TNumber>(it, grammars, state.Reader, state.Expected,
						state.Options))),
			subject,
			options);
	}

	/// <remarks>
	///     The value is read from the <see cref="JsonElement" /> that the node represents, so that a number fits a type
	///     regardless of the .NET type that a <see cref="JsonValue" /> holds, like for a <see cref="JsonElement" />.
	/// </remarks>
	private static AndOrResult<TNode, IThat<TNode?>> IsValue<TNode, TValue>(IThat<TNode?> subject,
		JsonValueReader<TValue> reader, Action<IThatSubject<TValue>>? expectations)
		where TNode : JsonNode
	{
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint(reader, static (reader, it, grammars)
			=> new JsonNodeConstraint<TNode>(new IsJsonValueConstraint<TValue>(it, grammars, reader)));
		if (expectations is not null)
		{
			JsonValueExpectations.AddOnValue(expectationBuilder, expectations);
		}

		return new AndOrResult<TNode, IThat<TNode?>>(expectationBuilder, subject);
	}
}
