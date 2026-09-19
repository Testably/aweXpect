namespace aweXpect.Helpers;

/// <summary>
///     A collection that must not satisfy a quantified expectation only because it is empty.
/// </summary>
/// <remarks>
///     Only affects the quantifiers that an empty collection satisfies without making any statement, which are
///     exactly the quantifiers that the same collection with a single non-matching item would not satisfy.
/// </remarks>
internal interface IRejectVacuousSuccess
{
}
