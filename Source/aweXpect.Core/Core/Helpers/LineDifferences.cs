using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace aweXpect.Core.Helpers;

/// <summary>
///     Lists the lines that differ between two multi-line values as a compact line diff.
/// </summary>
internal static class LineDifferences
{
	/// <summary>
	///     The maximum number of removed and added lines, beyond which the diff is abandoned, as its effort grows with
	///     the number of lines times the number of changed lines.
	/// </summary>
	internal const int MaximumNumberOfChangedLines = 1000;

	private static readonly char[] LineBreakCharacters = ['\r', '\n',];

	/// <summary>
	///     Splits the <paramref name="value" /> into its lines at every <c>\n</c>, <c>\r\n</c> and <c>\r</c>.
	/// </summary>
	public static List<Line> Split(string value)
	{
		List<Line> lines = [];
		int start = 0;
		int index = value.IndexOfAny(LineBreakCharacters);
		while (index >= 0)
		{
			int length = value[index] == '\r' && index + 1 < value.Length && value[index + 1] == '\n' ? 2 : 1;
			lines.Add(new Line(lines.Count + 1, value.Substring(start, index - start),
				value.Substring(index, length)));
			start = index + length;
			index = value.IndexOfAny(LineBreakCharacters, start);
		}

		lines.Add(new Line(lines.Count + 1, value.Substring(start), ""));
		return lines;
	}

	/// <summary>
	///     Formats the differences between the <paramref name="actual" /> and the <paramref name="expected" /> lines,
	///     or returns <see langword="null" /> when they differ in less than two places or in too many lines.
	/// </summary>
	/// <remarks>
	///     A difference is a run of removed and added lines between two equal lines. Each difference is shown with one
	///     equal line before and after it, and at most two equal lines between the shown lines are shown instead of
	///     being omitted.<br />
	///     The whitespace of a line is shown like the formatter shows it. A removed and an added line at the same
	///     position of a difference, which would look the same as they only differ in their line break or in trailing
	///     spaces, are quoted together with their line break.
	/// </remarks>
	public static string? Format(IReadOnlyList<Line> actual, IReadOnlyList<Line> expected,
		Func<Line, Line, bool> areEqual, int maximumNumberOfDifferences)
	{
		List<Edit>? edits = GetEdits(actual, expected, areEqual);
		if (edits is null)
		{
			return null;
		}

		(edits, List<(int Start, int End)> differences) = GroupDifferences(edits);
		if (differences.Count < 2)
		{
			return null;
		}

		int shownDifferences = Math.Max(0, Math.Min(maximumNumberOfDifferences, differences.Count));
		List<Edit?> rows = GetRows(edits, differences.Take(shownDifferences));
		StringBuilder sb = new();
		AppendRows(sb, rows);
		if (shownDifferences < differences.Count)
		{
			if (rows.Count > 0)
			{
				sb.AppendLine();
			}

			sb.Append("  (… and ").Append(differences.Count - shownDifferences).Append(" more)");
		}

		return sb.ToString();
	}

	/// <summary>
	///     Gets the rows of the shown <paramref name="differences" /> with their surrounding equal lines, where
	///     <see langword="null" /> marks omitted equal lines between them.
	/// </summary>
	private static List<Edit?> GetRows(List<Edit> edits, IEnumerable<(int Start, int End)> differences)
	{
		const int maximumNumberOfEqualLinesBetweenShownLines = 2;
		List<Edit?> rows = [];
		int shownEnd = -1;
		foreach ((int start, int end) in differences)
		{
			int rowStart = Math.Max(start - 1, 0);
			if (shownEnd < 0 || rowStart - shownEnd > maximumNumberOfEqualLinesBetweenShownLines)
			{
				if (shownEnd >= 0)
				{
					rows.Add(null);
				}

				shownEnd = rowStart;
			}

			int rowEnd = Math.Min(end + 1, edits.Count);
			rows.AddRange(edits.GetRange(shownEnd, rowEnd - shownEnd));
			shownEnd = rowEnd;
		}

		return rows;
	}

	private static void AppendRows(StringBuilder sb, List<Edit?> rows)
	{
		int width = rows.Where(row => row is not null)
			.Select(row => Math.Max(row!.Actual?.Number ?? 0, row.Expected?.Number ?? 0))
			.DefaultIfEmpty(0).Max().ToString(CultureInfo.InvariantCulture).Length;
		string blank = new(' ', width);
		for (int i = 0; i < rows.Count; i++)
		{
			if (i > 0)
			{
				sb.AppendLine();
			}

			if (rows[i] is not { } row)
			{
				sb.Append("  …");
				continue;
			}

			string numbers = "  " + GetMarker(row) + "  " +
			                 (row.Actual?.Number.ToString(CultureInfo.InvariantCulture).PadLeft(width) ?? blank) +
			                 "   " +
			                 (row.Expected?.Number.ToString(CultureInfo.InvariantCulture).PadLeft(width) ?? blank);
			Line line = (row.Actual ?? row.Expected)!;
			string text = row.IsQuoted
				? "\"" + (line.Text + line.LineBreak).Escape() + "\""
				: line.Text.DisplayWhitespace();
			sb.Append(text.Length > 0 ? numbers + "    " + text : numbers.TrimEnd());
		}
	}

	private static char GetMarker(Edit row)
	{
		if (row.Expected is null)
		{
			return '-';
		}

		return row.Actual is null ? '+' : ' ';
	}

	/// <summary>
	///     Groups the <paramref name="edits" /> into differences, i.e. maximal runs of removed and added lines, and
	///     moves the removed lines of each run before its added lines.
	/// </summary>
	private static (List<Edit> Edits, List<(int Start, int End)> Differences) GroupDifferences(List<Edit> edits)
	{
		List<Edit> ordered = new(edits.Count);
		List<(int Start, int End)> differences = [];
		List<Edit> added = [];
		int start = -1;
		foreach (Edit edit in edits)
		{
			if (edit.Actual is not null && edit.Expected is not null)
			{
				EndDifference();
				ordered.Add(edit);
				continue;
			}

			if (start < 0)
			{
				start = ordered.Count;
			}

			if (edit.Expected is null)
			{
				ordered.Add(edit);
			}
			else
			{
				added.Add(edit);
			}
		}

		EndDifference();
		return (ordered, differences);

		void EndDifference()
		{
			if (start >= 0)
			{
				QuoteLinesThatLookTheSame(ordered, start, added);
				ordered.AddRange(added);
				added.Clear();
				differences.Add((start, ordered.Count));
				start = -1;
			}
		}
	}

	/// <summary>
	///     Quotes each removed line from <paramref name="start" /> on in <paramref name="removed" /> together with the
	///     added line at the same position, when both would look the same.
	/// </summary>
	private static void QuoteLinesThatLookTheSame(List<Edit> removed, int start, List<Edit> added)
	{
		for (int i = 0; i < added.Count && start + i < removed.Count; i++)
		{
			if (removed[start + i].Actual!.Text.DisplayWhitespace().TrimEnd(' ') ==
			    added[i].Expected!.Text.DisplayWhitespace().TrimEnd(' '))
			{
				removed[start + i].IsQuoted = true;
				added[i].IsQuoted = true;
			}
		}
	}

	/// <summary>
	///     Gets a minimal edit script from the <paramref name="actual" /> to the <paramref name="expected" /> lines, or
	///     <see langword="null" /> when it would change more than <see cref="MaximumNumberOfChangedLines" /> lines.
	/// </summary>
	/// <remarks>
	///     The equal lines at the start and the end are skipped before the remaining lines are compared with the
	///     Myers algorithm.
	/// </remarks>
	private static List<Edit>? GetEdits(IReadOnlyList<Line> actual, IReadOnlyList<Line> expected,
		Func<Line, Line, bool> areEqual)
	{
		int prefix = 0;
		while (prefix < actual.Count && prefix < expected.Count && areEqual(actual[prefix], expected[prefix]))
		{
			prefix++;
		}

		int suffix = 0;
		while (suffix < actual.Count - prefix && suffix < expected.Count - prefix &&
		       areEqual(actual[actual.Count - 1 - suffix], expected[expected.Count - 1 - suffix]))
		{
			suffix++;
		}

		Window window = new(actual, expected, prefix, actual.Count - prefix - suffix,
			expected.Count - prefix - suffix, areEqual);
		List<int[]>? trace = GetTrace(window);
		if (trace is null)
		{
			return null;
		}

		List<Edit> edits = [];
		edits.AddRange(Enumerable.Range(0, prefix).Select(i => new Edit(actual[i], expected[i])));
		edits.AddRange(Backtrack(window, trace));
		edits.AddRange(Enumerable.Range(0, suffix).Select(i => new Edit(actual[actual.Count - suffix + i],
			expected[expected.Count - suffix + i])));
		return edits;
	}

	/// <summary>
	///     Records, for every number of changed lines, how far each diagonal reaches, until the end of both windows is
	///     reached.
	/// </summary>
	/// <remarks>
	///     Each recorded array holds the diagonals <c>-d</c> to <c>d</c> at the indices <c>0</c> to <c>2d</c>.
	/// </remarks>
	private static List<int[]>? GetTrace(Window window)
	{
		List<int[]> trace = [];
		int[] previous = [];
		int maximum = Math.Min(window.ActualCount + window.ExpectedCount, MaximumNumberOfChangedLines);
		for (int d = 0; d <= maximum; d++)
		{
			int[] current = new int[(2 * d) + 1];
			trace.Add(current);
			for (int k = -d; k <= d; k += 2)
			{
				int x = GetStart(previous, d, k);
				int y = x - k;
				while (x < window.ActualCount && y < window.ExpectedCount && window.AreEqual(x, y))
				{
					x++;
					y++;
				}

				current[k + d] = x;
				if (x >= window.ActualCount && y >= window.ExpectedCount)
				{
					return trace;
				}
			}

			previous = current;
		}

		return null;
	}

	private static int GetStart(int[] previous, int d, int k)
	{
		if (d == 0)
		{
			return 0;
		}

		return IsInsertion(previous, d, k) ? previous[k + d] : previous[k + d - 2] + 1;
	}

	/// <summary>
	///     Whether diagonal <paramref name="k" /> after <paramref name="d" /> changes is reached from diagonal
	///     <c>k + 1</c> by adding an expected line, instead of from diagonal <c>k - 1</c> by removing an actual line.
	/// </summary>
	private static bool IsInsertion(int[] previous, int d, int k)
		=> k == -d || (k != d && previous[k + d - 2] < previous[k + d]);

	private static List<Edit> Backtrack(Window window, List<int[]> trace)
	{
		List<Edit> reversed = [];
		int x = window.ActualCount;
		int y = window.ExpectedCount;
		for (int d = trace.Count - 1; d > 0; d--)
		{
			int k = x - y;
			bool isInsertion = IsInsertion(trace[d - 1], d, k);
			int previousK = isInsertion ? k + 1 : k - 1;
			int previousX = trace[d - 1][previousK + d - 1];
			int previousY = previousX - previousK;
			while (x > previousX && y > previousY)
			{
				x--;
				y--;
				reversed.Add(new Edit(window.Actual(x), window.Expected(y)));
			}

			reversed.Add(isInsertion
				? new Edit(null, window.Expected(previousY))
				: new Edit(window.Actual(previousX), null));
			x = previousX;
			y = previousY;
		}

		while (x > 0 && y > 0)
		{
			x--;
			y--;
			reversed.Add(new Edit(window.Actual(x), window.Expected(y)));
		}

		reversed.Reverse();
		return reversed;
	}

	/// <summary>
	///     A line of a value.
	/// </summary>
	/// <param name="number">The one-based line number in the original value.</param>
	/// <param name="text">The original text of the line, without its line break.</param>
	/// <param name="lineBreak">The line break that ends the line, or an empty string for the last line.</param>
	internal sealed class Line(int number, string text, string lineBreak)
	{
		/// <summary>
		///     The one-based line number in the original value.
		/// </summary>
		public int Number { get; } = number;

		/// <summary>
		///     The original text of the line, without its line break.
		/// </summary>
		public string Text { get; } = text;

		/// <summary>
		///     The text of the line that is compared, after the options of the comparison are applied.
		/// </summary>
		public string ComparedText { get; set; } = text;

		/// <summary>
		///     The line break that ends the line, or an empty string when it is the last line of the compared value.
		/// </summary>
		public string LineBreak { get; set; } = lineBreak;
	}

	/// <summary>
	///     An equal line, when both the <see cref="Actual" /> and the <see cref="Expected" /> line are set, otherwise a
	///     removed actual line or an added expected line.
	/// </summary>
	private sealed class Edit(Line? actual, Line? expected)
	{
		public Line? Actual { get; } = actual;
		public Line? Expected { get; } = expected;
		public bool IsQuoted { get; set; }
	}

	/// <summary>
	///     The lines that remain to be compared after the equal lines at the start and the end are skipped.
	/// </summary>
	private sealed class Window(
		IReadOnlyList<Line> actual,
		IReadOnlyList<Line> expected,
		int offset,
		int actualCount,
		int expectedCount,
		Func<Line, Line, bool> areEqual)
	{
		public int ActualCount { get; } = actualCount;
		public int ExpectedCount { get; } = expectedCount;

		public Line Actual(int index) => actual[offset + index];
		public Line Expected(int index) => expected[offset + index];
		public bool AreEqual(int x, int y) => areEqual(Actual(x), Expected(y));
	}
}
