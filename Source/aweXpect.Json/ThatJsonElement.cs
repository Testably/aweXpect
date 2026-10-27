using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Json;
using aweXpect.Json.Options;

namespace aweXpect;

/// <summary>
///     Expectations on <see cref="JsonElement" /> values.
/// </summary>
public static partial class ThatJsonElement
{
	internal static string DescribeValueKind(JsonElement element, string it, ExpectationGrammars grammars)
		=> it + grammars.SubjectVerb(it, " was ", " were ") + JsonValidation.Format(element.ValueKind);

	/// <summary>
	///     Adds the indented <paramref name="actual" /> value as the "Actual" context, as the failure message shows a value
	///     only in a single line and shortened, if at all.
	/// </summary>
	/// <remarks>
	///     An undefined element, e.g. a <see langword="default" /> <see cref="JsonElement" />, cannot be written.
	/// </remarks>
	internal static void AddActualContext(ResultContextCollector contexts, JsonElement? actual)
	{
		if (actual is { ValueKind: not JsonValueKind.Undefined, } value)
		{
			contexts.Add(new ResultContext.SyncCallback("Actual",
				() => Formatter.Format(value, FormattingOptions.MultipleLines)));
		}
	}

	/// <summary>
	///     The JSON path of the subject named <paramref name="it" />, which continues the path of a value that an
	///     enclosing expectation like <c>HasProperty</c> found, and is <c>$</c> otherwise.
	/// </summary>
	internal static string GetPath(string it)
		=> it.StartsWith("$.", StringComparison.Ordinal) || it.StartsWith("$[", StringComparison.Ordinal) ? it : "$";

	internal sealed class MatchesConstraint(
		string it,
		ExpectationGrammars grammars,
		object? expected,
		string expectedExpression,
		JsonOptions options)
		: ConstraintResult.WithNotNullValue<JsonElement?>(it, grammars),
			IAsyncContextConstraint<JsonElement>,
			IAsyncContextConstraint<JsonElement?>
	{
		private JsonElementValidator.JsonComparisonResult? _comparisonResult;

		public ValueTask<ConstraintResult> IsMetBy(JsonElement actual, IEvaluationContext context,
			CancellationToken cancellationToken)
			=> IsMetBy((JsonElement?)actual, context, cancellationToken);

		public async ValueTask<ConstraintResult> IsMetBy(JsonElement? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_comparisonResult = null;
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			ExpectationJsonConverter converter = new(context, cancellationToken);
			using JsonDocument expectedDocument = converter.ParseExpected(expected, options.DocumentOptions,
				options.ExpectedSerializerOptions);
			if (actual.Value.ValueKind == JsonValueKind.Undefined)
			{
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			_comparisonResult = await JsonElementValidator.Compare(
				GetPath(It),
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

			options.AddContext(contexts, _comparisonResult?.GetUnmatchedMembersToIgnore());
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("matches ", "match ")).Append(expectedExpression.TrimCommonWhiteSpace());
			if (!options.IgnoreAdditionalProperties && !options.IgnoreAdditionalArrayElements)
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
			if (!options.IgnoreAdditionalProperties && !options.IgnoreAdditionalArrayElements)
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


	/// <summary>
	///     Verifies the kind of a <see cref="JsonElement" /> for the expectations that cannot be generated, e.g. on a
	///     <see cref="System.Text.Json.Nodes.JsonNode" />, with the same messages as the generated ones, e.g.
	///     <see cref="IsBoolean(IThat{JsonElement})" />.
	/// </summary>
	internal sealed class IsValueKindConstraint(string it, ExpectationGrammars grammars, JsonValueKind expected)
		: ConstraintResult.WithNotNullValue<JsonElement?>(it, grammars),
			IValueConstraint<JsonElement>,
			IValueConstraint<JsonElement?>
	{
		public ConstraintResult IsMetBy(JsonElement actual)
			=> IsMetBy((JsonElement?)actual);

		public ConstraintResult IsMetBy(JsonElement? actual)
		{
			Actual = actual;
			Outcome = actual is not null && JsonValidation.IsKind(actual.Value.ValueKind, expected)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is ", "are "))
				.Append(JsonValidation.FormatExpected(expected, Grammars & ~ExpectationGrammars.Plural));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(DescribeValueKind(Actual!.Value, It, Grammars));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("is ", "are "))
				.Append(JsonValidation.FormatExpected(expected, Grammars & ~ExpectationGrammars.Plural));
	}
}
