using System;

namespace aweXpect.Snapshots;

/// <summary>
///     A <see cref="SnapshotComparer" /> for the snapshots that a condition selects, created by
///     <see cref="SnapshotComparer.When" /> or <see cref="SnapshotComparer.ForExtension" /> and set with
///     <c>Customize.aweXpect.Snapshots().Comparers</c>.
/// </summary>
public sealed class SnapshotComparerRegistration
{
	private readonly Func<SnapshotComparisonContext, bool> _predicate;

	internal SnapshotComparerRegistration(Func<SnapshotComparisonContext, bool> predicate, SnapshotComparer comparer)
	{
		_predicate = predicate;
		Comparer = comparer;
	}

	internal SnapshotComparer Comparer { get; }

	internal bool AppliesTo(SnapshotComparisonContext context) => _predicate(context);
}
