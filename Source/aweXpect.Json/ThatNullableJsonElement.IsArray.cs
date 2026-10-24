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
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Array" />.
	/// </summary>
	[GuaranteesNotNull]
	public static IsArrayResult<JsonElement, IThat<JsonElement?>> IsArray(
		this IThat<JsonElement?> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(static IValueConstraint<JsonElement?> (it, grammar) =>
				new ThatJsonElement.IsValueKindConstraint(it, grammar, JsonValueKind.Array)),
			subject,
			static element => element);

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Array" />
	///     whose value satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static IsArrayResult<JsonElement, IThat<JsonElement?>> IsArray(
		this IThat<JsonElement?> subject,
		Action<IJsonArrayResult> expectations,
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

		return new IsArrayResult<JsonElement, IThat<JsonElement?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expectations: expectations, Options: jsonOptions),
				static IAsyncContextConstraint<JsonElement?> (s, it, grammar) =>
					new ThatJsonElement.IsArrayConstraint(it, grammar, s.Expectations, s.Options)),
			subject,
			static element => element);
	}
}
