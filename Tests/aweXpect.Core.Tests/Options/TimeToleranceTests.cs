using aweXpect.Chronology;
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
			.Because("the default tolerance is not part of the expectation text, so a customized default below one day never reads as ± 0 days");
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
