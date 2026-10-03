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
	///     Appends the result of the item at the <paramref name="index" /> that decided a collection expectation,
	///     because its <paramref name="itemResult" /> is <see cref="Outcome.FailureBothWays" />.
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
