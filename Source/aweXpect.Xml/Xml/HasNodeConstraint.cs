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
	public static AndOrResult<TSubject?, IThat<TSubject?>> HasAttribute<TSubject>(IThat<TSubject?> source,
		XName name, bool negated, Func<TSubject, XElement?> getElement)
		where TSubject : XNode
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		return new AndOrResult<TSubject?, IThat<TSubject?>>(source.Get().ExpectationBuilder.AddConstraint(
				(Name: name, Negated: negated, GetElement: getElement),
				static (state, it, grammars) =>
				{
					HasNodeConstraint<TSubject, XAttribute> constraint =
						CreateAttribute(it, grammars, state.Name, state.GetElement);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			source);
	}

	public static AndOrResult<TSubject?, IThat<TSubject?>> HasAttribute<TSubject>(IThat<TSubject?> source,
		XName name, Action<IThat<string?>> expectations, Func<TSubject, XElement?> getElement)
		where TSubject : XNode
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint((Name: name, GetElement: getElement),
			static (state, it, grammars) => CreateAttribute(it, grammars, state.Name, state.GetElement));
		expectationBuilder
			.ForMember(MemberAccessor<FoundNode<XAttribute>, string?>.FromFunc(found => found.Node.Value,
					$"value of attribute {Formatter.Format(name.ToString())}"),
				(_, stringBuilder) => stringBuilder.Append(" whose value "))
			.AddExpectations(e => expectations(new ThatSubject<string?>(e)));
		return new AndOrResult<TSubject?, IThat<TSubject?>>(expectationBuilder, source);
	}

	public static AndOrResult<TSubject?, IThat<TSubject?>> HasElement<TSubject>(IThat<TSubject?> source,
		XName name, bool negated, Func<TSubject, XElement?> getElement)
		where TSubject : XNode
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		return new AndOrResult<TSubject?, IThat<TSubject?>>(source.Get().ExpectationBuilder.AddConstraint(
				(Name: name, Negated: negated, GetElement: getElement),
				static (state, it, grammars) =>
				{
					HasNodeConstraint<TSubject, XElement> constraint =
						CreateElement(it, grammars, state.Name, state.GetElement);
					return state.Negated ? constraint.Invert() : constraint;
				}),
			source);
	}

	public static AndOrResult<TSubject?, IThat<TSubject?>> HasElement<TSubject>(IThat<TSubject?> source,
		XName name, Action<IThat<XElement?>> expectations, Func<TSubject, XElement?> getElement)
		where TSubject : XNode
	{
		ThrowHelper.ThrowIfNull(name, nameof(name));
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint((Name: name, GetElement: getElement),
			static (state, it, grammars) => CreateElement(it, grammars, state.Name, state.GetElement));
		expectationBuilder
			.ForMember(MemberAccessor<FoundNode<XElement>, XElement?>.FromFunc(found => found.Node,
					$"element {Formatter.Format(name.ToString())}"),
				(_, stringBuilder) => stringBuilder.Append(" which "))
			.AddExpectations(e => expectations(new ThatSubject<XElement?>(e)));
		return new AndOrResult<TSubject?, IThat<TSubject?>>(expectationBuilder, source);
	}

	private static HasNodeConstraint<TSubject, XAttribute> CreateAttribute<TSubject>(string it,
		ExpectationGrammars grammars, XName name, Func<TSubject, XElement?> getElement)
		where TSubject : XNode
		=> new(it, grammars, $"attribute {Formatter.Format(name.ToString())}", getElement,
			element => element.Attribute(name));

	private static HasNodeConstraint<TSubject, XElement> CreateElement<TSubject>(string it,
		ExpectationGrammars grammars, XName name, Func<TSubject, XElement?> getElement)
		where TSubject : XNode
		=> new(it, grammars, $"element {Formatter.Format(name.ToString())}", getElement,
			element => element.Element(name));
}

/// <summary>
///     Verifies that the element of a subject has the node that the <paramref name="find" /> function looks up, e.g. an
///     attribute or a child element.
/// </summary>
/// <param name="it">The name of the subject.</param>
/// <param name="grammars">The grammars of the expectation.</param>
/// <param name="node">The description of the node, e.g. <c>attribute "id"</c>.</param>
/// <param name="getElement">Selects the element of the subject, e.g. the root element of a document.</param>
/// <param name="find">Looks up the node in the element, or returns <see langword="null" /> if it does not exist.</param>
internal sealed class HasNodeConstraint<TSubject, TNode>(
	string it,
	ExpectationGrammars grammars,
	string node,
	Func<TSubject, XElement?> getElement,
	Func<XElement, TNode?> find)
	: ConstraintResult.WithNotNullValue<TSubject?>(it, grammars),
		IValueConstraint<TSubject?>
	where TSubject : XNode
	where TNode : XObject
{
	private FoundNode<TNode>? _found;
	private bool _hasNoElement;

	public ConstraintResult IsMetBy(TSubject? actual)
	{
		Actual = actual;
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

		TNode? found = find(element);
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
	{
		if (!TryAppendMissingElement(stringBuilder))
		{
			stringBuilder.Append(It).Append(" did not have ").Append(node);
		}
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Grammars.Verb("does not have ", "do not have ")).Append(node);

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (!TryAppendMissingElement(stringBuilder))
		{
			stringBuilder.Append(It).Append(" did");
		}
	}

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
///     A node found by <see cref="HasNodeConstraint{TSubject,TNode}" />.
/// </summary>
/// <remarks>
///     The node is wrapped, so that a found child element is not taken for the subject element itself.
/// </remarks>
internal sealed class FoundNode<TNode>(TNode node)
	where TNode : XObject
{
	public TNode Node { get; } = node;
}
