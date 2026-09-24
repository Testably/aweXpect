namespace aweXpect.Core.Constraints;

/// <summary>
///     The outcome of a <see cref="ConstraintResult" />.
/// </summary>
public enum Outcome
{
	/// <summary>
	///     The constraint was successful.
	/// </summary>
	Success,

	/// <summary>
	///     The constraint failed.
	/// </summary>
	Failure,

	/// <summary>
	///     The result could not be determined (e.g. due to timeout).
	/// </summary>
	Undecided,
}
