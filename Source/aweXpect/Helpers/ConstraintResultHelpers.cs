using aweXpect.Core.Constraints;

namespace aweXpect.Helpers;

internal static class ConstraintResultHelpers
{
	/// <summary>
	///     Negates the <paramref name="constraintResult" /> when <paramref name="isNegated" /> is <see langword="true" />.
	/// </summary>
	public static T InvertIf<T>(this T constraintResult, bool isNegated) where T : ConstraintResult
		=> isNegated ? constraintResult.Invert() : constraintResult;
}
