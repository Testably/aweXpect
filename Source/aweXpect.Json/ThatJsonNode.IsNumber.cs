using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonNode
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is a <see cref="JsonValueKind.Number" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNode, IThat<TNode?>> IsNumber<TNode>(this IThat<TNode?> subject)
		where TNode : JsonNode
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(static (it, grammar) =>
				new JsonNodeConstraint<TNode>(
					new ThatJsonElement.IsValueKindConstraint(it, grammar, JsonValueKind.Number))),
			subject);

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
	///     <see langword="ulong" />, <see langword="float" />, <see langword="double" /> and <see langword="decimal" />.
	///     <para />
	///     The subject is a <see cref="JsonNode" /> of any type, so that the number type is the only type argument.
	/// </remarks>
	/// <exception cref="NotSupportedException">The <typeparamref name="TNumber" /> is not supported.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<JsonNode, IThat<JsonNode?>> IsNumber<TNumber>(this IThat<JsonNode?> subject)
		=> IsValue(subject, JsonValueReaders.GetNumber<TNumber>(), null);

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is a <see cref="JsonValueKind.Number" /> that fits into a
	///     <typeparamref name="TNumber" /> and whose value satisfies the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The supported number types are <see langword="byte" />, <see langword="sbyte" />, <see langword="short" />,
	///     <see langword="ushort" />, <see langword="int" />, <see langword="uint" />, <see langword="long" />,
	///     <see langword="ulong" />, <see langword="float" />, <see langword="double" /> and <see langword="decimal" />.
	///     <para />
	///     The subject is a <see cref="JsonNode" /> of any type, so that the number type is the only type argument.
	/// </remarks>
	/// <exception cref="NotSupportedException">The <typeparamref name="TNumber" /> is not supported.</exception>
	[GuaranteesNotNull]
	public static AndOrResult<JsonNode, IThat<JsonNode?>> IsNumber<TNumber>(this IThat<JsonNode?> subject,
		Action<IThatSubject<TNumber>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return IsValue(subject, JsonValueReaders.GetNumber<TNumber>(), expectations);
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
