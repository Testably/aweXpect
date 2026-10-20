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
///     Verifies that the element of an XML node matches the expected XML.
/// </summary>
/// <param name="it">The name of the subject.</param>
/// <param name="grammars">The grammars of the expectation.</param>
/// <param name="expected">The expected XML.</param>
/// <param name="expectedElement">The root element of the <paramref name="expected" /> XML.</param>
/// <param name="exactly">Whether the additional attributes and child nodes of the subject fail the expectation.</param>
/// <param name="getElement">Selects the compared element of the node, e.g. the root element of a document.</param>
/// <param name="options">The options of the comparison.</param>
internal sealed class MatchesXmlConstraint<TNode>(
	string it,
	ExpectationGrammars grammars,
	string expected,
	XElement expectedElement,
	bool exactly,
	Func<TNode, XElement?> getElement,
	XmlOptions options)
	: ConstraintResult.WithNotNullValue<TNode?>(it, grammars),
		IValueConstraint<TNode?>
	where TNode : XNode
{
	private XmlComparer.XmlComparisonResult? _comparisonResult;
	private bool _hasNoElement;

	/// <summary>
	///     Adds the constraint that the element of the <paramref name="source" /> matches the <paramref name="expected" />
	///     XML.
	/// </summary>
	/// <remarks>
	///     The <paramref name="expected" /> XML is parsed once when the expectation is specified, so that an invalid value
	///     is rejected for every subject, also when the constraint is not evaluated, e.g. after a failed
	///     <see cref="ThatXmlString.IsValidXml" />.
	/// </remarks>
	/// <exception cref="ArgumentNullException">The <paramref name="expected" /> value is <see langword="null" />.</exception>
	/// <exception cref="ArgumentException">The <paramref name="expected" /> value is no valid XML.</exception>
	public static AndOrResult<TNode?, IThat<TNode?>> Add(IThat<TNode?> source, string expected, bool exactly,
		Func<TNode, XElement?> getElement, Func<XmlOptions, XmlOptions>? options)
	{
		XElement expectedElement = SecureXml.LoadExpected(expected).Root!;
		XmlOptions xmlOptions = options?.Invoke(new XmlOptions()) ?? new XmlOptions();
		return new AndOrResult<TNode?, IThat<TNode?>>(source.Get().ExpectationBuilder.AddConstraint(
				(Expected: expected, ExpectedElement: expectedElement, Exactly: exactly, GetElement: getElement,
					Options: xmlOptions),
				static (state, it, grammars) => new MatchesXmlConstraint<TNode>(it, grammars, state.Expected,
					state.ExpectedElement, state.Exactly, state.GetElement, state.Options)),
			source);
	}

	public ConstraintResult IsMetBy(TNode? actual)
	{
		Actual = actual;
		_comparisonResult = null;
		if (actual is null)
		{
			Outcome = Outcome.Failure;
			return this;
		}

		XElement? element = getElement(actual);
		_hasNoElement = element is null;
		if (element is null)
		{
			Outcome = Outcome.FailureBothWays;
			return this;
		}

		_comparisonResult = XmlComparer.Compare(element, expectedElement, options, !exactly);
		Outcome = _comparisonResult.HasError ? Outcome.Failure : Outcome.Success;
		return this;
	}

	public override void AppendContexts(ResultContextCollector contexts)
	{
		if (Grammars.IsNegated() && Outcome == Outcome.Failure)
		{
			TNode? actual = Actual;
			contexts.Add(new ResultContext.SyncCallback("Actual", () => Formatter.Format(actual)));
		}

		options.AddContext(contexts, _comparisonResult?.GetUnmatchedMembersToIgnore());
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendMatchExpectation(stringBuilder, Grammars.Verb("matches ", "match "));

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (!TryAppendMissingElement(stringBuilder))
		{
			stringBuilder.Append(It).Append(" differed as")
				.Append(_comparisonResult?.ToString().Indent(indentation, false));
		}
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> AppendMatchExpectation(stringBuilder, Grammars.Verb("does not match ", "do not match "));

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (!TryAppendMissingElement(stringBuilder))
		{
			stringBuilder.Append(It).Append(" did");
		}
	}

	private void AppendMatchExpectation(StringBuilder stringBuilder, string verb)
	{
		stringBuilder.Append(verb).Append(expected);
		if (exactly)
		{
			stringBuilder.Append(" exactly");
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
