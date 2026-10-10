using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Object" />.
	/// </summary>
	[GuaranteesNotNull]
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
	[GuaranteesNotNull]
	public static AndOrResult<JsonElement?, IThat<JsonElement?>> IsObject(
		this IThat<JsonElement?> source,
		Func<IJsonObjectResult, IJsonObjectResult> expectation,
		Func<JsonOptions, JsonOptions>? options = null)
	{
		ThrowHelper.ThrowIfNull(expectation, nameof(expectation));
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
			IAsyncContextConstraint<JsonElement?>
	{
		private JsonValidation? _jsonValidation;

		public async ValueTask<ConstraintResult> IsMetBy(JsonElement? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_jsonValidation = new JsonValidation(actual, JsonValueKind.Object, options, context);
			_jsonValidation.Invoke(expectation);
			await _jsonValidation.ValidateAsync(cancellationToken);

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
