namespace aweXpect.Core;

/// <summary>
///     Offers the <typeparamref name="TOptions" /> of a single expectation to option methods.
/// </summary>
public interface IOptionsProvider<TOptions>
{
	/// <summary>
	///     The slot that holds the current options of the expectation.
	/// </summary>
	/// <remarks>
	///     This is primarily intended for extension methods. The option methods of the built-in options are also
	///     available on the slot, e.g. <c>result.Options.AtLeast(2)</c>; members that only read the options are on its
	///     <see cref="OptionsSlot{TOptions}.Value" />.
	/// </remarks>
	OptionsSlot<TOptions> Options { get; }
}
