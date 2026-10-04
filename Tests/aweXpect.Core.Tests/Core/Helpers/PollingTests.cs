using System.Diagnostics;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public sealed class PollingTests
{
	[Fact]
	public async Task WaitForNextCheck_AfterTheLastCheck_ShouldBeElapsed()
	{
		using Polling sut = Polling.Start(Stopwatch.GetTimestamp(), 20.Milliseconds(), 1.Hours(),
			EvaluationCancellation.None);
		await sut.WaitForNextCheck();

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Elapsed)
			.Because("no check is made after the last one at the end of the budget");
	}

	[Fact]
	public async Task WaitForNextCheck_WhenTheBudgetIsUsedUp_ShouldBeElapsed()
	{
		using Polling sut = Polling.Start(Stopwatch.GetTimestamp(), TimeSpan.Zero, 1.Hours(),
			EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Elapsed);
	}

	[Fact]
	public async Task WaitForNextCheck_WhenTheBudgetIsUnlimited_ShouldCheckAgain()
	{
		using Polling sut = Polling.Start(Stopwatch.GetTimestamp(), Timeout.InfiniteTimeSpan, 1.Milliseconds(),
			EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Check);
		await That(sut.Remaining).IsEqualTo(TimeSpan.MaxValue);
	}

	[Fact]
	public async Task WaitForNextCheck_WhenTheBudgetIsUnlimitedAndTheCallerCancels_ShouldBeCanceled()
	{
		using CancellationTokenSource cts = new();
		EvaluationCancellation cancellation = new(null, cts.Token);
		using Polling sut = Polling.Start(Stopwatch.GetTimestamp(), Timeout.InfiniteTimeSpan, 1.Hours(),
			cancellation);
		cts.CancelAfter(20.Milliseconds());

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Canceled)
			.Because("an unlimited budget can never be used up by a cancellation");
	}

	[Fact]
	public async Task WaitForNextCheck_WhenTheCallerCancels_ShouldBeCanceled()
	{
		using CancellationTokenSource cts = new();
		EvaluationCancellation cancellation = new(null, cts.Token);
		using Polling sut = Polling.Start(Stopwatch.GetTimestamp(), 30.Seconds(), 1.Hours(), cancellation);
		cts.CancelAfter(20.Milliseconds());

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Canceled);
	}

	[Fact]
	public async Task WaitForNextCheck_WhenTheIntervalExceedsTheBudget_ShouldMakeTheLastCheckAtTheEndOfTheBudget()
	{
		using Polling sut = Polling.Start(Stopwatch.GetTimestamp(), 50.Milliseconds(), 1.Hours(),
			EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.LastCheck);
		await That(sut.Elapsed).IsGreaterThanOrEqualTo(40.Milliseconds())
			.Because("the wait is shortened to the remaining budget, and a timer can complete a few milliseconds before the stopwatch agrees");
	}

	[Fact]
	public async Task WaitForNextCheck_WhenTheIntervalIsNotPositive_ShouldCheckAgainWithoutWaiting()
	{
		using Polling sut = Polling.Start(Stopwatch.GetTimestamp(), 30.Seconds(), TimeSpan.Zero,
			EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Check);
		await That(sut.Elapsed).IsLessThan(1.Seconds());
	}

	[Fact]
	public async Task WaitForNextCheck_WhenTheIntervalIsShorterThanTheBudget_ShouldCheckAgain()
	{
		using Polling sut = Polling.Start(Stopwatch.GetTimestamp(), 30.Seconds(), 1.Milliseconds(),
			EvaluationCancellation.None);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Check);
	}

	[Fact]
	public async Task WaitForNextCheck_WhenTheTimeoutIsShorterThanTheBudget_ShouldBeCanceled()
	{
		EvaluationCancellation cancellation = new(20.Milliseconds(), CancellationToken.None);
		using Polling sut = Polling.Start(Stopwatch.GetTimestamp(), 30.Seconds(), 1.Hours(), cancellation);

		PollStep step = await sut.WaitForNextCheck();

		await That(step).IsEqualTo(PollStep.Canceled)
			.Because("a shorter timeout ends the checks, so that it is reported as the timeout");
		cancellation.Release();
	}
}
