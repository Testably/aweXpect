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
	internal StringPropertyComparison(string? expected, OptionsSlot<StringEqualityOptions> options, bool isNegated)
	{
		Expected = expected;
		Options = options;
		IsNegated = isNegated;
	}

	internal StringPropertyComparison(string expected, OptionsSlot<StringEqualityOptions> options,
		OptionsSlot<Quantifier> quantifier)
	{
		Expected = expected;
		Options = options;
		Quantifier = quantifier;
	}

	/// <summary>
	///     The slot with the string options of the comparison, which the result you return provides to the option
	///     methods, e.g. via
	///     <see cref="StringEqualityTypeResult{TType, TThat}" />.
	/// </summary>
	public OptionsSlot<StringEqualityOptions> Options { get; }

	/// <summary>
	///     The slot with how often the expected value has to occur, which the result you return provides to the
	///     quantifiers, e.g. via
	///     <see cref="StringOccurrenceCountResult{TType, TThat}" />.
	/// </summary>
	/// <remarks>
	///     Only the comparison of <c>Containing(…)</c>, which is passed to an
	///     <see cref="IStringOccurrencePropertyResult{TResult}" />, has a quantifier.
	/// </remarks>
	public OptionsSlot<Quantifier>? Quantifier { get; }

	internal string? Expected { get; }

	/// <summary>
	///     The name of the parameter of the operator that received the <see cref="Expected" /> value.
	/// </summary>
	private string ParameterName => IsNegated ? "unexpected" : "expected";

	/// <summary>
	///     Flag indicating that the operator is a negated one, e.g. <c>NotStartingWith(…)</c>.
	/// </summary>
	/// <remarks>
	///     <see cref="StringPropertyOutcome.Outcome" /> of <see cref="Evaluate" /> already includes the negation, but
	///     <see cref="AppendExpectation(StringBuilder, ExpectationGrammars)" /> and
	///     <see cref="StringPropertyOutcome.AppendResult" /> do not, so that the "not" can be placed where the sentence
	///     needs it, e.g. "has title not starting with "Let"" or "does not have title starting with "Let"".
	/// </remarks>
	public bool IsNegated { get; }

	/// <summary>
	///     Appends the comparison without its negation, e.g. <c>starting with "Let" ignoring case</c> or
	///     <c>containing "e" at least twice</c>, to the <paramref name="stringBuilder" />.
	/// </summary>
	/// <remarks>
	///     A <see cref="ExpectationGrammars.Negated" /> flag in the <paramref name="grammars" /> writes the negation into
	///     the comparison, e.g. <c>not starting with "Let"</c>. Without <see cref="ExpectationGrammars.Active" />, the
	///     comparison has no subject, e.g. <c>equal to "Let It Be"</c>; with it, it reads like a verb of the property,
	///     e.g. <c>is equal to "Let It Be"</c> or <c>does not start with "Let"</c>.
	/// </remarks>
	public void AppendExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars)
	{
		if (Quantifier is null)
		{
			stringBuilder.Append(Options.Value.GetExpectation(Expected, grammars));
			return;
		}

		bool isActive = grammars.IsActive();
		AppendOccurrenceExpectation(stringBuilder, grammars.IsNegated(),
			isActive ? grammars.Verb("contains", "contain") : "containing",
			isActive ? grammars.Verb("does not contain", "do not contain") : "not containing");
	}

	/// <summary>
	///     Verifies that the expected value can be used with the options, e.g. that it is a valid regex pattern.
	/// </summary>
	/// <remarks>
	///     Call it before looking at the subject, so that an unusable expected value is rejected whichever value the
	///     property has, also when it is not evaluated, e.g. because the subject is <see langword="null" />.
	/// </remarks>
	/// <exception cref="System.ArgumentException">
	///     The expected value cannot be used with the options, e.g. an invalid regex pattern.
	/// </exception>
	public void ValidateExpected() => Options.Value.ValidateExpected(Expected, ParameterName);

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
		StringEqualityOptions options = Options.Value.ForEvaluation(context, cancellationToken);
		if (Quantifier is null)
		{
			MatchResult matchResult = await options.MatchWithExplanation(actual, Expected, ParameterName);
			Outcome outcome = matchResult.IsMatch ? Outcome.Success : Outcome.Failure;
			if (actual is null && Options.Value.InspectsSubject)
			{
				outcome = Outcome.FailureBothWays;
			}

			return new StringPropertyOutcome(this, outcome, actual, options, matchResult, default);
		}

		if (actual is null)
		{
			ValidateExpected();
			return new StringPropertyOutcome(this, Outcome.FailureBothWays, actual, options, default, default);
		}

		(int Count, bool IsFinished) occurrences =
			await StringOccurrences.Count(options, actual, Expected!, Quantifier.Value, ParameterName);
		Outcome occurrenceOutcome = Quantifier.Value.Check(occurrences.Count, true) == true ? Outcome.Success : Outcome.Failure;
		return new StringPropertyOutcome(this, occurrenceOutcome, actual, options, default, occurrences);
	}

	/// <summary>
	///     Appends the quantified occurrences with the <paramref name="verb" />, or the <paramref name="negatedVerb" />
	///     when the negation reads better on the verb.
	/// </summary>
	internal void AppendOccurrenceExpectation(StringBuilder stringBuilder, bool isNegated, string verb,
		string negatedVerb)
		=> StringOccurrences.AppendExpectation(stringBuilder, Quantifier!.Value, isNegated, verb, negatedVerb, Expected!,
			Options.Value);
}
