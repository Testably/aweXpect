using aweXpect.Chronology;
using aweXpect.Customization;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public class TimeToleranceTests
{
	[Test]
	public async Task GetToleranceOrDefault_WhenToleranceIsNotSet_ShouldReturnTheDefault()
	{
		TimeTolerance sut = new();
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(36.Hours());

		TimeSpan result = sut.GetToleranceOrDefault();

		await That(result).IsEqualTo(36.Hours());
	}

	[Test]
	public async Task GetToleranceOrDefault_WhenToleranceIsSet_ShouldReturnTheTolerance()
	{
		TimeTolerance sut = new();
		sut.SetTolerance(3.Seconds());
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(36.Hours());

		TimeSpan result = sut.GetToleranceOrDefault();

		await That(result).IsEqualTo(3.Seconds());
	}

	[Test]
	[Arguments(0)]
	[Arguments(2)]
	public async Task ToDayString_WhenToleranceIsNotOneDay_ShouldUsePlural(int days)
	{
		TimeTolerance sut = new();
		sut.SetTolerance(days.Days());

		string result = sut.ToDayString();

		await That(result).IsEqualTo($" ± {days} days");
	}

	[Test]
	public async Task ToDayString_WhenToleranceIsNotSet_ShouldBeEmpty()
	{
		TimeTolerance sut = new();

		string result = sut.ToDayString();

		await That(result).IsEmpty()
			.Because("without a customized default no tolerance applies");
	}

	[Test]
	public async Task ToDayString_WhenToleranceIsNotSet_ShouldIgnoreADefaultBelowOneDay()
	{
		TimeTolerance sut = new();
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(12.Hours());

		string result = sut.ToDayString();

		await That(result).IsEmpty()
			.Because("a default below one day is truncated to zero days and must not read as ± 0 days");
	}

	[Test]
	public async Task ToDayString_WhenToleranceIsNotSet_ShouldUseTheWholeDaysOfTheDefault()
	{
		TimeTolerance sut = new();
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(36.Hours());

		string result = sut.ToDayString();

		await That(result).IsEqualTo(" ± 1 day")
			.Because("only the whole days of the default tolerance apply to a date");
	}

	[Test]
	public async Task ToDayString_WhenToleranceIsOneDay_ShouldUseSingular()
	{
		TimeTolerance sut = new();
		sut.SetTolerance(1.Days());

		string result = sut.ToDayString();

		await That(result).IsEqualTo(" ± 1 day");
	}

	[Test]
	public async Task ToString_WhenToleranceIsNotSet_ShouldBeEmpty()
	{
		TimeTolerance sut = new();

		string result = sut.ToString();

		await That(result).IsEmpty()
			.Because("without a customized default no tolerance applies");
	}

	[Test]
	public async Task ToString_WhenToleranceIsNotSet_ShouldUseTheDefault()
	{
		TimeTolerance sut = new();
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(15.Milliseconds());

		string result = sut.ToString();

		await That(result).IsEqualTo(" ± 0:00.015")
			.Because("the applied default tolerance is part of the expectation");
	}

	[Test]
	public async Task ToString_WhenToleranceIsSet_ShouldIgnoreTheDefault()
	{
		TimeTolerance sut = new();
		sut.SetTolerance(TimeSpan.Zero);
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(15.Milliseconds());

		string result = sut.ToString();

		await That(result).IsEqualTo(" ± 0:00")
			.Because("an explicit tolerance replaces the default tolerance and is always named");
	}

	[Test]
	public async Task WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
	{
		TimeTolerance sut = new();

		void Act() => sut.SetTolerance(-1.Seconds());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("*The tolerance must not be negative.*").AsWildcard();
	}

	[Test]
	public async Task WhenToleranceIsSetTwice_ShouldThrowInvalidOperationException()
	{
		TimeTolerance sut = new();
		sut.SetTolerance(1.Seconds());

		void Act() => sut.SetTolerance(2.Seconds());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Within cannot be specified more than once.")
			.Because("the second tolerance would silently replace the first one");
	}

	[Test]
	public async Task WhenToleranceIsSetWithACustomizedDefault_ShouldNotThrow()
	{
		TimeTolerance sut = new();
		using IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(15.Milliseconds());

		void Act() => sut.SetTolerance(1.Seconds());

		await That(Act).DoesNotThrow()
			.Because("a customized default is not an explicit tolerance");
		await That(sut.Tolerance).IsEqualTo(1.Seconds());
	}

	[Test]
	public async Task WhenToleranceIsZero_ShouldNotThrow()
	{
		TimeTolerance sut = new();

		void Act() => sut.SetTolerance(TimeSpan.Zero);

		await That(Act).DoesNotThrow();
		await That(sut.Tolerance).IsEqualTo(TimeSpan.Zero);
	}
}
