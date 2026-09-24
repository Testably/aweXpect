using System.Collections.Generic;
using System.Linq;

namespace aweXpect.Docs.Tests;

/// <summary>
///     A <c>```csharp</c> code block of a documentation page, starting at the one-based <paramref name="Line" />.
/// </summary>
internal sealed record CodeBlock(string Page, int Line, string Code)
{
	/// <summary>
	///     The info-string marker that excludes a block from the compile check.
	/// </summary>
	public const string NoCompile = "no-compile";

	/// <summary>
	///     Extracts the <c>```csharp</c> blocks from the <paramref name="lines" /> of the <paramref name="page" />,
	///     except the ones marked with <see cref="NoCompile" />.
	/// </summary>
	public static List<CodeBlock> Extract(string page, string[] lines)
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

			if (info[0] == "csharp" && !info.Contains(NoCompile))
			{
				blocks.Add(new CodeBlock(page, start + 1, string.Join("\n", lines, start, i - start)));
			}
		}

		return blocks;
	}
}
