using System.Collections.Generic;

namespace aweXpect.Snapshots;

/// <summary>
///     The options of a snapshot expectation, which are set after it was added and read when it is evaluated.
/// </summary>
internal sealed class SnapshotOptions
{
	public bool? AcceptChanges { get; set; }

	public List<SnapshotMasking> Maskings { get; } = new();

	public string? Name { get; set; }

	/// <summary>
	///     Applies the <see cref="Maskings" /> in the order in which they were added, then the
	///     <paramref name="globalMaskings" /> in their order, so that a specific masking of the expectation goes before
	///     a general one for all snapshots.
	/// </summary>
	public string Mask(string value, IReadOnlyList<SnapshotMasking> globalMaskings)
	{
		string masked = value;
		foreach (SnapshotMasking masking in Maskings)
		{
			masked = masking.Apply(masked);
		}

		foreach (SnapshotMasking masking in globalMaskings)
		{
			masked = masking.Apply(masked);
		}

		return masked;
	}
}
