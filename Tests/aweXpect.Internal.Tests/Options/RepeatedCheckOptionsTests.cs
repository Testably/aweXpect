using aweXpect.Customization;
using aweXpect.Options;
using FluentAssertions.Extensions;

namespace aweXpect.Internal.Tests.Options;

public class RepeatedCheckOptionsTests
{
	[Fact]
	public async Task CheckEvery_WhenIntervalIsSpecified_ShouldThrowInvalidOperationException()
	{
		RepeatedCheckOptions sut = new();
		sut.CheckEvery(10.Milliseconds());

		void Act() => sut.CheckEvery(20.Milliseconds());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("CheckEvery cannot be specified more than once.")
			.Because("the second interval would silently replace the first one");
	}

	[Fact]
	public async Task CheckEvery_WhenTheDefaultIntervalWasRead_ShouldNotThrow()
	{
		RepeatedCheckOptions sut = new();
		_ = sut.Interval;

		void Act() => sut.CheckEvery(20.Milliseconds());

		await That(Act).DoesNotThrow()
			.Because("the customized default interval is not an explicit interval");
		await That(sut.Interval.NextCheckInterval()).IsEqualTo(20.Milliseconds());
	}

	[Fact]
	public async Task DefaultInterval_ShouldBe100Milliseconds()
	{
		TimeSpan result = RepeatedCheckOptions.DefaultInterval;

		await That(result).IsEqualTo(100.Milliseconds());
	}

	[Fact]
	public async Task Interval_ShouldBeReadOnlyOnceFromCustomization()
	{
		RepeatedCheckOptions sut = new();
		ICheckInterval interval1, interval2;
		using (Customize.aweXpect.Settings().DefaultCheckInterval.Set(103.Milliseconds()))
		{
			interval1 = sut.Interval;
		}

		using (Customize.aweXpect.Settings().DefaultCheckInterval.Set(107.Milliseconds()))
		{
			interval2 = sut.Interval;
		}

		await That(interval1.NextCheckInterval()).IsEqualTo(103.Milliseconds());
		await That(interval1.NextCheckInterval()).IsEqualTo(interval2.NextCheckInterval());
	}

	[Fact]
	public async Task ToString_WhenTimeoutIsNotSpecified_ShouldBeEmpty()
	{
		RepeatedCheckOptions sut = new();

		string result = sut.ToString();

		await That(result).IsEmpty();
	}

	[Fact]
	public async Task ToString_WhenTimeoutIsZero_ShouldIncludeTheTimeout()
	{
		RepeatedCheckOptions sut = new();
		sut.Within(TimeSpan.Zero);

		string result = sut.ToString();

		await That(result).IsEqualTo(" within 0:00")
			.Because("an explicit timeout is named like on a signaler, even when it is zero");
	}

	[Fact]
	public async Task Within_WhenTimeoutIsSpecified_ShouldThrowInvalidOperationException()
	{
		RepeatedCheckOptions sut = new();
		sut.Within(1.Seconds());

		void Act() => sut.Within(2.Seconds());

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Within cannot be specified more than once.")
			.Because("the second timeout would silently replace the first one");
	}
}
