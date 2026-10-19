using System.Threading;
using System.Threading.Tasks;

namespace aweXpect.Snapshots;

/// <summary>
///     Is told about the outcome of a snapshot expectation, e.g. to open a diff tool for a mismatch.
/// </summary>
/// <remarks>
///     The reporters are set with <c>Customize.aweXpect.Snapshots().Reporters</c> and called in this order. They are
///     never called on a build server, which is detected like for accepting changes.<br />
///     An exception of a reporter never changes the result of the expectation: it is written to the trace writer of
///     aweXpect and otherwise ignored.<br />
///     An abstract class, not an interface, so that further notifications can be added with a default.
/// </remarks>
public abstract class SnapshotReporter
{
	/// <summary>
	///     Is called when a missing or different snapshot was recorded for review, after the store recorded it.
	/// </summary>
	/// <remarks>
	///     It is only called when the whole expectation fails and the snapshot is not negated, like the store records a
	///     mismatch, and not when the change was accepted instead.
	/// </remarks>
	public virtual ValueTask OnMismatchAsync(SnapshotMismatch mismatch, CancellationToken cancellationToken)
		=> default;

	/// <summary>
	///     Is called when the snapshot matched, before the store records the match, e.g. by deleting a value that an
	///     earlier mismatch recorded, so that a diff tool that shows it can be closed first.
	/// </summary>
	public virtual ValueTask OnMatchAsync(SnapshotMatch match, CancellationToken cancellationToken)
		=> default;
}
