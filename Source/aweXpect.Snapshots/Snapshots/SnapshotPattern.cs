using System;
using aweXpect.Formatting;
using static aweXpect.Formatting.Format;

namespace aweXpect.Snapshots;

/// <summary>
///     The matches of a regex in the written snapshot, see <see cref="SnapshotTextFor{TResult}.Regex" />, which can only
///     be masked.
/// </summary>
/// <typeparam name="TResult">What an action returns.</typeparam>
public class SnapshotPattern<TResult>
{
	private readonly Func<SnapshotRule, TResult> _add;
	private readonly Func<string, TextMasking>? _createMasking;
	private readonly Func<string>? _describeSelection;

	/// <param name="add">Adds the rule of an action.</param>
	/// <param name="createMasking">Creates the masking with a template.</param>
	/// <param name="describeSelection">Describes the selected text for the failure.</param>
	internal SnapshotPattern(Func<SnapshotRule, TResult> add, Func<string, TextMasking>? createMasking,
		Func<string>? describeSelection)
	{
		_add = add;
		_createMasking = createMasking;
		_describeSelection = describeSelection;
	}

	/// <summary>
	///     Replaces each selected text with the <paramref name="template" />, or with <c>{0}</c> replaced by the number
	///     of the text, e.g. <c>{Port_{0}}</c>, where the same text keeps the same number.
	/// </summary>
	/// <remarks>
	///     <c>{0}</c> is replaced as plain text, not as a composite format, and every text masked with the same template
	///     shares one counter. The <paramref name="template" /> of a regex first gets the substitutions of
	///     <see cref="System.Text.RegularExpressions.Regex.Replace(string, string)" />, e.g. <c>$1</c> for the first group
	///     or <c>$$</c> for a <c>$</c>, and then the number, so <c>$1:{0}</c> counts per value of the group; any other
	///     template is inserted literally. An empty <paramref name="template" /> removes the text.<br />
	///     The text rules are applied in the order in which they are added, those of the expectation before those for all
	///     snapshots, so put a specific rule before a general one.
	/// </remarks>
	public TResult Mask(string template)
		=> _add(new SnapshotRule(CreateMasking(RuleArguments.ThrowIfNull(template, nameof(template))),
			() => $"mask {DescribeSelection()} with {Formatter.Format(template)}"));

	private protected virtual TextMasking CreateMasking(string template)
		=> _createMasking!(template);

	private protected virtual string DescribeSelection()
		=> _describeSelection!();
}
