using System;
using System.Text;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;

namespace aweXpect.Xml;

/// <summary>
///     Verifies that an element has the node that the <paramref name="find" /> function looks up, e.g. an attribute or
///     a child element.
/// </summary>
/// <param name="it">The name of the subject.</param>
/// <param name="grammars">The grammars of the expectation.</param>
/// <param name="node">The description of the node, e.g. <c>attribute "id"</c>.</param>
/// <param name="find">Looks up the node in the element, or returns <see langword="null" /> if it does not exist.</param>
internal sealed class HasNodeConstraint<TNode>(
	string it,
	ExpectationGrammars grammars,
	string node,
	Func<XElement, TNode?> find)
	: ConstraintResult.WithNotNullValue<XElement?>(it, grammars),
		IValueConstraint<XElement?>
	where TNode : XObject
{
	private FoundNode<TNode>? _found;

	public ConstraintResult IsMetBy(XElement? actual)
	{
		Actual = actual;
		TNode? found = actual is null ? null : find(actual);
		_found = found is null ? null : new FoundNode<TNode>(found);
		Outcome = found is not null ? Outcome.Success : Outcome.Failure;
		return this;
	}

	/// <remarks>
	///     Stores the found node, so that the expectations on it are only verified when it exists.
	/// </remarks>
	public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
	{
		if (_found is TValue found)
		{
			value = found;
			return true;
		}

		return base.TryGetStoredValue(out value);
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Grammars.Verb("has ", "have ")).Append(node);

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(It).Append(" did not have ").Append(node);

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Grammars.Verb("does not have ", "do not have ")).Append(node);

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(It).Append(" did");
}

/// <summary>
///     A node found by <see cref="HasNodeConstraint{TNode}" />.
/// </summary>
/// <remarks>
///     The node is wrapped, so that a found child element is not taken for the subject element itself.
/// </remarks>
internal sealed class FoundNode<TNode>(TNode node)
	where TNode : XObject
{
	public TNode Node { get; } = node;
}
