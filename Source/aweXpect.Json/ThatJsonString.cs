using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
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
			IAsyncContextConstraint<string?>
	{
		private JsonElementValidator.JsonComparisonResult? _comparisonResult;
		private string? _parseError;

		public async ValueTask<ConstraintResult> IsMetBy(string? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			ExpectationJsonConverter converter = new(context, cancellationToken);
			using JsonDocument expectedDocument = converter.ParseExpected(expected, options.DocumentOptions);
			JsonDocument actualDocument;
			try
			{
				actualDocument = JsonDocument.Parse(actual, options.DocumentOptions);
			}
			catch (JsonException exception)
			{
				_parseError = exception.Message;
				Outcome = Outcome.FailureBothWays;
				return this;
			}

			using (actualDocument)
			{
				_comparisonResult = await JsonElementValidator.Compare(
					actualDocument.RootElement,
					expectedDocument.RootElement,
					options,
					converter);
			}

			Outcome = _comparisonResult.HasError ? Outcome.Failure : Outcome.Success;
			return this;
		}

		private bool TryAppendParseError(StringBuilder stringBuilder)
		{
			if (_parseError is null)
			{
				return false;
			}

			stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
			Formatter.Format(stringBuilder, Actual);
			stringBuilder.Append(", which could not be parsed as JSON: ").Append(_parseError);
			return true;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is valid JSON which matches ", "are valid JSON which matches "))
				.Append(expectedExpression.TrimCommonWhiteSpace());
			if (!options.IgnoreAdditionalProperties)
			{
				stringBuilder.Append(" exactly");
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!TryAppendParseError(stringBuilder))
			{
				stringBuilder.Append(It).Append(" differed as")
					.Append(_comparisonResult?.ToString().IndentFollowingLines(indentation));
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("is not valid JSON which matches ", "are not valid JSON which matches "))
				.Append(expectedExpression.TrimCommonWhiteSpace());
			if (!options.IgnoreAdditionalProperties)
			{
				stringBuilder.Append(" exactly");
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!TryAppendParseError(stringBuilder))
			{
				stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
				Formatter.Format(stringBuilder, Actual);
			}
		}
	}
}
