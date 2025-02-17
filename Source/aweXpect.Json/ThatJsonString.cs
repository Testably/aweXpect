using System.Text.Json;
using aweXpect.Core.Constraints;
using aweXpect.Json;

namespace aweXpect;

/// <summary>
///     Json expectations on <see langword="string" /> values.
/// </summary>
public static partial class ThatJsonString
{
	private readonly struct MatchesJsonConstraint(
		string it,
		object? expected,
		string expectedExpression,
		JsonOptions options)
		: IValueConstraint<string?>
	{
		public ConstraintResult IsMetBy(string? actual)
		{
			if (actual is null)
			{
				return new ConstraintResult.Failure<string?>(null, ToString(), $"{it} was <null>");
			}

			using JsonDocument actualDocument = JsonDocument.Parse(
				actual, options.DocumentOptions);
			using JsonDocument expectedDocument = JsonDocument.Parse(
				JsonSerializer.Serialize(expected), options.DocumentOptions);

			JsonElementValidator.JsonComparisonResult comparisonResult = JsonElementValidator.Compare(
				actualDocument.RootElement,
				expectedDocument.RootElement,
				options);

			if (comparisonResult.HasError)
			{
				return new ConstraintResult.Failure<string?>(actual, ToString(),
					$"{it} differed as{comparisonResult}");
			}

			return new ConstraintResult.Success<string?>(actual, ToString());
		}

		public override string ToString()
			=> options.IgnoreAdditionalProperties switch
			{
				true => $"is valid JSON which matches {expectedExpression}",
				false => $"is valid JSON which matches {expectedExpression} exactly",
			};
	}
}
