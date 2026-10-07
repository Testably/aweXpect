using System.Linq;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Signaling;
using ThreadState = System.Threading.ThreadState;

namespace aweXpect.Core.Tests.Signaling;

// Blocking the thread pool stalls every test of the process, so these tests only run on request and on their own.
[Explicit]
[Category(TestCategories.Slow)]
[NotInParallel]
public sealed class SignalerThreadPoolTests
{
	[Test]
	public async Task Wait_WhenNoThreadOfTheThreadPoolIsFree_ShouldReturnAsSoonAsSignalWasRecorded()
	{
		Signaler signaler = new();

		bool hasEnded = await WhileTheThreadPoolIsBlocked(() => EndsWhenSignaled(
			() => signaler.Wait(Timeout.InfiniteTimeSpan),
			() => signaler.Signal()));

		await That(hasEnded).IsTrue()
			.Because("the thread that signals wakes the waiting thread itself");
	}

	[Test]
	public async Task Wait_WithParameter_WhenNoThreadOfTheThreadPoolIsFree_ShouldReturnAsSoonAsSignalWasRecorded()
	{
		Signaler<int> signaler = new();

		bool hasEnded = await WhileTheThreadPoolIsBlocked(() => EndsWhenSignaled(
			() => signaler.Wait(timeout: Timeout.InfiniteTimeSpan),
			() => signaler.Signal(1)));

		await That(hasEnded).IsTrue()
			.Because("the thread that signals wakes the waiting thread itself");
	}

	/// <summary>
	///     Runs the <paramref name="wait" /> on a thread of its own and, as soon as it blocks, the
	///     <paramref name="signal" />, and returns whether the wait ended.
	/// </summary>
	private static bool EndsWhenSignaled(Action wait, Action signal)
	{
		Thread thread = new(() => wait())
		{
			IsBackground = true,
		};
		thread.Start();
		SpinWait.SpinUntil(() => !thread.IsAlive || (thread.ThreadState & ThreadState.WaitSleepJoin) != 0);
		signal();
		return thread.Join(5.Seconds());
	}

	/// <summary>
	///     Runs the <paramref name="callback" /> on a thread of its own while all threads of the thread pool are
	///     blocked.
	/// </summary>
	/// <remarks>
	///     The thread pool is limited to its minimum number of threads, because it would otherwise start a new thread
	///     for the work that waits.
	/// </remarks>
	private static Task<TResult> WhileTheThreadPoolIsBlocked<TResult>(Func<TResult> callback)
	{
		TaskCompletionSource<TResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
		Thread thread = new(() =>
		{
			ThreadPool.GetMinThreads(out int workerThreads, out _);
			ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxCompletionPortThreads);
			using ManualResetEventSlim release = new();
			using CountdownEvent blocked = new(workerThreads);
			Task[] blockers = [];
			try
			{
				if (!ThreadPool.SetMaxThreads(workerThreads, maxCompletionPortThreads))
				{
					throw new InvalidOperationException("The thread pool could not be limited.");
				}

				blockers = Enumerable.Range(0, workerThreads)
					.Select(_ => Task.Run(() =>
					{
						blocked.Signal();
						release.Wait();
					}))
					.ToArray();
				if (!blocked.Wait(10.Seconds()))
				{
					throw new TimeoutException("The thread pool could not be blocked.");
				}

				completion.SetResult(callback());
			}
			catch (Exception exception)
			{
				completion.SetException(exception);
			}
			finally
			{
				release.Set();
				// The events are disposed afterwards, so no blocker must still use them.
				Task.WaitAll(blockers);
				ThreadPool.SetMaxThreads(maxWorkerThreads, maxCompletionPortThreads);
			}
		})
		{
			IsBackground = true,
		};
		thread.Start();
		return completion.Task;
	}
}
