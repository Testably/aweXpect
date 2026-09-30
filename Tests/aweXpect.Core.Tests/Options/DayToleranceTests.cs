using aweXpect.Chronology;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class DayToleranceTests
{
	[Fact]
	public async Task WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		DayTolerance sut = new();

		void Act() => sut.SetTolerance(-1.Days());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix()
			.Because("a negative tolerance is rejected before the whole days are checked");
	}

	[Theory]
	[InlineData(12, 0)]
	[InlineData(36, 0)]
	[InlineData(0, 1)]
	[InlineData(24, 1)]
	public async Task WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException(int hours, int minutes)
	{
		DayTolerance sut = new();

		void Act() => sut.SetTolerance(hours.Hours() + minutes.Minutes());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
			.Because("a date has no time of day, so the remainder would otherwise be dropped silently");
	}

	[Fact]
	public async Task WhenToleranceIsSetTwice_ShouldThrowInvalidOperationException()
	{
		DayTolerance sut = new();
		sut.SetTolerance(1.Days());

		void Act() => sut.SetTolerance(2.Days());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Within cannot be specified more than once.")
			.Because("the second tolerance would silently replace the first one");
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(3)]
	public async Task WhenToleranceIsWholeDays_ShouldSetIt(int days)
	{
		DayTolerance sut = new();

		void Act() => sut.SetTolerance(days.Days());

		await That(Act).DoesNotThrow();
		await That(sut.Tolerance).IsEqualTo(days.Days());
	}
}
