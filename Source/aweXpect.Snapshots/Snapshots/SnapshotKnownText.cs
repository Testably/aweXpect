using System;

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

	internal SnapshotKnownText(Func<SnapshotRule, TResult> add, Func<string?, TextMasking> createMasking)
	{
		_add = add;
		_createMasking = createMasking;
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
		=> _add(new SnapshotRule(_createMasking(null)));

	/// <summary>
	///     Replaces each selected text with the <paramref name="template" />, which is inserted literally, or with
	///     <c>{0}</c> replaced by the number of the text, e.g. <c>&lt;guid-{0}&gt;</c>.
	/// </summary>
	/// <remarks>
	///     <c>{0}</c> is replaced as plain text, and every text masked with the same template shares one counter.
	/// </remarks>
	public TResult Mask(string template)
		=> _add(new SnapshotRule(_createMasking(RuleArguments.ThrowIfNull(template, nameof(template)))));
}
