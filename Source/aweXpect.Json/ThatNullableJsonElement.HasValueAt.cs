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

public static partial class ThatNullableJsonElement
{
	/// <inheritdoc cref="ThatJsonElement.HasValueAt(IThat{JsonElement}, string)" />
	[GuaranteesNotNull]
	public static JsonPropertyResult<JsonElement, IThat<JsonElement?>> HasValueAt(this IThat<JsonElement?> subject,
		string path)
	{
		JsonValueTarget target = JsonValueTarget.ValueAt(path);
		OptionsHolder<JsonLookupOptions> options = new(JsonLookupOptions.Default);
		return new JsonPropertyResult<JsonElement, IThat<JsonElement?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Target: target, Options: options),
				static IValueConstraint<JsonElement?> (state, it, grammars)
					=> state.Target.CreateConstraint(it, grammars, state.Options)),
			subject, options);
	}

	/// <inheritdoc cref="ThatJsonElement.HasValueAt(IThat{JsonElement}, string, Action{IThatSubject{JsonElement}})" />
	[GuaranteesNotNull]
	public static JsonPropertyResult<JsonElement, IThat<JsonElement?>> HasValueAt(this IThat<JsonElement?> subject,
		string path,
		Action<IThatSubject<JsonElement>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		JsonValueTarget target = JsonValueTarget.ValueAt(path);
		OptionsHolder<JsonLookupOptions> options = new(JsonLookupOptions.Default);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint((Target: target, Options: options),
			static IValueConstraint<JsonElement?> (state, it, grammars)
				=> state.Target.CreateConstraint(it, grammars, state.Options));
		JsonValueExpectations.AddOnTarget(expectationBuilder, target, expectations);
		return new JsonPropertyResult<JsonElement, IThat<JsonElement?>>(expectationBuilder, subject, options);
	}

	/// <inheritdoc cref="ThatJsonElement.DoesNotHaveValueAt(IThat{JsonElement}, string)" />
	[GuaranteesNotNull]
	public static JsonPropertyResult<JsonElement, IThat<JsonElement?>> DoesNotHaveValueAt(
		this IThat<JsonElement?> subject,
		string path)
	{
		JsonValueTarget target = JsonValueTarget.ValueAt(path);
		OptionsHolder<JsonLookupOptions> options = new(JsonLookupOptions.Default);
		return new JsonPropertyResult<JsonElement, IThat<JsonElement?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Target: target, Options: options),
				static IValueConstraint<JsonElement?> (state, it, grammars)
					=> state.Target.CreateConstraint(it, grammars, state.Options).Invert()),
			subject, options);
	}
}
