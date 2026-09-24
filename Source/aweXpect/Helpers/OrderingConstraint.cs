using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Helpers;

/// <summary>
///     Base class for ordering and range constraints, which fail regardless of their negation when the subject cannot
///     be ordered against the expected value or a bound, e.g. because it is <see langword="null" />.
/// </summary>
/// <remarks>
///     The failure is decided in <see cref="Outcome" />, because a result can be negated after it was evaluated, e.g.
///     in <c>DoesNotComplyWith</c>, which would invert a failure decided in <c>IsMetBy</c> into a success.
/// </remarks>
internal abstract class OrderingConstraint<T>(string it, ExpectationGrammars grammars, bool isOrderedAgainstNull)
	: ConstraintResult.WithNotNullValue<T>(it, grammars)
{
	/// <summary>
	///     Flag indicating that the subject cannot be ordered against the expected value or a bound, which fails the
	///     expectation as well as its negation.
	/// </summary>
	protected bool IsIncomparable { get; set; } = isOrderedAgainstNull;

	/// <inheritdoc />
	public override Outcome Outcome
	{
		get => IsIncomparable ? Outcome.Failure : base.Outcome;
		protected set => base.Outcome = value;
	}
}
