using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aweXpect.Formatting;

namespace aweXpect.Snapshots;

/// <summary>
///     The options of a snapshot expectation, which are set after it was added and read when it is evaluated.
/// </summary>
internal sealed class SnapshotOptions
{
	private static readonly char[] LineBreaks = ['\r', '\n',];
	private readonly string _memberName;
	private readonly string _sourceFile;
	private readonly string?[] _variantParts = new string?[3];
	private string _extension = SnapshotId.DefaultExtension;
	private string? _name;
	private string? _variant;

	/// <remarks>
	///     The snapshot is claimed right away, see <see cref="ClaimedSnapshots" />.
	/// </remarks>
	public SnapshotOptions(string sourceFile, string memberName)
	{
		_sourceFile = sourceFile;
		_memberName = memberName;
		ClaimedSnapshots.Claim(Id);
	}

	public SnapshotAcceptance? Acceptance { get; set; }

	public SnapshotId Id => new(_sourceFile, _memberName, _name)
	{
		Variant = _variant,
		Extension = _extension,
	};

	/// <remarks>
	///     A new extension moves the claim of the snapshot like a new name, so that a file with the previous extension is
	///     an orphan.
	/// </remarks>
	public string Extension
	{
		get => _extension;
		set
		{
			ClaimedSnapshots.Release(Id);
			_extension = value;
			ClaimedSnapshots.Claim(Id);
		}
	}

	/// <summary>
	///     The rules of the expectation, in the order in which they were added.
	/// </summary>
	public List<SnapshotRule> Rules { get; } = new();

	/// <remarks>
	///     A new name moves the claim of the snapshot to the named one.
	/// </remarks>
	public string? Name
	{
		get => _name;
		set
		{
			ClaimedSnapshots.Release(Id);
			_name = value;
			ClaimedSnapshots.Claim(Id);
		}
	}

	/// <summary>
	///     Makes the snapshot unique for the <paramref name="value" /> of the <paramref name="part" /> of the environment.
	/// </summary>
	/// <remarks>
	///     The parts of the variant keep the order of <see cref="SnapshotEnvironment.Part" />, whatever the order of the
	///     calls, and a new variant moves the claim of the snapshot like a new name.
	/// </remarks>
	public void UniqueFor(SnapshotEnvironment.Part part, string value)
	{
		ClaimedSnapshots.Release(Id);
		_variantParts[(int)part] = value;
		_variant = string.Join(SnapshotEnvironment.Separator.ToString(), _variantParts.OfType<string>());
		ClaimedSnapshots.Claim(Id);
	}

	/// <summary>
	///     Applies the text rules of <see cref="Rules" /> in the order in which they were added, then those of the
	///     <paramref name="globalRules" /> in their order, so that a specific rule of the expectation goes before a general
	///     one for all snapshots.
	/// </summary>
	/// <remarks>
	///     The rules of the environment come first, the paths unless a rule keeps them, so that the other rules see their
	///     placeholders.
	/// </remarks>
	public string Mask(string value, IReadOnlyList<SnapshotRule> globalRules, SnapshotNumbering numbering)
	{
		SnapshotRule[] rules = Rules.Concat(globalRules).ToArray();
		TextMasking[] maskings = rules.Select(rule => rule.TextMasking).OfType<TextMasking>().ToArray();
		IEnumerable<TextMasking> paths = rules.Any(rule => rule.KeepsPaths)
			? []
			: EnvironmentMaskings.ForPaths(_sourceFile);
		return paths
			.Concat(maskings.Where(masking => masking.IsEnvironment))
			.Concat(maskings.Where(masking => !masking.IsEnvironment))
			.Aggregate(value, (masked, masking) => masking.Apply(masked, numbering));
	}

	/// <summary>
	///     Leaves out, masks or converts each line of the <paramref name="value" /> that a line rule of
	///     <see cref="Rules" /> or of the <paramref name="globalRules" /> matches.
	/// </summary>
	/// <remarks>
	///     A line ends with <c>\r\n</c>, <c>\n</c> or <c>\r</c>. A line rule that leaves lines out wins, otherwise the
	///     first matching rule replaces the line, which keeps its line break. A removed line takes its line break with it;
	///     when the last line is removed, the line break of the last remaining line goes as well, so that no empty line is
	///     left at the end. The other lines keep their line breaks.
	/// </remarks>
	public async ValueTask<string> ApplyLineRules(string value, IReadOnlyList<SnapshotRule> globalRules,
		SnapshotNumbering numbering)
	{
		LineRule[] rules = Rules.Concat(globalRules).Select(rule => rule.LineRule).OfType<LineRule>()
			.OrderByDescending(rule => rule.IsIgnoring).ToArray();
		if (rules.Length == 0)
		{
			return value;
		}

		StringBuilder result = new(value.Length);
		int lastLineBreakLength = 0;
		bool isLastLineRemoved;
		int start = 0;
		int lineBreak;
		do
		{
			lineBreak = value.IndexOfAny(LineBreaks, start);
			int end = lineBreak < 0 ? value.Length : lineBreak;
			int next = lineBreak < 0 ? end : end + GetLineBreakLength(value, lineBreak);
			string? line = await ApplyLineRules(value.Substring(start, end - start), rules, numbering);
			isLastLineRemoved = line is null;
			if (line is not null)
			{
				result.Append(line).Append(value, end, next - end);
				lastLineBreakLength = next - end;
			}

			start = next;
		} while (lineBreak >= 0);

		if (isLastLineRemoved)
		{
			result.Length -= lastLineBreakLength;
		}

		return result.ToString();
	}

	private static int GetLineBreakLength(string value, int lineBreak)
		=> value[lineBreak] == '\r' && lineBreak + 1 < value.Length && value[lineBreak + 1] == '\n' ? 2 : 1;

	/// <remarks>
	///     The <paramref name="rules" /> that leave lines out come first, so that they win.
	/// </remarks>
	private static async ValueTask<string?> ApplyLineRules(string line, LineRule[] rules,
		SnapshotNumbering numbering)
	{
		foreach (LineRule rule in rules)
		{
			if (await rule.Matches(line))
			{
				return rule.IsIgnoring ? null : rule.Replace(line, numbering);
			}
		}

		return line;
	}

	/// <summary>
	///     Creates the decision of the formatter for each value nested in an object snapshot from the member and value
	///     rules of the expectation and the global ones, or returns <see langword="null" /> when there is none, so that the
	///     formatter does not offer the values at all.
	/// </summary>
	/// <remarks>
	///     A rule that leaves values out wins over one that masks or converts them, the rules of the expectation come
	///     before the global ones, and the first masking or converter that matches a value replaces it.
	/// </remarks>
	public Func<FormattedValue, ValueFormatting>? CreateFormatValue(IReadOnlyList<SnapshotRule> globalRules,
		SnapshotNumbering numbering)
	{
		ValueRule[] rules = Rules.Concat(globalRules).Select(rule => rule.ValueRule).OfType<ValueRule>().ToArray();
		if (rules.Length == 0)
		{
			return null;
		}

		ValueRule[] ignorings = rules.Where(rule => rule.IsIgnoring).ToArray();
		ValueRule[] replacements = rules.Where(rule => !rule.IsIgnoring).ToArray();
		return value =>
		{
			if (ignorings.Any(rule => rule.Ignores(value)))
			{
				return ValueFormatting.Omit;
			}

			return replacements.Select(rule => rule.GetReplacement(value, numbering))
				.FirstOrDefault(replacement => replacement is not null) ?? ValueFormatting.Write;
		};
	}
}
