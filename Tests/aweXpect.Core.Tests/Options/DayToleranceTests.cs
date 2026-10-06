using aweXpect.Chronology;
using aweXpect.Customization;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class DayToleranceTests
{
	[Test]
	public async Task GetToleranceOrDefault_WhenToleranceIsNotSet_ShouldReturnTheWholeDaysOfTheDefault()
	{
		DayTolerance sut = new();
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(36.Hours());

		TimeSpan result = sut.GetToleranceOrDefault();

		await That(result).IsEqualTo(1.Days())
			.Because("only the whole days of the default tolerance apply to a date");
	}

	[Test]
	public async Task GetToleranceOrDefault_WhenToleranceIsSet_ShouldReturnTheTolerance()
	{
		DayTolerance sut = new();
		sut.SetTolerance(2.Days());
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(36.Hours());

		TimeSpan result = sut.GetToleranceOrDefault();

		await That(result).IsEqualTo(2.Days());
	}

	[Test]
	public async Task ToString_WhenDefaultToleranceHasWholeDays_ShouldShowThem()
	{
		TimeTolerance sut = new DayTolerance();
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(50.Hours());

		string result = sut.ToString();

		await That(result).IsEqualTo(" ± 2 days");
	}

	[Test]
	public async Task ToString_WhenDefaultToleranceIsBelowOneDay_ShouldBeEmpty()
	{
		TimeTolerance sut = new DayTolerance();
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(15.Milliseconds());

		string result = sut.ToString();

		await That(result).IsEmpty()
			.Because("only the whole days of the default tolerance apply to a date");
	}

	[Test]
	public async Task ToString_WhenToleranceIsOneDay_ShouldShowTheDay()
	{
		TimeTolerance sut = new DayTolerance();
		sut.SetTolerance(1.Days());

		string result = sut.ToString();

		await That(result).IsEqualTo(" ± 1 day");
	}

	[Test]
	public async Task ToString_WhenToleranceIsNotSet_ShouldBeEmpty()
	{
		TimeTolerance sut = new DayTolerance();
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(TimeSpan.Zero);

		string result = sut.ToString();

		await That(result).IsEmpty();
	}

	[Test]
	public async Task WhenToleranceIsBelowZeroAndNotWholeDays_ShouldReportTheNegativeTolerance()
	{
		DayTolerance sut = new();

		void Act() => sut.SetTolerance(-12.Hours());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix()
			.Because("a negative tolerance is rejected before the whole days are checked");
	}

	[Test]
	public async Task WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		DayTolerance sut = new();

		void Act() => sut.SetTolerance(-1.Days());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix()
			.Because("a negative tolerance is rejected before the whole days are checked");
	}

	[Test]
	[Arguments(12, 0)]
	[Arguments(36, 0)]
	[Arguments(0, 1)]
	[Arguments(24, 1)]
	public async Task WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException(int hours, int minutes)
	{
		DayTolerance sut = new();

		void Act() => sut.SetTolerance(hours.Hours() + minutes.Minutes());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
			.Because("a date has no time of day, so the remainder would otherwise be dropped silently");
	}

	[Test]
	public async Task WhenToleranceIsNotWholeDays_ShouldNotStoreIt()
	{
		DayTolerance sut = new();
		await That(() => sut.SetTolerance(12.Hours())).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The tolerance must be a whole number of days.").AsPrefix();

		void Act() => sut.SetTolerance(1.Days());

		await That(Act).DoesNotThrow()
			.Because("the rejected tolerance must leave the options unchanged");
		await That(sut.Tolerance).IsEqualTo(1.Days());
	}

	[Test]
	public async Task WhenToleranceIsSetTwice_ShouldThrowInvalidOperationException()
	{
		DayTolerance sut = new();
		sut.SetTolerance(1.Days());

		void Act() => sut.SetTolerance(2.Days());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Within cannot be specified more than once.")
			.Because("the second tolerance would silently replace the first one");
	}

	[Test]
	public async Task WhenToleranceIsSetTwice_WithNotWholeDays_ShouldThrowInvalidOperationException()
	{
		DayTolerance sut = new();
		sut.SetTolerance(1.Days());

		void Act() => sut.SetTolerance(12.Hours());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Within cannot be specified more than once.")
			.Because("the repetition is the misuse, whatever the second tolerance is");
	}

	[Test]
	[Arguments(0)]
	[Arguments(1)]
	[Arguments(3)]
	public async Task WhenToleranceIsWholeDays_ShouldSetIt(int days)
	{
		DayTolerance sut = new();

		void Act() => sut.SetTolerance(days.Days());

		await That(Act).DoesNotThrow();
		await That(sut.Tolerance).IsEqualTo(days.Days());
	}
}
