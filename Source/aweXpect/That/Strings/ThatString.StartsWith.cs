using System;
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

public static partial class ThatString
{
	/// <summary>
	///     Verifies that the subject starts with the <paramref name="expected" /> <see langword="string" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityResult<string, IThat<string?>> StartsWith(
		this IThat<string?> subject,
		string expected)
	{
		expected.ThrowIfNull();
		if (expected == string.Empty)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException("The 'expected' string cannot be empty.", nameof(expected)));
		}

		StringEqualityOptions options = StringEqualityOptions.Default.AsPrefix();
		return new StringEqualityResult<string, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Expected: expected, Options: options),
				static (state, it, grammars) =>
					new StartsWithConstraint(it, grammars, state.Expected, "expected", state.Options),
				out StartsWithConstraint constraint),
			subject,
			constraint);
	}

	/// <summary>
	///     Verifies that the subject does not start with the <paramref name="unexpected" /> <see langword="string" />.
	/// </summary>
	[GuaranteesNotNull]
	public static StringEqualityResult<string, IThat<string?>> DoesNotStartWith(
		this IThat<string?> subject,
		string unexpected)
	{
		unexpected.ThrowIfNull();
		if (unexpected == string.Empty)
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException("The 'unexpected' string cannot be empty.", nameof(unexpected)));
		}

		StringEqualityOptions options = StringEqualityOptions.Default.AsPrefix();
		return new StringEqualityResult<string, IThat<string?>>(
			subject.Get().ExpectationBuilder.AddConstraint((Unexpected: unexpected, Options: options),
				static (state, it, grammars) =>
					new StartsWithConstraint(it, grammars, state.Unexpected, "unexpected", state.Options).Invert(),
				out StartsWithConstraint constraint),
			subject,
			constraint);
	}

	private sealed class StartsWithConstraint(
		string it,
		ExpectationGrammars grammars,
		string? expected,
		string parameterName,
		StringEqualityOptions options)
		: ConstraintResult.WithNotNullValue<string?>(it, grammars),
			IAsyncContextConstraint<string?>,
			IOptionsHolderSource<StringEqualityOptions>
	{
		private OptionsHolder<StringEqualityOptions>? _optionsHolder;

		private StringEqualityOptions CurrentOptions => _optionsHolder is null ? options : _optionsHolder.Value;

		/// <inheritdoc cref="IOptionsHolderSource{TOptions}.Holder" />
		OptionsHolder<StringEqualityOptions> IOptionsHolderSource<StringEqualityOptions>.Holder
			=> _optionsHolder ??= new OptionsHolder<StringEqualityOptions>(options);

		private StringEqualityOptions? _evaluationOptions;
		private MatchResult _matchResult;

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
				_evaluationOptions = CurrentOptions.ForEvaluation(context, cancellationToken);
				ValueTask<MatchResult> match = _evaluationOptions.MatchWithExplanation(actual, expected, parameterName);
				if (match.IsCompletedSuccessfully)
				{
					return new ValueTask<ConstraintResult>(Complete(match.Result));
				}

				return CompleteAsync(match);
			}
			catch (Exception exception)
			{
				return new ValueTask<ConstraintResult>(Task.FromException<ConstraintResult>(exception));
			}
		}

		private async ValueTask<ConstraintResult> CompleteAsync(ValueTask<MatchResult> match)
			=> Complete(await match);

		private StartsWithConstraint Complete(MatchResult matchResult)
		{
			_matchResult = matchResult;
			Outcome = _matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			return this;
		}

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
		{
			if (!string.IsNullOrEmpty(Actual))
			{
				contexts.AddStringContext("Actual", Actual, this);
				contexts.AddStringContext("Expected", expected, this);
			}

			(_evaluationOptions ?? CurrentOptions).AppendContexts(contexts);
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(CurrentOptions.GetExpectation(expected, Grammars | ExpectationGrammars.Active));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append((_evaluationOptions ?? CurrentOptions).GetExtendedFailure(It, Grammars, Actual, expected, _matchResult)
				.Indent(indentation, false));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(CurrentOptions.GetExpectation(expected, Grammars | ExpectationGrammars.Active));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append((_evaluationOptions ?? CurrentOptions).GetExtendedFailure(It, Grammars, Actual, expected, _matchResult)
				.Indent(indentation, false));
	}
}
