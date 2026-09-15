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
			.WithMessage("*The parameter 'minimum' must be non-negative*").AsWildcard();
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
			.WithMessage("*The parameter 'maximum' must be non-negative*").AsWildcard();
	}

	[Theory]
	[InlineData(2, 1, true)]
	[InlineData(1, 1, false)]
	[InlineData(1, 2, false)]
	public async Task Between_WhenMaximumIsLessThanMinimum_ShouldThrowArgumentException(
		int minimum, int maximum, bool expectThrow)
	{
		Quantifier sut = new();

		void Act() => sut.Between(minimum, maximum);

		await That(Act).Throws<ArgumentException>().OnlyIf(expectThrow)
			.WithMessage("*The parameter 'maximum' must be greater than or equal to 'minimum'*").AsWildcard();
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
			.WithMessage("*The parameter 'maximum' must be non-negative*").AsWildcard();
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
			.WithMessage("*The parameter 'minimum' must be non-negative*").AsWildcard();
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
			.WithMessage("*The parameter 'expected' must be non-negative*").AsWildcard();
	}

	private static Quantifier Configure(Action<Quantifier> configure)
	{
		Quantifier quantifier = new();
		configure(quantifier);
		return quantifier;
	}
}
