using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Json.Options;
using aweXpect.Json.Results;

namespace aweXpect;

public static partial class ThatJsonNode
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is an <see cref="JsonValueKind.Object" /> with a property with
	///     the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="name" /> is taken literally, so that it can contain <c>.</c>, <c>[</c> or <c>]</c>. A property
	///     with the value <see langword="null" /> exists, unless
	///     <see cref="JsonPropertyResult{TType,TThat}.TreatingNullAsAbsent(bool)" /> is set.
	/// </remarks>
	[GuaranteesNotNull]
	public static JsonPropertyResult<TNode, IThat<TNode?>> HasProperty<TNode>(this IThat<TNode?> subject,
		string name)
		where TNode : JsonNode
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		JsonValueTarget target = JsonValueTarget.Property(name);
		OptionsHolder<JsonLookupOptions> options = new(JsonLookupOptions.Default);
		return new JsonPropertyResult<TNode, IThat<TNode?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Target: target, Options: options),
				static (state, it, grammars)
					=> new JsonNodeConstraint<TNode>(state.Target.CreateConstraint(it, grammars, state.Options))),
			subject, options);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is an <see cref="JsonValueKind.Object" /> with a property with
	///     the <paramref name="name" /> whose value satisfies the <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="name" /> is taken literally, so that it can contain <c>.</c>, <c>[</c> or <c>]</c>. The
	///     <paramref name="expectations" /> are only verified, when the property exists, and receive the property value,
	///     which is <see langword="null" /> for the JSON value <c>null</c>.
	/// </remarks>
	[GuaranteesNotNull]
	public static JsonPropertyResult<TNode, IThat<TNode?>> HasProperty<TNode>(this IThat<TNode?> subject,
		string name,
		Action<IThatSubject<JsonNode?>> expectations)
		where TNode : JsonNode
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		JsonValueTarget target = JsonValueTarget.Property(name);
		OptionsHolder<JsonLookupOptions> options = new(JsonLookupOptions.Default);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint((Target: target, Options: options),
			static (state, it, grammars)
				=> new JsonNodeConstraint<TNode>(state.Target.CreateConstraint(it, grammars, state.Options)));
		JsonValueExpectations.AddOnTarget(expectationBuilder, target, expectations);
		return new JsonPropertyResult<TNode, IThat<TNode?>>(expectationBuilder, subject, options);
	}

	/// <summary>
	///     Verifies that the subject <see cref="JsonNode" /> is an <see cref="JsonValueKind.Object" /> without a property
	///     with the <paramref name="name" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="name" /> is taken literally. A property with the value <see langword="null" /> exists and
	///     fails the expectation, unless <see cref="JsonPropertyResult{TType,TThat}.TreatingNullAsAbsent(bool)" /> is set.
	/// </remarks>
	[GuaranteesNotNull]
	public static JsonPropertyResult<TNode, IThat<TNode?>> DoesNotHaveProperty<TNode>(this IThat<TNode?> subject,
		string name)
		where TNode : JsonNode
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		JsonValueTarget target = JsonValueTarget.Property(name);
		OptionsHolder<JsonLookupOptions> options = new(JsonLookupOptions.Default);
		return new JsonPropertyResult<TNode, IThat<TNode?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Target: target, Options: options),
				static (state, it, grammars)
					=> new JsonNodeConstraint<TNode>(state.Target.CreateConstraint(it, grammars, state.Options).Invert())),
			subject, options);
	}
}
