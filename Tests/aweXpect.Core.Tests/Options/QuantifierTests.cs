using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class QuantifierTests
{
	[Test]
	[Arguments(-1, true)]
	[Arguments(0, false)]
	[Arguments(1, false)]
	public async Task AtLeast_WhenMinimumIsNegative_ShouldThrowArgumentOutOfRangeException(
		int minimum, bool expectThrow)
	{
		Quantifier sut = new();

		void Act() => sut.AtLeast(minimum);

		await That(Act).Throws<ArgumentOutOfRangeException>().OnlyIf(expectThrow)
			.WithMessage("*The minimum must not be negative.*").AsWildcard();
	}

	[Test]
	public async Task AtMost_WhenAMinimumIsSpecified_ShouldThrowInvalidOperationException()
	{
		Quantifier sut = new();
		sut.AtLeast(2);

		void Act() => sut.AtMost(5);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("AtMost cannot be combined with AtLeast.")
			.Because("the maximum would silently replace the minimum");
	}

	[Test]
	[Arguments(-1, true)]
	[Arguments(0, false)]
	[Arguments(1, false)]
	public async Task AtMost_WhenMaximumIsNegative_ShouldThrowArgumentOutOfRangeException(
		int maximum, bool expectThrow)
	{
		Quantifier sut = new();

		void Act() => sut.AtMost(maximum);

		await That(Act).Throws<ArgumentOutOfRangeException>().OnlyIf(expectThrow)
			.WithMessage("*The maximum must not be negative.*").AsWildcard();
	}

	[Test]
	[Arguments(2, 1, true)]
	[Arguments(1, 1, false)]
	[Arguments(1, 2, false)]
	public async Task Between_WhenMaximumIsLessThanMinimum_ShouldThrowArgumentOutOfRangeException(
		int minimum, int maximum, bool expectThrow)
	{
		Quantifier sut = new();

		void Act() => sut.Between(minimum, maximum);

		await That(Act).Throws<ArgumentOutOfRangeException>().OnlyIf(expectThrow)
			.WithParamName("maximum").And
			.WithMessage("*The maximum must be greater than or equal to the minimum.*").AsWildcard();
	}

	[Test]
	[Arguments(-1, true)]
	[Arguments(0, false)]
	[Arguments(1, false)]
	public async Task Between_WhenMaximumIsNegative_ShouldThrowArgumentOutOfRangeException(
		int maximum, bool expectThrow)
	{
		Quantifier sut = new();

		void Act() => sut.Between(0, maximum);

		await That(Act).Throws<ArgumentOutOfRangeException>().OnlyIf(expectThrow)
			.WithMessage("*The maximum must not be negative.*").AsWildcard();
	}

	[Test]
	[Arguments(-1, true)]
	[Arguments(0, false)]
	[Arguments(1, false)]
	public async Task Between_WhenMinimumIsNegative_ShouldThrowArgumentOutOfRangeException(
		int minimum, bool expectThrow)
	{
		Quantifier sut = new();

		void Act() => sut.Between(minimum, 1);

		await That(Act).Throws<ArgumentOutOfRangeException>().OnlyIf(expectThrow)
			.WithMessage("*The minimum must not be negative.*").AsWildcard();
	}

	[Test]
	[Arguments(0, false, false)]
	[Arguments(1, false, true)]
	[Arguments(2, false, true)]
	[Arguments(3, false, false)]
	[Arguments(0, true, true)]
	[Arguments(1, true, false)]
	[Arguments(2, true, false)]
	[Arguments(3, true, true)]
	public async Task Check_ShouldInvertTheDecisionWhenNegated(int amount, bool isNegated, bool expected)
	{
		Quantifier sut = Configure(q => q.Between(1, 2));

		bool? result = sut.Check(amount, true, isNegated);

		await That(result ?? isNegated).IsEqualTo(expected);
	}

	[Test]
	public async Task Check_ShouldNotChangeTheQuantifier()
	{
		Quantifier sut = Configure(q => q.AtLeast(2));

		bool? negated = sut.Check(2, true, true);
		bool? notNegated = sut.Check(2, true);

		await That(negated).IsFalse();
		await That(notNegated).IsTrue().Because("a negated check must not leave the shared quantifier negated");
	}

	[Test]
	[Arguments(0, 0)]
	[Arguments(1, 1)]
	[Arguments(3, 3)]
	public async Task DeterminableAmount_AtLeast_ShouldBeTheMinimum(int minimum, int expected)
	{
		Quantifier sut = new();
		sut.AtLeast(minimum);

		await That(sut.DeterminableAmount).IsEqualTo(expected);
	}

	[Test]
	[Arguments(0, 1)]
	[Arguments(3, 4)]
	public async Task DeterminableAmount_AtMost_ShouldBeOneAboveTheMaximum(int maximum, int expected)
	{
		Quantifier sut = new();
		sut.AtMost(maximum);

		await That(sut.DeterminableAmount).IsEqualTo(expected);
	}

	[Test]
	[Arguments(2, 4, 5)]
	[Arguments(0, 0, 1)]
	public async Task DeterminableAmount_Between_ShouldBeOneAboveTheMaximum(int minimum, int maximum, int expected)
	{
		Quantifier sut = new();
		sut.Between(minimum, maximum);

		await That(sut.DeterminableAmount).IsEqualTo(expected);
	}

	[Test]
	[Arguments(0, 1)]
	[Arguments(2, 3)]
	public async Task DeterminableAmount_Exactly_ShouldBeOneAboveTheExpected(int expectedOccurrences, int expected)
	{
		Quantifier sut = new();
		sut.Exactly(expectedOccurrences);

		await That(sut.DeterminableAmount).IsEqualTo(expected);
	}

	[Test]
	[Arguments(1, 1)]
	[Arguments(3, 3)]
	public async Task DeterminableAmount_LessThan_ShouldBeTheMaximum(int maximum, int expected)
	{
		Quantifier sut = new();
		sut.LessThan(maximum);

		await That(sut.DeterminableAmount).IsEqualTo(expected);
	}

	[Test]
	[Arguments(0, 1)]
	[Arguments(2, 3)]
	public async Task DeterminableAmount_MoreThan_ShouldBeOneAboveTheMinimum(int minimum, int expected)
	{
		Quantifier sut = new();
		sut.MoreThan(minimum);

		await That(sut.DeterminableAmount).IsEqualTo(expected);
	}

	[Test]
	public async Task DeterminableAmount_ShouldBeTheSmallestAmountThatCheckCanDecide()
	{
		Quantifier[] quantifiers =
		[
			new(),
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

	[Test]
	public async Task DeterminableAmount_WhenNotSpecified_ShouldBeOne()
	{
		Quantifier sut = new();

		await That(sut.DeterminableAmount).IsEqualTo(1);
	}

	[Test]
	public async Task DeterminableAmount_WhenTheBoundIsTheLargestValue_ShouldNotOverflow()
	{
		Quantifier atMost = new();
		atMost.AtMost(int.MaxValue);
		Quantifier moreThan = new();
		moreThan.MoreThan(int.MaxValue - 1);

		await That(atMost.DeterminableAmount).IsEqualTo(int.MaxValue);
		await That(moreThan.DeterminableAmount).IsEqualTo(int.MaxValue);
	}

	[Test]
	[Arguments(-1, true)]
	[Arguments(0, false)]
	[Arguments(1, false)]
	public async Task Exactly_WhenExpectedIsNegative_ShouldThrowArgumentOutOfRangeException(
		int expected, bool expectThrow)
	{
		Quantifier sut = new();

		void Act() => sut.Exactly(expected);

		await That(Act).Throws<ArgumentOutOfRangeException>().OnlyIf(expectThrow)
			.WithMessage("*The expected count must not be negative.*").AsWildcard();
	}

	[Test]
	public async Task Exactly_WhenSpecifiedTwice_ShouldThrowInvalidOperationException()
	{
		Quantifier sut = new();
		sut.Exactly(1);

		void Act() => sut.Exactly(2);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Exactly cannot be specified more than once.");
	}

	[Test]
	[Arguments("AtLeast", 1, true)]
	[Arguments("AtLeast", 2, false)]
	[Arguments("AtMost", 0, false)]
	[Arguments("Exactly", 0, false)]
	[Arguments("LessThan", 1, false)]
	[Arguments("MoreThan", 0, true)]
	[Arguments("MoreThan", 1, false)]
	public async Task IsNever_WhenNegated_ShouldBeTrueWhenTheComplementIsOnlyMetByZero(string method, int value,
		bool expected)
	{
		Quantifier sut = Configure(method, value);

		await That(sut.IsNever(true)).IsEqualTo(expected);
	}

	[Test]
	[Arguments("AtLeast", 1, false)]
	[Arguments("AtMost", 0, true)]
	[Arguments("AtMost", 1, false)]
	[Arguments("Between", 0, false)]
	[Arguments("Exactly", 0, true)]
	[Arguments("LessThan", 1, true)]
	[Arguments("LessThan", 2, false)]
	[Arguments("MoreThan", 0, false)]
	public async Task IsNever_WhenNotNegated_ShouldBeTrueWhenOnlyZeroMeetsTheQuantifier(string method, int value,
		bool expected)
	{
		Quantifier sut = Configure(method, value);

		await That(sut.IsNever(false)).IsEqualTo(expected);
	}

	[Test]
	public async Task LessThan_WhenMaximumIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		Quantifier sut = new();

		void Act() => sut.LessThan(-1);

		await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
			.WithParamName("maximum").And
			.WithMessage("The maximum must not be negative.").AsPrefix();
	}

	[Test]
	public async Task LessThan_WhenMaximumIsZero_ShouldThrowArgumentOutOfRangeException()
	{
		Quantifier sut = new();

		void Act() => sut.LessThan(0);

		await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
			.WithParamName("maximum").And
			.WithMessage("The maximum must be greater than zero.").AsPrefix()
			.Because("no count is fewer than zero, so the expectation could never succeed and its negation never fail");
	}

	[Test]
	public async Task MoreThan_WhenMinimumIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		Quantifier sut = new();

		void Act() => sut.MoreThan(-1);

		await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
			.WithParamName("minimum").And
			.WithMessage("The minimum must not be negative.").AsPrefix();
	}

	[Test]
	public async Task MoreThan_WhenMinimumIsTheLargestValue_ShouldThrowArgumentOutOfRangeException()
	{
		Quantifier sut = new();

		void Act() => sut.MoreThan(int.MaxValue);

		await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
			.WithParamName("minimum").And
			.WithMessage("The minimum must be less than 2147483647.").AsPrefix()
			.Because("no count is more than int.MaxValue, so the expectation could never succeed and its negation never fail");
	}

	[Test]
	[Arguments(2, "fewer than twice")]
	[Arguments(3, "fewer than 3 times")]
	public async Task ToString_LessThan_ShouldSayFewerThan(int maximum, string expected)
	{
		Quantifier sut = Configure(q => q.LessThan(maximum));

		await That(sut.ToString()).IsEqualTo(expected);
	}

	[Test]
	[Arguments("AtLeast", 1, "never")]
	[Arguments("AtLeast", 2, "fewer than twice")]
	[Arguments("AtLeast", 3, "fewer than 3 times")]
	[Arguments("AtMost", 0, "at least once")]
	[Arguments("AtMost", 1, "more than once")]
	[Arguments("AtMost", 3, "more than 3 times")]
	[Arguments("Between", 3, "not between 3 and 5 times")]
	[Arguments("Exactly", 0, "at least once")]
	[Arguments("Exactly", 1, "not exactly once")]
	[Arguments("Exactly", 2, "not exactly twice")]
	[Arguments("Exactly", 3, "not exactly 3 times")]
	[Arguments("LessThan", 1, "at least once")]
	[Arguments("LessThan", 4, "at least 4 times")]
	[Arguments("MoreThan", 0, "never")]
	[Arguments("MoreThan", 1, "at most once")]
	[Arguments("MoreThan", 2, "at most twice")]
	[Arguments("MoreThan", 3, "at most 3 times")]
	public async Task ToString_WhenNegated_ShouldDescribeTheComplement(string method, int value, string expected)
	{
		Quantifier sut = Configure(method, value);

		string result = sut.ToString(true);

		await That(result).IsEqualTo(expected);
	}

	[Test]
	public async Task ToString_WhenNegatedDefault_ShouldBeNever()
	{
		Quantifier sut = new();

		string result = sut.ToString(true);

		await That(result).IsEqualTo("never");
	}

	[Test]
	[Arguments("AtMost", 0)]
	[Arguments("Exactly", 0)]
	[Arguments("LessThan", 1)]
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
