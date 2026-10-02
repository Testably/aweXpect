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

internal class AndNode : Node
{
	private const string DefaultSeparator = " and ";
	private readonly List<(string, Node)> _nodes = new();
	private string? _currentSeparator;

	public AndNode(Node node)
	{
		Current = node;
	}

	private Node Current { get; set; }

	/// <inheritdoc />
	public override void AddConstraint(IConstraint constraint)
		=> Current.AddConstraint(constraint);

	/// <inheritdoc />
	public override Node AddMapping<TValue, TTarget>(MemberAccessor<TValue, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
		=> Current.AddMapping(memberAccessor, expectationTextGenerator);

	/// <inheritdoc />
	public override Node AddNarrowingMapping<TValue, TTarget, TNarrowed>(
		MemberAccessor<TValue, TTarget> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
		where TNarrowed : default
		=> Current.AddNarrowingMapping<TValue, TTarget, TNarrowed>(memberAccessor, expectationTextGenerator);

	/// <inheritdoc />
	public override Node AddAsyncMapping<TValue, TTarget>(
		MemberAccessor<TValue, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
		=> Current.AddAsyncMapping(memberAccessor, expectationTextGenerator);

	/// <inheritdoc />
	public override Node AddAsyncNarrowingMapping<TValue, TTarget, TNarrowed>(
		MemberAccessor<TValue, Task<TTarget>> memberAccessor,
		Action<MemberAccessor, StringBuilder>? expectationTextGenerator = null)
		where TValue : default
		where TTarget : default
		where TNarrowed : default
		=> Current.AddAsyncNarrowingMapping<TValue, TTarget, TNarrowed>(memberAccessor, expectationTextGenerator);

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
	public override async Task<ConstraintResult> IsMetBy<TValue>(TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken) where TValue : default
	{
		ConstraintResult? combinedResult = null;
		bool isSkipped = false;
		for (int index = 0; index <= _nodes.Count; index++)
		{
			(string separator, Node node) = GetNode(index);
			if (node is ExpectationNode expectationNode && expectationNode.IsEmpty())
			{
				continue;
			}

			ConstraintResult result;
			if (isSkipped)
			{
				result = await node.IsMetBy(value, ExpectationTextEvaluationContext.For(context), cancellationToken);
				result = result.AsExpectationOnly();
			}
			else
			{
				result = await node.IsMetBy(value, context, cancellationToken);
				// A failed operand which ignores the result of the following ones, e.g. a failed null check, decides the
				// combination, so the following operands are only evaluated for their expectation text.
				isSkipped = result.FurtherProcessingStrategy == FurtherProcessingStrategy.IgnoreResult &&
				            result.Outcome == Outcome.Failure;
			}

			combinedResult = CombineResults(combinedResult, result, separator,
				combinedResult?.FurtherProcessingStrategy);
			if (result.FurtherProcessingStrategy ==
			    FurtherProcessingStrategy.IgnoreCompletely)
			{
				return combinedResult;
			}
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
	public override bool Equals(object? obj) => obj is AndNode other && Equals(other);

	private bool Equals(AndNode other) => Current.Equals(other.Current) && _nodes.SequenceEqual(other._nodes);

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

	private static ConstraintResult CombineResults(
		ConstraintResult? combinedResult,
		ConstraintResult result,
		string separator,
		FurtherProcessingStrategy? furtherProcessingStrategy)
	{
		if (combinedResult == null)
		{
			return result;
		}

		return new JunctionResult(combinedResult, result, true, separator,
			furtherProcessingStrategy ?? FurtherProcessingStrategy.Continue);
	}
}
