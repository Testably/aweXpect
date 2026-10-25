using System.Threading.Tasks;
using aweXpect.Core.Helpers;

namespace aweXpect.Core;

/// <summary>
///     Compares the subjects of one evaluation, created by
///     <see cref="ObjectMatchType{TSubject}.ForEvaluation(MatchEnvironment)" />, with expected values of any type.
/// </summary>
/// <remarks>
///     The items of a collection are compared one after the other with the matcher of the evaluation, so it may keep
///     state across them, e.g. which options matched nothing in any comparison.<br />
///     Derive from <see cref="ObjectMatcher{TSubject, TExpected}" /> instead, when the expected value has a known type.
/// </remarks>
public abstract class ObjectMatcher<TSubject> : IEqualityEvaluator<TSubject>
{
	/// <inheritdoc cref="IEqualityEvaluator{TSubject}.AreConsideredEqual{TExpected}(TSubject, TExpected)" />
	/// <remarks>
	///     A subject that the matcher cannot compare throws, so that every collection fails for it in both polarities.
	/// </remarks>
	ValueTask<bool> IEqualityEvaluator<TSubject>.AreConsideredEqual<TExpected>(TSubject actual, TExpected expected)
	{
		ValueTask<MatchResult> result = Match(actual, expected);
		if (!result.IsCompletedSuccessfully)
		{
			return AwaitIsMatch(result);
		}

		return new ValueTask<bool>(ComparableOrThrow(result.Result).IsMatch);

		static async ValueTask<bool> AwaitIsMatch(ValueTask<MatchResult> result)
			=> ComparableOrThrow(await result).IsMatch;
	}

	/// <summary>
	///     Compares the <paramref name="actual" /> with the <paramref name="expected" /> value for a constraint, which
	///     keeps the result to explain a failure with
	///     <see cref="GetFailure{TExpected}(string, ExpectationGrammars, TSubject, TExpected, MatchResult, string?)" />.
	/// </summary>
	/// <remarks>
	///     Compares with <see cref="MatchWithExplanation{TExpected}(TSubject, TExpected)" />. Unlike it, a subject that the
	///     matcher cannot compare throws, so that the expectation fails in both polarities.
	/// </remarks>
	public ValueTask<MatchResult> CompareWithExplanation<TExpected>(TSubject actual, TExpected expected)
	{
		ValueTask<MatchResult> result = MatchWithExplanation(actual, expected);
		if (!result.IsCompletedSuccessfully)
		{
			return AwaitComparable(result);
		}

		return new ValueTask<MatchResult>(ComparableOrThrow(result.Result));

		static async ValueTask<MatchResult> AwaitComparable(ValueTask<MatchResult> result)
			=> ComparableOrThrow(await result);
	}

	/// <summary>
	///     Decides whether the <paramref name="actual" /> value matches the <paramref name="expected" /> value, whose type
	///     varies, e.g. an anonymous object for an equivalency.
	/// </summary>
	/// <remarks>
	///     Most comparisons, e.g. of the items of a collection, are never explained, so this only needs to decide.
	///     Return <see cref="MatchResult.NotComparable(string, System.Exception)" /> when the
	///     <paramref name="actual" /> value cannot be compared at all, so that the expectation fails in both polarities.
	/// </remarks>
	public abstract ValueTask<MatchResult> Match<TExpected>(TSubject actual, TExpected expected);

	/// <summary>
	///     Compares the <paramref name="actual" /> value with the <paramref name="expected" /> value like
	///     <see cref="Match{TExpected}(TSubject, TExpected)" />, for a comparison whose failure is explained.
	/// </summary>
	/// <remarks>
	///     Override it when explaining a mismatch costs more than deciding it, e.g. to collect all differences of an
	///     equivalency comparison into a <see cref="MatchExplanation" />.
	/// </remarks>
	public virtual ValueTask<MatchResult> MatchWithExplanation<TExpected>(TSubject actual, TExpected expected)
		=> Match(actual, expected);

	/// <summary>
	///     The failure text for the comparison of the <paramref name="actual" /> with the <paramref name="expected" />
	///     value that returned the <paramref name="result" />, starting with <paramref name="it" />.
	/// </summary>
	/// <param name="it">The name of the subject in the result text.</param>
	/// <param name="grammars">The grammars of the expectation.</param>
	/// <param name="actual">The actual value of the comparison.</param>
	/// <param name="expected">The expected value of the comparison.</param>
	/// <param name="result">The result of the comparison.</param>
	/// <param name="indentation">
	///     The indentation for the lines after the first, when the failure text is part of a nested result, e.g. of an
	///     expectation in <c>Expect.ThatAll</c>.
	/// </param>
	/// <remarks>
	///     The default writes the <see cref="MatchResult.Explanation" /> of the <paramref name="result" />, or else the
	///     <paramref name="actual" /> value.
	/// </remarks>
	public virtual string GetFailure<TExpected>(string it, ExpectationGrammars grammars, TSubject? actual,
		TExpected expected, MatchResult result, string? indentation)
		=> result.Explanation?.GetFailure(it, grammars, indentation) ??
		   $"{it}{grammars.SubjectVerb(it, " was ", " were ")}{Formatter.Format(actual, FormattingOptions.Indented(indentation))}";

	/// <summary>
	///     Adds the contexts that explain a failure of this evaluation, e.g. the options of the match type.
	/// </summary>
	/// <remarks>
	///     Only called while a failure message is created.
	/// </remarks>
	public virtual void AppendContexts(ResultContextCollector contexts)
	{
		// Most matchers have nothing to add.
	}

	/// <summary>
	///     Throws for a <paramref name="result" /> that could not compare the subject, so that every expectation and
	///     every collection fails for it in both polarities, like for an exception of the caller's code.
	/// </summary>
	private static MatchResult ComparableOrThrow(MatchResult result)
		=> result.Outcome != MatchOutcome.NotComparable
			? result
			: throw new UserCodeException(new NotComparableException(result.NotComparableReason!, result.Cause));
}

/// <summary>
///     Compares the subjects of one evaluation with expected values of the type <typeparamref name="TExpected" />,
///     e.g. two IP addresses.
/// </summary>
/// <remarks>
///     An expected value of another type never matches. A <see langword="null" /> expected value is compared when
///     <typeparamref name="TExpected" /> admits it.
/// </remarks>
public abstract class ObjectMatcher<TSubject, TExpected> : ObjectMatcher<TSubject>
{
	/// <summary>
	///     Decides whether the <paramref name="actual" /> value matches the <paramref name="expected" /> value.
	/// </summary>
	/// <remarks>
	///     Return <see cref="MatchResult.NotComparable(string, System.Exception)" /> when the
	///     <paramref name="actual" /> value cannot be compared at all, so that the expectation fails in both polarities.
	/// </remarks>
	public abstract ValueTask<MatchResult> Match(TSubject actual, TExpected expected);

	/// <inheritdoc />
	public sealed override ValueTask<MatchResult> Match<TOther>(TSubject actual, TOther expected)
		=> TryCast(expected, out TExpected typedExpected)
			? Match(actual, typedExpected)
			: new ValueTask<MatchResult>(MatchResult.NoMatch);

	/// <summary>
	///     Compares the <paramref name="actual" /> value with the <paramref name="expected" /> value like
	///     <see cref="Match(TSubject, TExpected)" />, for a comparison whose failure is explained.
	/// </summary>
	/// <remarks>
	///     Override it when explaining a mismatch costs more than deciding it, e.g. to collect all differences into a
	///     <see cref="MatchExplanation" />.
	/// </remarks>
	public virtual ValueTask<MatchResult> MatchWithExplanation(TSubject actual, TExpected expected)
		=> Match(actual, expected);

	/// <inheritdoc />
	public sealed override ValueTask<MatchResult> MatchWithExplanation<TOther>(TSubject actual, TOther expected)
		=> TryCast(expected, out TExpected typedExpected)
			? MatchWithExplanation(actual, typedExpected)
			: new ValueTask<MatchResult>(MatchResult.NoMatch);

	/// <summary>
	///     The failure text for the comparison of the <paramref name="actual" /> with the <paramref name="expected" />
	///     value that returned the <paramref name="result" />, starting with <paramref name="it" />.
	/// </summary>
	/// <remarks>
	///     The default writes the <see cref="MatchResult.Explanation" /> of the <paramref name="result" />, or else the
	///     <paramref name="actual" /> value.
	/// </remarks>
	public virtual string GetFailure(string it, ExpectationGrammars grammars, TSubject? actual, TExpected expected,
		MatchResult result, string? indentation)
		=> base.GetFailure(it, grammars, actual, expected, result, indentation);

	/// <inheritdoc />
	public sealed override string GetFailure<TOther>(string it, ExpectationGrammars grammars, TSubject? actual,
		TOther expected, MatchResult result, string? indentation)
		=> TryCast(expected, out TExpected typedExpected)
			? GetFailure(it, grammars, actual, typedExpected, result, indentation)
			: base.GetFailure(it, grammars, actual, expected, result, indentation);

	/// <remarks>
	///     A <see langword="null" /> value is not matched by a type pattern, but is a valid value whenever
	///     <typeparamref name="TExpected" /> admits it.
	/// </remarks>
	private static bool TryCast<TValue>(TValue value, out TExpected typedValue)
	{
		if (value is TExpected typed)
		{
			typedValue = typed;
			return true;
		}

		typedValue = default!;
		return value is null && default(TExpected) is null;
	}
}
