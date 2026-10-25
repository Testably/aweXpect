namespace aweXpect.Core;

/// <summary>
///     Marks a result that stays unchanged when an option is applied to it: the option is set on a copy of its
///     <typeparamref name="TOptions" />, from which the result returns a new <typeparamref name="TResult" />.
/// </summary>
/// <remarks>
///     A result usually belongs to a single expectation, so the options of its <see cref="IOptionsProvider{TOptions}" />
///     are changed in place. A result that can be stored and continued more than once, e.g. a filter in
///     <c>aweXpect.Reflection</c>, implements this interface, so that a continuation never changes the result it was
///     called on.<br />
///     It is only used when an option is applied, never per evaluation.
/// </remarks>
public interface ICopyOnOption<in TOptions, out TResult>
{
	/// <summary>
	///     Returns a copy of this result that uses the <paramref name="options" /> instead of its own.
	/// </summary>
	/// <remarks>
	///     The <paramref name="options" /> are a copy of the options of this result with the new option set, so they
	///     carry every option set so far: a repeated option replaces the earlier one, and a competing or incompatible
	///     option throws with the same message as without the copy. The returned result must use them for everything
	///     its own options were used for, e.g. its matches and its description, and keep the rest of this result.
	/// </remarks>
	TResult WithOptions(TOptions options);
}
