using aweXpect.Core.Constraints;

namespace aweXpect.Core.Nodes;

/// <summary>
///     Combines expectations that must all be met.
/// </summary>
internal sealed class AndNode(Node node) : JunctionNode(node, true)
{
	/// <inheritdoc />
	protected override string DefaultSeparator => " and ";

	/// <inheritdoc />
	protected override bool IsOperand(Node node) => !(node is ExpectationNode expectationNode && expectationNode.IsEmpty());

	/// <inheritdoc />
	/// <remarks>
	///     A failed operand which ignores the result of the following ones, e.g. a failed null check, decides the
	///     combination.
	/// </remarks>
	protected override bool SkipsFollowingOperands(ConstraintResult result, ConstraintResult combinedResult)
		=> result.FurtherProcessingStrategy == FurtherProcessingStrategy.IgnoreResult &&
		   result.Outcome == Outcome.Failure;
}
