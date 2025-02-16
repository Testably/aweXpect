using System;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Array" />.
	/// </summary>
	public static AndOrResult<JsonElement, IThat<JsonElement>> IsArray(this IThat<JsonElement> source)
		=> new(
			source.ThatIs().ExpectationBuilder.AddConstraint((it, grammar) =>
				new IsValueKindConstraint(it, JsonValueKind.Array)),
			source);

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Array" />
	///     whose value satisfies the <paramref name="expectation" />.
	/// </summary>
	public static AndOrResult<JsonElement, IThat<JsonElement>> IsArray(this IThat<JsonElement> source,
		Func<IJsonArrayResult, IJsonArrayResult> expectation,
		Func<JsonOptions, JsonOptions>? options = null)
	{
		JsonOptions jsonOptions = new()
		{
			IgnoreAdditionalProperties = true,
		};
		if (options != null)
		{
			jsonOptions = options(jsonOptions);
		}

		return new AndOrResult<JsonElement, IThat<JsonElement>>(
			source.ThatIs().ExpectationBuilder.AddConstraint((it, grammar) =>
				new IsArrayConstraint(it, expectation, jsonOptions)),
			source);
	}

	private readonly struct IsArrayConstraint(
		string it,
		Func<IJsonArrayResult, IJsonArrayResult> expectation,
		JsonOptions options)
		: IValueConstraint<JsonElement>
	{
		public ConstraintResult IsMetBy(JsonElement actual)
		{
			JsonValidation jsonValidation = new(actual, JsonValueKind.Array, options);
			expectation(jsonValidation);
			if (!jsonValidation.IsMet())
			{
				return new ConstraintResult.Failure<JsonElement>(actual, jsonValidation.GetExpectation(),
					jsonValidation.GetFailure(it));
			}

			return new ConstraintResult.Success<JsonElement>(actual, jsonValidation.GetExpectation());
		}
	}
}
