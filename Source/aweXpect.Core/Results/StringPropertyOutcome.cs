using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The outcome of a <see cref="StringPropertyComparison" /> for one value of the property, which explains a failure.
/// </summary>
public readonly struct StringPropertyOutcome
{
	private readonly StringPropertyComparison? _comparison;
	private readonly string? _actual;
	private readonly MatchResult _matchResult;
	private readonly (int Count, bool IsFinished) _occurrences;

	internal StringPropertyOutcome(StringPropertyComparison comparison, Outcome outcome, string? actual,
		StringEqualityOptions evaluationOptions, MatchResult matchResult, (int Count, bool IsFinished) occurrences)
	{
		_comparison = comparison;
		OutcomeOfOperator = outcome;
		_actual = actual;
		EvaluationOptions = evaluationOptions;
		_matchResult = matchResult;
		_occurrences = occurrences;
	}

	/// <summary>
	///     The outcome of the comparison, including the negation of a negated operator, e.g. <c>NotStartingWith(…)</c>.
	/// </summary>
	public Outcome Outcome => _comparison?.IsNegated == true
		? OutcomeOfOperator switch
		{
			Outcome.Success => Outcome.Failure,
			Outcome.Failure => Outcome.Success,
			_ => OutcomeOfOperator,
		}
		: OutcomeOfOperator;

	/// <summary>
	///     The outcome of the comparison without the negation of a negated operator.
	/// </summary>
	internal Outcome OutcomeOfOperator { get; }

	/// <summary>
	///     The options with which the value was compared, or <see langword="null" /> before an evaluation.
	/// </summary>
	internal StringEqualityOptions? EvaluationOptions { get; }

	/// <summary>
	///     Appends the result for the value of the property, named <paramref name="it" />, to the
	///     <paramref name="stringBuilder" />, e.g. <c>it was "Yesterday", which differs at index 0: …</c> or
	///     <c>it contained "e" once in "Let It Be"</c>.
	/// </summary>
	/// <remarks>
	///     A <see cref="ExpectationGrammars.Negated" /> flag in the <paramref name="grammars" /> toggles the negation of
	///     a negated operator, like for <see cref="StringPropertyComparison.AppendExpectation" />. A text over several
	///     lines indents the lines after the first by the <paramref name="indentation" />.
	/// </remarks>
	public void AppendResult(StringBuilder stringBuilder, string it, ExpectationGrammars grammars,
		string? indentation = null)
		=> AppendResultCore(stringBuilder, it, _comparison?.IsNegated == true ? grammars.Negate() : grammars,
			indentation);

	/// <summary>
	///     Adds the contexts of the options, e.g. of a custom match type, to the <paramref name="contexts" />.
	/// </summary>
	public void AppendContexts(ResultContextCollector contexts)
		=> EvaluationOptions?.AppendContexts(contexts);

	/// <summary>
	///     Appends the result like <see cref="AppendResult" />, with the negation of the operator already in the
	///     <paramref name="grammars" />.
	/// </summary>
	internal void AppendResultCore(StringBuilder stringBuilder, string it, ExpectationGrammars grammars,
		string? indentation)
	{
		if (_comparison is null || EvaluationOptions is null)
		{
			return;
		}

		if (_comparison.Quantifier is null)
		{
			stringBuilder.Append(EvaluationOptions
				.GetExtendedFailure(it, grammars, _actual, _comparison.Expected, _matchResult)
				.Indent(indentation, false));
			return;
		}

		if (_actual is null)
		{
			stringBuilder.Append(it).Append(grammars.SubjectVerb(it, " was ", " were ")).Append("<null>");
			return;
		}

		StringOccurrences.AppendResult(stringBuilder, it, _comparison.Expected!, _actual, _occurrences,
			EvaluationOptions, indentation);
	}

	/// <summary>
	///     Appends the result for the value of the property <paramref name="member" /> of the subject named
	///     <paramref name="it" />, e.g. <c>it had message "foo", which differs …</c>, with the negation of the operator
	///     already in the <paramref name="grammars" />.
	/// </summary>
	internal void AppendMemberResultCore(StringBuilder stringBuilder, string it, string member,
		ExpectationGrammars grammars, string? indentation)
	{
		if (_comparison is null || EvaluationOptions is null)
		{
			return;
		}

		stringBuilder.Append(EvaluationOptions
			.GetExtendedMemberFailure(it, member, grammars, _actual, _comparison.Expected, _matchResult)
			.Indent(indentation, false));
	}
}
