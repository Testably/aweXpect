using System.Collections.Generic;
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
	///     <paramref name="constraintResult" /> itself failed and not when another part of a combined expectation did.
	///     The formatter limits the number of items, so an infinite sequence is not enumerated to its end.
	/// </remarks>
	public static T WithExpectedValuesContext<T, TSubject, TItem>(this T constraintResult, IThat<TSubject> subject,
		string? expectedExpression, bool negated, IEnumerable<TItem> values)
		where T : ConstraintResult
	{
		if (expectedExpression is not null)
		{
			subject.Get().ExpectationBuilder.AddContext(new ResultContext.SyncCallback(
				negated ? "Unexpected values" : "Expected values",
				() => constraintResult.Outcome == Outcome.Failure ? Formatter.Format(values) : null));
		}

		return constraintResult;
	}
}
