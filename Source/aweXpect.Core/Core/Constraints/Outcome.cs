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
	///     The constraint did not decide its outcome, e.g. because the evaluation was canceled before it could.
	/// </summary>
	/// <remarks>
	///     An expectation only stays undecided when the caller canceled the evaluation. It fails when the timeout
	///     canceled the evaluation, and also when its outcome is undecided although nothing canceled the evaluation.
	/// </remarks>
	Undecided,

	/// <summary>
	///     The constraint failed the expectation and fails its negation as well, because it could not be answered, e.g.
	///     because the subject is <see langword="null" />, code of the caller threw or the values are not comparable.
	/// </summary>
	/// <remarks>
	///     It fails the expectation like <see cref="Failure" />, but a negation keeps it, as negating a question that
	///     cannot be answered does not make it true.
	/// </remarks>
	FailureBothWays,
}
