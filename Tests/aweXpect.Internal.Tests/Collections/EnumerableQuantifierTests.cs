using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;

namespace aweXpect.Internal.Tests.Collections;

public sealed class EnumerableQuantifierTests
{
	public sealed class AllTests
	{
		[Theory]
		[InlineData(1)]
		[InlineData(-1)]
		public async Task WhenMatchingDoesNotEqualTotalItems_ShouldReturnFailure(int difference)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.All();
			StringBuilder sb = new();
			int matchingCount = 4;
			int notMatchingCount = 2;
			int? totalCount = matchingCount + difference;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo($"only {matchingCount} of {totalCount} were");
		}

		[Fact]
		public async Task WhenMatchingEqualsTotalItems_ShouldReturnSuccess()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.All();
			int matchingCount = 4;
			int notMatchingCount = 0;
			int? totalCount = matchingCount;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Success);
		}

		[Fact]
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
		[Theory]
		[InlineData(3)]
		[InlineData(4)]
		public async Task WhenHavingSufficientItems_ShouldReturnSuccess(int foundItems)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.AtLeast(3);
			int matchingCount = foundItems;
			int notMatchingCount = 2;
			int? totalCount = 7;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Success);
		}

		[Fact]
		public async Task WhenHavingTooFewItems_ShouldReturnFailure()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.AtLeast(3);
			StringBuilder sb = new();
			int matchingCount = 2;
			int notMatchingCount = 6;
			int? totalCount = 8;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo("only 2 of 8 were");
		}

		[Fact]
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
		[Theory]
		[InlineData(3)]
		[InlineData(4)]
		public async Task WhenHavingSufficientItems_ShouldReturnSuccess(int foundItems)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.AtMost(4);
			int matchingCount = foundItems;
			int notMatchingCount = 2;
			int? totalCount = 7;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Success);
		}

		[Fact]
		public async Task WhenHavingTooManyItems_ShouldReturnFailure()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.AtMost(4);
			StringBuilder sb = new();
			int matchingCount = 5;
			int notMatchingCount = 2;
			int? totalCount = 8;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo("5 of 8 were");
		}

		[Fact]
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
		[Theory]
		[InlineData(3)]
		[InlineData(4)]
		[InlineData(5)]
		public async Task WhenHavingSufficientItems_ShouldReturnSuccess(int foundItems)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Between(3, 5);
			int matchingCount = foundItems;
			int notMatchingCount = 2;
			int? totalCount = 7;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Success);
		}

		[Fact]
		public async Task WhenHavingTooFewItems_ShouldReturnFailure()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Between(3, 5);
			StringBuilder sb = new();
			int matchingCount = 2;
			int notMatchingCount = 5;
			int? totalCount = 7;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo("only 2 of 7 were");
		}

		[Fact]
		public async Task WhenHavingTooManyItems_ShouldReturnFailure()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Between(3, 5);
			StringBuilder sb = new();
			int matchingCount = 6;
			int notMatchingCount = 2;
			int? totalCount = 8;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo("6 of 8 were");
		}

		[Fact]
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
		[Fact]
		public async Task WhenHavingSufficientItems_ShouldReturnSuccess()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Exactly(3);
			int matchingCount = 3;
			int notMatchingCount = 2;
			int? totalCount = 7;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Success);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		public async Task WhenHavingTooFewItems_ShouldReturnFailure(int foundItems)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Exactly(3);
			StringBuilder sb = new();
			int matchingCount = foundItems;
			int notMatchingCount = 6;
			int? totalCount = 8;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo($"only {foundItems} of 8 were");
		}

		[Theory]
		[InlineData(4)]
		[InlineData(5)]
		public async Task WhenHavingTooManyItems_ShouldReturnFailure(int foundItems)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.Exactly(3);
			StringBuilder sb = new();
			int matchingCount = foundItems;
			int notMatchingCount = 6;
			int? totalCount = 8;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo($"{foundItems} of 8 were");
		}

		[Fact]
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

	public sealed class NoneTests
	{
		[Theory]
		[InlineData(ExpectationGrammars.None, "no")]
		[InlineData(ExpectationGrammars.Plural, "no")]
		[InlineData(ExpectationGrammars.Nested, "none")]
		[InlineData(ExpectationGrammars.Nested | ExpectationGrammars.Plural, "none")]
		public async Task ShouldUseNoneOnlyWhenNested(ExpectationGrammars grammars, string expected)
		{
			EnumerableQuantifier sut = EnumerableQuantifier.None(grammars);

			await That(sut.ToString()).IsEqualTo(expected);
		}

		[Fact]
		public async Task WhenMatchingCountIsGreaterThanZero_ShouldReturnFailure()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.None();
			StringBuilder sb = new();
			int matchingCount = 1;
			int notMatchingCount = 3;
			int? totalCount = 4;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			sut.AppendResult(sb, ExpectationGrammars.None, matchingCount, notMatchingCount, totalCount, "were");
			await That(result).IsEqualTo(Outcome.Failure);
			await That(sb.ToString()).IsEqualTo("1 of 4 were");
		}

		[Fact]
		public async Task WhenNotEnumeratedCompletely_ShouldHaveUndecidedOutcome()
		{
			EnumerableQuantifier sut = EnumerableQuantifier.None();
			int matchingCount = 0;
			int notMatchingCount = 2;
			int? totalCount = null;

			Outcome result = sut.GetOutcome(matchingCount, notMatchingCount, totalCount);

			await That(result).IsEqualTo(Outcome.Undecided);
		}

		[Fact]
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
