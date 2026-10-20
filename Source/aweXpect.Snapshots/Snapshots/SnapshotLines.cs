using System;
using System.Threading.Tasks;
using aweXpect.Options;

namespace aweXpect.Snapshots;

/// <summary>
///     The lines of a <see cref="string" /> snapshot that a rule applies to, see
///     <see cref="SnapshotFor{TReturn}.Lines" />.
/// </summary>
/// <remarks>
///     The filters are combined, so a line has to match all of them. Without a filter, the rule applies to every line.
/// </remarks>
/// <typeparam name="TReturn">What an action returns.</typeparam>
public class SnapshotLines<TReturn>
{
	private readonly Func<SnapshotRule, TReturn> _add;
	private readonly Func<string, ValueTask<bool>>[] _filters;
	private readonly Action[] _validations;

	internal SnapshotLines(Func<SnapshotRule, TReturn> add, Func<string, ValueTask<bool>>[] filters,
		Action[]? validations = null)
	{
		_add = add;
		_filters = filters;
		_validations = validations ?? [];
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
	public SnapshotLinesContaining<TReturn> Containing(string text)
	{
		RuleArguments.ThrowIfNullOrEmpty(text, nameof(text));
		StringEqualityOptions options = new(nameof(text));
		return new SnapshotLinesContaining<TReturn>(_add,
			[.. _filters, line => StringRules.Contains(options, line, text),],
			[.. _validations, () => options.ValidateExpected(text),],
			options);
	}

	/// <summary>
	///     Filters for the lines for which the <paramref name="predicate" /> returns <see langword="true" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="predicate" /> gets each line without its line break. An exception of the
	///     <paramref name="predicate" /> is thrown.
	/// </remarks>
	public SnapshotLines<TReturn> Which(Func<string, bool> predicate)
	{
		RuleArguments.ThrowIfNull(predicate, nameof(predicate));
		return new SnapshotLines<TReturn>(_add, [.. _filters, line => new ValueTask<bool>(predicate(line)),],
			_validations);
	}

	/// <summary>
	///     Filters for the lines for which the <paramref name="predicate" /> returns <see langword="false" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="predicate" /> is asked like in <see cref="Which" />.
	/// </remarks>
	public SnapshotLines<TReturn> Except(Func<string, bool> predicate)
	{
		RuleArguments.ThrowIfNull(predicate, nameof(predicate));
		return new SnapshotLines<TReturn>(_add, [.. _filters, line => new ValueTask<bool>(!predicate(line)),],
			_validations);
	}

	/// <summary>
	///     Leaves out the selected lines.
	/// </summary>
	/// <remarks>
	///     A removed line takes its line break with it, and removing the last line also removes the line break before it;
	///     the other lines keep theirs. It wins over every masking and converter of lines.
	/// </remarks>
	public TReturn Ignore()
		=> Add(LineRule.Ignore(_filters));

	/// <summary>
	///     Replaces each selected line as a whole with the <paramref name="template" />, e.g.
	///     <c>Now playing: &lt;track&gt;</c>, or with <c>{0}</c> replaced by the number of the line, e.g.
	///     <c>Now playing: {Track_{0}}</c>, where equal lines keep the same number.
	/// </summary>
	/// <remarks>
	///     <c>{0}</c> is replaced as plain text, and every text masked with the same template shares one counter. The line
	///     keeps its line break. The first masking or converter that matches a line wins.
	/// </remarks>
	public TReturn Mask(string template)
		=> Add(LineRule.Mask(_filters, RuleArguments.ThrowIfNull(template, nameof(template))));

	/// <summary>
	///     Replaces each selected line with the text that <paramref name="convert" /> returns for it, e.g. only its
	///     beginning, or leaves it out when it returns <see langword="null" />.
	/// </summary>
	/// <remarks>
	///     The <paramref name="convert" /> gets the line without its line break, and the result keeps the line break. An
	///     exception of <paramref name="convert" /> is thrown. The first masking or converter that matches a line wins.
	/// </remarks>
	public TReturn Convert(Func<string, string?> convert)
		=> Add(LineRule.Convert(_filters, RuleArguments.ThrowIfNull(convert, nameof(convert))));

	private TReturn Add(LineRule rule)
	{
		Array.ForEach(_validations, validate => validate());
		return _add(new SnapshotRule(rule));
	}
}
