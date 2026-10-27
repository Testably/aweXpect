namespace aweXpect.Core;

/// <summary>
///     The source of the <see cref="OptionsHolder{TOptions}" /> that a result hands to its option methods.
/// </summary>
/// <remarks>
///     A holder is its own source. A constraint can be the source instead, and create the holder only when an option
///     is set, so that an expectation without options does not allocate it; until then it compares with the options it
///     started with.
/// </remarks>
public interface IOptionsHolderSource<TOptions>
{
	/// <summary>
	///     The holder with the current options of the expectation.
	/// </summary>
	OptionsHolder<TOptions> Holder { get; }
}
