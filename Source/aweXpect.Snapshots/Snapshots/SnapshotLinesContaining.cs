using System;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Snapshots;

/// <summary>
///     The lines that contain a text, see <see cref="SnapshotLines{TReturn}.Containing" />, which also allows setting
///     how the text is matched.
/// </summary>
/// <remarks>
///     The options of aweXpect's <c>Contains</c>, e.g. <c>.IgnoringCase()</c>, <c>.AsWildcard()</c> or <c>.AsRegex()</c>,
///     apply to the text only, not to the comparison with the snapshot.
/// </remarks>
/// <typeparam name="TReturn">What an action returns.</typeparam>
public sealed class SnapshotLinesContaining<TReturn> : SnapshotLines<TReturn>,
	IOptionsProvider<StringEqualityOptions>,
	IStringPatternMatchTypeOptions
{
	private readonly StringEqualityOptions _options;

	internal SnapshotLinesContaining(Func<SnapshotRule, TReturn> add, Func<string, ValueTask<bool>>[] filters,
		Action[] validations, StringEqualityOptions options)
		: base(add, filters, validations)
	{
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;
}
