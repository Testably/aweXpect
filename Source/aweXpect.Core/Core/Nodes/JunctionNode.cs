using System;
using System.Collections.Generic;
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
	private string? _currentSeparator;

	/// <summary>
	///     The operands before the first one in <see cref="_furtherOperands" />.
	/// </summary>
	/// <remarks>
	///     Most junctions combine two operands, so the first one before the <see cref="Current" /> one is kept in this
	///     field, and only further ones in a list, which is then allocated.
	/// </remarks>
	private (string, Node) _firstOperand;

	/// <summary>
	///     The operands after the <see cref="_firstOperand" /> and before the <see cref="Current" /> one.
	/// </summary>
	private List<(string, Node)>? _furtherOperands;

	/// <summary>
	///     The number of operands before the <see cref="Current" /> one.
	/// </summary>
	private int _operandCount;

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
		(string, Node) operand = (_currentSeparator ?? DefaultSeparator, Current);
		if (_operandCount == 0)
		{
			_firstOperand = operand;
		}
		else
		{
			(_furtherOperands ??= []).Add(operand);
		}

		_operandCount++;
		Current = node;
		_currentSeparator = separator;
	}

	/// <inheritdoc />
	public override Node ReplaceRightMostOperand<TState>(TState state, Func<TState, Node, Node> replace)
	{
		Current = Current.ReplaceRightMostOperand(state, replace);
		return this;
	}

	/// <inheritdoc />
	/// <remarks>
	///     An operand that continues the preceding one, e.g. from <c>AndWhose</c>, receives the result of the last
	///     operand that does not, so that its members continue from the same stored value.
	///     <para />
	///     The operands are evaluated without a state machine as long as they complete synchronously, because most
	///     expectations do, and only the remaining ones are awaited.
	/// </remarks>
	public override ValueTask<ConstraintResult> IsMetBy<TValue>(TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken) where TValue : default
	{
		Evaluation evaluation = default;
		for (int index = 0; index <= _operandCount; index++)
		{
			(string separator, Node node) = GetNode(index);
			if (!IsOperand(node))
			{
				continue;
			}

			ValueTask<ConstraintResult> isMet = IsOperandMetBy(node, value, context, evaluation, cancellationToken);
			if (!isMet.IsCompletedSuccessfully)
			{
				return IsMetByAsync(isMet, index, separator, evaluation, value, context, cancellationToken);
			}

			if (evaluation.Add(this, node, isMet.Result, separator))
			{
				return new ValueTask<ConstraintResult>(evaluation.CombinedResult!);
			}
		}

		return new ValueTask<ConstraintResult>(evaluation.CombinedResult!);
	}

	/// <summary>
	///     Continues the evaluation of <see cref="IsMetBy{TValue}" /> with the operand at the <paramref name="index" />,
	///     whose result <paramref name="isMet" /> did not complete synchronously.
	/// </summary>
	private async ValueTask<ConstraintResult> IsMetByAsync<TValue>(ValueTask<ConstraintResult> isMet,
		int index,
		string separator,
		Evaluation evaluation,
		TValue? value,
		IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (evaluation.Add(this, GetNode(index).Item2, await isMet, separator))
		{
			return evaluation.CombinedResult!;
		}

		for (index++; index <= _operandCount; index++)
		{
			(string nextSeparator, Node node) = GetNode(index);
			if (!IsOperand(node))
			{
				continue;
			}

			ConstraintResult result = await IsOperandMetBy(node, value, context, evaluation, cancellationToken);
			if (evaluation.Add(this, node, result, nextSeparator))
			{
				return evaluation.CombinedResult!;
			}
		}

		return evaluation.CombinedResult!;
	}

	/// <summary>
	///     Evaluates the operand <paramref name="node" /> as part of the <paramref name="evaluation" />.
	/// </summary>
	private static ValueTask<ConstraintResult> IsOperandMetBy<TValue>(Node node,
		TValue? value,
		IEvaluationContext context,
		Evaluation evaluation,
		CancellationToken cancellationToken)
	{
		if (evaluation.IsSkipped)
		{
			return AsExpectationOnly(node.IsMetBy(value, ExpectationTextEvaluationContext.For(context),
				cancellationToken));
		}

		if (node is ExpectationNode { ContinuesPrecedingOperand: true, } continuingNode)
		{
			return continuingNode.IsMetByContinuing(value, evaluation.Source, context, cancellationToken);
		}

		return node.IsMetBy(value, context, cancellationToken);
	}

	private static ValueTask<ConstraintResult> AsExpectationOnly(ValueTask<ConstraintResult> isMet)
		=> isMet.IsCompletedSuccessfully
			? new ValueTask<ConstraintResult>(isMet.Result.AsExpectationOnly())
			: AsExpectationOnlyAsync(isMet);

	private static async ValueTask<ConstraintResult> AsExpectationOnlyAsync(ValueTask<ConstraintResult> isMet)
		=> (await isMet).AsExpectationOnly();

	/// <summary>
	///     All nodes, including the <see cref="Current" /> one.
	/// </summary>
	/// <remarks>
	///     The <see cref="Current" /> node must not be added to the operands here, because the expectation can be
	///     evaluated multiple times (e.g. by <see cref="EventuallyExpectationBuilder{TValue}" />).
	/// </remarks>
	private IEnumerable<(string, Node)> GetNodes()
	{
		for (int index = 0; index <= _operandCount; index++)
		{
			yield return GetNode(index);
		}
	}

	/// <summary>
	///     The node at the <paramref name="index" /> of <see cref="GetNodes" />.
	/// </summary>
	/// <remarks>
	///     Every evaluation walks the nodes, so it reads them by index instead of allocating an iterator.
	/// </remarks>
	private (string, Node) GetNode(int index)
	{
		if (index == _operandCount)
		{
			return (_currentSeparator ?? DefaultSeparator, Current);
		}

		return index == 0 ? _firstOperand : _furtherOperands![index - 1];
	}

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
	{
		if (_isAnd != other._isAnd || _operandCount != other._operandCount || !Current.Equals(other.Current))
		{
			return false;
		}

		for (int index = 0; index < _operandCount; index++)
		{
			if (!GetNode(index).Equals(other.GetNode(index)))
			{
				return false;
			}
		}

		return true;
	}

	/// <inheritdoc cref="object.GetHashCode()" />
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 19;
			foreach ((_, Node node) in GetNodes())
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

	/// <summary>
	///     The state of an evaluation of <see cref="IsMetBy{TValue}" />, which is passed on when an operand does not
	///     complete synchronously.
	/// </summary>
	private struct Evaluation
	{
		/// <summary>
		///     The results of the operands so far.
		/// </summary>
		public ConstraintResult? CombinedResult { get; private set; }

		/// <summary>
		///     The result of the last operand that does not continue the preceding one.
		/// </summary>
		public ConstraintResult? Source { get; private set; }

		/// <summary>
		///     Whether the following operands are only evaluated for their expectation text.
		/// </summary>
		public bool IsSkipped { get; private set; }

		/// <summary>
		///     Adds the <paramref name="result" /> of the operand <paramref name="node" />.
		/// </summary>
		/// <returns><see langword="true" />, when the following operands are ignored completely.</returns>
		public bool Add(JunctionNode junction, Node node, ConstraintResult result, string separator)
		{
			if (!IsSkipped && node is not ExpectationNode { ContinuesPrecedingOperand: true, })
			{
				Source = result;
			}

			CombinedResult = junction.CombineResults(CombinedResult, result, separator,
				CombinedResult?.FurtherProcessingStrategy);
			if (result.FurtherProcessingStrategy == FurtherProcessingStrategy.IgnoreCompletely)
			{
				return true;
			}

			IsSkipped = IsSkipped || junction.SkipsFollowingOperands(result, CombinedResult);
			return false;
		}
	}
}
