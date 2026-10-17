namespace aweXpect.Snapshots;

/// <summary>
///     Which snapshots are accepted, i.e. replaced by the actual value, when the expectation fails.
/// </summary>
/// <remarks>
///     An accepted snapshot still fails the expectation, saying that it was accepted, so that it never goes unnoticed.
/// </remarks>
public enum SnapshotAcceptance
{
	/// <summary>
	///     No snapshot is accepted: a missing or different snapshot writes the pending file for review.
	/// </summary>
	None,

	/// <summary>
	///     Only a missing snapshot is accepted; a different snapshot is kept and writes the pending file for review.
	/// </summary>
	New,

	/// <summary>
	///     A missing or different snapshot is accepted.
	/// </summary>
	All,
}
