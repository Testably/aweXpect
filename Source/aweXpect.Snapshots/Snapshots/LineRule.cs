using System;
using System.Linq;
using System.Threading.Tasks;
using aweXpect.Formatting;
using static aweXpect.Formatting.Format;

namespace aweXpect.Snapshots;

/// <summary>
///     Leaves out, masks or converts the lines of a <see cref="string" /> snapshot that all its filters match.
/// </summary>
internal sealed class LineRule
{
	private readonly string _action;
	private readonly Func<string, string?>? _convert;
	private readonly Func<string>[] _descriptions;
	private readonly Func<string, ValueTask<bool>>[] _filters;
	private readonly string? _how;
	private readonly string? _template;

	private LineRule(Func<string, ValueTask<bool>>[] filters, Func<string>[] descriptions, bool isIgnoring,
		string? template, Func<string, string?>? convert, string action, string? how)
	{
		_filters = filters;
		_descriptions = descriptions;
		IsIgnoring = isIgnoring;
		_template = template;
		_convert = convert;
		_action = action;
		_how = how;
	}

	public bool IsIgnoring { get; }

	/// <param name="filters">The filters that a line has to match.</param>
	/// <param name="descriptions">Describe the <paramref name="filters" /> for the failure.</param>
	public static LineRule Ignore(Func<string, ValueTask<bool>>[] filters, Func<string>[] descriptions)
		=> new(filters, descriptions, true, null, null, "ignore", null);

	/// <param name="filters">The filters that a line has to match.</param>
	/// <param name="descriptions">Describe the <paramref name="filters" /> for the failure.</param>
	/// <param name="template">The mask of a line.</param>
	public static LineRule Mask(Func<string, ValueTask<bool>>[] filters, Func<string>[] descriptions,
		string template)
		=> new(filters, descriptions, false, template, null, "mask", "with " + Formatter.Format(template));

	/// <param name="filters">The filters that a line has to match.</param>
	/// <param name="descriptions">Describe the <paramref name="filters" /> for the failure.</param>
	/// <param name="convert">Converts a line.</param>
	/// <param name="how">
	///     Describes the <paramref name="convert" /> for the failure, e.g. <c>using line =&gt; line.Trim()</c>.
	/// </param>
	public static LineRule Convert(Func<string, ValueTask<bool>>[] filters, Func<string>[] descriptions,
		Func<string, string?> convert, string how)
		=> new(filters, descriptions, false, null, convert, "convert", how);

	/// <summary>
	///     Describes the rule for the failure, e.g. <c>ignore the lines containing "elapsed"</c>.
	/// </summary>
	public string Describe()
	{
		string lines = _descriptions.Length == 0
			? "the lines"
			: "the lines " + string.Join(" and ", _descriptions.Select(describe => describe()));
		return _how is null ? $"{_action} {lines}" : $"{_action} {lines} {_how}";
	}

	/// <summary>
	///     Whether every filter matches the <paramref name="line" />, which has no line break.
	/// </summary>
	public async ValueTask<bool> Matches(string line)
	{
		foreach (Func<string, ValueTask<bool>> filter in _filters)
		{
			if (!await filter(line))
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	///     Returns the text that replaces the matched <paramref name="line" />, or <see langword="null" /> to leave it
	///     out.
	/// </summary>
	public string? Replace(string line, SnapshotNumbering numbering)
		=> _convert is null ? numbering.Apply(_template!, line) : _convert(line);
}
