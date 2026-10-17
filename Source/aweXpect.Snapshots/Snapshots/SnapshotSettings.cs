using System.Collections.Generic;
using aweXpect.Customization;

namespace aweXpect.Snapshots;

/// <summary>
///     The settings of <c>Customize.aweXpect.Snapshots()</c> that a snapshot expectation uses.
/// </summary>
/// <remarks>
///     They are read together when <c>MatchesSnapshot()</c> is called, so that a scoped setting applies to the
///     expectations created within the scope.
/// </remarks>
internal sealed record SnapshotSettings(
	SnapshotStore Store,
	bool? AcceptChanges,
	IReadOnlyList<SnapshotIgnoring> Ignorings,
	IReadOnlyList<SnapshotMasking> Maskings)
{
	public static SnapshotSettings Read()
	{
		SnapshotsAwexpectCustomizationExtensions.SnapshotsCustomization customization = Customize.aweXpect.Snapshots();
		return new SnapshotSettings(customization.Store.Get(), customization.AcceptChanges.Get(),
			customization.Ignorings.Get(), customization.Maskings.Get());
	}
}
