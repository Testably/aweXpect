using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Options;

namespace aweXpect.Results;

/// <summary>
///     The comparison of a <see langword="string" /> property that an operator like <c>StartingWith(…)</c> passes to the
///     <c>Compare</c> method of a property result, e.g. of an <see cref="IStringEqualityPropertyResult{TResult}" />.
/// </summary>
/// <remarks>
///     The string options are set on the result after the operator returned it, e.g. <c>IgnoringCase()</c>, so the
///     comparison is only complete when the expectation is evaluated. Pass the <see cref="Options" />, and for
///     <c>Containing(…)</c> the <see cref="Quantifier" />, to the result you return, so that it offers these options,
///     and evaluate the comparison only in your constraint. It can be evaluated several times.
/// </remarks>
public sealed class StringPropertyComparison
{
	internal StringPropertyComparison(string? expected, StringEqualityOptions options, bool isNegated)
	{
		Expected = expected;
		Options = options;
		IsNegated = isNegated;
	}

	internal StringPropertyComparison(string expected, StringEqualityOptions options, Quantifier quantifier)
	{
		Expected = expected;
		Options = options;
		Quantifier = quantifier;
	}

	/// <summary>
	///     The string options of the comparison, which the result you return provides to the option methods, e.g. via
	///     <see cref="StringEqualityTypeResult{TType, TThat}" />.
	/// </summary>
	public StringEqualityOptions Options { get; }

	/// <summary>
	///     How often the expected value has to occur, which the result you return provides to the quantifiers, e.g. via
	///     <see cref="StringOccurrenceCountResult{TType, TThat}" />.
	/// </summary>
	/// <remarks>
	///     Only the comparison of <c>Containing(…)</c>, which is passed to an
	///     <see cref="IStringOccurrencePropertyResult{TResult}" />, has a quantifier.
	/// </remarks>
	public Quantifier? Quantifier { get; }

	internal string? Expected { get; }

	internal bool IsNegated { get; }

	/// <summary>
	///     Appends the comparison, e.g. <c>starting with "Let" ignoring case</c> or <c>containing "e" at least twice</c>,
	///     to the <paramref name="stringBuilder" />.
	/// </summary>
	/// <remarks>
	///     The text includes the negation of a negated operator, e.g. <c>not starting with "Let"</c> for
	///     <c>NotStartingWith("Let")</c>, which a <see cref="ExpectationGrammars.Negated" /> flag in the
	///     <paramref name="grammars" /> toggles. Without <see cref="ExpectationGrammars.Active" />, the comparison has no
	///     subject, e.g. <c>equal to "Let It Be"</c>; with it, it reads like a verb of the property, e.g.
	///     <c>is equal to "Let It Be"</c> or <c>starts with "Let"</c>.
	/// </remarks>
	public void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
		=> AppendExpectationCore(stringBuilder, IsNegated ? grammars.Negate() : grammars);

	/// <summary>
	///     Evaluates the comparison against the <paramref name="actual" /> value of the property in the
	///     <paramref name="context" /> of the evaluation.
	/// </summary>
	/// <remarks>
	///     The <see cref="StringPropertyOutcome.Outcome" /> includes the negation of a negated operator. A
	///     <see langword="null" /> value has no content that a part of it, e.g. a prefix, or an occurrence could be found
	///     in, so the outcome is then <see cref="Outcome.FailureBothWays" />.
	/// </remarks>
	/// <exception cref="System.ArgumentException">
	///     The expected value cannot be used with the options, e.g. an invalid regex pattern.
	/// </exception>
	public async ValueTask<StringPropertyOutcome> Evaluate(string? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		StringEqualityOptions options = Options.ForEvaluation(context, cancellationToken);
		if (Quantifier is null)
		{
			MatchResult matchResult = await options.MatchWithExplanation(actual, Expected);
			Outcome outcome = matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			if (actual is null && Options.InspectsSubject)
			{
				outcome = Outcome.FailureBothWays;
			}

			return new StringPropertyOutcome(this, outcome, actual, options, matchResult, default);
		}

		(int Count, bool IsFinished) occurrences =
			await StringOccurrences.Count(options, actual ?? string.Empty, Expected!, Quantifier);
		Outcome occurrenceOutcome = actual is null
			? Outcome.FailureBothWays
			: Quantifier.Check(occurrences.Count, true) == true
				? Outcome.Success
				: Outcome.Failure;
		return new StringPropertyOutcome(this, occurrenceOutcome, actual, options, default, occurrences);
	}

	/// <summary>
	///     Appends the comparison like <see cref="AppendExpectation(StringBuilder, ExpectationGrammars)" />, with the
	///     negation of the operator already in the <paramref name="grammars" />.
	/// </summary>
	internal void AppendExpectationCore(StringBuilder stringBuilder, ExpectationGrammars grammars)
	{
		if (Quantifier is null)
		{
			stringBuilder.Append(Options.GetExpectation(Expected, grammars));
			return;
		}

		bool isActive = grammars.IsActive();
		AppendOccurrenceExpectation(stringBuilder, grammars.IsNegated(),
			isActive ? grammars.Verb("contains", "contain") : "containing",
			isActive ? grammars.Verb("does not contain", "do not contain") : "not containing");
	}

	/// <summary>
	///     Appends the quantified occurrences with the <paramref name="verb" />, or the <paramref name="negatedVerb" />
	///     when the negation reads better on the verb.
	/// </summary>
	internal void AppendOccurrenceExpectation(StringBuilder stringBuilder, bool isNegated, string verb,
		string negatedVerb)
		=> StringOccurrences.AppendExpectation(stringBuilder, Quantifier!, isNegated, verb, negatedVerb, Expected!,
			Options);
}
