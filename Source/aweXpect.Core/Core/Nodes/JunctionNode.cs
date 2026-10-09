using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Nodes;

/// <summary>
///     Combines expectations on the same subject with <c>And</c> or <c>Or</c>.
/// </summary>
internal abstract class JunctionNode : Node
{
	private readonly bool _isAnd;
	private readonly List<(string, Node)> _nodes = new();
	private string? _currentSeparator;

	protected JunctionNode(Node node, bool isAnd)
	{
		Current = node;
		_isAnd = isAnd;
	}

	internal Node Current { get; set; }

	/// <summary>
	///     The separator between the expectations, unless another one is given.
	/// </summary>
	protected abstract string DefaultSeparator { get; }

	/// <summary>
	///     Whether the <paramref name="node" /> is one of the operands.
	/// </summary>
	protected virtual bool IsOperand(Node node) => true;

	/// <summary>
	///     Whether the operands following the one with the <paramref name="result" /> are only evaluated for their
	///     expectation text, as the <paramref name="combinedResult" /> is decided.
	/// </summary>
	protected abstract bool SkipsFollowingOperands(ConstraintResult result, ConstraintResult combinedResult);

	/// <inheritdoc />
	public override void AddConstraint(IConstraint constraint, string it = "it")
		=> Current.AddConstraint(constraint, it);

	/// <inheritdoc />
	public override Node AddMapping(MappingNode mappingNode)
		=> Current.AddMapping(mappingNode);

	public override void AddNode(Node node, string? separator = null)
	{
		_nodes.Add((_currentSeparator ?? DefaultSeparator, Current));
		Current = node;
		_currentSeparator = separator;
	}

	/// <inheritdoc />
	public override Node ReplaceRightMostOperand(Func<Node, Node> replace)
	{
		Current = Current.ReplaceRightMostOperand(replace);
		return this;
	}

	/// <inheritdoc />
	/// <remarks>
	///     An operand that continues the preceding one, e.g. from <c>AndWhose</c>, receives the result of the last
	///     operand that does not, so that its members continue from the same stored value.
	/// </remarks>
	public override async ValueTask<ConstraintResult> IsMetBy<TValue>(TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken) where TValue : default
	{
		ConstraintResult? combinedResult = null;
		ConstraintResult? source = null;
		bool isSkipped = false;
		for (int index = 0; index <= _nodes.Count; index++)
		{
			(string separator, Node node) = GetNode(index);
			if (!IsOperand(node))
			{
				continue;
			}

			ConstraintResult result;
			if (isSkipped)
			{
				result = await node.IsMetBy(value, ExpectationTextEvaluationContext.For(context), cancellationToken);
				result = result.AsExpectationOnly();
			}
			else if (node is ExpectationNode { ContinuesPrecedingOperand: true, } continuingNode)
			{
				result = await continuingNode.IsMetByContinuing(value, source, context, cancellationToken);
			}
			else
			{
				result = await node.IsMetBy(value, context, cancellationToken);
				source = result;
			}

			combinedResult = CombineResults(combinedResult, result, separator,
				combinedResult?.FurtherProcessingStrategy);
			if (result.FurtherProcessingStrategy == FurtherProcessingStrategy.IgnoreCompletely)
			{
				return combinedResult;
			}

			isSkipped = isSkipped || SkipsFollowingOperands(result, combinedResult);
		}

		return combinedResult!;
	}

	/// <summary>
	///     All nodes, including the <see cref="Current" /> one.
	/// </summary>
	/// <remarks>
	///     The <see cref="Current" /> node must not be added to <see cref="_nodes" /> here, because the expectation
	///     can be evaluated multiple times (e.g. by <see cref="EventuallyExpectationBuilder{TValue}" />).
	/// </remarks>
	private IEnumerable<(string, Node)> GetNodes()
	{
		foreach ((string, Node) node in _nodes)
		{
			yield return node;
		}

		yield return (_currentSeparator ?? DefaultSeparator, Current);
	}

	/// <summary>
	///     The node at the <paramref name="index" /> of <see cref="GetNodes" />.
	/// </summary>
	/// <remarks>
	///     Every evaluation walks the nodes, so it reads them by index instead of allocating an iterator.
	/// </remarks>
	private (string, Node) GetNode(int index)
		=> index < _nodes.Count ? _nodes[index] : (_currentSeparator ?? DefaultSeparator, Current);

	/// <inheritdoc />
	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		bool isFirst = true;
		foreach ((string separator, Node node) in GetNodes())
		{
			if (isFirst)
			{
				node.AppendExpectation(stringBuilder, indentation);
				isFirst = false;
			}
			else
			{
				stringBuilder.AppendSeparatedExpectation(separator, sb => node.AppendExpectation(sb, indentation));
			}
		}
	}

	/// <inheritdoc cref="object.Equals(object?)" />
	public override bool Equals(object? obj) => obj is JunctionNode other && Equals(other);

	private bool Equals(JunctionNode other)
		=> _isAnd == other._isAnd && Current.Equals(other.Current) && _nodes.SequenceEqual(other._nodes);

	/// <inheritdoc cref="object.GetHashCode()" />
	public override int GetHashCode()
	{
		unchecked
		{
			// ReSharper disable once NonReadonlyMemberInGetHashCode
			int hash = 19 * Current.GetHashCode();
			foreach (Node node in _nodes.Select(x => x.Item2))
			{
				hash = (hash * 31) + node.GetHashCode();
			}

			return hash;
		}
	}

	private ConstraintResult CombineResults(
		ConstraintResult? combinedResult,
		ConstraintResult result,
		string separator,
		FurtherProcessingStrategy? furtherProcessingStrategy)
	{
		if (combinedResult == null)
		{
			return result;
		}

		return new JunctionResult(combinedResult, result, _isAnd, separator,
			furtherProcessingStrategy ?? FurtherProcessingStrategy.Continue);
	}
}
