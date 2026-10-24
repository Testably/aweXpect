using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Json.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Object" />.
	/// </summary>
	[GuaranteesNotNull]
	public static IsObjectResult<JsonElement, IThat<JsonElement?>> IsObject(
		this IThat<JsonElement?> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(static IValueConstraint<JsonElement?> (it, grammar) =>
				new ThatJsonElement.IsValueKindConstraint(it, grammar, JsonValueKind.Object)),
			subject,
			static element => element);

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Object" />
	///     whose value satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static IsObjectResult<JsonElement, IThat<JsonElement?>> IsObject(
		this IThat<JsonElement?> subject,
		Action<IJsonObjectResult> expectations,
		Func<JsonOptions, JsonOptions>? options = null)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		JsonOptions jsonOptions = new()
		{
			IgnoreAdditionalProperties = true,
		};
		if (options != null)
		{
			jsonOptions = options(jsonOptions);
		}

		return new IsObjectResult<JsonElement, IThat<JsonElement?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expectations: expectations, Options: jsonOptions),
				static IAsyncContextConstraint<JsonElement?> (s, it, grammar) =>
					new ThatJsonElement.IsObjectConstraint(it, grammar, s.Expectations, s.Options)),
			subject,
			static element => element);
	}
}
