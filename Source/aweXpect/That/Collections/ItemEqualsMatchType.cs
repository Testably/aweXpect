using aweXpect.Core;

namespace aweXpect;

/// <summary>
///     The comparison of the items of a collection by <see cref="object.Equals(object)" />, like
///     <see cref="ObjectMatchType{TSubject}.Default" />, which the comparer of a set subject may replace.
/// </summary>
/// <remarks>
///     A separate value, so that an expectation tells that it still compares its items with the comparison it started
///     with, see <see cref="Helpers.DefaultEquality" />.
/// </remarks>
internal sealed class ItemEqualsMatchType<TItem> : ObjectMatchType<TItem>
{
	private ItemEqualsMatchType()
	{
	}

	/// <summary>
	///     The shared value, as it never changes.
	/// </summary>
	public static ItemEqualsMatchType<TItem> Instance { get; } = new();

	/// <inheritdoc />
	public override MatchPhrase Phrase => Default.Phrase;

	/// <inheritdoc />
	public override ObjectMatcher<TItem> ForEvaluation(MatchEnvironment environment)
		=> Default.ForEvaluation(environment);
}
