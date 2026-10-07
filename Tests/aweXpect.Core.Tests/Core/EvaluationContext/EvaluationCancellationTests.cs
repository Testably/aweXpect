using System.Text;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Customization;
using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect.Core.Tests.Core.EvaluationContext;

public class EvaluationCancellationTests
{
	[Test]
	public async Task Cancellation_ShouldHaveTheTokenThatTheConstraintReceives()
	{
		CancellationCapturingConstraint constraint = new();

		await Evaluate(constraint).WithTimeout(10.Seconds());

		await That(constraint.HasSameToken).IsTrue()
			.Because("the token of the cancellation and the token parameter describe the same evaluation");
	}

	[Test]
	public async Task Cancellation_WhenTestCancellationTimeoutIsShorter_ShouldUseItAsTimeout()
	{
		CancellationCapturingConstraint constraint = new();
		using (IDisposable _ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromTimeout(300.Milliseconds())))
		{
			await Evaluate(constraint).WithTimeout(10.Seconds());
		}

		await That(constraint.Timeout).IsEqualTo(300.Milliseconds())
			.Because("the effective timeout is the tighter of WithTimeout and the TestCancellation timeout");
	}

	[Test]
	public async Task Cancellation_WhenWithTimeoutIsShorter_ShouldUseItAsTimeout()
	{
		CancellationCapturingConstraint constraint = new();
		using (IDisposable _ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromTimeout(10.Seconds())))
		{
			await Evaluate(constraint).WithTimeout(300.Milliseconds());
		}

		await That(constraint.Timeout).IsEqualTo(300.Milliseconds())
			.Because("the effective timeout is the tighter of WithTimeout and the TestCancellation timeout");
	}

	[Test]
	public async Task Cancellation_WithoutTimeout_ShouldHaveNoTimeout()
	{
		CancellationCapturingConstraint constraint = new();

		await Evaluate(constraint);

		await That(constraint.Timeout).IsNull();
	}

	[Test]
	public async Task ForRemainingTimeout_ShouldMeasureAndElapseOnTheTimeSystem()
	{
		VirtualTimeSystem timeSystem = new();
		EvaluationCancellation sut = EvaluationCancellation.Create(10.Seconds(), CancellationToken.None, timeSystem);
		timeSystem.Advance(4.Seconds());
		sut.Release();

		EvaluationCancellation result = sut.ForRemainingTimeout();
		timeSystem.Advance(5.Seconds());
		bool isCanceledBeforeTheTimeout = result.Token.IsCancellationRequested;
		timeSystem.Advance(1.Seconds());

		await That(result.Timeout).IsEqualTo(6.Seconds())
			.Because("the timeout is measured from the start of the evaluation on its time system");
		await That(isCanceledBeforeTheTimeout).IsFalse();
		await That(result.Reason).IsEqualTo(CancellationReason.Timeout)
			.Because("the rest of the timeout elapses on the time system of the evaluation as well");
		result.Release();
	}

	[Test]
	public async Task ForRemainingTimeout_WithOuterTimeout_ShouldHaveTheRestOfTheOuterTimeout()
	{
		EvaluationCancellation sut = new(null, CancellationToken.None, 30.Seconds());

		EvaluationCancellation result = sut.ForRemainingTimeout();
		result.Release();

		await That(sut.Timeout).IsNull()
			.Because("the outer timeout does not cancel the evaluation");
		await That(result.Timeout).IsNotNull().And.IsLessThanOrEqualTo(30.Seconds())
			.Because("the outer timeout still limits what is awaited after the evaluation");
		await That(result.Timeout).IsGreaterThan(TimeSpan.Zero);
	}

	[Test]
	public async Task ForRemainingTimeout_WithoutTimeout_ShouldBeTheSameInstance()
	{
		using CancellationTokenSource cts = new();
		EvaluationCancellation sut = EvaluationCancellation.Create(null, cts.Token);

		EvaluationCancellation result = sut.ForRemainingTimeout();

		await That(result).IsSameAs(sut)
			.Because("the token of the caller needs no timer that the evaluation could have released");
	}

	[Test]
	public async Task ForRemainingTimeout_WithTimeout_ShouldBeCanceledByTheCaller()
	{
		using CancellationTokenSource cts = new();
		EvaluationCancellation sut = EvaluationCancellation.Create(30.Seconds(), cts.Token);
		sut.Release();

		EvaluationCancellation result = sut.ForRemainingTimeout();
		bool isCanceledBefore = result.Token.IsCancellationRequested;
		cts.Cancel();
		bool isCanceledAfter = result.Token.IsCancellationRequested;
		result.Release();

		await That(isCanceledBefore).IsFalse();
		await That(isCanceledAfter).IsTrue();
		await That(result.Reason).IsEqualTo(CancellationReason.Caller);
	}

	[Test]
	public async Task ForRemainingTimeout_WithTimeout_ShouldHaveTheRestOfTheTimeout()
	{
		EvaluationCancellation sut = EvaluationCancellation.Create(30.Seconds(), CancellationToken.None);
		sut.Release();

		EvaluationCancellation result = sut.ForRemainingTimeout();
		result.Release();

		await That(result).IsNotSameAs(sut)
			.Because("the evaluation already released its timer");
		await That(result.Timeout).IsNotNull().And.IsLessThanOrEqualTo(30.Seconds())
			.Because("the timeout is measured from the start of the evaluation");
		await That(result.Timeout).IsGreaterThan(TimeSpan.Zero);
	}

	[Test]
	public async Task ForRemainingTimeout_WithTimeoutOfZero_ShouldHaveNoTimeLeft()
	{
		EvaluationCancellation sut = EvaluationCancellation.Create(TimeSpan.Zero, CancellationToken.None);
		sut.Release();

		EvaluationCancellation result = sut.ForRemainingTimeout();
		result.Release();

		await That(result.Timeout).IsEqualTo(TimeSpan.Zero);
	}

	[Test]
	public async Task HasWaitElapsed_WhenTheCallerCanceled_ShouldBeFalse()
	{
		using CancellationTokenSource cts = new();
		EvaluationCancellation sut = new(10.Seconds(), cts.Token);
		cts.Cancel();

		bool result = sut.HasWaitElapsed(1.Seconds(), 100.Milliseconds());

		await That(result).IsFalse()
			.Because("a cancellation by the caller before the end of the wait decides nothing");
		sut.Release();
	}

	[Test]
	public async Task HasWaitElapsed_WhenTheCancellationCameAtTheEndOfTheWait_ShouldBeTrue()
	{
		using CancellationTokenSource cts = new();
		EvaluationCancellation sut = new(null, cts.Token);
		cts.Cancel();

		bool result = sut.HasWaitElapsed(1.Seconds(), 999.Milliseconds());

		await That(result).IsTrue()
			.Because("the timers of the wait and of the cancellation do not share the clock of the stopwatch");
	}

	[Test]
	public async Task HasWaitElapsed_WhenTheTimeoutElapsedAndIsNotShorterThanTheWait_ShouldBeTrue()
	{
		EvaluationCancellation sut = TimedOutAfter(10.Milliseconds());

		bool result = sut.HasWaitElapsed(10.Milliseconds(), 1.Milliseconds());

		await That(result).IsTrue()
			.Because("the timer of the timeout started before the wait, so it can expire slightly earlier");
		sut.Release();
	}

	[Test]
	public async Task HasWaitElapsed_WhenTheTimeoutElapsedAndIsNotShorterThanTheWait_WhenTheWaitStartedLate_ShouldBeFalse()
	{
		EvaluationCancellation sut = TimedOutAfter(100.Milliseconds());

		bool result = sut.HasWaitElapsed(100.Milliseconds(), 20.Milliseconds());

		await That(result).IsFalse()
			.Because("the wait started well into the evaluation, so the timeout cut it short");
		sut.Release();
	}

	[Test]
	public async Task HasWaitElapsed_WhenTheTimeoutElapsedAndIsLongerThanTheWait_WhenTheWaitStartedLate_ShouldBeFalse()
	{
		EvaluationCancellation sut = TimedOutAfter(100.Milliseconds());

		bool result = sut.HasWaitElapsed(60.Milliseconds(), 20.Milliseconds());

		await That(result).IsFalse()
			.Because("a longer timeout only ends a wait that started so late that it could not complete");
		sut.Release();
	}

	[Test]
	[Arguments(1, true)]
	[Arguments(0, false)]
	public async Task HasWaitElapsed_WhenTheTimeoutElapsedAndIsNotShorterThanTheWait_ShouldAllowTheStartSlack(
		int millisecondsWithinTheSlack, bool expectedResult)
	{
		EvaluationCancellation sut = TimedOutAfter(100.Milliseconds());
		TimeSpan waited = 100.Milliseconds() - EvaluationCancellation.StartSlack +
		                  millisecondsWithinTheSlack.Milliseconds();

		bool result = sut.HasWaitElapsed(100.Milliseconds(), waited);

		await That(result).IsEqualTo(expectedResult);
		sut.Release();
	}

	[Test]
	public async Task HasWaitElapsed_WhenTheTimeoutElapsedAndIsShorterThanTheWait_ShouldBeFalse()
	{
		EvaluationCancellation sut = TimedOutAfter(10.Milliseconds());

		bool result = sut.HasWaitElapsed(1.Seconds(), 10.Milliseconds());

		await That(result).IsFalse()
			.Because("a shorter timeout is reported as the timeout, not as the result of the wait");
		sut.Release();
	}

	[Test]
	public async Task HasWaitElapsed_WhenTheWaitIsInfinite_ShouldBeFalse()
	{
		EvaluationCancellation sut = TimedOutAfter(10.Milliseconds());

		bool result = sut.HasWaitElapsed(Timeout.InfiniteTimeSpan, 10.Milliseconds());

		await That(result).IsFalse()
			.Because("an infinite wait is never used up");
		sut.Release();
	}

	[Test]
	public async Task None_ShouldNotBeCanceledAndHaveNoTimeout()
	{
		EvaluationCancellation sut = EvaluationCancellation.None;

		await That(sut.Token.CanBeCanceled).IsFalse();
		await That(sut.Timeout).IsNull();
		await That(sut.Reason).IsEqualTo(CancellationReason.None);
	}

	[Test]
	public async Task Reason_WhenTheCallerCanceled_ShouldBeCaller()
	{
		using CancellationTokenSource cts = new();
		EvaluationCancellation sut = new(10.Seconds(), cts.Token);

		cts.Cancel();

		await That(sut.Reason).IsEqualTo(CancellationReason.Caller);
		await That(sut.Token.IsCancellationRequested).IsTrue();
		sut.Release();
	}

	[Test]
	public async Task Reason_WhenTheTimeoutElapsed_ShouldBeTimeout()
	{
		EvaluationCancellation sut = TimedOutAfter(10.Milliseconds());

		await That(sut.Reason).IsEqualTo(CancellationReason.Timeout);
		sut.Release();
	}

	[Test]
	public async Task Reason_WhenTheTimeoutElapsedAndTheCallerCanceled_ShouldBeCaller()
	{
		using CancellationTokenSource cts = new();
		EvaluationCancellation sut = TimedOutAfter(10.Milliseconds(), cts.Token);

		cts.Cancel();

		await That(sut.Reason).IsEqualTo(CancellationReason.Caller)
			.Because("a cancellation by the caller is never reported as an elapsed timeout");
		sut.Release();
	}

	[Test]
	public async Task Reason_WhenTheTimeoutElapsedOnTheTimeSystem_ShouldBeTimeout()
	{
		VirtualTimeSystem timeSystem = new();
		EvaluationCancellation sut = EvaluationCancellation.Create(1.Hours(), CancellationToken.None, timeSystem);

		timeSystem.Advance(1.Hours() - 1.Milliseconds());
		CancellationReason reasonBeforeTheTimeout = sut.Reason;
		timeSystem.Advance(1.Milliseconds());

		await That(reasonBeforeTheTimeout).IsEqualTo(CancellationReason.None);
		await That(sut.Reason).IsEqualTo(CancellationReason.Timeout)
			.Because("the timeout elapses when the clock of the time system reaches it");
		await That(sut.Token.IsCancellationRequested).IsTrue();
		sut.Release();
	}

	[Test]
	public async Task Reason_WithoutCancellation_ShouldBeNone()
	{
		EvaluationCancellation sut = new(10.Seconds(), CancellationToken.None);

		await That(sut.Reason).IsEqualTo(CancellationReason.None);
		sut.Release();
	}

	[Test]
	public async Task Release_ShouldReleaseTheTimeoutOnTheTimeSystem()
	{
		VirtualTimeSystem timeSystem = new();
		EvaluationCancellation sut = EvaluationCancellation.Create(1.Hours(), CancellationToken.None, timeSystem);
		CancellationToken token = sut.Token;
		sut.Release();

		void Act()
			=> timeSystem.Advance(2.Hours());

		await That(Act).DoesNotThrow()
			.Because("a released timeout is not due anymore when the clock reaches it");
		await That(token.IsCancellationRequested).IsFalse();
	}

	[Test]
	public async Task Timeout_WhenItCutsShortTheWaitOfALaterConstraint_ShouldBeReported()
	{
		Signaler<int> signaler = new();
		signaler.Signal(1);
		VirtualTimeSystem timeSystem = new();
		int calls = 0;

		bool TakesAWhile(int _)
		{
			if (Interlocked.Increment(ref calls) == 1)
			{
				timeSystem.Advance(200.Milliseconds());
			}

			return true;
		}

		async Task Act()
			=> await That(signaler).Signaled().With(TakesAWhile)
				.And.DidNotSignal(2.Times()).Within(300.Milliseconds())
				.WithTimeout(300.Milliseconds())
				.UseTimeSystem(timeSystem);

		await That(Act).Throws<FailException>()
			.WithMessage("*but it did not finish within 0:00.300").AsWildcard().And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.300."))
			.Because("the second constraint only watched for the rest of the timeout instead of its own 300 ms");
		await That(timeSystem.Now).IsEqualTo(300.Milliseconds())
			.Because("the timeout ended the wait of the second constraint 100 ms after it started");
	}

	private static AndOrResult<bool, IExpectThat<bool>> Evaluate(CancellationCapturingConstraint constraint)
	{
#pragma warning disable aweXpect0001
		ThatBoolSubject that = That(true);
#pragma warning restore aweXpect0001
		return new AndOrResult<bool, IExpectThat<bool>>(
			((IExpectThat<bool>)that).ExpectationBuilder.AddConstraint((_, _) => constraint),
			that);
	}

	/// <summary>
	///     An evaluation cancellation whose <paramref name="timeout" /> elapsed on a virtual clock.
	/// </summary>
	private static EvaluationCancellation TimedOutAfter(TimeSpan timeout, CancellationToken callerToken = default)
	{
		VirtualTimeSystem timeSystem = new();
		EvaluationCancellation cancellation = new(timeout, callerToken, null, timeSystem);
		timeSystem.Advance(timeout);
		return cancellation;
	}

	private sealed class CancellationCapturingConstraint : IAsyncContextConstraint<bool>
	{
		public bool HasSameToken { get; private set; }
		public TimeSpan? Timeout { get; private set; }

		public ValueTask<ConstraintResult> IsMetBy(bool actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Timeout = context.Cancellation.Timeout;
			HasSameToken = context.Cancellation.Token == cancellationToken;
			return new ValueTask<ConstraintResult>(new DummyConstraintResult<bool>(Outcome.Success, actual, ""));
		}

		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null) { }
	}
}
