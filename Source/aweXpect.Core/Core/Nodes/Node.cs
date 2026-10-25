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
	/// <param name="constraint">The constraint.</param>
	/// <param name="it">The name of the subject that the <paramref name="constraint" /> received.</param>
	public abstract void AddConstraint(IConstraint constraint, string it = "it");

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
	/// <remarks>
	///     A <see cref="ValueTask{TResult}" />, because most expectations complete synchronously, and every node of an
	///     evaluation would otherwise allocate and await a completed task.
	/// </remarks>
	public abstract ValueTask<ConstraintResult> IsMetBy<TValue>(
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
	///     is evaluated as <c>A or (B which C)</c>, like it reads.<br />
	///     The <paramref name="state" /> is passed to <paramref name="replace" />, so that it needs no closure.
	/// </remarks>
	public virtual Node ReplaceRightMostOperand<TState>(TState state, Func<TState, Node, Node> replace)
		=> replace(state, this);

	/// <summary>
	///     Evaluates the constraints of the node on the <paramref name="value" /> right away, without combining their
	///     results, and keeps each result for the next <see cref="IsMetBy{TValue}" />, so that a regular evaluation that
	///     follows does not evaluate them again.
	/// </summary>
	/// <param name="value">The value of the subject.</param>
	/// <param name="context">The context of the evaluation.</param>
	/// <param name="lastResult">The result of the last constraint that was evaluated.</param>
	internal virtual RightAwayOutcome IsMetRightAway<TValue>(TValue value, IEvaluationContext context,
		ref ConstraintResult? lastResult)
		=> RightAwayOutcome.Unsupported;

	/// <summary>
	///     Forgets the results that <see cref="IsMetRightAway{TValue}" /> kept and that were not read.
	/// </summary>
	internal virtual void ForgetRightAwayResults()
	{
	}
}
