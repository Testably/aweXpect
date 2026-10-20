using System;
using System.Threading.Tasks;

namespace aweXpect.Snapshots;

/// <summary>
///     Leaves out, masks or converts the lines of a <see cref="string" /> snapshot that all its filters match.
/// </summary>
internal sealed class LineRule
{
	private readonly Func<string, string?>? _convert;
	private readonly Func<string, ValueTask<bool>>[] _filters;
	private readonly string? _template;

	private LineRule(Func<string, ValueTask<bool>>[] filters, bool isIgnoring, string? template,
		Func<string, string?>? convert)
	{
		_filters = filters;
		IsIgnoring = isIgnoring;
		_template = template;
		_convert = convert;
	}

	public bool IsIgnoring { get; }

	public static LineRule Ignore(Func<string, ValueTask<bool>>[] filters)
		=> new(filters, true, null, null);

	public static LineRule Mask(Func<string, ValueTask<bool>>[] filters, string template)
		=> new(filters, false, template, null);

	public static LineRule Convert(Func<string, ValueTask<bool>>[] filters, Func<string, string?> convert)
		=> new(filters, false, null, convert);

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
