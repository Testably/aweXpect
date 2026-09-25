using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Sources;

public class DelegateAsyncSourceTests
{
	[Fact]
	public async Task ForExecutionTime_ShouldUseElapsedFromTimeSystem()
	{
		TimeSystemMock timeSystem = new TimeSystemMock().SetElapsed(1100.Milliseconds());

		async Task Act() =>
			await That(() => Task.CompletedTask).ExecutesIn().AtLeast(1000.Milliseconds())
				.UseTimeSystem(timeSystem);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenCancellationIsRequestedBeforeTheDelegateCompletes_ShouldBeInconclusive()
	{
		Func<Task> @delegate = () => PendingTask.Of<int>();
		using CancellationTokenSource cts = new();
		cts.CancelAfter(50.Milliseconds());

		async Task Act()
			=> await That(@delegate).DoesNotThrow().WithCancellation(cts.Token);

		await That(Act).Throws<InconclusiveException>()
			.WithMessage("""
			             Expected that @delegate
			             does not throw any exception,
			             but it could not be verified, because it was already canceled
			             """)
			.Because("the cancellation must stop waiting for a delegate that does not observe it");
	}

	[Fact]
	public async Task WhenDelegateCompletesWithinTheTimeout_ShouldSucceed()
	{
		Func<Task> @delegate = () => Task.Delay(50.Milliseconds());

		async Task Act()
			=> await That(@delegate).DoesNotThrow().WithTimeout(5.Seconds());

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenDelegateDoesNotCompleteWithinTheTimeout_ShouldFail()
	{
		Func<Task> @delegate = () => PendingTask.Of<int>();

		async Task Act()
			=> await That(@delegate).DoesNotThrow().WithTimeout(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that @delegate
			             does not throw any exception,
			             but it did not finish within 0:00.050
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
			.Because("the timeout must abandon the task instead of awaiting it to completion");
	}

	[Fact]
	public async Task WhenDelegateDoesNotCompleteWithinTheTimeout_Throws_ShouldFail()
	{
		Func<Task> @delegate = () => PendingTask.Of<int>();

		async Task Act()
			=> await That(@delegate).Throws().WithTimeout(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that @delegate
			             throws an exception,
			             but it did not finish within 0:00.050
			             """).And
			.WithInner<TimeoutException>()
			.Because("the timeout must not count as the expected exception");
	}

	[Fact]
	public async Task WhenDelegateExceedsTheDurationOfExecutesWithin_ShouldFail()
	{
		Func<Task> @delegate = () => PendingTask.Of<int>();

		async Task Act()
			=> await That(@delegate).ExecutesWithin(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that @delegate
			             executes within 0:00.050,
			             but it did not finish within 0:00.050
			             """)
			.Because("the duration must abandon the task instead of awaiting it to completion");
	}

	[Fact]
	public async Task WhenDelegateExceedsTheDurationOfThrowsWithin_ShouldFail()
	{
		Func<Task> @delegate = () => PendingTask.Of<int>();

		async Task Act()
			=> await That(@delegate).Throws<ArgumentException>().Within(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that @delegate
			             throws an ArgumentException within 0:00.050,
			             but it did not finish within 0:00.050
			             """).And
			.WithInner<TimeoutException>()
			.Because("the duration must abandon the task instead of awaiting it to completion");
	}

	[Fact]
	public async Task WhenDelegateExceedsTheUpperBoundOfExecutesIn_ShouldFail()
	{
		Func<Task> @delegate = () => PendingTask.Of<int>();

		async Task Act()
			=> await That(@delegate).ExecutesIn().AtMost(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that @delegate
			             executes in at most 0:00.050,
			             but it did not finish within 0:00.050
			             """)
			.Because("the upper bound must abandon the task instead of awaiting it to completion");
	}

	[Fact]
	public async Task WhenDelegateIgnoresTheCancellationToken_ShouldFail()
	{
		Func<CancellationToken, Task> @delegate = _ => PendingTask.Of<int>();

		async Task Act()
			=> await That(@delegate).ExecutesWithin(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that @delegate
			             executes within 0:00.050,
			             but it did not finish within 0:00.050
			             """)
			.Because("a delegate that ignores the cancelled token must be abandoned as well");
	}

	[Fact]
	public async Task WhenDelegateIsCanceledByTheTimeout_ShouldReportThatItDidNotFinish()
	{
		Func<CancellationToken, Task> @delegate = token => Task.Delay(30.Seconds(), token);

		async Task Act()
			=> await That(@delegate).Throws<TaskCanceledException>().WithTimeout(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that @delegate
			             throws a TaskCanceledException,
			             but it did not finish within 0:00.050
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
			.Because("a cancellation by the timeout is reported the same way, whether the delegate or the timeout won");
	}

	[Fact]
	public async Task WhenDelegateThrowsItsOwnCancellationOnTimeout_ShouldReportThatItDidNotFinish()
	{
		Func<CancellationToken, Task> @delegate = async token =>
		{
			try
			{
				await Task.Delay(30.Seconds(), token);
			}
			catch (OperationCanceledException)
			{
				throw new OperationCanceledException("my cancellation", token);
			}
		};

		async Task Act()
			=> await That(@delegate).DoesNotThrow().WithTimeout(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that @delegate
			             does not throw any exception,
			             but it did not finish within 0:00.050
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
			.Because("a cancellation by the timeout is reported the same way, whether the delegate or the timeout won");
	}

	[Fact]
	public async Task WhenDelegateThrowsItsOwnOperationCanceledException_ShouldFail()
	{
		OperationCanceledException exception = new("my own reason");
		Func<CancellationToken, Task> @delegate = async _ =>
		{
			await Task.Yield();
			throw exception;
		};

		async Task Act()
			=> await That(@delegate).DoesNotThrow().WithTimeout(30.Seconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that @delegate
			             does not throw any exception,
			             but it did throw an OperationCanceledException:
			               my own reason
			             """).And
			.WithInner<OperationCanceledException>(inner => inner.IsSameAs(exception))
			.Because("a cancellation that neither the timeout nor the caller requested is an ordinary exception");
	}

	[Fact]
	public async Task WhenDelegateThrowsItsOwnOperationCanceledException_Throws_ShouldSucceed()
	{
		Func<CancellationToken, Task> @delegate = async _ =>
		{
			await Task.Yield();
			throw new OperationCanceledException("my own reason");
		};

		async Task Act()
			=> await That(@delegate).Throws<OperationCanceledException>().WithTimeout(30.Seconds());

		await That(Act).DoesNotThrow()
			.Because("a cancellation that neither the timeout nor the caller requested is an ordinary exception");
	}

	[Fact]
	public async Task WhenTaskSubjectDoesNotCompleteWithinTheTimeout_ShouldFail()
	{
		Task subject = PendingTask.Of<int>();

		async Task Act()
			=> await That(subject).DoesNotThrow().WithTimeout(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             does not throw any exception,
			             but it did not finish within 0:00.050
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
			.Because("the timeout must abandon the task instead of awaiting it to completion");
	}
}
