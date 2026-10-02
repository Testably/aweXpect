using System.Collections.Generic;
using aweXpect.Core;

namespace aweXpect.Helpers;

internal static class ExpectedValuesContextHelpers
{
	/// <summary>
	///     Adds the <paramref name="values" /> as a context when the failure message names only the
	///     <paramref name="expectedExpression" /> instead of the values themselves.
	/// </summary>
	/// <remarks>
	///     The title follows the final negation of the constraint. The formatter limits the number of items, so an
	///     infinite sequence is not enumerated to its end.
	/// </remarks>
	public static void AddExpectedValuesContext<TItem>(this ResultContextCollector contexts,
		string? expectedExpression, IEnumerable<TItem> values, bool isNegated)
	{
		if (expectedExpression is not null)
		{
			contexts.Add(new ResultContext.SyncCallback(isNegated ? "Unexpected values" : "Expected values",
				() => Formatter.Format(values)));
		}
	}
}
