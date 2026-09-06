using System.Collections.Generic;

namespace aweXpect.Core;

/// <summary>
///     The comparison settings used to display the <see cref="StringDifference" />.
/// </summary>
public class StringDifferenceSettings(int ignoredTrailingLines, int ignoredTrailingColumns,
	IReadOnlyList<int>? ignoredColumnsPerLine = null)
{
	/// <summary>
	///     The number of ignored trailing lines.
	/// </summary>
	public int IgnoredTrailingLines
		=> ignoredTrailingLines;

	/// <summary>
	///     The number of ignored trailing columns in the first line.
	/// </summary>
	public int IgnoredTrailingColumns
		=> ignoredTrailingColumns;

	/// <summary>
	///     The number of ignored columns per line, or <see langword="null" /> when the ignored columns are the same
	///     for all lines.
	/// </summary>
	/// <remarks>
	///     This is used when the ignored white-space differs per line, e.g. when the indentation is ignored.<br />
	///     When set, it takes precedence over <see cref="IgnoredTrailingColumns" />.
	/// </remarks>
	public IReadOnlyList<int>? IgnoredColumnsPerLine
		=> ignoredColumnsPerLine;

	/// <summary>
	///     The match type used to display the <see cref="StringDifference" />.
	/// </summary>
	public StringDifference.MatchType MatchType { get; internal set; } = StringDifference.MatchType.Equality;
}
