using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Results;
using aweXpect.Xml.Results;

namespace aweXpect.Xml;

/// <summary>
///     Adds the constraint that the value of the element of a subject satisfies expectations.
/// </summary>
internal static class HasValueConstraint
{
	public static XmlValueResult<TSubject, IThat<TSubject?>> HasValue<TSubject>(IThat<TSubject?> source,
		Func<TSubject, XElement?> getElement, string memberName)
		where TSubject : XNode
	{
		XmlComparisonSlot slot = new("HasValue()");
		return new XmlValueResult<TSubject, IThat<TSubject?>>(source.Get().ExpectationBuilder.AddConstraint(
				(GetElement: getElement, MemberName: memberName, Slot: slot),
				static (state, it, grammars) => new HasValueConstraint<TSubject>(it, grammars, state.GetElement,
					state.MemberName, new NodeValueComparison<XElement>(state.Slot, static element => element.Value))),
			source,
			slot);
	}

	public static AndOrResult<TSubject, IThat<TSubject?>> HasValue<TSubject>(IThat<TSubject?> source,
		Action<IThatSubject<string>> expectations, Func<TSubject, XElement?> getElement, string memberName)
		where TSubject : XNode
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint((GetElement: getElement, MemberName: memberName),
			static (state, it, grammars)
				=> new HasValueConstraint<TSubject>(it, grammars, state.GetElement, state.MemberName));
		expectationBuilder
			.ForMember(MemberAccessor<FoundNode<XElement>, string>.FromFuncWithName(found => found.Node.Value, memberName),
				(_, stringBuilder) => stringBuilder.Append(" which "))
			.AddExpectations(expectations);
		return new AndOrResult<TSubject, IThat<TSubject?>>(expectationBuilder, source);
	}
}

/// <summary>
///     Verifies that the subject is not <see langword="null" /> and has the element selected by
///     <paramref name="getElement" />, so that the expectations on its value are only verified when it exists, and when
///     an operator on the result handed a comparison to the <paramref name="comparison" />, that its value satisfies it.
/// </summary>
/// <param name="it">The name of the subject.</param>
/// <param name="grammars">The grammars of the expectation.</param>
/// <param name="getElement">Selects the element of the subject, e.g. the root element of a document.</param>
/// <param name="memberName">Names the value in the result, e.g. <c>value of the element</c>.</param>
/// <param name="comparison">Compares the value of the element, if the result offers operators.</param>
internal sealed class HasValueConstraint<TSubject>(
	string it,
	ExpectationGrammars grammars,
	Func<TSubject, XElement?> getElement,
	string memberName,
	NodeValueComparison<XElement>? comparison = null)
	: ConstraintResult.WithNotNullValue<TSubject?>(it, grammars),
		IAsyncContextConstraint<TSubject?>
	where TSubject : XNode
{
	private FoundNode<XElement>? _found;

	public async ValueTask<ConstraintResult> IsMetBy(TSubject? actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		Actual = actual;
		_found = null;
		comparison?.Reset();
		if (actual is null)
		{
			Outcome = Outcome.Failure;
			return this;
		}

		XElement? element = getElement(actual);
		if (element is null)
		{
			Outcome = Outcome.FailureBothWays;
			return this;
		}

		_found = new FoundNode<XElement>(element);
		Outcome = comparison is null
			? Outcome.Success
			: await comparison.Evaluate(element, context, cancellationToken);
		return this;
	}

	/// <remarks>
	///     Stores the element, so that the expectations on its value are only verified when it exists.
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
		comparison?.AppendContexts(contexts);
		if (Outcome != Outcome.Success)
		{
			HasNodeConstraint.AddActualContext(contexts, _found?.Node);
		}
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("has a value", "have a value"));
		comparison?.AppendExpectation(stringBuilder);
	}

	/// <remarks>
	///     A subject that is not <see langword="null" /> fails only without an element or with a value that does not
	///     satisfy the comparison, and the negation also when the element exists, so that the negated result shares the
	///     text.
	/// </remarks>
	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (comparison is { IsCompared: true, })
		{
			comparison.AppendResult(stringBuilder, memberName, Grammars, indentation);
			return;
		}

		stringBuilder.Append(It).Append(_found is null ? " had no root element" : " did");
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("does not have a value", "do not have a value"));
		comparison?.AppendExpectation(stringBuilder);
	}
}
