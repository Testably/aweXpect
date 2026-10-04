using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class QuantifierTests
{
	[Theory]
	[InlineData(-1, true)]
	[InlineData(0, false)]
	[InlineData(1, false)]
	public async Task AtLeast_WhenMinimumIsNegative_ShouldThrowArgumentOutOfRangeException(
		int minimum, bool expectThrow)
	{
		Quantifier sut = new();

		void Act() => sut.AtLeast(minimum);

		await That(Act).Throws<ArgumentOutOfRangeException>().OnlyIf(expectThrow)
			.WithMessage("*The minimum must not be negative.*").AsWildcard();
	}

	[Fact]
	public async Task AtMost_WhenAMinimumIsSpecified_ShouldThrowInvalidOperationException()
	{
		Quantifier sut = new();
		sut.AtLeast(2);

		void Act() => sut.AtMost(5);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AtMost cannot be combined with AtLeast.")
			.Because("the maximum would silently replace the minimum");
	}

	[Theory]
	[InlineData(-1, true)]
	[InlineData(0, false)]
	[InlineData(1, false)]
	public async Task AtMost_WhenMaximumIsNegative_ShouldThrowArgumentOutOfRangeException(
		int maximum, bool expectThrow)
	{
		Quantifier sut = new();

		void Act() => sut.AtMost(maximum);

		await That(Act).Throws<ArgumentOutOfRangeException>().OnlyIf(expectThrow)
			.WithMessage("*The maximum must not be negative.*").AsWildcard();
	}

	[Theory]
	[InlineData(2, 1, true)]
	[InlineData(1, 1, false)]
	[InlineData(1, 2, false)]
	public async Task Between_WhenMaximumIsLessThanMinimum_ShouldThrowArgumentOutOfRangeException(
		int minimum, int maximum, bool expectThrow)
	{
		Quantifier sut = new();

		void Act() => sut.Between(minimum, maximum);

		await That(Act).Throws<ArgumentOutOfRangeException>().OnlyIf(expectThrow)
			.WithParamName("maximum").And
			.WithMessage("*The maximum must be greater than or equal to the minimum.*").AsWildcard();
	}

	[Theory]
	[InlineData(-1, true)]
	[InlineData(0, false)]
	[InlineData(1, false)]
	public async Task Between_WhenMaximumIsNegative_ShouldThrowArgumentOutOfRangeException(
		int maximum, bool expectThrow)
	{
		Quantifier sut = new();

		void Act() => sut.Between(0, maximum);

		await That(Act).Throws<ArgumentOutOfRangeException>().OnlyIf(expectThrow)
			.WithMessage("*The maximum must not be negative.*").AsWildcard();
	}

	[Theory]
	[InlineData(-1, true)]
	[InlineData(0, false)]
	[InlineData(1, false)]
	public async Task Between_WhenMinimumIsNegative_ShouldThrowArgumentOutOfRangeException(
		int minimum, bool expectThrow)
	{
		Quantifier sut = new();

		void Act() => sut.Between(minimum, 1);

		await That(Act).Throws<ArgumentOutOfRangeException>().OnlyIf(expectThrow)
			.WithMessage("*The minimum must not be negative.*").AsWildcard();
	}

	[Theory]
	[InlineData(0, false, false)]
	[InlineData(1, false, true)]
	[InlineData(2, false, true)]
	[InlineData(3, false, false)]
	[InlineData(0, true, true)]
	[InlineData(1, true, false)]
	[InlineData(2, true, false)]
	[InlineData(3, true, true)]
	public async Task Check_ShouldInvertTheDecisionWhenNegated(int amount, bool isNegated, bool expected)
	{
		Quantifier sut = Configure(q => q.Between(1, 2));

		bool? result = sut.Check(amount, true, isNegated);

		await That(result ?? isNegated).IsEqualTo(expected);
	}

	[Fact]
	public async Task Check_ShouldNotChangeTheQuantifier()
	{
		Quantifier sut = Configure(q => q.AtLeast(2));

		bool? negated = sut.Check(2, true, true);
		bool? notNegated = sut.Check(2, true);

		await That(negated).IsFalse();
		await That(notNegated).IsTrue().Because("a negated check must not leave the shared quantifier negated");
	}

	[Theory]
	[InlineData(0, 0)]
	[InlineData(1, 1)]
	[InlineData(3, 3)]
	public async Task DeterminableAmount_AtLeast_ShouldBeTheMinimum(int minimum, int expected)
	{
		Quantifier sut = new();
		sut.AtLeast(minimum);

		await That(sut.DeterminableAmount).IsEqualTo(expected);
	}

	[Theory]
	[InlineData(0, 1)]
	[InlineData(3, 4)]
	public async Task DeterminableAmount_AtMost_ShouldBeOneAboveTheMaximum(int maximum, int expected)
	{
		Quantifier sut = new();
		sut.AtMost(maximum);

		await That(sut.DeterminableAmount).IsEqualTo(expected);
	}

	[Theory]
	[InlineData(2, 4, 5)]
	[InlineData(0, 0, 1)]
	public async Task DeterminableAmount_Between_ShouldBeOneAboveTheMaximum(int minimum, int maximum, int expected)
	{
		Quantifier sut = new();
		sut.Between(minimum, maximum);

		await That(sut.DeterminableAmount).IsEqualTo(expected);
	}

	[Theory]
	[InlineData(0, 1)]
	[InlineData(2, 3)]
	public async Task DeterminableAmount_Exactly_ShouldBeOneAboveTheExpected(int expectedOccurrences, int expected)
	{
		Quantifier sut = new();
		sut.Exactly(expectedOccurrences);

		await That(sut.DeterminableAmount).IsEqualTo(expected);
	}

	[Theory]
	[InlineData(0, 0)]
	[InlineData(3, 3)]
	public async Task DeterminableAmount_LessThan_ShouldBeTheMaximum(int maximum, int expected)
	{
		Quantifier sut = new();
		sut.LessThan(maximum);

		await That(sut.DeterminableAmount).IsEqualTo(expected);
	}

	[Theory]
	[InlineData(0, 1)]
	[InlineData(2, 3)]
	public async Task DeterminableAmount_MoreThan_ShouldBeOneAboveTheMinimum(int minimum, int expected)
	{
		Quantifier sut = new();
		sut.MoreThan(minimum);

		await That(sut.DeterminableAmount).IsEqualTo(expected);
	}

	[Fact]
	public async Task DeterminableAmount_ShouldBeTheSmallestAmountThatCheckCanDecide()
	{
		Quantifier[] quantifiers =
		[
			new Quantifier(),
			Configure(q => q.AtLeast(3)),
			Configure(q => q.AtMost(3)),
			Configure(q => q.Between(2, 4)),
			Configure(q => q.Exactly(2)),
			Configure(q => q.LessThan(3)),
			Configure(q => q.MoreThan(2)),
			Quantifier.Never(),
		];

		foreach (Quantifier sut in quantifiers)
		{
			int amount = sut.DeterminableAmount;

			await That(sut.Check(amount, false)).IsNotNull()
				.Because($"'{sut}' must be decided once {amount} occurred");

			if (amount > 0)
			{
				await That(sut.Check(amount - 1, false)).IsNull()
					.Because($"'{sut}' must still be undecided at {amount - 1}");
			}
		}
	}

	[Fact]
	public async Task DeterminableAmount_WhenNotSpecified_ShouldBeOne()
	{
		Quantifier sut = new();

		await That(sut.DeterminableAmount).IsEqualTo(1);
	}

	[Fact]
	public async Task DeterminableAmount_WhenTheBoundIsTheLargestValue_ShouldNotOverflow()
	{
		Quantifier atMost = new();
		atMost.AtMost(int.MaxValue);
		Quantifier moreThan = new();
		moreThan.MoreThan(int.MaxValue);

		await That(atMost.DeterminableAmount).IsEqualTo(int.MaxValue);
		await That(moreThan.DeterminableAmount).IsEqualTo(int.MaxValue);
	}

	[Theory]
	[InlineData(-1, true)]
	[InlineData(0, false)]
	[InlineData(1, false)]
	public async Task Exactly_WhenExpectedIsNegative_ShouldThrowArgumentOutOfRangeException(
		int expected, bool expectThrow)
	{
		Quantifier sut = new();

		void Act() => sut.Exactly(expected);

		await That(Act).Throws<ArgumentOutOfRangeException>().OnlyIf(expectThrow)
			.WithMessage("*The expected count must not be negative.*").AsWildcard();
	}

	[Fact]
	public async Task Exactly_WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
	{
		Quantifier sut = new();
		sut.Exactly(1);

		void Act() => sut.Exactly(2);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Exactly cannot be specified more than once.");
	}

	[Theory]
	[InlineData("AtLeast", 1, true)]
	[InlineData("AtLeast", 2, false)]
	[InlineData("AtMost", 0, false)]
	[InlineData("Exactly", 0, false)]
	[InlineData("LessThan", 1, false)]
	[InlineData("MoreThan", 0, true)]
	[InlineData("MoreThan", 1, false)]
	public async Task IsNever_WhenNegated_ShouldBeTrueWhenTheComplementIsOnlyMetByZero(string method, int value,
		bool expected)
	{
		Quantifier sut = Configure(method, value);

		await That(sut.IsNever(true)).IsEqualTo(expected);
	}

	[Theory]
	[InlineData("AtLeast", 1, false)]
	[InlineData("AtMost", 0, true)]
	[InlineData("AtMost", 1, false)]
	[InlineData("Between", 0, false)]
	[InlineData("Exactly", 0, true)]
	[InlineData("LessThan", 1, true)]
	[InlineData("LessThan", 2, false)]
	[InlineData("MoreThan", 0, false)]
	public async Task IsNever_WhenNotNegated_ShouldBeTrueWhenOnlyZeroMeetsTheQuantifier(string method, int value,
		bool expected)
	{
		Quantifier sut = Configure(method, value);

		await That(sut.IsNever(false)).IsEqualTo(expected);
	}

	[Theory]
	[InlineData(2, "fewer than twice")]
	[InlineData(3, "fewer than 3 times")]
	public async Task ToString_LessThan_ShouldSayFewerThan(int maximum, string expected)
	{
		Quantifier sut = Configure(q => q.LessThan(maximum));

		await That(sut.ToString()).IsEqualTo(expected);
	}

	[Theory]
	[InlineData("AtLeast", 1, "never")]
	[InlineData("AtLeast", 2, "fewer than twice")]
	[InlineData("AtLeast", 3, "fewer than 3 times")]
	[InlineData("AtMost", 0, "at least once")]
	[InlineData("AtMost", 1, "more than once")]
	[InlineData("AtMost", 3, "more than 3 times")]
	[InlineData("Between", 3, "not between 3 and 5 times")]
	[InlineData("Exactly", 0, "at least once")]
	[InlineData("Exactly", 1, "not exactly once")]
	[InlineData("Exactly", 2, "not exactly twice")]
	[InlineData("Exactly", 3, "not exactly 3 times")]
	[InlineData("LessThan", 1, "at least once")]
	[InlineData("LessThan", 4, "at least 4 times")]
	[InlineData("MoreThan", 0, "never")]
	[InlineData("MoreThan", 1, "at most once")]
	[InlineData("MoreThan", 2, "at most twice")]
	[InlineData("MoreThan", 3, "at most 3 times")]
	public async Task ToString_WhenNegated_ShouldDescribeTheComplement(string method, int value, string expected)
	{
		Quantifier sut = Configure(method, value);

		string result = sut.ToString(true);

		await That(result).IsEqualTo(expected);
	}

	[Fact]
	public async Task ToString_WhenNegatedDefault_ShouldBeNever()
	{
		Quantifier sut = new();

		string result = sut.ToString(true);

		await That(result).IsEqualTo("never");
	}

	[Theory]
	[InlineData("AtMost", 0)]
	[InlineData("Exactly", 0)]
	[InlineData("LessThan", 1)]
	public async Task ToString_WhenOnlyZeroMeetsTheQuantifier_ShouldBeNever(string method, int value)
	{
		Quantifier sut = Configure(method, value);

		await That(sut.ToString()).IsEqualTo("never");
	}

	private static Quantifier Configure(Action<Quantifier> configure)
	{
		Quantifier quantifier = new();
		configure(quantifier);
		return quantifier;
	}

	private static Quantifier Configure(string method, int value)
		=> Configure(method switch
		{
			"AtLeast" => q => q.AtLeast(value),
			"AtMost" => q => q.AtMost(value),
			"Between" => q => q.Between(value, 5),
			"Exactly" => q => q.Exactly(value),
			"LessThan" => q => q.LessThan(value),
			_ => q => q.MoreThan(value),
		});
}
