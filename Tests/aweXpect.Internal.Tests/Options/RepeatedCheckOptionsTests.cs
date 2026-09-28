using System.Threading;
using aweXpect.Core;
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
		await That(sut.Interval).IsEqualTo(20.Milliseconds());
	}

	[Fact]
	public async Task CheckRepeatedly_WhenIntervalIsNotPositive_ShouldYieldBetweenChecks()
	{
		RepeatedCheckOptions sut = new();
		sut.Within(Timeout.InfiniteTimeSpan);
		using CancellationTokenSource cts = new();
		cts.CancelAfter(5.Seconds());
		Task<bool> result;
		using (Customize.aweXpect.Settings().Update(s => s with
		       {
			       DefaultCheckInterval = TimeSpan.Zero,
		       }))
		{
			result = sut.CheckRepeatedly(() => Task.FromResult(false), new ManualExpectationBuilder<int>(null),
				cts.Token);
		}

		bool isCompletedSynchronously = result.IsCompleted;
		cts.Cancel();

		await That(isCompletedSynchronously).IsFalse()
			.Because("checking without waiting must still hand the thread back between the checks");
		await That(async () => await result).Throws<OperationCanceledException>()
			.Whose(e => e.CancellationToken, token => token.IsEqualTo(cts.Token));
	}

	[Fact]
	public async Task Interval_ShouldBeReadOnlyOnceFromCustomization()
	{
		RepeatedCheckOptions sut = new();
		TimeSpan interval1, interval2;
		using (Customize.aweXpect.Settings().DefaultCheckInterval.Set(103.Milliseconds()))
		{
			interval1 = sut.Interval;
		}

		using (Customize.aweXpect.Settings().DefaultCheckInterval.Set(107.Milliseconds()))
		{
			interval2 = sut.Interval;
		}

		await That(interval1).IsEqualTo(103.Milliseconds());
		await That(interval2).IsEqualTo(interval1);
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
