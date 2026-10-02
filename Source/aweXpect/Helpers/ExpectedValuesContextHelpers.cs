using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Helpers;

internal static class ExpectedValuesContextHelpers
{
	/// <summary>
	///     Adds the <paramref name="values" /> as a context when the failure message names only the
	///     <paramref name="expectedExpression" /> instead of the values themselves.
	/// </summary>
	/// <remarks>
	///     The context is decided when the failure message is built, so it only appears when the
	///     <paramref name="constraintResult" /> itself failed and not when another part of a combined expectation did,
	///     and its title follows a negation after the evaluation (e.g. by <c>DoesNotComplyWith</c>).
	///     The formatter limits the number of items, so an infinite sequence is not enumerated to its end.
	/// </remarks>
	public static T WithExpectedValuesContext<T, TSubject, TItem>(this T constraintResult, IThat<TSubject> subject,
		string? expectedExpression, bool negated, IEnumerable<TItem> values)
		where T : ConstraintResult
	{
		if (expectedExpression is not null)
		{
			subject.Get().ExpectationBuilder.AddContext(
				new ExpectedValuesContext<TItem>(GetTitle(negated), constraintResult, values));
		}

		return constraintResult;
	}

	private static string GetTitle(bool negated)
		=> negated ? "Unexpected values" : "Expected values";

	private sealed class ExpectedValuesContext<TItem>(
		string title,
		ConstraintResult constraintResult,
		IEnumerable<TItem> values)
		: ResultContext(title)
	{
		/// <remarks>
		///     The failure message reads the title only after the content.
		/// </remarks>
		public override Task<string?> GetContent(CancellationToken cancellationToken = default)
		{
			Title = GetTitle(constraintResult.Grammars.IsNegated());
			return Task.FromResult(constraintResult.Outcome == Outcome.Failure ? Formatter.Format(values) : null);
		}
	}
}
