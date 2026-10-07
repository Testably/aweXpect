namespace aweXpect.Core.Constraints;

/// <summary>
///     A simple constraint on type <typeparamref name="TValue" />.
/// </summary>
public interface IValueConstraint<in TValue> : IConstraint
{
	/// <summary>
	///     Checks if the <paramref name="actual" /> value meets the expectation.
	/// </summary>
	ConstraintResult IsMetBy(TValue actual);
}
