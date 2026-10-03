using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;

namespace aweXpect;

/// <summary>
///     Base class for constraints that classify every item of a collection as matching or not matching and report
///     the outcome through an <see cref="EnumerableQuantifier" />.
/// </summary>
/// <remarks>
///     Use it for an expectation on the elements of a collection, e.g. an extension method on
///     <see cref="IEnumerableElements{TItem}" />, which exposes the <see cref="EnumerableQuantifier" /> and the subject.
///     Pass the quantifier to the constructor, set <see cref="ConstraintResult.WithValue{T}.Actual" /> in
///     <c>IsMetBy</c>, call <see cref="QuantifiedCollectionConstraintBase{TValue,TItem}.Record(TItem, bool)" /> for every item and
///     <see cref="QuantifiedCollectionConstraintBase{TValue,TItem}.Complete" /> afterwards. For a
///     <see langword="null" /> subject, only set the <see cref="ConstraintResult.WithValue{T}.Actual" /> and
///     return, as the expectation fails for it.
///     <para />
///     When the expectation text or the verb are only known while the failure message is created, derive from
///     <see cref="QuantifiedCollectionConstraintBase{TValue,TItem}" /> instead.
/// </remarks>
/// <typeparam name="TValue">The type of the collection.</typeparam>
/// <typeparam name="TItem">The type of the items in the collection.</typeparam>
/// <param name="it">The name of the subject.</param>
/// <param name="grammars">The grammars of the expectation.</param>
/// <param name="quantifier">The quantifier for the items, e.g. from <see cref="IEnumerableElements{TItem}" />.</param>
/// <param name="expectationText">
///     Returns the expectation for a single item for the given grammars, e.g. "is even", or "are even" when the
///     grammars are <see cref="ExpectationGrammars.Plural" />. The quantifier carries the negation, so the text is not
///     negated.
/// </param>
/// <param name="verb">
///     The verb in the past tense in the result, e.g. "were" in "but only 1 of 3 were".
/// </param>
public abstract class QuantifiedCollectionConstraint<TValue, TItem>(
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier,
	Func<ExpectationGrammars, string> expectationText,
	string verb)
	: QuantifiedCollectionConstraintBase<TValue, TItem>(it, grammars, quantifier)
{
	/// <inheritdoc />
	protected sealed override string Verb => verb;

	/// <inheritdoc />
	protected sealed override void AppendItemExpectation(StringBuilder stringBuilder, ExpectationGrammars grammars,
		string? indentation)
		=> stringBuilder.Append(expectationText(grammars));
}
