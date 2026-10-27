using System;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Formatting;
using aweXpect.Options;
using aweXpect.Results;
using static aweXpect.Formatting.Format;

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
	private readonly OptionsHolder<StringEqualityOptions> _options;
	private readonly Func<string>[] _sourceDescriptions;
	private readonly Func<string, ValueTask<bool>>[] _sourceFilters;
	private readonly Action[] _sourceValidations;
	private readonly string _text;

	internal SnapshotLinesContaining(Func<SnapshotRule, TResult> add, Func<string, ValueTask<bool>>[] sourceFilters,
		Action[] sourceValidations, Func<string>[] sourceDescriptions, string text, StringEqualityOptions options)
		: this(add, sourceFilters, sourceValidations, sourceDescriptions, text,
			new OptionsHolder<StringEqualityOptions>(options))
	{
	}

	/// <remarks>
	///     The filter and the validation read the <paramref name="options" /> from the holder, so that an option that
	///     changes the holder directly also applies.
	/// </remarks>
	private SnapshotLinesContaining(Func<SnapshotRule, TResult> add, Func<string, ValueTask<bool>>[] sourceFilters,
		Action[] sourceValidations, Func<string>[] sourceDescriptions, string text,
		OptionsHolder<StringEqualityOptions> options)
		: base(add,
			[.. sourceFilters, line => StringRules.Contains(options.Value, line, text, nameof(text)),],
			[.. sourceValidations, () => options.Value.ValidateExpected(text, nameof(text)),],
			[.. sourceDescriptions, () => "containing " + Formatter.Format(text) + options.Value,])
	{
		_add = add;
		_sourceFilters = sourceFilters;
		_sourceValidations = sourceValidations;
		_sourceDescriptions = sourceDescriptions;
		_text = text;
		_options = options;
	}

	/// <inheritdoc cref="IOptionsProvider{TOptions}.Options" />
	OptionsHolder<StringEqualityOptions> IOptionsProvider<StringEqualityOptions>.Options => _options;

	/// <inheritdoc cref="ICopyOnOption{TOptions, TSelf}.WithOptions(TOptions)" />
	SnapshotLinesContaining<TResult> ICopyOnOption<StringEqualityOptions, SnapshotLinesContaining<TResult>>.WithOptions(
		StringEqualityOptions options)
		=> new(_add, _sourceFilters, _sourceValidations, _sourceDescriptions, _text, options);
}
