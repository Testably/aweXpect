using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace aweXpect.Snapshots;

/// <summary>
///     The snapshots that the expectations of this process refer to, which <see cref="OrphanedSnapshots" /> keeps.
/// </summary>
/// <remarks>
///     A snapshot is claimed when <c>MatchesSnapshot()</c> is called, and moved to its name or variant by
///     <c>.Named(…)</c> or <c>.UniqueFor</c>, not when the expectation is evaluated, so that also a snapshot that is
///     not compared, e.g. after another branch of an
///     <c>Or</c> was met or for a <see langword="null" /> subject, is not taken for an orphan. The claims are counted, so
///     that renaming one expectation does not release a snapshot that another expectation still claims.<br />
///     The ids are compared ignoring the case, so that a snapshot whose file name only differs in case from its test
///     is never deleted on a file system that ignores the case; on one that does not, such a file is only missed.
/// </remarks>
internal static class ClaimedSnapshots
{
	/// <summary>
	///     Compares two ids like the claims, ignoring the case.
	/// </summary>
	public static readonly IEqualityComparer<SnapshotId> Comparer = IgnoringCase.Instance;

	private static readonly ConcurrentDictionary<SnapshotId, int> Claims = new(IgnoringCase.Instance);

	public static void Claim(SnapshotId id)
		=> Claims.AddOrUpdate(id, 1, static (_, count) => count + 1);

	public static void Release(SnapshotId id)
		=> Claims.AddOrUpdate(id, 0, static (_, count) => count - 1);

	public static bool IsClaimed(SnapshotId id)
		=> Claims.TryGetValue(id, out int count) && count > 0;

	private sealed class IgnoringCase : IEqualityComparer<SnapshotId>
	{
		public static readonly IgnoringCase Instance = new();

		public bool Equals(SnapshotId? x, SnapshotId? y)
			=> ReferenceEquals(x, y) ||
			   (x is not null && y is not null &&
			    string.Equals(x.SourceFile, y.SourceFile, StringComparison.OrdinalIgnoreCase) &&
			    string.Equals(x.MemberName, y.MemberName, StringComparison.OrdinalIgnoreCase) &&
			    string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase) &&
			    string.Equals(x.Variant, y.Variant, StringComparison.OrdinalIgnoreCase));

		public int GetHashCode(SnapshotId obj)
			=> StringComparer.OrdinalIgnoreCase.GetHashCode(obj.SourceFile) ^
			   StringComparer.OrdinalIgnoreCase.GetHashCode(obj.MemberName);
	}
}
