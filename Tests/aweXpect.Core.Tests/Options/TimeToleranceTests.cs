using aweXpect.Chronology;
using aweXpect.Customization;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class TimeToleranceTests
{
	[Test]
	public async Task Days_GetToleranceOrDefault_WhenToleranceIsNotSet_ShouldReturnTheWholeDaysOfTheDefault()
	{
		TimeTolerance sut = TimeTolerance.Days;
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(36.Hours());

		TimeSpan result = sut.GetToleranceOrDefault();

		await That(result).IsEqualTo(1.Days())
			.Because("only the whole days of the default tolerance apply to a date");
	}

	[Test]
	public async Task Days_GetToleranceOrDefault_WhenToleranceIsSet_ShouldReturnTheTolerance()
	{
		TimeTolerance sut = TimeTolerance.Days.Within(2.Days());
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(36.Hours());

		TimeSpan result = sut.GetToleranceOrDefault();

		await That(result).IsEqualTo(2.Days());
	}

	[Test]
	public async Task Days_ToString_WhenDefaultToleranceHasWholeDays_ShouldShowThem()
	{
		TimeTolerance sut = TimeTolerance.Days;
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(50.Hours());

		string result = sut.ToString();

		await That(result).IsEqualTo(" ± 2 days");
	}

	[Test]
	public async Task Days_ToString_WhenDefaultToleranceIsBelowOneDay_ShouldBeEmpty()
	{
		TimeTolerance sut = TimeTolerance.Days;
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(15.Milliseconds());

		string result = sut.ToString();

		await That(result).IsEmpty()
			.Because("only the whole days of the default tolerance apply to a date");
	}

	[Test]
	public async Task Days_ToString_WhenToleranceIsNotSet_ShouldBeEmpty()
	{
		TimeTolerance sut = TimeTolerance.Days;
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(TimeSpan.Zero);

		string result = sut.ToString();

		await That(result).IsEmpty();
	}

	[Test]
	public async Task Days_ToString_WhenToleranceIsOneDay_ShouldShowTheDay()
	{
		TimeTolerance sut = TimeTolerance.Days.Within(1.Days());

		string result = sut.ToString();

		await That(result).IsEqualTo(" ± 1 day");
	}

	[Test]
	public async Task Days_WhenToleranceIsBelowZeroAndNotWholeDays_ShouldReportTheNegativeTolerance()
	{
		TimeTolerance sut = TimeTolerance.Days;

		void Act() => sut.Within(-12.Hours());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix()
			.Because("a negative tolerance is rejected before the whole days are checked");
	}

	[Test]
	public async Task Days_WhenToleranceIsMaximal_ShouldKeepTheWholeDays()
	{
		TimeSpan maximum = TimeSpan.FromDays(TimeSpan.MaxValue.Days);

		TimeTolerance sut = TimeTolerance.Days.Within(maximum);

		await That(sut.Tolerance).IsEqualTo(maximum);
	}

	[Test]
	public async Task Days_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		TimeTolerance sut = TimeTolerance.Days;

		void Act() => sut.Within(-1.Days());

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
	public async Task Days_WhenToleranceIsNotWholeDays_ShouldThrowArgumentOutOfRangeException(int hours, int minutes)
	{
		TimeTolerance sut = TimeTolerance.Days;

		void Act() => sut.Within(hours.Hours() + minutes.Minutes());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
			.Because("a date has no time of day, so the remainder would otherwise be dropped silently");
	}

	[Test]
	public async Task Days_WhenToleranceIsSetTwice_ShouldUseTheLastTolerance()
	{
		TimeTolerance sut = TimeTolerance.Days.Within(1.Days());

		sut = sut.Within(2.Days());

		await That(sut.Tolerance).IsEqualTo(2.Days());
		await That(sut.ToString()).IsEqualTo(" ± 2 days");
	}

	[Test]
	public async Task Days_WhenToleranceIsSetTwice_WithNotWholeDays_ShouldThrowArgumentOutOfRangeException()
	{
		TimeTolerance sut = TimeTolerance.Days.Within(1.Days());

		void Act() => sut.Within(12.Hours());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The tolerance must be a whole number of days.").AsPrefix()
			.Because("a set tolerance keeps the day kind");
	}

	[Test]
	[Arguments(0)]
	[Arguments(1)]
	[Arguments(3)]
	public async Task Days_WhenToleranceIsWholeDays_ShouldSetIt(int days)
	{
		TimeTolerance sut = TimeTolerance.Days;
		TimeTolerance result = sut;

		void Act() => result = sut.Within(days.Days());

		await That(Act).DoesNotThrow();
		await That(result.Tolerance).IsEqualTo(days.Days());
	}

	[Test]
	public async Task Days_Within_ShouldNotChangeTheOriginalTolerance()
	{
		TimeTolerance sut = TimeTolerance.Days;

		TimeTolerance result = sut.Within(1.Days());

		await That(result.Tolerance).IsEqualTo(1.Days());
		await That(sut.Tolerance).IsNull();
		await That(TimeTolerance.Days.Tolerance).IsNull();
	}

	[Test]
	public async Task Default_ShouldBeTheDefaultValue()
	{
		TimeTolerance sut = TimeTolerance.Default;

		await That(sut.Tolerance).IsNull();
		await That(sut.ToString()).IsEqualTo(default(TimeTolerance).ToString());
		await That(() => sut.Within(12.Hours())).DoesNotThrow()
			.Because("the default tolerance is not limited to whole days");
	}

	[Test]
	public async Task GetToleranceOrDefault_WhenToleranceIsNotSet_ShouldReturnTheDefault()
	{
		TimeTolerance sut = TimeTolerance.Default;
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(36.Hours());

		TimeSpan result = sut.GetToleranceOrDefault();

		await That(result).IsEqualTo(36.Hours());
	}

	[Test]
	public async Task GetToleranceOrDefault_WhenToleranceIsSet_ShouldReturnTheTolerance()
	{
		TimeTolerance sut = TimeTolerance.Default.Within(3.Seconds());
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(36.Hours());

		TimeSpan result = sut.GetToleranceOrDefault();

		await That(result).IsEqualTo(3.Seconds());
	}

	[Test]
	[Arguments(0)]
	[Arguments(2)]
	public async Task ToDayString_WhenToleranceIsNotOneDay_ShouldUsePlural(int days)
	{
		TimeTolerance sut = TimeTolerance.Default.Within(days.Days());

		string result = sut.ToDayString();

		await That(result).IsEqualTo($" ± {days} days");
	}

	[Test]
	public async Task ToDayString_WhenToleranceIsNotSet_ShouldBeEmpty()
	{
		TimeTolerance sut = TimeTolerance.Default;

		string result = sut.ToDayString();

		await That(result).IsEmpty()
			.Because("without a customized default no tolerance applies");
	}

	[Test]
	public async Task ToDayString_WhenToleranceIsNotSet_ShouldIgnoreADefaultBelowOneDay()
	{
		TimeTolerance sut = TimeTolerance.Default;
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(12.Hours());

		string result = sut.ToDayString();

		await That(result).IsEmpty()
			.Because("a default below one day is truncated to zero days and must not read as ± 0 days");
	}

	[Test]
	public async Task ToDayString_WhenToleranceIsNotSet_ShouldUseTheWholeDaysOfTheDefault()
	{
		TimeTolerance sut = TimeTolerance.Default;
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(36.Hours());

		string result = sut.ToDayString();

		await That(result).IsEqualTo(" ± 1 day")
			.Because("only the whole days of the default tolerance apply to a date");
	}

	[Test]
	public async Task ToDayString_WhenToleranceIsOneDay_ShouldUseSingular()
	{
		TimeTolerance sut = TimeTolerance.Default.Within(1.Days());

		string result = sut.ToDayString();

		await That(result).IsEqualTo(" ± 1 day");
	}

	[Test]
	public async Task ToString_WhenToleranceIsNotSet_ShouldBeEmpty()
	{
		TimeTolerance sut = TimeTolerance.Default;

		string result = sut.ToString();

		await That(result).IsEmpty()
			.Because("without a customized default no tolerance applies");
	}

	[Test]
	public async Task ToString_WhenToleranceIsNotSet_ShouldUseTheDefault()
	{
		TimeTolerance sut = TimeTolerance.Default;
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(15.Milliseconds());

		string result = sut.ToString();

		await That(result).IsEqualTo(" ± 0:00.015")
			.Because("the applied default tolerance is part of the expectation");
	}

	[Test]
	public async Task ToString_WhenToleranceIsSet_ShouldIgnoreTheDefault()
	{
		TimeTolerance sut = TimeTolerance.Default.Within(TimeSpan.Zero);
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(15.Milliseconds());

		string result = sut.ToString();

		await That(result).IsEqualTo(" ± 0:00")
			.Because("an explicit tolerance replaces the default tolerance and is always named");
	}

	[Test]
	public async Task WhenToleranceIsMaximal_ShouldKeepIt()
	{
		TimeTolerance sut = TimeTolerance.Default.Within(TimeSpan.MaxValue);

		await That(sut.Tolerance).IsEqualTo(TimeSpan.MaxValue);
	}

	[Test]
	public async Task WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		TimeTolerance sut = TimeTolerance.Default;

		void Act() => sut.Within(-1.Seconds());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("*The tolerance must not be negative.*").AsWildcard();
	}

	[Test]
	public async Task WhenToleranceIsSetTwice_ShouldUseTheLastTolerance()
	{
		TimeTolerance sut = TimeTolerance.Default.Within(1.Seconds());

		sut = sut.Within(2.Seconds());

		await That(sut.Tolerance).IsEqualTo(2.Seconds());
	}

	[Test]
	public async Task WhenToleranceIsSetWithACustomizedDefault_ShouldNotThrow()
	{
		TimeTolerance sut = TimeTolerance.Default;
		TimeTolerance result = sut;
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(15.Milliseconds());

		void Act() => result = sut.Within(1.Seconds());

		await That(Act).DoesNotThrow()
			.Because("a customized default is not an explicit tolerance");
		await That(result.Tolerance).IsEqualTo(1.Seconds());
	}

	[Test]
	public async Task WhenToleranceIsZero_ShouldNotThrow()
	{
		TimeTolerance sut = TimeTolerance.Default;
		TimeTolerance result = sut;

		void Act() => result = sut.Within(TimeSpan.Zero);

		await That(Act).DoesNotThrow();
		await That(result.Tolerance).IsEqualTo(TimeSpan.Zero);
	}

	[Test]
	public async Task Within_ShouldNotChangeTheOriginalTolerance()
	{
		TimeTolerance sut = TimeTolerance.Default;

		TimeTolerance result = sut.Within(1.Seconds());

		await That(result.Tolerance).IsEqualTo(1.Seconds());
		await That(sut.Tolerance).IsNull();
	}
}
