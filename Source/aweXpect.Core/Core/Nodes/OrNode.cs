using aweXpect.Core.Constraints;

namespace aweXpect.Core.Nodes;

/// <summary>
///     Combines expectations of which one must be met.
/// </summary>
internal sealed class OrNode(Node node) : JunctionNode(node, false)
{
	/// <inheritdoc />
	protected override string DefaultSeparator => " or ";

	/// <inheritdoc />
	/// <remarks>
	///     Short-circuit: the remaining nodes only contribute their expectation text, so that neither their constraints
	///     nor their member accessors are evaluated, but the expectation still names them all.
	/// </remarks>
	protected override bool SkipsFollowingOperands(ConstraintResult result, ConstraintResult combinedResult)
		=> combinedResult.Outcome == Outcome.Success;
}
