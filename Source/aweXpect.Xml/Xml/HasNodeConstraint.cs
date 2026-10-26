using System;
using System.Text;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Formatting;
using aweXpect.Helpers;
using aweXpect.Results;
using static aweXpect.Formatting.Format;

namespace aweXpect.Xml;

/// <summary>
///     Adds the constraints that the element of a subject has an attribute or a child element.
/// </summary>
internal static class HasNodeConstraint
{
	public static AndOrResult<TSubject, IThat<TSubject?>> HasAttribute<TSubject>(IThat<TSubject?> source,
		XName name, bool negated, Func<TSubject, XElement?> getElement)
		where TSubject : XNode
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		return new AndOrResult<TSubject, IThat<TSubject?>>(source.Get().ExpectationBuilder.AddConstraint(
				(Name: name, Negated: negated, GetElement: getElement),
				static (state, it, grammars) =>
				{
					HasNodeConstraint<TSubject, XAttribute> constraint =
						CreateAttribute(it, grammars, state.Name, state.GetElement);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			source);
	}

	public static AndOrResult<TSubject, IThat<TSubject?>> HasAttribute<TSubject>(IThat<TSubject?> source,
		XName name, Action<IThatSubject<string?>> expectations, Func<TSubject, XElement?> getElement)
		where TSubject : XNode
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint((Name: name, GetElement: getElement),
			static (state, it, grammars) => CreateAttribute(it, grammars, state.Name, state.GetElement));
		expectationBuilder
			.ForMember(MemberAccessor<FoundNode<XAttribute>, string?>.FromFuncWithName(found => found.Node.Value,
					$"value of attribute {Formatter.Format(name.ToString())}"),
				(_, stringBuilder) => stringBuilder.Append(" whose value "))
			.AddExpectations(expectations);
		return new AndOrResult<TSubject, IThat<TSubject?>>(expectationBuilder, source);
	}

	public static AndOrResult<TSubject, IThat<TSubject?>> HasElement<TSubject>(IThat<TSubject?> source,
		XName name, bool negated, Func<TSubject, XElement?> getElement)
		where TSubject : XNode
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		return new AndOrResult<TSubject, IThat<TSubject?>>(source.Get().ExpectationBuilder.AddConstraint(
				(Name: name, Negated: negated, GetElement: getElement),
				static (state, it, grammars) =>
				{
					HasNodeConstraint<TSubject, XElement> constraint =
						CreateElement(it, grammars, state.Name, state.GetElement);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			source);
	}

	public static AndOrResult<TSubject, IThat<TSubject?>> HasElement<TSubject>(IThat<TSubject?> source,
		XName name, Action<IThatSubject<XElement?>> expectations, Func<TSubject, XElement?> getElement)
		where TSubject : XNode
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint((Name: name, GetElement: getElement),
			static (state, it, grammars) => CreateElement(it, grammars, state.Name, state.GetElement));
		expectationBuilder
			.ForMember(MemberAccessor<FoundNode<XElement>, XElement?>.FromFuncWithName(found => found.Node,
					$"element {Formatter.Format(name.ToString())}"),
				(_, stringBuilder) => stringBuilder.Append(" which "))
			.AddExpectations(expectations);
		return new AndOrResult<TSubject, IThat<TSubject?>>(expectationBuilder, source);
	}

	/// <summary>
	///     Adds the <paramref name="element" /> that a lookup ran on as the "Actual" context, as the failure message only
	///     names the node that was looked up.
	/// </summary>
	/// <remarks>
	///     The element is written with <see cref="XNode.ToString()" />, which indents nested elements, as the formatter
	///     would indent the lines after the first one once more.
	/// </remarks>
	internal static void AddActualContext(ResultContextCollector contexts, XElement? element)
	{
		if (element is not null)
		{
			contexts.Add(new ResultContext.SyncCallback("Actual", element.ToString));
		}
	}

	private static HasNodeConstraint<TSubject, XAttribute> CreateAttribute<TSubject>(string it,
		ExpectationGrammars grammars, XName name, Func<TSubject, XElement?> getElement)
		where TSubject : XNode
		=> new(it, grammars, "attribute", name, getElement, static (element, name) => element.Attribute(name));

	private static HasNodeConstraint<TSubject, XElement> CreateElement<TSubject>(string it,
		ExpectationGrammars grammars, XName name, Func<TSubject, XElement?> getElement)
		where TSubject : XNode
		=> new(it, grammars, "element", name, getElement, static (element, name) => element.Element(name));
}

/// <summary>
///     Verifies that the element of a subject has the node that the <paramref name="find" /> function looks up, e.g. an
///     attribute or a child element.
/// </summary>
/// <param name="it">The name of the subject.</param>
/// <param name="grammars">The grammars of the expectation.</param>
/// <param name="kind">The kind of the node, e.g. <c>attribute</c>.</param>
/// <param name="name">The name of the node.</param>
/// <param name="getElement">Selects the element of the subject, e.g. the root element of a document.</param>
/// <param name="find">
///     Looks up the node with the <paramref name="name" /> in the element, or returns <see langword="null" /> if it does
///     not exist.
/// </param>
/// <remarks>
///     The node is only described in a failure, so that a met expectation does not format its name.
/// </remarks>
internal sealed class HasNodeConstraint<TSubject, TNode>(
	string it,
	ExpectationGrammars grammars,
	string kind,
	XName name,
	Func<TSubject, XElement?> getElement,
	Func<XElement, XName, TNode?> find)
	: ConstraintResult.WithNotNullValue<TSubject?>(it, grammars),
		IValueConstraint<TSubject?>
	where TSubject : XNode
	where TNode : XObject
{
	private XElement? _element;
	private FoundNode<TNode>? _found;
	private bool _hasNoElement;

	public ConstraintResult IsMetBy(TSubject? actual)
	{
		Actual = actual;
		_element = null;
		_found = null;
		_hasNoElement = false;
		if (actual is null)
		{
			Outcome = Outcome.Failure;
			return this;
		}

		XElement? element = getElement(actual);
		if (element is null)
		{
			_hasNoElement = true;
			Outcome = Outcome.FailureBothWays;
			return this;
		}

		_element = element;
		TNode? found = find(element, name);
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

	public override void AppendContexts(ResultContextCollector contexts)
	{
		if (Outcome != Outcome.Success)
		{
			HasNodeConstraint.AddActualContext(contexts, _element);
		}
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendNode(stringBuilder.Append(Grammars.Verb("has ", "have ")));

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (!TryAppendMissingElement(stringBuilder))
		{
			AppendNode(stringBuilder.Append(It).Append(" did not have "));
		}
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendNode(stringBuilder.Append(Grammars.Verb("does not have ", "do not have ")));

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (!TryAppendMissingElement(stringBuilder))
		{
			stringBuilder.Append(It).Append(" did");
		}
	}

	private void AppendNode(StringBuilder stringBuilder)
		=> Formatter.Format(stringBuilder.Append(kind).Append(' '), name.ToString());

	private bool TryAppendMissingElement(StringBuilder stringBuilder)
	{
		if (!_hasNoElement)
		{
			return false;
		}

		stringBuilder.Append(It).Append(" had no root element");
		return true;
	}
}

/// <summary>
///     A node found by <see cref="HasNodeConstraint{TSubject,TNode}" /> or <see cref="HasValueConstraint{TSubject}" />.
/// </summary>
/// <remarks>
///     The node is wrapped, so that a found child element is not taken for the subject element itself.
/// </remarks>
internal sealed class FoundNode<TNode>(TNode node)
	where TNode : XObject
{
	public TNode Node { get; } = node;
}
