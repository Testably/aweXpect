using System;
using System.Text;
using System.Xml.Linq;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect.Xml;

/// <summary>
///     Adds the constraint that the value of the element of a subject satisfies expectations.
/// </summary>
internal static class HasValueConstraint
{
	public static AndOrResult<TSubject, IThat<TSubject?>> HasValue<TSubject>(IThat<TSubject?> source,
		Action<IThatSubject<string?>> expectations, Func<TSubject, XElement?> getElement, string memberName)
		where TSubject : XNode
	{
		ThrowHelper.ThrowIfNull(expectations, nameof(expectations));
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		expectationBuilder.AddConstraint(getElement,
			static (getElement, it, grammars) => new HasValueConstraint<TSubject>(it, grammars, getElement));
		expectationBuilder
			.ForMember(MemberAccessor<FoundNode<XElement>, string?>.FromFuncWithName(found => found.Node.Value, memberName),
				(_, stringBuilder) => stringBuilder.Append(" which "))
			.AddExpectations(expectations, static (expectations, e) => expectations(new ThatSubject<string?>(e)));
		return new AndOrResult<TSubject, IThat<TSubject?>>(expectationBuilder, source);
	}
}

/// <summary>
///     Verifies that the subject is not <see langword="null" /> and has the element selected by
///     <paramref name="getElement" />, so that the expectations on its value are only verified when it exists.
/// </summary>
/// <param name="it">The name of the subject.</param>
/// <param name="grammars">The grammars of the expectation.</param>
/// <param name="getElement">Selects the element of the subject, e.g. the root element of a document.</param>
internal sealed class HasValueConstraint<TSubject>(
	string it,
	ExpectationGrammars grammars,
	Func<TSubject, XElement?> getElement)
	: ConstraintResult.WithNotNullValue<TSubject?>(it, grammars),
		IValueConstraint<TSubject?>
	where TSubject : XNode
{
	private FoundNode<XElement>? _found;

	public ConstraintResult IsMetBy(TSubject? actual)
	{
		Actual = actual;
		_found = null;
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
		Outcome = Outcome.Success;
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

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Grammars.Verb("has a value", "have a value"));

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		=> AppendResult(stringBuilder);

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(Grammars.Verb("does not have a value", "do not have a value"));

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> AppendResult(stringBuilder);

	/// <remarks>
	///     A subject that is not <see langword="null" /> fails only without an element, and the negation also when the
	///     element exists, so that both results share the text.
	/// </remarks>
	private void AppendResult(StringBuilder stringBuilder)
		=> stringBuilder.Append(It).Append(_found is null ? " had no root element" : " did");
}
