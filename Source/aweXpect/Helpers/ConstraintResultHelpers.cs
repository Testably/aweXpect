using aweXpect.Core.Constraints;

namespace aweXpect.Helpers;

internal static class ConstraintResultHelpers
{
	/// <summary>
	///     Negates the <paramref name="constraintResult" /> when <paramref name="isNegated" /> is <see langword="true" />.
	/// </summary>
	public static T InvertIf<T>(this T constraintResult, bool isNegated) where T : ConstraintResult
		=> isNegated ? constraintResult.Invert() : constraintResult;

	/// <summary>
	///     Checks if the <paramref name="constraintResult" /> fails its expectation and the negation alike, e.g. because
	///     code of the caller threw, so that the expectation was not answered.
	/// </summary>
	/// <remarks>
	///     The negation is undone afterwards, because the results of an item expectation are reused for every item.
	/// </remarks>
	public static bool FailsBothWays(this ConstraintResult constraintResult)
	{
		if (constraintResult.Outcome != Outcome.Failure)
		{
			return false;
		}

		bool isNegationFailed = constraintResult.Negate().Outcome == Outcome.Failure;
		constraintResult.Negate();
		return isNegationFailed;
	}

	/// <summary>
	///     Appends the result of the item at the <paramref name="index" /> that decided a collection expectation,
	///     because its <paramref name="itemResult" /> <see cref="FailsBothWays">fails both ways</see>.
	/// </summary>
	/// <remarks>
	///     The item result refers to the item as "it", so the item is named first.
	/// </remarks>
	public static void AppendUnansweredItem(this StringBuilder stringBuilder, ConstraintResult itemResult, int index,
		string? indentation)
	{
		stringBuilder.Append("for the item at index ").Append(index).Append(", ");
		itemResult.AppendResult(stringBuilder, indentation);
	}
}
