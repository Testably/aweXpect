using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Json;

namespace aweXpect;

/// <summary>
///     Json expectations on <see langword="string" /> values.
/// </summary>
public static partial class ThatJsonString
{
	private sealed class MatchesJsonConstraint(
		string it,
		ExpectationGrammars grammars,
		object? expected,
		string expectedExpression,
		JsonOptions options)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IAsyncConstraint<string?>
	{
		private JsonElementValidator.JsonComparisonResult? _comparisonResult;

		public async Task<ConstraintResult> IsMetBy(string? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			using JsonDocument actualDocument = JsonDocument.Parse(
				actual, options.DocumentOptions);
#pragma warning disable CA1869
			JsonSerializerOptions serializerOptions = new(JsonSerializerOptions.Default);
#pragma warning restore CA1869
			ExpectationJsonConverter? converter = new();
			serializerOptions.Converters.Add(converter);
			using JsonDocument expectedDocument = JsonDocument.Parse(
				JsonSerializer.Serialize(expected, serializerOptions),
				options.DocumentOptions);

			_comparisonResult = await JsonElementValidator.Compare(
				actualDocument.RootElement,
				expectedDocument.RootElement,
				options,
				converter);

			Outcome = _comparisonResult.HasError ? Outcome.Failure : Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is valid JSON which matches ").Append(expectedExpression);
			if (!options.IgnoreAdditionalProperties)
			{
				stringBuilder.Append(" exactly");
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" differed as").Append(_comparisonResult);

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("is no valid JSON which matches ").Append(expectedExpression);
			if (!options.IgnoreAdditionalProperties)
			{
				stringBuilder.Append(" exactly");
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(It).Append(" was ");
			Formatter.Format(stringBuilder, Actual);
		}
	}
}
