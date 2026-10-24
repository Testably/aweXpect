namespace aweXpect.Core;

/// <summary>
///     The outcome of a <see cref="MatchResult" />.
/// </summary>
public enum MatchOutcome
{
	/// <summary>
	///     The values do not match.
	/// </summary>
	NoMatch = 0,

	/// <summary>
	///     The values match.
	/// </summary>
	Match = 1,

	/// <summary>
	///     The subject cannot be compared at all, so that the expectation fails in both polarities.
	/// </summary>
	NotComparable = 2,
}
