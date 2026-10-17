using System.Collections.Generic;

namespace aweXpect.Snapshots;

/// <summary>
///     The options of a snapshot expectation, which are set after it was added and read when it is evaluated.
/// </summary>
internal sealed class SnapshotOptions
{
	public bool? AcceptChanges { get; set; }

	public List<Masking> Maskings { get; } = new();

	public string? Name { get; set; }

	/// <summary>
	///     Applies the <see cref="Maskings" /> in the order in which they were added.
	/// </summary>
	public string Mask(string value)
	{
		string masked = value;
		foreach (Masking masking in Maskings)
		{
			masked = masking.Apply(masked);
		}

		return masked;
	}
}
