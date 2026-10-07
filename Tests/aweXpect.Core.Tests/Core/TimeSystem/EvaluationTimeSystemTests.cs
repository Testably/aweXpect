using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Recording;
using aweXpect.Signaling;

namespace aweXpect.Core.Tests.Core.TimeSystem;

/// <summary>
///     Every wait and every timeout of an evaluation runs on the time system of the evaluation.
/// </summary>
/// <remarks>
///     On the <see cref="VirtualTimeSystem" /> a wait takes no real time and reports the duration of the virtual clock.
///     No wait is longer than half a minute, so that a regression which waits in real time fails the test instead of
///     hanging the test run.
/// </remarks>
public sealed class EvaluationTimeSystemTests
{
	[Test]
	public async Task Signaled_Within_ShouldWaitOnTheTimeSystem()
	{
		VirtualTimeSystem timeSystem = new();
		Signaler signaler = new();

		async Task Act()
			=> await That(signaler).Signaled().Within(30.Seconds()).WithTimeSystem(timeSystem);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that signaler
			             has recorded the callback at least once within 0:30,
			             but it was never recorded within 0:30
			             """);
		await That(timeSystem.Now).IsEqualTo(30.Seconds());
	}

	[Test]
	public async Task Signaled_Within_WhenAlreadySignaled_ShouldNotWait()
	{
		VirtualTimeSystem timeSystem = new();
		Signaler signaler = new();
		signaler.Signal();

		await That(signaler).Signaled().Within(30.Seconds()).WithTimeSystem(timeSystem);

		await That(timeSystem.Now).IsEqualTo(TimeSpan.Zero);
	}

	[Test]
	public async Task Signaled_Within_WhenTheTimeoutIsNotShorter_ShouldLetTheWaitDecide()
	{
		VirtualTimeSystem timeSystem = new();
		Signaler signaler = new();

		async Task Act()
			=> await That(signaler).Signaled().Within(30.Seconds()).WithTimeout(30.Seconds())
				.WithTimeSystem(timeSystem);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that signaler
			             has recorded the callback at least once within 0:30,
			             but it was never recorded within 0:30
			             """)
			.Because("the timeout elapses on the same clock as the wait, so it ends the wait exactly at its end");
		await That(timeSystem.Now).IsEqualTo(30.Seconds());
	}

	[Test]
	public async Task Signaled_Within_WhenTheTimeoutIsShorter_ShouldFailWithTheTimeout()
	{
		VirtualTimeSystem timeSystem = new();
		Signaler signaler = new();

		async Task Act()
			=> await That(signaler).Signaled().Within(30.Seconds()).WithTimeout(10.Seconds())
				.WithTimeSystem(timeSystem);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that signaler
			             has recorded the callback at least once within 0:30,
			             but it did not finish within 0:10
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:10."));
		await That(timeSystem.Now).IsEqualTo(10.Seconds())
			.Because("the timeout cuts the wait short on the virtual clock");
	}

	[Test]
	public async Task Signaled_WithParameter_Within_ShouldWaitOnTheTimeSystem()
	{
		VirtualTimeSystem timeSystem = new();
		Signaler<int> signaler = new();
		signaler.Signal(1);

		async Task Act()
			=> await That(signaler).Signaled(2.Times()).Within(30.Seconds()).WithTimeSystem(timeSystem);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that signaler
			             has recorded the callback at least twice within 0:30,
			             but it was only recorded once in [
			               1
			             ] within 0:30
			             """);
		await That(timeSystem.Now).IsEqualTo(30.Seconds());
	}

	[Test]
	public async Task Triggered_Within_ShouldWaitOnTheTimeSystem()
	{
		VirtualTimeSystem timeSystem = new();
		CustomEventClass sut = new();
		IEventRecording<CustomEventClass> recording = sut.Record().Events();

		async Task Act()
			=> await That(recording).Triggered(nameof(CustomEventClass.CustomEvent)).Within(30.Seconds())
				.WithTimeSystem(timeSystem);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that recording
			             has recorded the CustomEvent event on sut at least once within 0:30,
			             but it was never recorded within 0:30
			             """);
		await That(timeSystem.Now).IsEqualTo(30.Seconds());
	}

	[Test]
	public async Task Triggered_Within_WhenTheTimeoutIsShorter_ShouldFailWithTheTimeout()
	{
		VirtualTimeSystem timeSystem = new();
		CustomEventClass sut = new();
		IEventRecording<CustomEventClass> recording = sut.Record().Events();

		async Task Act()
			=> await That(recording).Triggered(nameof(CustomEventClass.CustomEvent)).Within(30.Seconds())
				.WithTimeout(10.Seconds()).WithTimeSystem(timeSystem);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that recording
			             has recorded the CustomEvent event on sut at least once within 0:30,
			             but it did not finish within 0:10
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:10."));
		await That(timeSystem.Now).IsEqualTo(10.Seconds())
			.Because("the timeout cuts the wait short on the virtual clock");
	}

	[Test]
	public async Task WithTimeout_ShouldElapseOnTheTimeSystem()
	{
		VirtualTimeSystem timeSystem = new();
		bool? isCanceledAfterTheTimeout = null;
		Func<CancellationToken, Task> subject = token =>
		{
			timeSystem.Advance(2.Hours());
			isCanceledAfterTheTimeout = token.IsCancellationRequested;
			return Task.CompletedTask;
		};

		async Task Act()
			=> await That(subject).DoesNotThrow().WithTimeout(1.Hours()).WithTimeSystem(timeSystem);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             does not throw any exception,
			             but it did not finish within 1:00:00
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 1:00:00."));
		await That(isCanceledAfterTheTimeout).IsTrue()
			.Because("the timeout canceled the subject when the virtual clock reached it");
	}

	private sealed class CustomEventClass
	{
		public delegate void CustomEventDelegate(int arg1);

		public event CustomEventDelegate? CustomEvent;

		public void NotifyCustomEvent(int arg1)
			=> CustomEvent?.Invoke(arg1);
	}
}
