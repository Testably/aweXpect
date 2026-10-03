using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Helpers;

/// <summary>
///     Base class for ordering and range constraints, which fail regardless of their negation when the subject cannot
///     be ordered against the expected value or a bound, e.g. because it is <see langword="null" />.
/// </summary>
/// <remarks>
///     Such a constraint sets <see cref="Outcome.FailureBothWays" />, which a negation keeps, e.g. in
///     <c>DoesNotComplyWith</c>.
/// </remarks>
internal abstract class OrderingConstraint<T>(string it, ExpectationGrammars grammars, bool isOrderedAgainstNull)
	: ConstraintResult.WithNotNullValue<T>(it, grammars)
{
	/// <summary>
	///     Flag indicating that the subject cannot be ordered against the expected value or a bound.
	/// </summary>
	protected bool IsIncomparable { get; set; } = isOrderedAgainstNull;
}
