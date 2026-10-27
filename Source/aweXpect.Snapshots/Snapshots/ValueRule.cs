using System;
using System.Globalization;
using aweXpect.Formatting;
using static aweXpect.Formatting.Format;

namespace aweXpect.Snapshots;

/// <summary>
///     Leaves out, masks or converts the values of a <see cref="ValueSelection" /> while an object snapshot is written.
/// </summary>
internal sealed class ValueRule
{
	private readonly string _action;
	private readonly Func<object, ValueFormatting?>? _convert;
	private readonly string? _how;
	private readonly ValueSelection _selection;
	private readonly Func<FormattedValue, string?>? _template;

	private ValueRule(ValueSelection selection, bool isIgnoring, Func<FormattedValue, string?>? template,
		Func<object, ValueFormatting?>? convert, string action, string? how)
	{
		_selection = selection;
		IsIgnoring = isIgnoring;
		_template = template;
		_convert = convert;
		_action = action;
		_how = how;
	}

	public bool IsIgnoring { get; }

	public static ValueRule Ignore(ValueSelection selection)
		=> new(selection, true, null, null, "ignore", null);

	/// <summary>
	///     Masks the selected values with the mask that the <paramref name="template" /> returns for each, see
	///     <see cref="SnapshotNumbering" />, or keeps a value for which it returns <see langword="null" />.
	/// </summary>
	/// <param name="selection">The selected values.</param>
	/// <param name="template">Returns the mask of each value.</param>
	/// <param name="how">
	///     Describes the <paramref name="template" /> for the failure, e.g. <c>with "&lt;id&gt;"</c>.
	/// </param>
	public static ValueRule Mask(ValueSelection selection, Func<FormattedValue, string?> template, string how)
		=> new(selection, false, template, null, "mask", how);

	/// <summary>
	///     Masks the selected values with a numbered placeholder named after the selection, e.g. <c>{Id_{0}}</c>.
	/// </summary>
	public static ValueRule Mask(ValueSelection selection)
		=> new(selection, false,
			value => SnapshotNumbering.GetTemplate(selection.GetPlaceholderName(value)), null, "mask", null);

	/// <summary>
	///     Replaces the selected values with the result of <paramref name="convert" />, or keeps them when it returns
	///     <see langword="null" />, e.g. for a value of another type.
	/// </summary>
	/// <param name="selection">The selected values.</param>
	/// <param name="convert">Converts each value.</param>
	/// <param name="how">
	///     Describes the <paramref name="convert" /> for the failure, e.g. <c>using date =&gt; date.Year</c>.
	/// </param>
	public static ValueRule Convert(ValueSelection selection, Func<object, ValueFormatting?> convert, string how)
		=> new(selection, false, null, convert, "convert", how);

	/// <summary>
	///     Describes the rule for the failure, e.g. <c>ignore the members whose name is equal to "PlayCount"</c>.
	/// </summary>
	public string Describe()
		=> _how is null
			? $"{_action} {_selection.Describe()}"
			: $"{_action} {_selection.Describe()} {_how}";

	public bool Ignores(FormattedValue value)
		=> IsIgnoring && _selection.Matches(value);

	/// <summary>
	///     Returns how the <paramref name="value" /> is written instead, or <see langword="null" /> when the rule does not
	///     replace it.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> value, or one that cannot be read, is neither masked nor converted, so that it stays
	///     visible; the formatter writes the exception of a value that cannot be read. The key of a number is the text of
	///     the value, so that e.g. a <see cref="Guid" /> member and the same <see cref="Guid" /> in a text share a
	///     number.
	/// </remarks>
	public ValueFormatting? GetReplacement(FormattedValue value, SnapshotNumbering numbering)
	{
		if (IsIgnoring || !_selection.Matches(value))
		{
			return null;
		}

		object? actual;
		try
		{
			actual = value.GetValue();
		}
		catch (Exception)
		{
			return null;
		}

		if (actual is null)
		{
			return null;
		}

		if (_convert is not null)
		{
			return _convert(actual);
		}

		string? template = _template!(value);
		if (template is null)
		{
			return null;
		}

		return ValueFormatting.Replace(new SnapshotPlaceholder(SnapshotNumbering.IsNumbered(template)
			? numbering.Apply(template, GetKey(actual))
			: template));
	}

	private static string GetKey(object value)
		=> value switch
		{
			string text => text,
			IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
			_ => Formatter.Format(value, FormattingOptions.Snapshot),
		};
}
