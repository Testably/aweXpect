using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;

namespace aweXpect.Core;

/// <summary>
///     Appends the contexts of the parts of a failure that explain it to the failure message.
/// </summary>
internal static class ResultContextRenderer
{
	/// <summary>
	///     Appends the contexts of the <paramref name="failure" /> to the <paramref name="stringBuilder" />.
	/// </summary>
	/// <remarks>
	///     A context of a member is labelled with it, e.g. <c>Collection (Items):</c>, and the contexts of an item of a
	///     collection follow those of the collection. Contexts with the same title and member are shown once when their
	///     content is the same, and are numbered otherwise, e.g. <c>Expected #1:</c>.
	/// </remarks>
	public static async Task AppendContexts(StringBuilder stringBuilder, ConstraintResult failure,
		CancellationToken cancellationToken)
	{
		ResultContextCollector collector = new();
		collector.Visit(failure);
		IEnumerable<(ResultContext Context, string? Subject, string TitlePrefix)> candidates = collector.Entries
			.Select(entry => (entry.Context, entry.GetSubjectLabel(), entry.TitlePrefix, entry.IsOfItem))
			.OrderBy(candidate => candidate.Item4)
			.ThenByDescending(candidate => candidate.Item1.Priority)
			.Select(candidate => (candidate.Item1, candidate.Item2, candidate.Item3));

		List<Block> blocks = [];
		foreach ((ResultContext context, string? subject, string titlePrefix) in candidates)
		{
			string? content = await context.GetContentUnlessUserCodeThrows(cancellationToken);
			// The title is read after the content, as a context can choose its title while it creates the content.
			if (content is not null)
			{
				AddUnlessDuplicate(blocks, new Block(titlePrefix + context.Title, subject, content));
			}
		}

		foreach (Block block in blocks)
		{
			stringBuilder.AppendLine().AppendLine();
			stringBuilder.Append(block.Title);
			if (block.Subject is not null)
			{
				stringBuilder.Append(" (").Append(block.Subject).Append(')');
			}

			if (block.Number > 0)
			{
				stringBuilder.Append(" #").Append(block.Number);
			}

			stringBuilder.Append(':').AppendLine();
			stringBuilder.Append(block.Content);
		}
	}

	private static void AddUnlessDuplicate(List<Block> blocks, Block block)
	{
		List<Block> sameTitle = blocks.Where(other => other.Title == block.Title && other.Subject == block.Subject)
			.ToList();
		if (sameTitle.Any(other => other.Content == block.Content))
		{
			return;
		}

		if (sameTitle.Count > 0)
		{
			if (sameTitle[0].Number == 0)
			{
				sameTitle[0].Number = 1;
			}

			block.Number = sameTitle.Count + 1;
		}

		blocks.Add(block);
	}

	private sealed class Block(string title, string? subject, string content)
	{
		public string Title { get; } = title;
		public string? Subject { get; } = subject;
		public string Content { get; } = content;
		public int Number { get; set; }
	}
}
