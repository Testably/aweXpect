using System.Runtime.CompilerServices;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Sources;

public class AsyncValueSourceTests
{
	[Fact]
	public async Task WhenAbandonedTaskFaultsLater_ShouldNotRaiseUnobservedTaskException()
	{
		MyException exception = new();
		bool isRaised = false;
		EventHandler<UnobservedTaskExceptionEventArgs> handler = (_, e) =>
		{
			if (e.Exception.InnerExceptions.Contains(exception))
			{
				isRaised = true;
			}
		};

		TaskScheduler.UnobservedTaskException += handler;
		try
		{
			await AbandonTaskThatFaultsLater(exception);
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
		finally
		{
			TaskScheduler.UnobservedTaskException -= handler;
		}

		await That(isRaised).IsFalse()
			.Because("the exception of an abandoned task must be observed, as nobody else awaits it");
	}

	[Fact]
	public async Task WhenCancellationIsRequestedBeforeTheTaskCompletes_ShouldBeInconclusive()
	{
		Task<int> subject = PendingTask.Of<int>();
		using CancellationTokenSource cts = new();
		cts.CancelAfter(50.Milliseconds());

		async Task Act()
			=> await That(subject).IsEqualTo(1).WithCancellation(cts.Token);

		await That(Act).Throws<InconclusiveException>()
			.WithMessage("""
			             Expected that subject
			             is equal to 1,
			             but it could not be verified, because it was already canceled
			             """)
			.Because("the cancellation must stop waiting for a task that does not observe it");
	}

	[Fact]
	public async Task WhenTaskCompletesWithinTheTimeout_ShouldSucceed()
	{
		Task<int> subject = Task.Delay(50.Milliseconds()).ContinueWith(_ => 1);

		async Task Act()
			=> await That(subject).IsEqualTo(1).WithTimeout(5.Seconds());

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenTaskDoesNotCompleteWithinTheTimeout_ShouldFail()
	{
		Task<int> subject = PendingTask.Of<int>();

		async Task Act()
			=> await That(subject).IsEqualTo(1).WithTimeout(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is equal to 1,
			             but it did not finish within 0:00.050
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
			.Because("the timeout must abandon the task instead of awaiting it to completion");
	}

	[Fact]
	public async Task WhenTaskFaultsWithATimeout_ShouldFail()
	{
		Task<int> subject = Task.FromException<int>(new MyException());

		async Task Act()
			=> await That(subject).IsEqualTo(1).WithTimeout(5.Seconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is equal to 1,
			             but it did throw a MyException:
			               WhenTaskFaultsWithATimeout_ShouldFail
			             """).And
			.WithInner<MyException>()
			.Because("a faulted task fails the expectation, with or without a timeout");
	}

	[Fact]
	public async Task WhenTaskIsCanceledWithoutACancellationRequest_ShouldFail()
	{
		TaskCanceledException exception = new("the task canceled itself");
		Task<int> subject = Task.FromException<int>(exception);

		async Task Act()
			=> await That(subject).IsEqualTo(1).WithTimeout(5.Seconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is equal to 1,
			             but it did throw a TaskCanceledException:
			               the task canceled itself
			             """).And
			.WithInner<TaskCanceledException>(inner => inner.IsSameAs(exception))
			.Because("a task that cancels itself for its own reasons throws an ordinary exception");
	}

	/// <remarks>
	///     The task is only reachable from within this method, so that it can be collected afterwards.
	/// </remarks>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static async Task AbandonTaskThatFaultsLater(Exception exception)
	{
		TaskCompletionSource<int> tcs = new();
		using CancellationTokenSource safetyNet = new(30.Seconds());
		using CancellationTokenRegistration _ = safetyNet.Token.Register(() => tcs.TrySetException(exception));

		async Task Act()
			=> await That(tcs.Task).IsEqualTo(1).WithTimeout(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that tcs.Task
			             is equal to 1,
			             but it did not finish within 0:00.050
			             """);
		tcs.TrySetException(exception);
	}
}
