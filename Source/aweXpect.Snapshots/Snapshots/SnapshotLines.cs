using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using aweXpect.Core.Extending;
using aweXpect.Options;

namespace aweXpect.Snapshots;

/// <summary>
///     The lines of a <see cref="string" /> snapshot that a rule applies to, see
///     <see cref="SnapshotFor{TResult}.Lines" />.
/// </summary>
/// <remarks>
///     The filters are combined, so a line has to match all of them. Without a filter, the rule applies to every line.
/// </remarks>
/// <typeparam name="TResult">What an action returns.</typeparam>
public class SnapshotLines<TResult>
{
	private readonly Func<SnapshotRule, TResult> _add;
	private readonly Func<string>[] _descriptions;
	private readonly Func<string, ValueTask<bool>>[] _filters;
	private readonly Action[] _validations;

	/// <param name="add">Adds the rule of an action.</param>
	/// <param name="filters">The filters that a line has to match.</param>
	/// <param name="validations">Throw when a filter cannot be applied.</param>
	/// <param name="descriptions">Describe the <paramref name="filters" /> for the failure.</param>
	internal SnapshotLines(Func<SnapshotRule, TResult> add, Func<string, ValueTask<bool>>[] filters,
		Action[]? validations = null, Func<string>[]? descriptions = null)
	{
		_add = add;
		_filters = filters;
		_validations = validations ?? [];
		_descriptions = descriptions ?? [];
	}

	/// <summary>
	///     Filters for the lines that contain the <paramref name="text" />, e.g. <c>"elapsed"</c>.
	/// </summary>
	/// <remarks>
	///     The <paramref name="text" /> can be anywhere in the line and is compared ordinally, like in aweXpect's
	///     <c>Contains</c>. The options that follow apply to this filter only, not to the comparison with the snapshot:
	///     <c>.IgnoringCase()</c> ignores the case of the <paramref name="text" />, and <c>.AsWildcard()</c> or
	///     <c>.AsRegex()</c> interpret it as a pattern, with the timeout of
	///     <c>Customize.aweXpect.Settings().DefaultRegexTimeout</c>. An invalid pattern throws an
	///     <see cref="ArgumentException" /> at the action, and a pattern that times out fails the expectation.
	/// </remarks>
	public SnapshotLinesContaining<TResult> Containing(string text)
		=> new(_add, _filters, _validations, _descriptions, RuleArguments.ThrowIfNullOrEmpty(text, nameof(text)),
			StringEqualityOptions.Default);

	/// <summary>
	///     Filters for the lines for which the <paramref name="predicate" /> returns <see langword="true" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="predicate" /> gets each line without its line break. An exception of the
	///     <paramref name="predicate" /> is thrown.
	/// </remarks>
	public SnapshotLines<TResult> Which(Func<string, bool> predicate,
		[CallerArgumentExpression("predicate")] string doNotPopulateThisValue = "")
	{
		RuleArguments.ThrowIfNull(predicate, nameof(predicate));
		return new SnapshotLines<TResult>(_add, [.. _filters, line => new ValueTask<bool>(predicate(line)),],
			_validations, [.. _descriptions, () => "satisfying " + doNotPopulateThisValue.TrimCommonWhiteSpace(),]);
	}

	/// <summary>
	///     Filters for the lines for which the <paramref name="predicate" /> returns <see langword="false" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="predicate" /> is asked like in <see cref="Which" />.
	/// </remarks>
	public SnapshotLines<TResult> Except(Func<string, bool> predicate,
		[CallerArgumentExpression("predicate")] string doNotPopulateThisValue = "")
	{
		RuleArguments.ThrowIfNull(predicate, nameof(predicate));
		return new SnapshotLines<TResult>(_add, [.. _filters, line => new ValueTask<bool>(!predicate(line)),],
			_validations,
			[.. _descriptions, () => "not satisfying " + doNotPopulateThisValue.TrimCommonWhiteSpace(),]);
	}

	/// <summary>
	///     Leaves out the selected lines.
	/// </summary>
	/// <remarks>
	///     A removed line takes its line break with it, and removing the last line also removes the line break before it;
	///     the other lines keep theirs. It wins over every masking and converter of lines.
	/// </remarks>
	public TResult Ignore()
		=> Add(LineRule.Ignore(_filters, _descriptions));

	/// <summary>
	///     Replaces each selected line as a whole with the <paramref name="template" />, e.g.
	///     <c>Now playing: &lt;track&gt;</c>, or with <c>{0}</c> replaced by the number of the line, e.g.
	///     <c>Now playing: {Track_{0}}</c>, where equal lines keep the same number.
	/// </summary>
	/// <remarks>
	///     <c>{0}</c> is replaced as plain text, and every text masked with the same template shares one counter. The line
	///     keeps its line break. The first masking or converter that matches a line wins.
	/// </remarks>
	public TResult Mask(string template)
		=> Add(LineRule.Mask(_filters, _descriptions, RuleArguments.ThrowIfNull(template, nameof(template))));

	/// <summary>
	///     Replaces each selected line with the text that <paramref name="convert" /> returns for it, e.g. only its
	///     beginning, or leaves it out when it returns <see langword="null" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="convert" /> gets the line without its line break, and the result keeps the line break. An
	///     exception of <paramref name="convert" /> is thrown. The first masking or converter that matches a line wins.
	/// </remarks>
	public TResult Convert(Func<string, string?> convert,
		[CallerArgumentExpression("convert")] string doNotPopulateThisValue = "")
		=> Add(LineRule.Convert(_filters, _descriptions, RuleArguments.ThrowIfNull(convert, nameof(convert)),
			"using " + doNotPopulateThisValue.TrimCommonWhiteSpace()));

	private TResult Add(LineRule rule)
	{
		Array.ForEach(_validations, validate => validate());
		return _add(new SnapshotRule(rule));
	}
}
