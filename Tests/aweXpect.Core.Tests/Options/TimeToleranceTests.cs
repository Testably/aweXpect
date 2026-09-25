using aweXpect.Chronology;
using aweXpect.Customization;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class TimeToleranceTests
{
	[Theory]
	[InlineData(0)]
	[InlineData(2)]
	public async Task ToDayString_WhenToleranceIsNotOneDay_ShouldUsePlural(int days)
	{
		TimeTolerance sut = new();
		sut.SetTolerance(days.Days());

		string result = sut.ToDayString();

		await That(result).IsEqualTo($" ± {days} days");
	}

	[Fact]
	public async Task ToDayString_WhenToleranceIsNotSet_ShouldBeEmpty()
	{
		TimeTolerance sut = new();

		string result = sut.ToDayString();

		await That(result).IsEmpty()
			.Because("without a customized default no tolerance applies");
	}

	[Fact]
	public async Task ToDayString_WhenToleranceIsNotSet_ShouldIgnoreADefaultBelowOneDay()
	{
		TimeTolerance sut = new();
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(12.Hours());

		string result = sut.ToDayString();

		await That(result).IsEmpty()
			.Because("a default below one day is truncated to zero days and must not read as ± 0 days");
	}

	[Fact]
	public async Task ToDayString_WhenToleranceIsNotSet_ShouldUseTheWholeDaysOfTheDefault()
	{
		TimeTolerance sut = new();
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(36.Hours());

		string result = sut.ToDayString();

		await That(result).IsEqualTo(" ± 1 day")
			.Because("only the whole days of the default tolerance apply to a date");
	}

	[Fact]
	public async Task ToDayString_WhenToleranceIsOneDay_ShouldUseSingular()
	{
		TimeTolerance sut = new();
		sut.SetTolerance(1.Days());

		string result = sut.ToDayString();

		await That(result).IsEqualTo(" ± 1 day");
	}

	[Fact]
	public async Task ToString_WhenToleranceIsNotSet_ShouldBeEmpty()
	{
		TimeTolerance sut = new();

		string result = sut.ToString();

		await That(result).IsEmpty()
			.Because("without a customized default no tolerance applies");
	}

	[Fact]
	public async Task ToString_WhenToleranceIsNotSet_ShouldUseTheDefault()
	{
		TimeTolerance sut = new();
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(15.Milliseconds());

		string result = sut.ToString();

		await That(result).IsEqualTo(" ± 0:00.015")
			.Because("the applied default tolerance is part of the expectation");
	}

	[Fact]
	public async Task ToString_WhenToleranceIsSet_ShouldIgnoreTheDefault()
	{
		TimeTolerance sut = new();
		sut.SetTolerance(TimeSpan.Zero);
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(15.Milliseconds());

		string result = sut.ToString();

		await That(result).IsEqualTo(" ± 0:00")
			.Because("an explicit tolerance replaces the default tolerance and is always named");
	}

	[Fact]
	public async Task WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		TimeTolerance sut = new();

		void Act() => sut.SetTolerance(-1.Seconds());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("*Tolerance must be non-negative*").AsWildcard();
	}

	[Fact]
	public async Task WhenToleranceIsZero_ShouldNotThrow()
	{
		TimeTolerance sut = new();

		void Act() => sut.SetTolerance(TimeSpan.Zero);

		await That(Act).DoesNotThrow();
		await That(sut.Tolerance).IsEqualTo(TimeSpan.Zero);
	}
}
