using System.Text;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Customization;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Core.EvaluationContext;

public class EvaluationCancellationTests
{
	[Fact]
	public async Task Cancellation_ShouldHaveTheTokenThatTheConstraintReceives()
	{
		CancellationCapturingConstraint constraint = new();

		await Evaluate(constraint).WithTimeout(10.Seconds());

		await That(constraint.HasSameToken).IsTrue()
			.Because("the token of the cancellation and the token parameter describe the same evaluation");
	}

	[Fact]
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

	[Fact]
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

	[Fact]
	public async Task Cancellation_WithoutTimeout_ShouldHaveNoTimeout()
	{
		CancellationCapturingConstraint constraint = new();

		await Evaluate(constraint);

		await That(constraint.Timeout).IsNull();
	}

	[Fact]
	public async Task CountsAsElapsed_WhenTheCallerCanceled_ShouldBeFalse()
	{
		using CancellationTokenSource cts = new();
		EvaluationCancellation sut = new(10.Seconds(), cts.Token);
		cts.Cancel();

		bool result = sut.CountsAsElapsed(1.Seconds(), 100.Milliseconds());

		await That(result).IsFalse()
			.Because("a cancellation by the caller before the end of the wait decides nothing");
		sut.Release();
	}

	[Fact]
	public async Task CountsAsElapsed_WhenTheCancellationCameAtTheEndOfTheWait_ShouldBeTrue()
	{
		using CancellationTokenSource cts = new();
		EvaluationCancellation sut = new(null, cts.Token);
		cts.Cancel();

		bool result = sut.CountsAsElapsed(1.Seconds(), 999.Milliseconds());

		await That(result).IsTrue()
			.Because("the timers of the wait and of the cancellation do not share the clock of the stopwatch");
	}

	[Fact]
	public async Task CountsAsElapsed_WhenTheTimeoutElapsedAndIsNotShorterThanTheWait_ShouldBeTrue()
	{
		EvaluationCancellation sut = new(10.Milliseconds(), CancellationToken.None);
		await WaitForCancellation(sut.Token);

		bool result = sut.CountsAsElapsed(10.Milliseconds(), 1.Milliseconds());

		await That(result).IsTrue()
			.Because("the timer of the timeout started before the wait, so it can expire slightly earlier");
		sut.Release();
	}

	[Fact]
	public async Task CountsAsElapsed_WhenTheTimeoutElapsedAndIsShorterThanTheWait_ShouldBeFalse()
	{
		EvaluationCancellation sut = new(10.Milliseconds(), CancellationToken.None);
		await WaitForCancellation(sut.Token);

		bool result = sut.CountsAsElapsed(1.Seconds(), 10.Milliseconds());

		await That(result).IsFalse()
			.Because("a shorter timeout is reported as the timeout, not as the result of the wait");
		sut.Release();
	}

	[Fact]
	public async Task CountsAsElapsed_WhenTheWaitIsInfinite_ShouldBeFalse()
	{
		EvaluationCancellation sut = new(10.Milliseconds(), CancellationToken.None);
		await WaitForCancellation(sut.Token);

		bool result = sut.CountsAsElapsed(Timeout.InfiniteTimeSpan, 10.Milliseconds());

		await That(result).IsFalse()
			.Because("an infinite wait is never used up");
		sut.Release();
	}

	[Fact]
	public async Task IsCallerCanceled_WhenTheCallerCanceled_ShouldBeTrue()
	{
		using CancellationTokenSource cts = new();
		EvaluationCancellation sut = new(10.Seconds(), cts.Token);

		cts.Cancel();

		await That(sut.IsCallerCanceled).IsTrue();
		await That(sut.IsTimeoutElapsed).IsFalse()
			.Because("the caller canceled before the timeout elapsed");
		await That(sut.Token.IsCancellationRequested).IsTrue();
		sut.Release();
	}

	[Fact]
	public async Task IsTimeoutElapsed_WhenTheTimeoutElapsed_ShouldBeTrue()
	{
		EvaluationCancellation sut = new(10.Milliseconds(), CancellationToken.None);

		await WaitForCancellation(sut.Token);

		await That(sut.IsTimeoutElapsed).IsTrue();
		await That(sut.IsCallerCanceled).IsFalse();
		sut.Release();
	}

	[Fact]
	public async Task IsTimeoutElapsed_WhenTheTimeoutElapsedAndTheCallerCanceled_ShouldBeFalse()
	{
		using CancellationTokenSource cts = new();
		EvaluationCancellation sut = new(10.Milliseconds(), cts.Token);
		await WaitForCancellation(sut.Token);

		cts.Cancel();

		await That(sut.IsTimeoutElapsed).IsFalse()
			.Because("a cancellation by the caller is never reported as an elapsed timeout");
		await That(sut.IsCallerCanceled).IsTrue();
		sut.Release();
	}

	[Fact]
	public async Task None_ShouldNotBeCanceledAndHaveNoTimeout()
	{
		EvaluationCancellation sut = EvaluationCancellation.None;

		await That(sut.Token.CanBeCanceled).IsFalse();
		await That(sut.Timeout).IsNull();
		await That(sut.IsTimeoutElapsed).IsFalse();
		await That(sut.IsCallerCanceled).IsFalse();
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

	private static async Task WaitForCancellation(CancellationToken token)
	{
		TaskCompletionSource<bool> canceled = new();
		using CancellationTokenRegistration _ = token.Register(() => canceled.TrySetResult(true));
		await canceled.Task;
	}

	private sealed class CancellationCapturingConstraint : IAsyncContextConstraint<bool>
	{
		public bool HasSameToken { get; private set; }
		public TimeSpan? Timeout { get; private set; }

		public Task<ConstraintResult> IsMetBy(bool actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Timeout = context.Cancellation.Timeout;
			HasSameToken = context.Cancellation.Token == cancellationToken;
			return Task.FromResult<ConstraintResult>(new DummyConstraintResult<bool>(Outcome.Success, actual, ""));
		}

		public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null) { }
	}
}
