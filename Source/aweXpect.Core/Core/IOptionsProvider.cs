namespace aweXpect.Core;

/// <summary>
///     Gives access to the underlying <typeparamref name="TOptions" /> for extension methods.
/// </summary>
public interface IOptionsProvider<out TOptions>
{
	/// <summary>
	///     The underlying options of type <typeparamref name="TOptions" />.
	/// </summary>
	/// <remarks>
	///     This is primarily intended for extension methods.
	/// </remarks>
	TOptions Options { get; }
}
