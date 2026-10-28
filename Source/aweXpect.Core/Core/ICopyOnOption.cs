namespace aweXpect.Core;

/// <summary>
///     Marks a result that stays unchanged when an option is applied to it: it returns a new
///     <typeparamref name="TSelf" /> with the new <typeparamref name="TOptions" />.
/// </summary>
/// <remarks>
///     A result usually belongs to a single expectation, so an option changes the options in the
///     <see cref="OptionsHolder{TOptions}" /> of its <see cref="IOptionsProvider{TOptions}" />. A result that can be stored
///     and continued more than once, e.g. a filter in
///     <c>aweXpect.Reflection</c>, implements this interface, so that a continuation never changes the result it was
///     called on. An option method applies its options with <see cref="OptionsProvider.WithOptions{TSelf, TOptions}" />,
///     which respects this interface.<br />
///     It is only used when an option is applied, never per evaluation.
/// </remarks>
public interface ICopyOnOption<in TOptions, out TSelf>
{
	/// <summary>
	///     Returns a copy of this result that uses the <paramref name="options" /> instead of its own.
	/// </summary>
	/// <remarks>
	///     The <paramref name="options" /> are the options of this result with the new option set, so they carry
	///     every option set so far: a repeated option replaces the earlier one, and a competing or incompatible
	///     option throws with the same message as without the copy. The returned result must use them for everything
	///     its own options were used for, e.g. its matches and its description, and keep the rest of this result; it
	///     offers them in a new <see cref="OptionsHolder{TOptions}" />, so that the holder of this result stays unchanged.
	/// </remarks>
	TSelf WithOptions(TOptions options);
}
