using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Formatting;

namespace aweXpect.Snapshots;

/// <summary>
///     The options of a snapshot expectation, which are set after it was added and read when it is evaluated.
/// </summary>
internal sealed class SnapshotOptions
{
	public bool? AcceptChanges { get; set; }

	public List<SnapshotIgnoring> Ignorings { get; } = new();

	public List<SnapshotMasking> Maskings { get; } = new();

	public string? Name { get; set; }

	/// <summary>
	///     Applies the text maskings of <see cref="Maskings" /> in the order in which they were added, then those of the
	///     <paramref name="globalMaskings" /> in their order, so that a specific masking of the expectation goes before
	///     a general one for all snapshots.
	/// </summary>
	public string Mask(string value, IReadOnlyList<SnapshotMasking> globalMaskings, SnapshotNumbering numbering)
	{
		string masked = value;
		foreach (SnapshotMasking masking in Maskings)
		{
			masked = masking.Apply(masked, numbering);
		}

		foreach (SnapshotMasking masking in globalMaskings)
		{
			masked = masking.Apply(masked, numbering);
		}

		return masked;
	}

	/// <summary>
	///     Creates the decision of the formatter for each value nested in an object snapshot from the ignorings and the
	///     structural maskings of the expectation and the global ones, or returns <see langword="null" /> when there is
	///     none, so that the formatter does not offer the values at all.
	/// </summary>
	/// <remarks>
	///     An ignoring wins over a masking, the rules of the expectation come before the global ones, and the first
	///     masking that matches a value replaces it.
	/// </remarks>
	public Func<FormattedValue, ValueFormatting>? CreateFormatValue(IReadOnlyList<SnapshotIgnoring> globalIgnorings,
		IReadOnlyList<SnapshotMasking> globalMaskings, SnapshotNumbering numbering)
	{
		SnapshotIgnoring[] ignorings = Ignorings.Concat(globalIgnorings).ToArray();
		SnapshotMasking[] maskings = Maskings.Concat(globalMaskings).Where(masking => masking.IsStructural).ToArray();
		if (ignorings.Length == 0 && maskings.Length == 0)
		{
			return null;
		}

		return value =>
		{
			if (ignorings.Any(ignoring => ignoring.Matches(value)))
			{
				return ValueFormatting.Omit;
			}

			foreach (SnapshotMasking masking in maskings)
			{
				SnapshotPlaceholder? replacement = masking.GetReplacement(value, numbering);
				if (replacement is not null)
				{
					return ValueFormatting.Replace(replacement);
				}
			}

			return ValueFormatting.Write;
		};
	}
}
