using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatHttpResponseMessage
{
	/// <summary>
	///     Verifies that the string content is equal to <paramref name="expected" />
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>
		HasContent(this IThat<HttpResponseMessage?> subject, string expected)
	{
		ThrowHelper.ThrowIfNull(expected, nameof(expected));
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint((expected, options), static (state, it, grammars) =>
					new HasContentConstraint(it, grammars, state.expected, "expected", state.options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the string content satisfies the <paramref name="expectations" />
	/// </summary>
	[GuaranteesNotNull]
	public static AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>>
		HasContent(this IThat<HttpResponseMessage?> subject, Action<IThatSubject<string?>> expectations)
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		return new AndOrResult<HttpResponseMessage, IThat<HttpResponseMessage?>>(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.ForAsyncMember(MemberAccessor<HttpResponseMessage, Task<string?>>.FromFuncWithName(
						async (m, _, cancellationToken) => await m.Content.ReadAsStringOrEmptyAsync(cancellationToken),
						" the string content"),
					(_, stringBuilder) => stringBuilder.Append("has a string content which "))
				.AddExpectations(expectations, static (expectations, e) => expectations(new ThatSubject<string?>(e)),
					grammars => grammars | ExpectationGrammars.Nested),
			subject);
	}

	/// <summary>
	///     Verifies that the string content is not equal to <paramref name="unexpected" />
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>
		DoesNotHaveContent(this IThat<HttpResponseMessage?> subject, string unexpected)
	{
		ThrowHelper.ThrowIfNull(unexpected, nameof(unexpected));
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<HttpResponseMessage, IThat<HttpResponseMessage?>>(
			subject.Get().ExpectationBuilder
				.AddSubjectContexts(ResultContextExtensions.ResponseContexts)
				.AddConstraint((unexpected, options), static (state, it, grammars) =>
					new HasContentConstraint(it, grammars, state.unexpected, "unexpected", state.options).Invert()),
			subject,
			options);
	}

	private sealed class HasContentConstraint(
		string it,
		ExpectationGrammars grammars,
		string expected,
		string parameterName,
		OptionsHolder<StringEqualityOptions> options)
		: ConstraintResult.WithNotNullValue<HttpResponseMessage>(it, grammars),
			IAsyncContextConstraint<HttpResponseMessage>
	{
		private StringEqualityOptions? _evaluationOptions;
		private MatchResult _matchResult;
		private string? _message;

		public async ValueTask<ConstraintResult> IsMetBy(
			HttpResponseMessage? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual == null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_message = await actual.Content.ReadAsStringOrEmptyAsync(cancellationToken);
			_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
			_matchResult = await _evaluationOptions.MatchWithExplanation(_message, expected, parameterName);
			if (_matchResult.IsMatch)
			{
				Outcome = Outcome.Success;
				return this;
			}

			Outcome = Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> (_evaluationOptions ?? options.Value).AppendContexts(contexts);

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("has a string content ", "have a string content "))
				.Append(options.Value.GetExpectation(expected, ExpectationGrammars.None));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append((_evaluationOptions ?? options.Value).GetExtendedMemberFailure(It, "string content",
				Grammars, _message, expected, _matchResult));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(Grammars.Verb("does not have a string content ", "do not have a string content "))
				.Append(options.Value.GetExpectation(expected, ExpectationGrammars.None));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had");
	}
}
