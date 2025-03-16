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
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Object" />.
	/// </summary>
	public static AndOrResult<JsonElement?, IThat<JsonElement?>> IsObject(
		this IThat<JsonElement?> source)
		=> new(
			source.Get().ExpectationBuilder.AddConstraint((it, grammar) =>
				new IsValueKindConstraint(it, grammar, JsonValueKind.Object)),
			source);

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Object" />
	///     whose value satisfies the <paramref name="expectation" />.
	/// </summary>
	public static AndOrResult<JsonElement?, IThat<JsonElement?>> IsObject(
		this IThat<JsonElement?> source,
		Func<IJsonObjectResult, IJsonObjectResult> expectation,
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
			source.Get().ExpectationBuilder.AddConstraint((it, grammar) =>
				new IsObjectConstraint(it, grammar, expectation, jsonOptions)),
			source);
	}

	private sealed class IsObjectConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<IJsonObjectResult, IJsonObjectResult> expectation,
		JsonOptions options)
		: ConstraintResult.WithNotNullValue<JsonElement?>(it, grammars),
			IValueConstraint<JsonElement?>
	{
		private JsonValidation? _jsonValidation;

		public ConstraintResult IsMetBy(JsonElement? actual)
		{
			Actual = actual;
			_jsonValidation = new JsonValidation(actual, JsonValueKind.Object, options);
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
