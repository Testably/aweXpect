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

	public bool? AcceptChanges { get; set; }

	public SnapshotId Id => new(_sourceFile, _memberName, _name)
	{
		Variant = _variant,
	};

	public List<SnapshotIgnoring> Ignorings { get; } = new();

	public List<SnapshotMasking> Maskings { get; } = new();

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
	///     Removes each line of the <paramref name="value" /> that a line rule of <see cref="Ignorings" /> or of the
	///     <paramref name="globalIgnorings" /> matches.
	/// </summary>
	/// <remarks>
	///     A line ends with <c>\r\n</c>, <c>\n</c> or <c>\r</c>. A removed line takes its line break with it; when the last
	///     line is removed, the line break of the last remaining line goes as well, so that no empty line is left at the
	///     end. The other lines keep their line breaks.<br />
	///     Every rule is validated first, so that an invalid pattern throws, whichever lines the subject has.
	/// </remarks>
	public async ValueTask<string> RemoveLines(string value, IReadOnlyList<SnapshotIgnoring> globalIgnorings)
	{
		SnapshotIgnoring[] rules = Ignorings.Concat(globalIgnorings).Where(ignoring => ignoring.IsLineRule).ToArray();
		if (rules.Length == 0)
		{
			return value;
		}

		Array.ForEach(rules, rule => rule.ValidateLineRule());
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
			isLastLineRemoved = await IsRemoved(value.Substring(start, end - start), rules);
			if (!isLastLineRemoved)
			{
				result.Append(value, start, next - start);
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

	private static async ValueTask<bool> IsRemoved(string line, SnapshotIgnoring[] rules)
	{
		foreach (SnapshotIgnoring rule in rules)
		{
			if (await rule.MatchesLine(line))
			{
				return true;
			}
		}

		return false;
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
		SnapshotIgnoring[] ignorings = Ignorings.Concat(globalIgnorings).Where(ignoring => !ignoring.IsLineRule)
			.ToArray();
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
