using System.Text;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Formatting;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Core.EvaluationContext;

public sealed class ExternalWaitTests
{
	[Test]
	public async Task WhenTheTimeoutElapsesOnTheTimeProvider_ShouldFail()
	{
		VirtualTimeProvider timeProvider = new();
		Radio radio = new();

		async Task Act()
			=> await That(radio).PlaysWithin(30.Seconds()).WithTimeProvider(timeProvider);

		Task expectation = Act();
		await radio.IsWaiting;
		timeProvider.Advance(29.Seconds());
		bool isCompletedBeforeTheTimeout = await IsCompletedSoon(expectation);
		timeProvider.Advance(1.Seconds());

		await That(isCompletedBeforeTheTimeout).IsFalse()
			.Because("the timeout of the extension elapses on the clock of the evaluation");
		await That(expectation).Throws<FailException>()
			.WithMessage("""
			             Expected that radio
			             plays within 0:30,
			             but it did not play within 0:30
			             """)
			.Because("the time measured on the same clock reached the timeout, so the wait decides");
	}

	[Test]
	public async Task WithoutTimeProvider_ShouldLetTheTimeoutElapseInRealTime()
	{
		Radio radio = new();

		async Task Act()
			=> await That(radio).PlaysWithin(50.Milliseconds());

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that radio
			             plays within 0:00.050,
			             but it did not play within 0:00.050
			             """)
			.Because("an expectation without a time provider runs on the system clock");
	}

	private static async Task<bool> IsCompletedSoon(Task task)
		=> await Task.WhenAny(task, Task.Delay(100.Milliseconds())) == task;
}

file static class RadioExpectations
{
	public static AndOrResult<Radio, IThat<Radio?>> PlaysWithin(this IThat<Radio?> subject, TimeSpan timeout)
		=> new(subject.Get().ExpectationBuilder
				.AddConstraint((it, grammars) => new PlaysWithinConstraint(it, grammars, timeout)),
			subject);

	private sealed class PlaysWithinConstraint(string it, ExpectationGrammars grammars, TimeSpan timeout)
		: ConstraintResult.WithNotNullValue<Radio>(it, grammars),
			IAsyncContextConstraint<Radio?>
	{
		public async ValueTask<ConstraintResult> IsMetBy(Radio? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			using CancellationTokenSource waitCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			context.CancelAfter(waitCts, timeout);
			long startTimestamp = context.GetTimestamp();
			try
			{
				await actual.WaitForTrack(waitCts.Token);
				Outcome = Outcome.Success;
			}
			catch (OperationCanceledException)
			{
				Outcome = context.Cancellation.HasWaitElapsed(timeout, context.GetElapsedTime(startTimestamp))
					? Outcome.Failure
					: Outcome.Undecided;
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("plays within ").Append(Format.Formatter.Format(timeout));

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did not play within ").Append(Format.Formatter.Format(timeout));

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("does not play within ").Append(Format.Formatter.Format(timeout));

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" did play");
	}
}

file sealed class Radio
{
	private readonly TaskCompletionSource<bool> _isWaiting = new(TaskCreationOptions.RunContinuationsAsynchronously);

	public Task IsWaiting => _isWaiting.Task;

	public Task WaitForTrack(CancellationToken cancellationToken)
	{
		_isWaiting.TrySetResult(true);
		return Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
	}
}
