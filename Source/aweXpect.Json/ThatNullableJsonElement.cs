using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Helpers;
using aweXpect.Json;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="JsonElement" />? values.
/// </summary>
public static partial class ThatNullableJsonElement
{
	private sealed class MatchesConstraint(
		string it,
		ExpectationGrammars grammars,
		object? expected,
		string expectedExpression,
		JsonOptions options)
		: ConstraintResult.WithValue<JsonElement?>(grammars),
			IAsyncConstraint<JsonElement?>
	{
		private JsonElementValidator.JsonComparisonResult? _comparisonResult;

		public async Task<ConstraintResult> IsMetBy(JsonElement? actual, CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

#pragma warning disable CA1869
			JsonSerializerOptions serializerOptions = new(JsonSerializerOptions.Default);
#pragma warning restore CA1869
			ExpectationJsonConverter? converter = new();
			serializerOptions.Converters.Add(converter);
			using JsonDocument expectedDocument =
				JsonDocument.Parse(JsonSerializer.Serialize(expected, serializerOptions), options.DocumentOptions);

			_comparisonResult = await JsonElementValidator.Compare(
				actual.Value,
				expectedDocument.RootElement,
				options,
				converter);

			Outcome = _comparisonResult.HasError ? Outcome.Failure : Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("matches ").Append(expectedExpression.TrimCommonWhiteSpace());
			if (!options.IgnoreAdditionalProperties)
			{
				stringBuilder.Append(" exactly");
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual == null)
			{
				stringBuilder.Append(it).Append(" was <null>");
				return;
			}

			stringBuilder.Append(it).Append(" differed as").Append(_comparisonResult);
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("does not match ").Append(expectedExpression.TrimCommonWhiteSpace());
			if (!options.IgnoreAdditionalProperties)
			{
				stringBuilder.Append(" exactly");
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(it).Append(" did match in ");
			Formatter.Format(stringBuilder, Actual);
		}
	}


	private sealed class IsValueKindConstraint(string it, ExpectationGrammars grammars, JsonValueKind expected)
		: ConstraintResult.WithNotNullValue<JsonElement?>(it, grammars),
			IValueConstraint<JsonElement?>
	{
		public ConstraintResult IsMetBy(JsonElement? actual)
		{
			Actual = actual;
			Outcome = actual is not null && actual.Value.ValueKind == expected ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is ").Append(JsonValidation.Format(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was ").Append(JsonValidation.Format(Actual!.Value.ValueKind))
				.Append(" instead of ").Append(JsonValidation.Format(expected));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("is ").Append(JsonValidation.Format(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" was");
	}
}
