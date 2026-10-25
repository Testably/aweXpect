namespace aweXpect.Core;

/// <summary>
///     Holds the current <typeparamref name="TOptions" /> of a single expectation.
/// </summary>
/// <remarks>
///     The expectation creates the slot and passes it to its constraint and to its result: an option method on the
///     result replaces or changes the value in the slot, and the constraint reads <see cref="Value" /> when it
///     evaluates, never when it is built.<br />
///     Not thread-safe, like building an expectation.
/// </remarks>
public sealed class OptionsSlot<TOptions>(TOptions value) : IReadOnlyOptionsSlot<TOptions>
{
	/// <summary>
	///     The current options of the expectation.
	/// </summary>
	/// <remarks>
	///     An option method replaces it with the options that the method of the options returned.
	/// </remarks>
	public TOptions Value { get; set; } = value;

	/// <summary>
	///     Returns the text of the current options.
	/// </summary>
	/// <remarks>
	///     An expectation text can append the slot like the options it holds.
	/// </remarks>
	public override string ToString() => Value?.ToString() ?? "";
}
