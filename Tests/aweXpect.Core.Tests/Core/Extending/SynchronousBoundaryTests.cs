using System.Threading;
using aweXpect.Core.Extending;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Extending;

public sealed class SynchronousBoundaryTests
{
	[Test]
	public async Task Wait_WhenOperationCompletesRightAway_ShouldReturnTheResult()
	{
		int result = SynchronousBoundary.Wait(() => new ValueTask<int>(42));

		await That(result).IsEqualTo(42);
	}

	[Test]
	public async Task Wait_WhenOperationCompletesRightAway_ShouldReturnTheResultOnABlockedSynchronizationContext()
	{
		int result = 0;

		bool completed = BlockedSynchronizationContext.Run(()
			=> result = SynchronousBoundary.Wait(() => new ValueTask<int>(42)));

		await That(completed).IsTrue();
		await That(result).IsEqualTo(42);
	}

	[Test]
	public async Task Wait_WhenOperationIsCanceled_ShouldThrowTaskCanceledException()
	{
		void Act()
			=> SynchronousBoundary.Wait(() => new ValueTask<int>(Task.FromCanceled<int>(new CancellationToken(true))));

		await That(Act).ThrowsExactly<TaskCanceledException>()
			.WithMessage(new TaskCanceledException().Message)
			.Because("a canceled operation throws like an awaited canceled task");
	}

	[Test]
	public async Task Wait_WhenOperationIsCanceled_ShouldThrowTaskCanceledExceptionOnABlockedSynchronizationContext()
	{
		void Act()
			=> BlockedSynchronizationContext.Run(() => SynchronousBoundary.Wait(()
				=> new ValueTask<int>(Task.FromCanceled<int>(new CancellationToken(true)))));

		await That(Act).ThrowsExactly<TaskCanceledException>()
			.WithMessage(new TaskCanceledException().Message)
			.Because("a canceled operation throws like an awaited canceled task");
	}

	[Test]
	public async Task Wait_WhenOperationIsFaulted_ShouldThrowTheException()
	{
		void Act()
			=> SynchronousBoundary.Wait(()
				=> new ValueTask<int>(Task.FromException<int>(new InvalidOperationException("the catalog is offline"))));

		await That(Act).ThrowsExactly<InvalidOperationException>()
			.WithMessage("the catalog is offline")
			.Because("the exception is not wrapped in an AggregateException");
	}

	[Test]
	public async Task Wait_WhenOperationIsFaulted_ShouldThrowTheExceptionOnABlockedSynchronizationContext()
	{
		void Act()
			=> BlockedSynchronizationContext.Run(() => SynchronousBoundary.Wait(()
				=> new ValueTask<int>(Task.FromException<int>(new InvalidOperationException("the catalog is offline")))));

		await That(Act).ThrowsExactly<InvalidOperationException>()
			.WithMessage("the catalog is offline")
			.Because("the exception is not wrapped in an AggregateException");
	}

	[Test]
	public async Task Wait_WhenOperationIsNull_ShouldThrowArgumentNullException()
	{
		void Act()
			=> SynchronousBoundary.Wait<int>(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithMessage("The 'operation' cannot be null.").AsPrefix();
	}

	[Test]
	public async Task Wait_WhenOperationThrowsAfterYielding_ShouldThrowTheException()
	{
		void Act()
			=> SynchronousBoundary.Wait<int>(async () =>
			{
				await Task.Yield();
				throw new InvalidOperationException("the catalog is offline");
			});

		await That(Act).ThrowsExactly<InvalidOperationException>()
			.WithMessage("the catalog is offline")
			.Because("the exception is not wrapped in an AggregateException");
	}

	[Test]
	public async Task Wait_WhenOperationYieldsOnABlockedSynchronizationContext_ShouldNotDeadlock()
	{
		int result = 0;

		bool completed = BlockedSynchronizationContext.Run(() => result = SynchronousBoundary.Wait(async () =>
		{
			await Task.Yield();
			return 42;
		}));

		await That(completed).IsTrue()
			.Because("the continuation must not wait for the thread that is blocked by the synchronous boundary");
		await That(result).IsEqualTo(42);
	}

	[Test]
	public async Task Wait_WhenOperationYieldsOnABlockedSynchronizationContext_ShouldRestoreIt()
	{
		SynchronizationContext? contextAfterwards = null;
		SynchronizationContext? contextBefore = null;

		BlockedSynchronizationContext.Run(() =>
		{
			contextBefore = SynchronizationContext.Current;
			SynchronousBoundary.Wait(async () =>
			{
				await Task.Yield();
				return 42;
			});
			contextAfterwards = SynchronizationContext.Current;
		});

		await That(contextAfterwards).IsSameAs(contextBefore)
			.Because("the caller keeps running on its synchronization context after the synchronous boundary");
	}

	[Test]
	public async Task Wait_WhenOperationYieldsOnABlockedTaskScheduler_ShouldNotDeadlock()
	{
		int result = 0;

		bool completed = BlockedTaskScheduler.Run(() => result = SynchronousBoundary.Wait(async () =>
		{
			await Task.Yield();
			return 42;
		}));

		await That(completed).IsTrue()
			.Because("the continuation must not wait for the scheduler that is blocked by the synchronous boundary");
		await That(result).IsEqualTo(42);
	}
}
