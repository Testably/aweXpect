using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Json;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatNullableJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Array" />.
	/// </summary>
	[GuaranteesNotNull]
	public static IsArrayResult<JsonElement, IThat<JsonElement?>> IsArray(
		this IThat<JsonElement?> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(static (it, grammar) =>
				new IsValueKindConstraint(it, grammar, JsonValueKind.Array)),
			subject,
			static element => element);

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Array" />
	///     whose value satisfies the <paramref name="expectations" />.
	/// </summary>
	[GuaranteesNotNull]
	public static IsArrayResult<JsonElement, IThat<JsonElement?>> IsArray(
		this IThat<JsonElement?> subject,
		Action<IJsonArrayResult> expectations,
		Func<JsonOptions, JsonOptions>? options = null)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		JsonOptions jsonOptions = new()
		{
			IgnoreAdditionalProperties = true,
		};
		if (options != null)
		{
			jsonOptions = options(jsonOptions);
		}

		return new IsArrayResult<JsonElement, IThat<JsonElement?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expectations: expectations, Options: jsonOptions),
				static (s, it, grammar) => new IsArrayConstraint(it, grammar, s.Expectations, s.Options)),
			subject,
			static element => element);
	}

	private sealed class IsArrayConstraint(
		string it,
		ExpectationGrammars grammars,
		Action<IJsonArrayResult> expectations,
		JsonOptions options)
		: ConstraintResult.WithNotNullValue<JsonElement?>(it, grammars),
			IAsyncContextConstraint<JsonElement?>
	{
		private JsonValidation? _jsonValidation;

		public async ValueTask<ConstraintResult> IsMetBy(JsonElement? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_jsonValidation = new JsonValidation(actual, JsonValueKind.Array, options, context);
			_jsonValidation.Invoke(expectations);
			await _jsonValidation.ValidateAsync(cancellationToken);

			Outcome = _jsonValidation.IsMet() ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (Grammars.IsNegated() && Outcome == Outcome.Failure && Actual is { } actual)
			{
				contexts.Add(new ResultContext.SyncCallback("Actual",
					() => Formatter.Format(actual, FormattingOptions.MultipleLines)));
			}

			options.AddContext(contexts, _jsonValidation?.UnmatchedMembersToIgnore);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> _jsonValidation?.GetExpectation(stringBuilder, Grammars);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(_jsonValidation?.GetFailure(It, Grammars, indentation));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> _jsonValidation?.GetExpectation(stringBuilder, Grammars);

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was", " were"));
	}
}
