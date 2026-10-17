using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Snapshots;

/// <summary>
///     Leaves out each line of a <see cref="string" /> snapshot that contains a text, see
///     <see cref="SnapshotIgnoring.LinesContaining(string)" />.
/// </summary>
/// <remarks>
///     The options of aweXpect's <c>Contains</c>, e.g. <c>.IgnoringCase()</c>, <c>.AsWildcard()</c> or <c>.AsRegex()</c>,
///     apply to this line rule only, not to the comparison with the snapshot.
/// </remarks>
public sealed class SnapshotIgnoringLinesContaining : SnapshotIgnoring,
	IOptionsProvider<StringEqualityOptions>,
	IStringPatternMatchTypeOptions
{
	private readonly string _text;

	internal SnapshotIgnoringLinesContaining(string text)
	{
		_text = text;
	}

	/// <summary>
	///     The options that match the text in a line.
	/// </summary>
	internal StringEqualityOptions Options { get; } = new("text");

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => Options;

	internal override void ValidateLineRule()
		=> Options.ValidateExpected(_text);

	/// <remarks>
	///     A pattern that times out is reported with the <see cref="RegexMatchTimeoutException" /> for the text of the
	///     rule, like the timeout of a masking, instead of the <see cref="ArgumentException" /> of aweXpect's
	///     <c>Contains</c>.
	/// </remarks>
	internal override async ValueTask<bool> MatchesLine(string line)
	{
		try
		{
			return await Options.CountOccurrences(line, _text, 1) > 0;
		}
		catch (ArgumentException exception) when (exception.InnerException is RegexMatchTimeoutException timeout)
		{
			throw new RegexMatchTimeoutException(line, _text, timeout.MatchTimeout);
		}
	}
}
