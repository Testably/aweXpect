#if NET8_0_OR_GREATER
using System.Text.Json;
using aweXpect.Core.Constraints;
using aweXpect.Json;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="JsonElement" />? values.
/// </summary>
public static partial class ThatNullableJsonElement
{
	private readonly struct MatchesConstraint(
		string it,
		object? expected,
		string expectedExpression,
		JsonOptions options)
		: IValueConstraint<JsonElement?>
	{
		public ConstraintResult IsMetBy(JsonElement? actual)
		{
			if (actual == null)
			{
				return new ConstraintResult.Failure<JsonElement?>(actual, ToString(),
					$"{it} was <null>");
			}

			using JsonDocument expectedDocument =
				JsonDocument.Parse(JsonSerializer.Serialize(expected), options.DocumentOptions);

			JsonElementValidator.JsonComparisonResult comparisonResult = JsonElementValidator.Compare(
				actual.Value,
				expectedDocument.RootElement,
				options);

			if (comparisonResult.HasError)
			{
				return new ConstraintResult.Failure<JsonElement?>(actual, ToString(),
					$"{it} differed as{comparisonResult}");
			}

			return new ConstraintResult.Success<JsonElement?>(actual, ToString());
		}

		public override string ToString()
			=> options.IgnoreAdditionalProperties switch
			{
				true => $"matches {expectedExpression}",
				false => $"matches {expectedExpression} exactly",
			};
	}


	private readonly struct IsValueKindConstraint(string it, JsonValueKind expected)
		: IValueConstraint<JsonElement?>
	{
		public ConstraintResult IsMetBy(JsonElement? actual)
		{
			if (actual == null)
			{
				return new ConstraintResult.Failure<JsonElement?>(actual, ToString(),
					$"{it} was <null>");
			}

			if (actual.Value.ValueKind != expected)
			{
				return new ConstraintResult.Failure<JsonElement?>(actual, ToString(),
					$"{it} was {JsonValidation.Format(actual.Value.ValueKind)} instead of {JsonValidation.Format(expected)}");
			}

			return new ConstraintResult.Success<JsonElement?>(actual, ToString());
		}

		public override string ToString()
			=> $"is {JsonValidation.Format(expected)}";
	}
}
#endif
