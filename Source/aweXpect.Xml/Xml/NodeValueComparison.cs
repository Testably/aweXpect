using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Results;

namespace aweXpect.Xml;

/// <summary>
///     Compares the value of a found node with the comparison of an operator on an <see cref="Results.XmlValueResult{TType,TThat}" />,
///     e.g. <c>StartingWith("Let")</c>, for one evaluation of a constraint.
/// </summary>
/// <param name="slot">The slot that receives the comparison of the operator.</param>
/// <param name="getValue">Reads the value of the found node.</param>
internal sealed class NodeValueComparison<TNode>(
	XmlComparisonSlot slot,
	Func<TNode, string> getValue)
{
	private StringPropertyOutcome _outcome;

	/// <summary>
	///     Whether the value was compared in the last evaluation.
	/// </summary>
	public bool IsCompared { get; private set; }

	/// <summary>
	///     Forgets the last evaluation and rejects a comparison that cannot be evaluated, before the subject is looked at.
	/// </summary>
	public void Reset()
	{
		IsCompared = false;
		_outcome = default;
		slot.Comparison?.ValidateExpected();
	}

	/// <summary>
	///     Compares the value of the <paramref name="node" />, or succeeds without a comparison.
	/// </summary>
	public async ValueTask<Outcome> Evaluate(TNode node, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		if (slot.Comparison is null)
		{
			return Outcome.Success;
		}

		IsCompared = true;
		_outcome = await slot.Comparison.Evaluate(getValue(node), context, cancellationToken);
		return _outcome.Outcome;
	}

	public void AppendContexts(ResultContextCollector contexts)
	{
		if (IsCompared)
		{
			_outcome.AppendContexts(contexts);
		}
	}

	/// <summary>
	///     Appends the comparison, e.g. <c> starting with "Let"</c>, when an operator was called.
	/// </summary>
	public void AppendExpectation(StringBuilder stringBuilder)
	{
		if (slot.Comparison is not null)
		{
			slot.Comparison.AppendExpectation(stringBuilder.Append(' '),
				slot.Comparison.IsNegated ? ExpectationGrammars.Negated : ExpectationGrammars.None);
		}
	}

	/// <summary>
	///     Appends the result for a compared value, named <paramref name="member" />, e.g.
	///     <c>value of attribute "id" was "8", which differs …</c>.
	/// </summary>
	public void AppendResult(StringBuilder stringBuilder, string member, ExpectationGrammars grammars,
		string? indentation)
		=> _outcome.AppendResult(stringBuilder, member, slot.Comparison!.IsNegated ? grammars.Negate() : grammars,
			indentation);
}
