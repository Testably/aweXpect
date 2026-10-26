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

namespace aweXpect;

public static partial class ThatJsonNode
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is a <see cref="JsonValueKind.String" />.
	/// </summary>
	/// <remarks>
	///     The value can be compared on the result in the same expectation, e.g. with <c>StartingWith("Let")</c>,
	///     <c>EqualTo("Let It Be").IgnoringCase()</c> or <c>Containing("e").AtLeast(2)</c>. A value of another kind fails
	///     the expectation, but satisfies its negation.
	/// </remarks>
	[GuaranteesNotNull]
	public static IsStringResult<TNode, IThat<TNode?>> IsString<TNode>(this IThat<TNode?> subject)
		where TNode : JsonNode
	{
		JsonComparisonSlot<StringPropertyComparison> slot = new("IsString()");
		return new IsStringResult<TNode, IThat<TNode?>>(
			subject.Get().ExpectationBuilder.AddConstraint(slot, static (slot, it, grammars) =>
				new JsonNodeConstraint<TNode>(new IsJsonStringConstraint(it, grammars, slot))),
			subject,
			slot);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is a <see cref="JsonValueKind.String" /> whose value
	///     satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<TNode, IThat<TNode?>> IsString<TNode>(this IThat<TNode?> subject,
		Action<IThatSubject<string>> expectations)
		where TNode : JsonNode
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return IsValue(subject, JsonValueReaders.String, expectations);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is a <see cref="JsonValueKind.String" /> equal to the
	///     <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     The match type and the other string options are specified on the result, e.g. <c>AsPrefix()</c> or
	///     <c>IgnoringCase()</c>. A value of another kind fails the expectation, but satisfies its negation.
	/// </remarks>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<TNode, IThat<TNode?>> IsString<TNode>(this IThat<TNode?> subject,
		string expected)
		where TNode : JsonNode
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<TNode, IThat<TNode?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options),
				static (state, it, grammars) => new JsonNodeConstraint<TNode>(
					new IsJsonStringEqualToConstraint(it, grammars, state.Expected, state.Options))),
			subject,
			options);
	}
}
