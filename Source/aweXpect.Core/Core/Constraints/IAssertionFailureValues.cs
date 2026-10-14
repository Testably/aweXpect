namespace aweXpect.Core.Constraints;

/// <summary>
///     A <see cref="ConstraintResult" /> that can describe its failure by an expected and an actual value, which test
///     platforms can render as a diff.
/// </summary>
public interface IAssertionFailureValues
{
	/// <summary>
	///     The text of the expected value, or <see langword="null" /> when the expectation has no natural expected value.
	/// </summary>
	string? ExpectedText { get; }

	/// <summary>
	///     The text of the actual value, or <see langword="null" /> when the expectation has no natural actual value.
	/// </summary>
	string? ActualText { get; }
}
