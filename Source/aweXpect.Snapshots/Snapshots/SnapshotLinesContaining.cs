using System;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Snapshots;

/// <summary>
///     The lines that contain a text, see <see cref="SnapshotLines{TResult}.Containing" />, which also allows setting
///     how the text is matched.
/// </summary>
/// <remarks>
///     The options of aweXpect's <c>Contains</c>, e.g. <c>.IgnoringCase()</c>, <c>.AsWildcard()</c> or <c>.AsRegex()</c>,
///     apply to the text only, not to the comparison with the snapshot. Each option returns a new selection, so a
///     stored selection can be continued more than once and never changes a rule created from it.
/// </remarks>
/// <typeparam name="TResult">What an action returns.</typeparam>
public sealed class SnapshotLinesContaining<TResult> : SnapshotLines<TResult>,
	IOptionsProvider<StringEqualityOptions>,
	IStringPatternMatchTypeOptions,
	ICopyOnOption<StringEqualityOptions, SnapshotLinesContaining<TResult>>
{
	private readonly Func<SnapshotRule, TResult> _add;
	private readonly StringEqualityOptions _options;
	private readonly Func<string, ValueTask<bool>>[] _sourceFilters;
	private readonly Action[] _sourceValidations;
	private readonly string _text;

	internal SnapshotLinesContaining(Func<SnapshotRule, TResult> add, Func<string, ValueTask<bool>>[] sourceFilters,
		Action[] sourceValidations, string text, StringEqualityOptions options)
		: base(add,
			[.. sourceFilters, line => StringRules.Contains(options, line, text),],
			[.. sourceValidations, () => options.ValidateExpected(text),])
	{
		_add = add;
		_sourceFilters = sourceFilters;
		_sourceValidations = sourceValidations;
		_text = text;
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	StringEqualityOptions IOptionsProvider<StringEqualityOptions>.Options => _options;

	/// <inheritdoc cref="ICopyOnOption{TOptions, TResult}.WithOptions(TOptions)" />
	SnapshotLinesContaining<TResult> ICopyOnOption<StringEqualityOptions, SnapshotLinesContaining<TResult>>.WithOptions(
		StringEqualityOptions options)
		=> new(_add, _sourceFilters, _sourceValidations, _text, options);
}
