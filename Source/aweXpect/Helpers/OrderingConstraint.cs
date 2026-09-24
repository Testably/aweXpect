using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Helpers;

/// <summary>
///     Base class for ordering and range constraints, which fail for a <see langword="null" /> expected value or bound
///     regardless of their negation, because nothing can be ordered against <see langword="null" />.
/// </summary>
/// <remarks>
///     The failure is decided in <see cref="Outcome" />, because a result can be negated after it was evaluated, e.g.
///     in <c>DoesNotComplyWith</c>, which would invert a failure decided in <c>IsMetBy</c> into a success.
/// </remarks>
internal abstract class OrderingConstraint<T>(string it, ExpectationGrammars grammars, bool isOrderedAgainstNull)
	: ConstraintResult.WithNotNullValue<T>(it, grammars)
{
	/// <inheritdoc />
	public override Outcome Outcome
	{
		get => isOrderedAgainstNull ? Outcome.Failure : base.Outcome;
		protected set => base.Outcome = value;
	}
}
