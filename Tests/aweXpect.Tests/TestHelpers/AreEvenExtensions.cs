using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Tests;

public static class AreEvenExtensions
{
	public static AndOrResult<IEnumerable<int>, IThat<IEnumerable<int>?>> AreEven(
		this aweXpect.ThatEnumerable.Elements<int> elements)
	{
		aweXpect.ThatEnumerable.IElements<int> source = elements;
		ExpectationBuilder expectationBuilder =
			((IExpectThat<IEnumerable<int>?>)source.Subject).ExpectationBuilder;
		return new AndOrResult<IEnumerable<int>, IThat<IEnumerable<int>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AreEvenConstraint(expectationBuilder, it, grammars, source.Quantifier)),
			source.Subject);
	}

	private sealed class AreEvenConstraint(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier)
		: QuantifiedCollectionConstraint<IEnumerable<int>?, int>(expectationBuilder, it, grammars, quantifier,
				g => g.IsPlural() ? "are even" : "is even", "were"),
			IValueConstraint<IEnumerable<int>?>
	{
		public ConstraintResult IsMetBy(IEnumerable<int>? actual)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			foreach (int item in actual)
			{
				Record(item, item % 2 == 0);
			}

			Complete();
			return this;
		}
	}
}
