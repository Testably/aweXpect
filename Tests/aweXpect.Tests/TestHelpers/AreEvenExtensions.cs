using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Tests;

public static class AreEvenExtensions
{
	public static AndOrResult<IEnumerable<int>, IThat<IEnumerable<int>?>> AreEven(
		this IEnumerableItems<int> items)
	{
		ExpectationBuilder expectationBuilder =
			((IExpectThat<IEnumerable<int>?>)items.Subject).ExpectationBuilder;
		return new AndOrResult<IEnumerable<int>, IThat<IEnumerable<int>?>>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new AreEvenConstraint(it, grammars, items.Quantifier)),
			items.Subject);
	}

	private sealed class AreEvenConstraint(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier)
		: QuantifiedCollectionConstraint<IEnumerable<int>?, int>(it, grammars, quantifier,
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
