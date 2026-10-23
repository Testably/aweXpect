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
using aweXpect.Json.Results;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatJsonElement
{
	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Object" />.
	/// </summary>
	public static IsObjectResult<JsonElement, IThat<JsonElement>> IsObject(
		this IThat<JsonElement> subject)
		=> new(
			subject.Get().ExpectationBuilder.AddConstraint(static (it, grammar) =>
				new IsValueKindConstraint(it, grammar, JsonValueKind.Object)),
			subject,
			static element => element);

	/// <summary>
	///     Verifies that the subject <see cref="JsonElement" /> is an <see cref="JsonValueKind.Object" />
	///     whose value satisfies the <paramref name="expectations" />.
	/// </summary>
	public static IsObjectResult<JsonElement, IThat<JsonElement>> IsObject(
		this IThat<JsonElement> subject,
		Action<IJsonObjectResult> expectations,
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

		return new IsObjectResult<JsonElement, IThat<JsonElement>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expectations: expectations, Options: jsonOptions),
				static (s, it, grammar) => new IsObjectConstraint(it, grammar, s.Expectations, s.Options)),
			subject,
			static element => element);
	}

	internal sealed class IsObjectConstraint(
		string it,
		ExpectationGrammars grammars,
		Action<IJsonObjectResult> expectations,
		JsonOptions options)
		: ConstraintResult.WithValue<JsonElement>(it, grammars),
			IAsyncContextConstraint<JsonElement>
	{
		private JsonValidation? _jsonValidation;

		public async ValueTask<ConstraintResult> IsMetBy(JsonElement actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			_jsonValidation = new JsonValidation(actual, JsonValueKind.Object, options, context);
			_jsonValidation.Invoke(expectations);
			await _jsonValidation.ValidateAsync(cancellationToken);

			Outcome = _jsonValidation.IsMet() ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (Grammars.IsNegated() && Outcome == Outcome.Failure)
			{
				JsonElement actual = Actual;
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
