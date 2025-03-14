using System;
using System.Text;
using System.Text.Json;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Array" />.
	/// </summary>
	public static AndOrResult<JsonElement?, IThat<JsonElement?>> IsArray(
		this IThat<JsonElement?> source)
		=> new(
			source.ThatIs().ExpectationBuilder.AddConstraint((it, grammar) =>
				new IsValueKindConstraint(it, grammar, JsonValueKind.Array)),
			source);

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Array" />
	///     whose value satisfies the <paramref name="expectation" />.
	/// </summary>
	public static AndOrResult<JsonElement?, IThat<JsonElement?>> IsArray(
		this IThat<JsonElement?> source,
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

		return new AndOrResult<JsonElement?, IThat<JsonElement?>>(
			source.ThatIs().ExpectationBuilder.AddConstraint((it, grammar) =>
				new IsArrayConstraint(it, grammar, expectation, jsonOptions)),
			source);
	}

	private sealed class IsArrayConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<IJsonArrayResult, IJsonArrayResult> expectation,
		JsonOptions options)
		: ConstraintResult.WithNotNullValue<JsonElement?>(it, grammars),
			IValueConstraint<JsonElement?>
	{
		private JsonValidation? _jsonValidation;

		public ConstraintResult IsMetBy(JsonElement? actual)
		{
			Actual = actual;
			_jsonValidation = new JsonValidation(actual, JsonValueKind.Array, options);
			expectation(_jsonValidation);

			Outcome = _jsonValidation.IsMet() ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> _jsonValidation?.GetExpectation(stringBuilder, Grammars);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_jsonValidation?.GetFailure(It));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> _jsonValidation?.GetExpectation(stringBuilder, Grammars);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was in ");
			Formatter.Format(stringBuilder, Actual, FormattingOptions.MultipleLines);
		}
	}
}
