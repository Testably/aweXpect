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
	///     Adds a constraint that verifies that the subject is equal to the <paramref name="expected" /> value, e.g.
	///     <c>is equal to 42</c>, or, when <paramref name="isNegated" />, that it is not equal to it, e.g.
	///     <c>is not equal to 42</c>.
	/// </summary>
	/// <param name="expectationBuilder">The expectation builder of the subject.</param>
	/// <param name="expected">The value that the subject is compared with.</param>
	/// <param name="options">The holder with how the subject is compared with the <paramref name="expected" /> value.</param>
	/// <param name="isNegated">Whether the constraint is negated.</param>
	/// <param name="expectedExpression">
	///     The code of the caller for the <paramref name="expected" /> value, e.g. from a
	///     <see cref="System.Runtime.CompilerServices.CallerArgumentExpressionAttribute" />, which the expectation text
	///     shows instead of the formatted value, e.g. <c>is equal to expected</c>. Its common leading whitespace is
	///     removed with <see cref="StringExtensions.TrimCommonWhiteSpace(string)" />. It is ignored when the
	///     <paramref name="expected" /> value is <see langword="null" />, which is always shown as <c>&lt;null&gt;</c>.
	/// </param>
	/// <returns>The <paramref name="expectationBuilder" />, e.g. for an <see cref="Results.ObjectEqualityResult{TType,TThat,TElement}" />.</returns>
	/// <remarks>
	///     Return an <see cref="Results.ObjectEqualityResult{TType,TThat,TElement}" /> that shares the
	///     <paramref name="options" />, so that the comparison can be specified on it, e.g. with <c>Using(comparer)</c>
	///     or a match type of your own.<br />
	///     A <see langword="null" /> subject is never equal to an <paramref name="expected" /> value that is not
	///     <see langword="null" />, whatever the comparison.<br />
	///     The match type validates the <paramref name="expected" /> value first, see
	///     <see cref="ObjectMatchType{TElement}.ValidateExpected(TElement, string)" />, as the parameter <c>expected</c>,
	///     or <c>unexpected</c> when <paramref name="isNegated" />.
	/// </remarks>
	public static ExpectationBuilder AddEqualityConstraint<TSubject, TExpected>(
		this ExpectationBuilder expectationBuilder,
		TExpected expected, OptionsHolder<ObjectMatchType<TSubject>> options, bool isNegated = false,
		string? expectedExpression = null)
		=> expectationBuilder.AddConstraint(
			(Expected: expected, Options: options, IsNegated: isNegated, ExpectedExpression: expectedExpression),
			static (state, it, grammars) =>
			{
				ObjectEqualityConstraint<TSubject, TExpected> constraint =
					new(it, grammars, state.Expected,
						state.Expected is null ? null : state.ExpectedExpression?.TrimCommonWhiteSpace(), state.Options,
						state.IsNegated ? "unexpected" : "expected");
				return state.IsNegated ? constraint.Invert() : constraint;
			});

	/// <summary>
	///     Adds a constraint that verifies that the subject is equal to the <paramref name="expected" /> value, like
	///     <see cref="AddEqualityConstraint{TSubject, TExpected}(ExpectationBuilder, TExpected, OptionsHolder{ObjectMatchType{TSubject}}, bool, string?)" />,
	///     but starts with the <paramref name="options" /> and creates their holder only when an option is set.
	/// </summary>
	/// <param name="expectationBuilder">The expectation builder of the subject.</param>
	/// <param name="expected">The value that the subject is compared with.</param>
	/// <param name="options">How the subject is compared with the <paramref name="expected" /> value.</param>
	/// <param name="optionsSource">Provides the holder of the options to the result.</param>
	/// <param name="isNegated">Whether the constraint is negated.</param>
	/// <param name="expectedExpression">
	///     The code of the caller for the <paramref name="expected" /> value, see
	///     <see cref="AddEqualityConstraint{TSubject, TExpected}(ExpectationBuilder, TExpected, OptionsHolder{ObjectMatchType{TSubject}}, bool, string?)" />.
	/// </param>
	/// <returns>The <paramref name="expectationBuilder" />, e.g. for an <see cref="Results.ObjectEqualityResult{TType,TThat,TElement}" />.</returns>
	/// <remarks>
	///     Return an <see cref="Results.ObjectEqualityResult{TType,TThat,TElement}" /> with the
	///     <paramref name="optionsSource" />, so that the comparison can be specified on it.
	/// </remarks>
	public static ExpectationBuilder AddEqualityConstraint<TSubject, TExpected>(
		this ExpectationBuilder expectationBuilder,
		TExpected expected,
		ObjectMatchType<TSubject> options,
		out IOptionsHolderSource<ObjectMatchType<TSubject>> optionsSource,
		bool isNegated = false,
		string? expectedExpression = null)
	{
		expectationBuilder.AddConstraint(
			(Expected: expected, Options: options, IsNegated: isNegated, ExpectedExpression: expectedExpression),
			static (state, it, grammars) =>
			{
				ObjectEqualityConstraint<TSubject, TExpected> constraint =
					new(it, grammars, state.Expected,
						state.Expected is null ? null : state.ExpectedExpression?.TrimCommonWhiteSpace(), state.Options,
						state.IsNegated ? "unexpected" : "expected");
				return state.IsNegated ? constraint.Invert() : constraint;
			},
			out ObjectEqualityConstraint<TSubject, TExpected> constraint);
		optionsSource = constraint;
		return expectationBuilder;
	}

	/// <summary>
	///     Adds a constraint that verifies how often the <paramref name="expected" /> <see langword="string" /> occurs in
	///     the <see langword="string" /> subject, e.g. <c>contains "a" at least twice</c>, or, when
	///     <paramref name="isNegated" />, that it does not occur that often, e.g. <c>does not contain "a"</c>.
	/// </summary>
	/// <param name="expectationBuilder">The expectation builder of the <see langword="string" /> subject.</param>
	/// <param name="expected">The <see langword="string" /> whose occurrences are counted.</param>
	/// <param name="quantifier">The holder with how often the <paramref name="expected" /> value has to occur.</param>
	/// <param name="options">
	///     The holder with how the <paramref name="expected" /> value is matched, e.g. as a wildcard pattern.
	/// </param>
	/// <param name="isNegated">Whether the constraint is negated.</param>
	/// <returns>The <paramref name="expectationBuilder" />, e.g. for a <see cref="Results.StringOccurrenceCountResult{TType,TThat}" />.</returns>
	/// <remarks>
	///     Return a <see cref="Results.StringOccurrenceCountResult{TType,TThat}" /> that shares the
	///     <paramref name="quantifier" /> and the <paramref name="options" />, so that both can be specified on it.<br />
	///     A <see langword="null" /> subject fails the expectation and its negation. Reject a <see langword="null" /> or
	///     empty <paramref name="expected" /> value before, as neither can be counted. An unusable pattern is reported
	///     against the parameter <c>expected</c>, or <c>unexpected</c> when <paramref name="isNegated" />.
	/// </remarks>
	public static ExpectationBuilder AddStringOccurrenceConstraint(this ExpectationBuilder expectationBuilder,
		string expected, OptionsHolder<Quantifier> quantifier, OptionsHolder<StringEqualityOptions> options,
		bool isNegated = false)
		=> expectationBuilder.AddConstraint(
			(Expected: expected, Quantifier: quantifier, Options: options, IsNegated: isNegated),
			static (state, it, grammars) =>
			{
				StringOccurrenceConstraint constraint =
					new(it, grammars, state.Expected, state.IsNegated ? "unexpected" : "expected", state.Quantifier,
						state.Options);
				return state.IsNegated ? constraint.Invert() : constraint;
			});

	/// <summary>
	///     Adds a constraint that verifies how often the <paramref name="expected" /> <see langword="string" /> occurs in
	///     the <see langword="string" /> subject, like
	///     <see cref="AddStringOccurrenceConstraint(ExpectationBuilder, string, OptionsHolder{Quantifier}, OptionsHolder{StringEqualityOptions}, bool)" />,
	///     but starts with the <paramref name="quantifier" /> and <paramref name="options" /> and creates their holders
	///     only when an option is set.
	/// </summary>
	/// <param name="expectationBuilder">The expectation builder of the <see langword="string" /> subject.</param>
	/// <param name="expected">The <see langword="string" /> whose occurrences are counted.</param>
	/// <param name="quantifier">How often the <paramref name="expected" /> value has to occur.</param>
	/// <param name="options">How the <paramref name="expected" /> value is matched, e.g. as a wildcard pattern.</param>
	/// <param name="quantifierSource">Provides the holder of the <paramref name="quantifier" /> to the result.</param>
	/// <param name="optionsSource">Provides the holder of the <paramref name="options" /> to the result.</param>
	/// <param name="isNegated">Whether the constraint is negated.</param>
	/// <returns>The <paramref name="expectationBuilder" />, e.g. for a <see cref="Results.StringOccurrenceCountResult{TType,TThat}" />.</returns>
	/// <remarks>
	///     Return a <see cref="Results.StringOccurrenceCountResult{TType,TThat}" /> with both providers, so that the
	///     quantifier and the options can be specified on it.
	/// </remarks>
	public static ExpectationBuilder AddStringOccurrenceConstraint(
		this ExpectationBuilder expectationBuilder,
		string expected,
		Quantifier quantifier,
		StringEqualityOptions options,
		out IOptionsHolderSource<Quantifier> quantifierSource,
		out IOptionsHolderSource<StringEqualityOptions> optionsSource,
		bool isNegated = false)
	{
		expectationBuilder.AddConstraint(
			(Expected: expected, Quantifier: quantifier, Options: options, IsNegated: isNegated),
			static (state, it, grammars) =>
			{
				StringOccurrenceConstraint constraint =
					new(it, grammars, state.Expected, state.IsNegated ? "unexpected" : "expected", state.Quantifier,
						state.Options);
				return state.IsNegated ? constraint.Invert() : constraint;
			},
			out StringOccurrenceConstraint constraint);
		quantifierSource = constraint;
		optionsSource = constraint;
		return expectationBuilder;
	}
}
