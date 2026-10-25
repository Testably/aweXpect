namespace aweXpect.Core;

/// <summary>
///     Reads the current <typeparamref name="TOptions" /> of a single expectation.
/// </summary>
/// <remarks>
///     Covariant, so that a constraint that compares with string or object options alike can read either.
/// </remarks>
public interface IReadOnlyOptionsSlot<out TOptions>
{
	/// <summary>
	///     The current options of the expectation.
	/// </summary>
	TOptions Value { get; }
}
