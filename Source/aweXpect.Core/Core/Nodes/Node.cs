using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Core.Nodes;

internal abstract class Node
{
	/// <summary>
	///     Add a constraint to the current node.
	/// </summary>
	public abstract void AddConstraint(IConstraint constraint);

	/// <summary>
	///     Adds the <paramref name="mappingNode" />, which maps the value to a member and applies the following
	///     expectations to it.
	/// </summary>
	/// <returns>The node to which the expectations on the member are added.</returns>
	public abstract Node AddMapping(MappingNode mappingNode);

	/// <summary>
	///     Add a node as inner node.
	/// </summary>
	public abstract void AddNode(Node node, string? separator = null);

	/// <summary>
	///     Verifies if the <paramref name="value" /> satisfies the expectations of the node.
	/// </summary>
	public abstract Task<ConstraintResult> IsMetBy<TValue>(
		TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken);

	/// <summary>
	///     Appends the expectation to the <paramref name="stringBuilder" />.
	/// </summary>
	public abstract void AppendExpectation(StringBuilder stringBuilder, string? indentation = null);

	/// <summary>
	///     Replaces the right-most operand of the expectation with the node returned by <paramref name="replace" />.
	/// </summary>
	/// <remarks>
	///     A continuation such as <c>Which</c> applies only to the operand in front of it, so that <c>A or B which C</c>
	///     is evaluated as <c>A or (B which C)</c>, like it reads.
	/// </remarks>
	public virtual Node ReplaceRightMostOperand(Func<Node, Node> replace) => replace(this);
}
