using System.Collections.Generic;
using System.Linq;

namespace aweXpect.Docs.Tests;

/// <summary>
///     A <c>```csharp</c> code block of a documentation page, starting at the one-based <paramref name="Line" />.
/// </summary>
/// <param name="Expect">
///     The comma-separated diagnostic ids that the block shows on purpose, from an <c>expect=</c> marker, or empty
///     when the block has to compile cleanly.
/// </param>
internal sealed record CodeBlock(string Page, int Line, string Code, string Expect = "")
{
	/// <summary>
	///     The info-string marker that excludes a block from the compile check.
	/// </summary>
	public const string NoCompile = "no-compile";

	/// <summary>
	///     The info-string prefix of the diagnostic ids that a block reports on purpose, e.g.
	///     <c>expect=aweXpect0001</c>.
	/// </summary>
	public const string ExpectPrefix = "expect=";

	/// <summary>
	///     The distinct diagnostic ids of the <see cref="Expect" /> marker.
	/// </summary>
	public IReadOnlyCollection<string> ExpectedIds
		=> Expect.Split(',').Where(id => id.Length > 0).Distinct().ToArray();

	/// <summary>
	///     Whether the one-based <paramref name="line" /> of the <paramref name="page" /> belongs to this block.
	/// </summary>
	public bool Contains(string page, int line)
		=> page == Page && line >= Line && line < Line + Code.Split('\n').Length;

	/// <summary>
	///     Extracts the <c>```csharp</c> blocks from the <paramref name="lines" /> of the <paramref name="page" />,
	///     except the ones marked with <see cref="NoCompile" />.
	/// </summary>
	public static List<CodeBlock> Extract(string page, string[] lines)
		=> Extract(page, lines, false);

	/// <summary>
	///     Extracts all <c>```csharp</c> blocks from the <paramref name="lines" /> of the <paramref name="page" />,
	///     including the ones marked with <see cref="NoCompile" />.
	/// </summary>
	public static List<CodeBlock> ExtractAll(string page, string[] lines)
		=> Extract(page, lines, true);

	private static List<CodeBlock> Extract(string page, string[] lines, bool includeNoCompile)
	{
		List<CodeBlock> blocks = [];
		for (int i = 0; i < lines.Length; i++)
		{
			string fence = lines[i].TrimStart();
			if (!fence.StartsWith("```"))
			{
				continue;
			}

			string[] info = fence.Substring(3).Split(' ');
			int start = i + 1;
			do
			{
				i++;
			} while (i < lines.Length && lines[i].Trim() != "```");

			if (info[0] == "csharp" && (includeNoCompile || !info.Contains(NoCompile)))
			{
				string expect = info.FirstOrDefault(part => part.StartsWith(ExpectPrefix))?.Substring(ExpectPrefix.Length)
				                ?? "";
				blocks.Add(new CodeBlock(page, start + 1, string.Join("\n", lines, start, i - start), expect));
			}
		}

		return blocks;
	}
}
