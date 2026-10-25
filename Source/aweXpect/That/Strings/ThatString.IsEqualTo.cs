using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatString
{
	/// <summary>
	///     Verifies that the subject is equal to the <paramref name="expected" /> value.
	/// </summary>
	public static StringEqualityTypeResult<string?, IThat<string?>> IsEqualTo(
		this IThat<string?> subject,
		string? expected)
	{
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<string?, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options),
				static (state, it, grammars) =>
					new IsEqualToConstraint(it, grammars, state.Expected, "expected", state.Options)),
			subject,
			options);
	}

	/// <summary>
	///     Verifies that the subject is not equal to the <paramref name="unexpected" /> value.
	/// </summary>
	public static StringEqualityTypeResult<string?, IThat<string?>> IsNotEqualTo(
		this IThat<string?> subject,
		string? unexpected)
	{
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringEqualityTypeResult<string?, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Options: options),
				static (state, it, grammars) =>
					new IsEqualToConstraint(it, grammars, state.Unexpected, "unexpected", state.Options).Invert()),
			subject,
			options);
	}

	private sealed class IsEqualToConstraint(
		string it,
		ExpectationGrammars grammars,
		string? expected,
		string parameterName,
		OptionsHolder<StringEqualityOptions> options)
		: ConstraintResult.WithEqualToValue<string?>(it, grammars, expected is null),
			IAsyncContextConstraint<string?>
	{
		private StringEqualityOptions? _evaluationOptions;
		private MatchResult _matchResult;

		/// <remarks>
		///     A match type that inspects the content of the subject, e.g. a prefix or a pattern, cannot answer for a
		///     <see langword="null" /> subject, because it has no content.
		/// </remarks>
		/// <remarks>
		///     Most comparisons complete synchronously, so the result is returned without a state machine; an exception is
		///     returned in the task, like from an asynchronous method.
		/// </remarks>
		public ValueTask<ConstraintResult> IsMetBy(string? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			try
			{
				Actual = actual;
				_evaluationOptions = options.Value.ForEvaluation(context, cancellationToken);
				ValueTask<MatchResult> match = _evaluationOptions.MatchWithExplanation(actual, expected, parameterName);
				return match.IsCompletedSuccessfully
					? new ValueTask<ConstraintResult>(Complete(match.Result))
					: CompleteAsync(match);
			}
			catch (Exception exception)
			{
				return new ValueTask<ConstraintResult>(Task.FromException<ConstraintResult>(exception));
			}
		}

		private async ValueTask<ConstraintResult> CompleteAsync(ValueTask<MatchResult> match)
			=> Complete(await match);

		private ConstraintResult Complete(MatchResult matchResult)
		{
			_matchResult = matchResult;
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			if (Actual is null && options.Value.InspectsSubject)
			{
				Outcome = Outcome.FailureBothWays;
			}

			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (!string.IsNullOrEmpty(Actual))
			{
				contexts.AddStringDifferencesContext(Actual, expected, _evaluationOptions ?? options.Value, Grammars);
				contexts.AddStringContext("Actual", Actual, this);
				contexts.AddStringContext("Expected", expected, this);
			}

			(_evaluationOptions ?? options.Value).AppendContexts(contexts);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.Value.GetExpectation(expected, Grammars | ExpectationGrammars.Active));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append((_evaluationOptions ?? options.Value).GetExtendedFailure(It, Grammars, Actual, expected, _matchResult)
				.Indent(indentation, false));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(options.Value.GetExpectation(expected, Grammars | ExpectationGrammars.Active));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append((_evaluationOptions ?? options.Value).GetExtendedFailure(It, Grammars, Actual, expected, _matchResult)
				.Indent(indentation, false));
	}
}
