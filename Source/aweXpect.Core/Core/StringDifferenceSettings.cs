using System.Collections.Generic;

namespace aweXpect.Core;

/// <summary>
///     The comparison settings used to display the <see cref="StringDifference" />.
/// </summary>
public class StringDifferenceSettings(int ignoredTrailingLines, int ignoredTrailingColumns,
	IReadOnlyList<int>? ignoredColumnsPerLine)
{
	/// <summary>
	///     The comparison settings used to display the <see cref="StringDifference" />.
	/// </summary>
	public StringDifferenceSettings(int ignoredTrailingLines, int ignoredTrailingColumns)
		: this(ignoredTrailingLines, ignoredTrailingColumns, null)
	{
	}

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
	///     The number of ignored columns at the start of each line, indexed by the line number, or
	///     <see langword="null" /> when no columns are ignored at the start of the lines.
	/// </summary>
	/// <remarks>
	///     Each value is the width of the white-space that was removed at the start of the corresponding line, so that
	///     a position in the compared value can be mapped back to the position in the original value, e.g. when the
	///     indentation is ignored.<br />
	///     When set, it takes precedence over <see cref="IgnoredTrailingColumns" />.
	/// </remarks>
	public IReadOnlyList<int>? IgnoredColumnsPerLine
		=> ignoredColumnsPerLine;

	/// <summary>
	///     The match type used to display the <see cref="StringDifference" />.
	/// </summary>
	public StringDifference.MatchType MatchType { get; internal set; } = StringDifference.MatchType.Equality;
}
