using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Results;

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
	///     <see langword="ulong" />, <see langword="float" />, <see langword="double" /> and <see langword="decimal" />.
	/// </remarks>
	/// <exception cref="NotSupportedException">The <typeparamref name="TNumber" /> is not supported.</exception>
	public static AndOrResult<JsonElement, IThat<JsonElement>> IsNumber<TNumber>(this IThat<JsonElement> subject)
		=> IsValue(subject, JsonValueReaders.GetNumber<TNumber>(), null);

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is a <see cref="JsonValueKind.Number" /> that fits into a
	///     <typeparamref name="TNumber" /> and whose value satisfies the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The supported number types are <see langword="byte" />, <see langword="sbyte" />, <see langword="short" />,
	///     <see langword="ushort" />, <see langword="int" />, <see langword="uint" />, <see langword="long" />,
	///     <see langword="ulong" />, <see langword="float" />, <see langword="double" /> and <see langword="decimal" />.
	/// </remarks>
	/// <exception cref="NotSupportedException">The <typeparamref name="TNumber" /> is not supported.</exception>
	public static AndOrResult<JsonElement, IThat<JsonElement>> IsNumber<TNumber>(this IThat<JsonElement> subject,
		Action<IThatSubject<TNumber>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return IsValue(subject, JsonValueReaders.GetNumber<TNumber>(), expectations);
	}

	[GenerateExpectation("IsNumber", "is {not} a number",
		NullableOverload = false,
		Result = nameof(DescribeValueKind),
		Summary = "Verifies that the subject <see cref=\"JsonElement\" /> is a <see cref=\"JsonValueKind.Number\" />.")]
	private static bool IsNumberKind(JsonElement element)
		=> element.ValueKind == JsonValueKind.Number;

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
