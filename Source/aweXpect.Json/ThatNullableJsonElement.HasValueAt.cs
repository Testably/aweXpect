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
	/// <inheritdoc cref="ThatJsonElement.HasValueAt(IThat{JsonElement}, string, Action{IThatSubject{JsonElement}})" />
	[GuaranteesNotNull]
	public static JsonPropertyResult<JsonElement, IThat<JsonElement?>> HasValueAt(this IThat<JsonElement?> subject,
		string path,
		Action<IThatSubject<JsonElement>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		JsonValueTarget target = JsonValueTarget.ValueAt(path);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint(target,
			static IValueConstraint<JsonElement?> (target, it, grammars) => target.CreateConstraint(it, grammars));
		JsonValueExpectations.AddOnTarget(expectationBuilder, target, expectations);
		return new JsonPropertyResult<JsonElement, IThat<JsonElement?>>(expectationBuilder, subject, target);
	}
}
