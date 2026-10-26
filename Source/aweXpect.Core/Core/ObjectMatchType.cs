using System;
using System.Threading;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Options;

namespace aweXpect.Core;

/// <summary>
///     Defines how a <typeparamref name="TSubject" /> is compared: configured once, and asked for an
///     <see cref="ObjectMatcher{TSubject}" /> for each evaluation.
/// </summary>
/// <remarks>
///     The match type is the immutable value of the object comparison of an expectation, held in its
///     <see cref="OptionsHolder{TOptions}" />. Set it with <c>SetMatchType</c> on the holder, which rejects a match type of
///     another option. The match type itself keeps no state of a comparison, as one instance serves every evaluation of
///     the expectation; state of an evaluation belongs into its matcher, and state of a single comparison into the
///     <see cref="MatchExplanation" /> of its <see cref="MatchResult" />.
/// </remarks>
public abstract class ObjectMatchType<TSubject> : IEqualityOptions<TSubject>
{
	/// <summary>
	///     A match type without an option name, i.e. the comparison an expectation starts with.
	/// </summary>
	protected ObjectMatchType()
	{
	}

	/// <summary>
	///     A match type that the option named <paramref name="optionName" /> sets.
	/// </summary>
	/// <exception cref="ArgumentNullException">The <paramref name="optionName" /> is <see langword="null" />.</exception>
	protected ObjectMatchType(string optionName)
	{
		optionName.ThrowIfNull();
		OptionName = optionName;
	}

	/// <summary>
	///     The comparison by <see cref="object.Equals(object)" />, which also considers numbers of different types equal
	///     when they have the same value.
	/// </summary>
	public static ObjectMatchType<TSubject> Default => EqualsMatchType<TSubject>.Instance;

	/// <summary>
	///     The name of the option that set the match type, or <see langword="null" /> for the comparison an expectation
	///     starts with.
	/// </summary>
	/// <remarks>
	///     A match type of another option cannot replace it, while one of the same option can.
	/// </remarks>
	public string? OptionName { get; }

	/// <summary>
	///     The words with which the match type describes its comparison.
	/// </summary>
	/// <remarks>
	///     Core writes the expectation text for every grammar from them, and the text for a single expected item, e.g.
	///     <c>contains an item equivalent to …</c>.
	/// </remarks>
	public abstract MatchPhrase Phrase { get; }

	/// <inheritdoc cref="IEqualityOptions{TSubject}.ForEvaluation(IEvaluationContext, CancellationToken)" />
	IEqualityEvaluator<TSubject> IEqualityOptions<TSubject>.ForEvaluation(IEvaluationContext context,
		CancellationToken cancellationToken)
		=> ForEvaluation(new MatchEnvironment(context, cancellationToken));

	/// <summary>
	///     Rejects an <paramref name="expected" /> value that the match type cannot use, by throwing, e.g. an
	///     <see cref="ArgumentException" /> for the <paramref name="parameterName" /> of the caller.
	/// </summary>
	/// <remarks>
	///     This is called before a subject is compared with the value and also when there is none to compare, e.g. for
	///     an empty collection, so that an unusable value is rejected whichever subject it is verified for.
	/// </remarks>
	public virtual void ValidateExpected(TSubject expected, string parameterName)
	{
		// Every value is usable unless a match type says otherwise.
	}

	/// <summary>
	///     Creates the starting value of an expectation whose values are compared within a tolerance, which
	///     <see cref="ObjectToleranceMatchType{TSubject, TTolerance}.Within(TTolerance)" /> sets.
	/// </summary>
	/// <param name="isWithinTolerance">Whether the actual value is within the tolerance of the expected value.</param>
	/// <param name="toString">The text of a tolerance, by default e.g. <c> ± 0.1</c>.</param>
	/// <param name="validation">Rejects a tolerance that the comparison of <typeparamref name="TSubject" /> cannot honour.</param>
	/// <param name="defaultTolerance">
	///     The tolerance that applies until one is specified, read once per evaluation; without one the values are
	///     compared like <see cref="Default" />.
	/// </param>
	/// <remarks>
	///     The value can be shared by every expectation, as it never changes.
	/// </remarks>
	public static ObjectToleranceMatchType<TSubject, TTolerance> WithTolerance<TTolerance>(
		Func<TSubject, TSubject, TTolerance, bool> isWithinTolerance,
		Func<TTolerance, string>? toString = null,
		Action<TTolerance>? validation = null,
		Func<TTolerance>? defaultTolerance = null)
		=> ObjectToleranceMatchType<TSubject, TTolerance>.Create(isWithinTolerance, toString, validation,
			defaultTolerance);

	/// <summary>
	///     Creates the matcher that compares the subjects of one evaluation in the <paramref name="environment" />.
	/// </summary>
	/// <remarks>
	///     A match type without state per evaluation returns a shared matcher, so that an evaluation allocates nothing.
	/// </remarks>
	public abstract ObjectMatcher<TSubject> ForEvaluation(MatchEnvironment environment);

	/// <summary>
	///     Returns the expectation string, e.g. <c>be equal to {expectedExpression}</c>.
	/// </summary>
	public string GetExpectation(string expectedExpression, ExpectationGrammars grammars)
		=> Phrase.GetExpectation(expectedExpression, grammars, true);

	/// <summary>
	///     Get the expectation text for a single expected item, e.g. <c>an item equivalent to {expected}</c>.
	/// </summary>
	/// <remarks>
	///     A comparison other than the equality is always named after the <paramref name="itemNoun" />, so that a verb
	///     such as <c>contains</c> keeps a direct object.<br />
	///     The equality is named with the <paramref name="comparison" /> instead, or left out without one, so that a
	///     verb which already names the item reads <c>has item equal to 3</c>.
	/// </remarks>
	public string GetItemExpectation(string expected, string? itemNoun = null, string? comparison = null)
		=> Phrase.GetItemExpectation(expected, itemNoun, comparison);

	/// <summary>
	///     Returns the short description of the comparison, e.g. <c> using equivalency</c>.
	/// </summary>
	public override string ToString() => Phrase.Name;
}
