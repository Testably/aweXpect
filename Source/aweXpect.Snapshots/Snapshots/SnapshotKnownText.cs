using System;
using aweXpect.Formatting;
using static aweXpect.Formatting.Format;

namespace aweXpect.Snapshots;

/// <summary>
///     A kind of text in the written snapshot that a rule applies to, e.g. every <see cref="Guid" />, see
///     <see cref="SnapshotTextFor{TResult}.Guids" />, <see cref="SnapshotTextFor{TResult}.Dates" />,
///     <see cref="SnapshotTextFor{TResult}.MachineName" /> and <see cref="SnapshotTextFor{TResult}.UserName" />.
/// </summary>
/// <typeparam name="TResult">What an action returns.</typeparam>
public sealed class SnapshotKnownText<TResult>
{
	private readonly Func<SnapshotRule, TResult> _add;
	private readonly Func<string?, TextMasking> _createMasking;
	private readonly string _selection;

	/// <param name="add">Adds the rule of an action.</param>
	/// <param name="createMasking">Creates the masking with a template, or with the default one.</param>
	/// <param name="selection">Describes the selected text for the failure, e.g. <c>the Guids</c>.</param>
	internal SnapshotKnownText(Func<SnapshotRule, TResult> add, Func<string?, TextMasking> createMasking,
		string selection)
	{
		_add = add;
		_createMasking = createMasking;
		_selection = selection;
	}

	/// <summary>
	///     Replaces each selected text with its placeholder: a numbered one for a <see cref="Guid" /> or a date, e.g.
	///     <c>{Guid_1}</c>, where the same value keeps the same number, or <c>{MachineName}</c> and <c>{UserName}</c>.
	/// </summary>
	/// <remarks>
	///     It is the shorthand for <c>Mask("{Guid_{0}}")</c>, <c>Mask("{Date_{0}}")</c>, <c>Mask("{MachineName}")</c> and
	///     <c>Mask("{UserName}")</c>. The numbers are counted per subject in the order of appearance, after the rules that
	///     come before.
	/// </remarks>
	public TResult Mask()
		=> _add(new SnapshotRule(_createMasking(null), () => $"mask {_selection}"));

	/// <summary>
	///     Replaces each selected text with the <paramref name="template" />, which is inserted literally, or with
	///     <c>{0}</c> replaced by the number of the text, e.g. <c>&lt;guid-{0}&gt;</c>.
	/// </summary>
	/// <remarks>
	///     <c>{0}</c> is replaced as plain text, and every text masked with the same template shares one counter.
	/// </remarks>
	public TResult Mask(string template)
		=> _add(new SnapshotRule(_createMasking(RuleArguments.ThrowIfNull(template, nameof(template))),
			() => $"mask {_selection} with {Formatter.Format(template)}"));
}
