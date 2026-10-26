using aweXpect.Core;
using aweXpect.Options;

namespace aweXpect.Helpers;

/// <summary>
///     Tells whether options still compare with their default equality.
/// </summary>
internal static class DefaultEquality
{
	/// <summary>
	///     Whether the comparison of the <paramref name="options" /> was not changed, so that the comparer of a set subject
	///     may decide.
	/// </summary>
	/// <remarks>
	///     An object comparison was not changed while it has no option name, i.e. it is still the value that the
	///     expectation started with, e.g. <see cref="ItemEqualsMatchType{TItem}" /> or the default tolerance. An
	///     expectation that starts with <see cref="ObjectMatchType{TElement}.Default" /> never lets the comparer of the
	///     subject decide.
	/// </remarks>
	public static bool IsUsedBy<T>(IEqualityOptions<T> options)
		=> options switch
		{
			StringEqualityOptions stringOptions => stringOptions.ComparesByOrdinalEquality,
			ObjectMatchType<T> matchType => matchType.OptionName is null &&
			                                !ReferenceEquals(matchType, ObjectMatchType<T>.Default),
			_ => false,
		};
}
