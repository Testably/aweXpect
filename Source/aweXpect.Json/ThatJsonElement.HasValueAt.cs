using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Json.Options;
using aweXpect.Json.Results;

namespace aweXpect;

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject has a value at the <paramref name="path" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="path" /> is a property name, followed by any number of <c>.name</c> or zero-based
	///     <c>[index]</c> segments, e.g. <c>"customer.address.city"</c> or <c>"items[0].sku"</c>, and is written without a
	///     leading <c>$</c>. Wildcards, filters and recursive descent are not supported, and property names that contain
	///     <c>.</c>, <c>[</c> or <c>]</c> can only be addressed with <c>HasProperty</c>.
	///     <para />
	///     Only the first segment that cannot be reached is reported. A property with the value <see langword="null" />
	///     exists, unless <see cref="JsonPropertyResult{TType,TThat}.TreatingNullAsAbsent(bool)" /> is set.
	/// </remarks>
	public static JsonPropertyResult<JsonElement, IThat<JsonElement>> HasValueAt(this IThat<JsonElement> subject,
		string path)
	{
		JsonValueTarget target = JsonValueTarget.ValueAt(path);
		OptionsHolder<JsonLookupOptions> options = new(JsonLookupOptions.Default);
		return new JsonPropertyResult<JsonElement, IThat<JsonElement>>(
			subject.Get().ExpectationBuilder.AddConstraint((Target: target, Options: options),
				static IValueConstraint<JsonElement> (state, it, grammars)
					=> state.Target.CreateConstraint(it, grammars, state.Options)),
			subject, options);
	}

	/// <summary>
	///     Verifies that the subject has a value at the <paramref name="path" /> which satisfies the
	///     <paramref name="expectations" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="path" /> is a property name, followed by any number of <c>.name</c> or zero-based
	///     <c>[index]</c> segments, e.g. <c>"customer.address.city"</c> or <c>"items[0].sku"</c>, and is written without a
	///     leading <c>$</c>. Wildcards, filters and recursive descent are not supported, and property names that contain
	///     <c>.</c>, <c>[</c> or <c>]</c> can only be addressed with <c>HasProperty</c>.
	///     <para />
	///     Only the first segment that cannot be reached is reported, and the <paramref name="expectations" /> are only
	///     verified, when the value exists.
	/// </remarks>
	public static JsonPropertyResult<JsonElement, IThat<JsonElement>> HasValueAt(this IThat<JsonElement> subject,
		string path,
		Action<IThatSubject<JsonElement>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		JsonValueTarget target = JsonValueTarget.ValueAt(path);
		OptionsHolder<JsonLookupOptions> options = new(JsonLookupOptions.Default);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint((Target: target, Options: options),
			static IValueConstraint<JsonElement> (state, it, grammars)
				=> state.Target.CreateConstraint(it, grammars, state.Options));
		JsonValueExpectations.AddOnTarget(expectationBuilder, target, expectations);
		return new JsonPropertyResult<JsonElement, IThat<JsonElement>>(expectationBuilder, subject, options);
	}

	/// <summary>
	///     Verifies that the subject has no value at the <paramref name="path" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="path" /> has the format of
	///     <see cref="HasValueAt(IThat{JsonElement}, string)" />. A value on the way that has another kind than the next
	///     segment requires fails the expectation, as the path cannot be looked up in it, while a missing property or
	///     index on the way satisfies it. A property with the value <see langword="null" /> exists and fails the
	///     expectation, unless <see cref="JsonPropertyResult{TType,TThat}.TreatingNullAsAbsent(bool)" /> is set.
	/// </remarks>
	public static JsonPropertyResult<JsonElement, IThat<JsonElement>> DoesNotHaveValueAt(
		this IThat<JsonElement> subject,
		string path)
	{
		JsonValueTarget target = JsonValueTarget.ValueAt(path);
		OptionsHolder<JsonLookupOptions> options = new(JsonLookupOptions.Default);
		return new JsonPropertyResult<JsonElement, IThat<JsonElement>>(
			subject.Get().ExpectationBuilder.AddConstraint((Target: target, Options: options),
				static IValueConstraint<JsonElement> (state, it, grammars)
					=> state.Target.CreateConstraint(it, grammars, state.Options).Invert()),
			subject, options);
	}
}
