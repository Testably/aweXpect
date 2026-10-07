using System.Collections.Generic;
using System.Text;
using aweXpect.Core.Constraints;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class CountBoundsTests
{
	public static IEnumerable<(string, int)> Bounds
		=>
		[
			("AtLeast", 0),
			("AtLeast", 2),
			("AtMost", 0),
			("AtMost", 2),
			("Between", 1),
			("Exactly", 0),
			("Exactly", 2),
			("LessThan", 1),
			("LessThan", 3),
			("MoreThan", 0),
			("MoreThan", 2),
		];

	[Test]
	[Arguments(1, "exactly one")]
	[Arguments(2, "exactly 2")]
	public async Task AppendItems_Exactly_ShouldUseNumeralsExceptForOne(int count, string expected)
	{
		CountBounds sut = CountBounds.Exactly(count);
		StringBuilder sb = new();

		sut.AppendItems(sb);

		await That(sb.ToString()).IsEqualTo(expected);
	}

	[Test]
	[Arguments("AtLeast", 1, "at least one")]
	[Arguments("AtMost", 2, "at most 2")]
	[Arguments("Between", 1, "between 1 and 3")]
	[Arguments("LessThan", 2, "fewer than 2")]
	[Arguments("MoreThan", 1, "more than one")]
	public async Task AppendItems_ShouldDescribeTheBounds(string method, int value, string expected)
	{
		CountBounds sut = Create(method, value);
		StringBuilder sb = new();

		sut.AppendItems(sb);

		await That(sb.ToString()).IsEqualTo(expected);
	}

	[Test]
	[Arguments(0, "never")]
	[Arguments(1, "exactly once")]
	[Arguments(2, "exactly twice")]
	[Arguments(3, "exactly 3 times")]
	public async Task AppendTimes_Exactly_ShouldUseWordsForOnceAndTwice(int count, string expected)
	{
		CountBounds sut = CountBounds.Exactly(count);
		StringBuilder sb = new();

		sut.AppendTimes(sb, false);

		await That(sb.ToString()).IsEqualTo(expected);
	}

	[Test]
	[Arguments("AtLeast", 1, "at least once")]
	[Arguments("AtMost", 2, "at most twice")]
	[Arguments("Between", 1, "between 1 and 3 times")]
	[Arguments("LessThan", 2, "fewer than twice")]
	[Arguments("MoreThan", 3, "more than 3 times")]
	public async Task AppendTimes_ShouldDescribeTheBounds(string method, int value, string expected)
	{
		CountBounds sut = Create(method, value);
		StringBuilder sb = new();

		sut.AppendTimes(sb, false);

		await That(sb.ToString()).IsEqualTo(expected);
	}

	[Test]
	[Arguments("AtLeast", 2, "fewer than twice")]
	[Arguments("Between", 1, "not between 1 and 3 times")]
	[Arguments("Exactly", 0, "at least once")]
	public async Task AppendTimes_WhenNegated_ShouldDescribeTheComplement(string method, int value, string expected)
	{
		CountBounds sut = Create(method, value);
		StringBuilder sb = new();

		sut.AppendTimes(sb, true);

		await That(sb.ToString()).IsEqualTo(expected);
	}

	[Test]
	[MethodDataSource(nameof(Bounds))]
	public async Task Complement_ShouldBeMetExactlyWhenTheBoundsAreNot(string method, int value)
	{
		CountBounds sut = Create(method, value);

		CountBounds? complement = sut.Complement();

		await That(complement is null).IsEqualTo(method is "Between" || (method is "Exactly" && value > 0))
			.Because("only a range with two bounds has no single complement, except for exactly zero");
		for (int amount = 0; amount <= 5 && complement is { } c; amount++)
		{
			await That(c.Check(amount, true) == true).IsNotEqualTo(sut.Check(amount, true) == true)
				.Because($"the complement of {method}({value}) must decide {amount} the other way");
		}
	}

	[Test]
	[MethodDataSource(nameof(Bounds))]
	public async Task EnumerableQuantifier_ShouldDecideLikeQuantifier(string method, int value)
	{
		Quantifier quantifier = new();
		EnumerableQuantifier sut = method switch
		{
			"AtLeast" => EnumerableQuantifier.AtLeast(value),
			"AtMost" => EnumerableQuantifier.AtMost(value),
			"Between" => EnumerableQuantifier.Between(value, value + 2),
			"Exactly" => EnumerableQuantifier.Exactly(value),
			"LessThan" => EnumerableQuantifier.LessThan(value),
			_ => EnumerableQuantifier.MoreThan(value),
		};
		Action configure = method switch
		{
			"AtLeast" => () => quantifier.AtLeast(value),
			"AtMost" => () => quantifier.AtMost(value),
			"Between" => () => quantifier.Between(value, value + 2),
			"Exactly" => () => quantifier.Exactly(value),
			"LessThan" => () => quantifier.LessThan(value),
			_ => () => quantifier.MoreThan(value),
		};
		configure();

		for (int amount = 0; amount <= 5; amount++)
		{
			Outcome outcome = sut.GetOutcome(amount, 0, amount);

			await That(outcome == Outcome.Success).IsEqualTo(quantifier.Check(amount, true) == true)
				.Because($"both quantifiers for {method}({value}) must decide {amount} the same way");
		}
	}

	[Test]
	[Arguments("LessThan", 0, "maximum", "The maximum must be greater than zero.")]
	[Arguments("MoreThan", int.MaxValue, "minimum", "The minimum must be less than 2147483647.")]
	public async Task EnumerableQuantifier_WhenNoCountCanMeetTheBounds_ShouldThrowArgumentOutOfRangeException(
		string method, int value, string paramName, string expectedMessage)
	{
		void Act() => _ = method == "LessThan"
			? EnumerableQuantifier.LessThan(value)
			: EnumerableQuantifier.MoreThan(value);

		await That(Act).ThrowsExactly<ArgumentOutOfRangeException>()
			.WithParamName(paramName).And
			.WithMessage(expectedMessage).AsPrefix()
			.Because("a collection quantifier must reject an empty range like the occurrence quantifier does");
	}

	[Test]
	[Arguments("AtLeast", 1, true)]
	[Arguments("AtLeast", 2, false)]
	[Arguments("AtMost", 1, true)]
	[Arguments("Between", 1, false)]
	[Arguments("Exactly", 1, true)]
	[Arguments("Exactly", 2, false)]
	[Arguments("LessThan", 2, false)]
	public async Task IsSingleItem_ShouldOnlyBeTrueWhenTheNumberIsOne(string method, int value, bool expected)
	{
		CountBounds sut = Create(method, value);

		bool result = sut.IsSingleItem();

		await That(result).IsEqualTo(expected);
	}

	private static CountBounds Create(string method, int value)
		=> method switch
		{
			"AtLeast" => CountBounds.AtLeast(value),
			"AtMost" => CountBounds.AtMost(value),
			"Between" => CountBounds.Between(value, value + 2),
			"Exactly" => CountBounds.Exactly(value),
			"LessThan" => CountBounds.LessThan(value),
			_ => CountBounds.MoreThan(value),
		};
}
