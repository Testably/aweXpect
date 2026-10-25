using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Json.Results;

namespace aweXpect;

public static partial class ThatNullableJsonElement
{
	/// <inheritdoc cref="ThatJsonElement.HasProperty(IThat{JsonElement}, string)" />
	[GuaranteesNotNull]
	public static JsonPropertyResult<JsonElement, IThat<JsonElement?>> HasProperty(this IThat<JsonElement?> subject,
		string name)
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		JsonValueTarget target = JsonValueTarget.Property(name);
		return new JsonPropertyResult<JsonElement, IThat<JsonElement?>>(
			subject.Get().ExpectationBuilder.AddConstraint(target,
				static IValueConstraint<JsonElement?> (target, it, grammars) => target.CreateConstraint(it, grammars)),
			subject, target);
	}

	/// <inheritdoc cref="ThatJsonElement.HasProperty(IThat{JsonElement}, string, Action{IThatSubject{JsonElement}})" />
	[GuaranteesNotNull]
	public static JsonPropertyResult<JsonElement, IThat<JsonElement?>> HasProperty(this IThat<JsonElement?> subject,
		string name,
		Action<IThatSubject<JsonElement>> expectations)
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		JsonValueTarget target = JsonValueTarget.Property(name);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint(target,
			static IValueConstraint<JsonElement?> (target, it, grammars) => target.CreateConstraint(it, grammars));
		JsonValueExpectations.AddOnTarget(expectationBuilder, target, expectations);
		return new JsonPropertyResult<JsonElement, IThat<JsonElement?>>(expectationBuilder, subject, target);
	}

	/// <inheritdoc cref="ThatJsonElement.DoesNotHaveProperty(IThat{JsonElement}, string)" />
	[GuaranteesNotNull]
	public static JsonPropertyResult<JsonElement, IThat<JsonElement?>> DoesNotHaveProperty(
		this IThat<JsonElement?> subject,
		string name)
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		JsonValueTarget target = JsonValueTarget.Property(name);
		return new JsonPropertyResult<JsonElement, IThat<JsonElement?>>(
			subject.Get().ExpectationBuilder.AddConstraint(target,
				static IValueConstraint<JsonElement?> (target, it, grammars)
					=> target.CreateConstraint(it, grammars).Invert()),
			subject, target);
	}
}
