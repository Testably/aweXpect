using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
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
		: ConstraintResult.WithNotNullValue<JsonElement?>(it, grammars),
			IAsyncContextConstraint<JsonElement?>
	{
		private JsonElementValidator.JsonComparisonResult? _comparisonResult;

		public async ValueTask<ConstraintResult> IsMetBy(JsonElement? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			ExpectationJsonConverter converter = new(context, cancellationToken);
			using JsonDocument expectedDocument = converter.ParseExpected(expected, options.DocumentOptions);
			if (actual.Value.ValueKind == JsonValueKind.Undefined)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_comparisonResult = await JsonElementValidator.Compare(
				actual.Value,
				expectedDocument.RootElement,
				options,
				converter);

			Outcome = _comparisonResult.HasError ? Outcome.Failure : Outcome.Success;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (Grammars.IsNegated() && Outcome == Outcome.Failure && Actual is { } actual)
			{
				contexts.Add(new ResultContext.SyncCallback("Actual",
					() => Formatter.Format(actual, FormattingOptions.MultipleLines)));
			}
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("matches ", "match ")).Append(expectedExpression.TrimCommonWhiteSpace());
			if (!options.IgnoreAdditionalProperties)
			{
				stringBuilder.Append(" exactly");
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual?.ValueKind == JsonValueKind.Undefined)
			{
				stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were")).Append(" undefined");
			}
			else
			{
				stringBuilder.Append(It).Append(" differed as")
					.Append(_comparisonResult?.ToString().Indent(indentation, false));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not match ", "do not match "))
				.Append(expectedExpression.TrimCommonWhiteSpace());
			if (!options.IgnoreAdditionalProperties)
			{
				stringBuilder.Append(" exactly");
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual?.ValueKind == JsonValueKind.Undefined)
			{
				stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were")).Append(" undefined");
			}
			else
			{
				stringBuilder.Append(It).Append(" did");
			}
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
			=> stringBuilder.Append(Grammars.Verb("is ", "are ")).Append(JsonValidation.Format(expected, Grammars));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "))
				.Append(JsonValidation.Format(Actual!.Value.ValueKind));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is ", "are ")).Append(JsonValidation.Format(expected, Grammars));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were"));
	}
}
