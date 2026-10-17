namespace aweXpect.Formatting;

/// <summary>
///     Decides whether a <see cref="FormattedValue" /> is written, left out or replaced.
/// </summary>
/// <seealso cref="FormattingOptions.FormatValue" />
public sealed class ValueFormatting
{
	private ValueFormatting(bool isOmitted, bool isReplaced, object? replacement)
	{
		IsOmitted = isOmitted;
		IsReplaced = isReplaced;
		Replacement = replacement;
	}

	/// <summary>
	///     Writes the value as usual.
	/// </summary>
	public static ValueFormatting Write { get; } = new(false, false, null);

	/// <summary>
	///     Leaves the value out, together with its name or key.
	/// </summary>
	public static ValueFormatting Omit { get; } = new(true, false, null);

	internal bool IsOmitted { get; }

	internal bool IsReplaced { get; }

	internal object? Replacement { get; }

	/// <summary>
	///     Writes the <paramref name="value" /> in place of the value, formatted like any other value.
	/// </summary>
	/// <remarks>
	///     The name of a member and the key of a dictionary entry are kept. The values nested in the
	///     <paramref name="value" /> are not offered to <see cref="FormattingOptions.FormatValue" /> again.
	/// </remarks>
	public static ValueFormatting Replace(object? value) => new(false, true, value);
}
