using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;

namespace aweXpect.Internal.Tests.Collections;

public sealed class EnumerableQuantifierTests
{
	public sealed class AllTests
	{
		[Test]
		[Arguments(1)]
		[Arguments(-1)]
		public async Task WhenMatchingDoesNotEqualTotalItems_ShouldReturnFailure(int difference)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.All();
			StringBuilder sb = new();
			int matchingCount = 4;
			int notMatchingCount = 2;
			int? totalCount = matchingCount + difference;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, "it", matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo($"only {matchingCount} of {totalCount} were");
		}

		[Test]
		public async Task WhenMatchingEqualsTotalItems_ShouldReturnSuccess()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.All();
			int matchingCount = 4;
			int notMatchingCount = 0;
			int? totalCount = matchingCount;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Success);
		}

		[Test]
		public async Task WhenNotEnumeratedCompletely_ShouldHaveUndecidedOutcome()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.All();
			int matchingCount = 2;
			int notMatchingCount = 0;
			int? totalCount = null;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Undecided);
		}
	}

	public sealed class AtLeastTests
	{
		[Test]
		[Arguments(3)]
		[Arguments(4)]
		public async Task WhenHavingSufficientItems_ShouldReturnSuccess(int foundItems)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.AtLeast(3);
			int matchingCount = foundItems;
			int notMatchingCount = 2;
			int? totalCount = 7;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Success);
		}

		[Test]
		public async Task WhenHavingTooFewItems_ShouldReturnFailure()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.AtLeast(3);
			StringBuilder sb = new();
			int matchingCount = 2;
			int notMatchingCount = 6;
			int? totalCount = 8;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, "it", matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo("only 2 of 8 were");
		}

		[Test]
		[Arguments(1, " for no items")]
		[Arguments(2, " for fewer than 2 items")]
		public async Task WhenNegated_ShouldAppendTheComplement(int minimum, string expected)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.AtLeast(minimum);
			StringBuilder sb = new();

			sut.AppendExpectation(sb, ExpectationGrammars.Negated, (_, _) => { });

			await That(sb.ToString()).IsEqualTo(expected)
				.Because("the negation of a quantifier is its complement, not a prefixed 'not'");
		}

		[Test]
		public async Task WhenNotEnumeratedCompletely_ShouldHaveUndecidedOutcome()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.AtLeast(3);
			int matchingCount = 2;
			int notMatchingCount = 0;
			int? totalCount = null;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Undecided);
		}
	}

	public sealed class AtMostTests
	{
		[Test]
		[Arguments(3)]
		[Arguments(4)]
		public async Task WhenHavingSufficientItems_ShouldReturnSuccess(int foundItems)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.AtMost(4);
			int matchingCount = foundItems;
			int notMatchingCount = 2;
			int? totalCount = 7;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Success);
		}

		[Test]
		public async Task WhenHavingTooManyItems_ShouldReturnFailure()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.AtMost(4);
			StringBuilder sb = new();
			int matchingCount = 5;
			int notMatchingCount = 2;
			int? totalCount = 8;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, "it", matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo("5 of 8 were");
		}

		[Test]
		[Arguments(0, " for at least one item")]
		[Arguments(1, " for more than one item")]
		[Arguments(4, " for more than 4 items")]
		public async Task WhenNegated_ShouldAppendTheComplement(int maximum, string expected)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.AtMost(maximum);
			StringBuilder sb = new();

			sut.AppendExpectation(sb, ExpectationGrammars.Negated, (_, _) => { });

			await That(sb.ToString()).IsEqualTo(expected)
				.Because("the negation of a quantifier is its complement, not a prefixed 'not'");
		}

		[Test]
		public async Task WhenNotEnumeratedCompletely_ShouldHaveUndecidedOutcome()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.AtMost(4);
			int matchingCount = 2;
			int notMatchingCount = 0;
			int? totalCount = null;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Undecided);
		}
	}

	public sealed class BetweenTests
	{
		[Test]
		[Arguments(3)]
		[Arguments(4)]
		[Arguments(5)]
		public async Task WhenHavingSufficientItems_ShouldReturnSuccess(int foundItems)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Between(3, 5);
			int matchingCount = foundItems;
			int notMatchingCount = 2;
			int? totalCount = 7;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Success);
		}

		[Test]
		public async Task WhenHavingTooFewItems_ShouldReturnFailure()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Between(3, 5);
			StringBuilder sb = new();
			int matchingCount = 2;
			int notMatchingCount = 5;
			int? totalCount = 7;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, "it", matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo("only 2 of 7 were");
		}

		[Test]
		public async Task WhenHavingTooManyItems_ShouldReturnFailure()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Between(3, 5);
			StringBuilder sb = new();
			int matchingCount = 6;
			int notMatchingCount = 2;
			int? totalCount = 8;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, "it", matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo("6 of 8 were");
		}

		[Test]
		[Arguments(1, " for exactly one item")]
		[Arguments(2, " for exactly 2 items")]
		public async Task WhenMinimumEqualsMaximum_ShouldReadLikeExactly(int count, string expected)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Between(count, count);
			StringBuilder sb = new();

			sut.AppendExpectation(sb, ExpectationGrammars.None, (_, _) => { });

			await That(sb.ToString()).IsEqualTo(expected)
				.Because("a range of a single count is the same as exactly this count");
		}

		[Test]
		public async Task WhenNotEnumeratedCompletely_ShouldHaveUndecidedOutcome()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Between(3, 5);
			int matchingCount = 2;
			int notMatchingCount = 0;
			int? totalCount = null;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Undecided);
		}
	}

	public sealed class ExactlyTests
	{
		[Test]
		public async Task WhenHavingSufficientItems_ShouldReturnSuccess()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Exactly(3);
			int matchingCount = 3;
			int notMatchingCount = 2;
			int? totalCount = 7;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Success);
		}

		[Test]
		[Arguments(1)]
		[Arguments(2)]
		public async Task WhenHavingTooFewItems_ShouldReturnFailure(int foundItems)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Exactly(3);
			StringBuilder sb = new();
			int matchingCount = foundItems;
			int notMatchingCount = 6;
			int? totalCount = 8;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, "it", matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo($"only {foundItems} of 8 were");
		}

		[Test]
		[Arguments(4)]
		[Arguments(5)]
		public async Task WhenHavingTooManyItems_ShouldReturnFailure(int foundItems)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Exactly(3);
			StringBuilder sb = new();
			int matchingCount = foundItems;
			int notMatchingCount = 6;
			int? totalCount = 8;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, "it", matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo($"{foundItems} of 8 were");
		}

		[Test]
		[Arguments(0, " for at least one item")]
		[Arguments(2, " for not exactly 2 items")]
		public async Task WhenNegated_ShouldAppendTheComplementIfThereIsOne(int expectedCount, string expected)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Exactly(expectedCount);
			StringBuilder sb = new();

			sut.AppendExpectation(sb, ExpectationGrammars.Negated, (_, _) => { });

			await That(sb.ToString()).IsEqualTo(expected)
				.Because("only the negation of exactly zero is a single range");
		}

		[Test]
		public async Task WhenNotEnumeratedCompletely_ShouldHaveUndecidedOutcome()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Exactly(4);
			int matchingCount = 2;
			int notMatchingCount = 0;
			int? totalCount = null;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Undecided);
		}
	}

	public sealed class LessThanTests
	{
		[Test]
		[Arguments(1, " for at least one item")]
		[Arguments(4, " for at least 4 items")]
		public async Task WhenNegated_ShouldAppendTheComplement(int maximum, string expected)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.LessThan(maximum);
			StringBuilder sb = new();

			sut.AppendExpectation(sb, ExpectationGrammars.Negated, (_, _) => { });

			await That(sb.ToString()).IsEqualTo(expected)
				.Because("the negation of a quantifier is its complement, not a prefixed 'not'");
		}
	}

	public sealed class MoreThanTests
	{
		[Test]
		[Arguments(0, " for no items")]
		[Arguments(1, " for at most one item")]
		[Arguments(4, " for at most 4 items")]
		public async Task WhenNegated_ShouldAppendTheComplement(int minimum, string expected)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.MoreThan(minimum);
			StringBuilder sb = new();

			sut.AppendExpectation(sb, ExpectationGrammars.Negated, (_, _) => { });

			await That(sb.ToString()).IsEqualTo(expected)
				.Because("the negation of a quantifier is its complement, not a prefixed 'not'");
		}
	}

	public sealed class NoneTests
	{
		[Test]
		[Arguments(ExpectationGrammars.None, "no")]
		[Arguments(ExpectationGrammars.Plural, "no")]
		[Arguments(ExpectationGrammars.Nested, "none")]
		[Arguments(ExpectationGrammars.Nested | ExpectationGrammars.Plural, "none")]
		public async Task ShouldUseNoneOnlyWhenNested(ExpectationGrammars grammars, string expected)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.None(grammars);

			await That(sut.ToString()).IsEqualTo(expected);
		}

		[Test]
		public async Task WhenMatchingCountIsGreaterThanZero_ShouldReturnFailure()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.None();
			StringBuilder sb = new();
			int matchingCount = 1;
			int notMatchingCount = 3;
			int? totalCount = 4;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, "it", matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo("1 of 4 were");
		}

		[Test]
		public async Task WhenNotEnumeratedCompletely_ShouldHaveUndecidedOutcome()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.None();
			int matchingCount = 0;
			int notMatchingCount = 2;
			int? totalCount = null;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Undecided);
		}

		[Test]
		public async Task WhenNotMatchingEqualsTotalItems_ShouldReturnSuccess()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.None();
			int matchingCount = 0;
			int notMatchingCount = 4;
			int? totalCount = notMatchingCount;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Success);
		}
	}
}
