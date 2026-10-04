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
	public static bool IsUsedBy(object options)
		=> options switch
		{
			IHasDefaultMatchType itemOptions => itemOptions.HasDefaultMatchType,
			StringEqualityOptions stringOptions => stringOptions.ComparesByOrdinalEquality,
			_ => false,
		};
}
