using aweXpect.Core.Constraints;
using aweXpect.Options;

namespace aweXpect.Core.Extending;

/// <summary>
///     Registers the built-in constraints of aweXpect on an <see cref="ExpectationBuilder" />, so that an extension can
///     offer the same checks and failure messages on its own subjects.
/// </summary>
/// <remarks>
///     This class is in its own namespace, so that its methods are only suggested to extension authors who import it.
/// </remarks>
public static class ExpectationBuilderExtensions
{
	/// <summary>
	///     Adds a constraint that verifies how often the <paramref name="expected" /> <see langword="string" /> occurs in
	///     the <see langword="string" /> subject, e.g. <c>contains "a" at least twice</c>, or, when
	///     <paramref name="isNegated" />, that it does not occur that often, e.g. <c>does not contain "a"</c>.
	/// </summary>
	/// <param name="expectationBuilder">The expectation builder of the <see langword="string" /> subject.</param>
	/// <param name="expected">The <see langword="string" /> whose occurrences are counted.</param>
	/// <param name="quantifier">How often the <paramref name="expected" /> value has to occur.</param>
	/// <param name="options">How the <paramref name="expected" /> value is matched, e.g. as a wildcard pattern.</param>
	/// <param name="isNegated">Whether the constraint is negated.</param>
	/// <returns>The <paramref name="expectationBuilder" />, e.g. for a <see cref="Results.StringOccurrenceCountResult{TType,TThat}" />.</returns>
	/// <remarks>
	///     Return a <see cref="Results.StringOccurrenceCountResult{TType,TThat}" /> that shares the
	///     <paramref name="quantifier" /> and the <paramref name="options" />, so that both can be specified on it.<br />
	///     A <see langword="null" /> subject fails the expectation and its negation. Reject a <see langword="null" /> or
	///     empty <paramref name="expected" /> value before, as neither can be counted.
	/// </remarks>
	public static ExpectationBuilder AddStringOccurrenceConstraint(this ExpectationBuilder expectationBuilder,
		string expected, Quantifier quantifier, StringEqualityOptions options, bool isNegated = false)
		=> expectationBuilder.AddConstraint(
			(Expected: expected, Quantifier: quantifier, Options: options, IsNegated: isNegated),
			static (state, it, grammars) =>
			{
				StringOccurrenceConstraint constraint =
					new(it, grammars, state.Expected, state.Quantifier, state.Options);
				return state.IsNegated ? constraint.Invert() : constraint;
			});
}
