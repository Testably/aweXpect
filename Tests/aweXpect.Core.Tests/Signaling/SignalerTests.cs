using System.Diagnostics;
using System.Linq;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Customization;
using aweXpect.Signaling;
using ThreadState = System.Threading.ThreadState;

namespace aweXpect.Core.Tests.Signaling;

public sealed class SignalerTests
{
	private static TimeSpan DefaultTimeout => Customize.aweXpect.Settings().DefaultSignalerTimeout.Get();

	/// <summary>
	///     Runs the <paramref name="wait" /> on a thread of its own and, as soon as it blocks, the
	///     <paramref name="whenWaiting" /> action on the calling thread.
	/// </summary>
	/// <remarks>
	///     A synchronous wait must neither block a thread of the thread pool nor be ended through it: when the tests
	///     that run in parallel occupy all its threads, a queued task or the callback of a timer runs later than any
	///     timeout of the wait, and on .NET Framework a signaled wait only returns once a thread of the pool is free.
	///     <para />
	///     It gives up after the default signaler timeout, so that a wait that never ends fails the test instead of
	///     hanging the test run.
	/// </remarks>
	private static async Task<TResult> WhileWaiting<TResult>(Func<TResult> wait, Action whenWaiting)
	{
		TaskCompletionSource<TResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
		Thread thread = new(() =>
		{
			try
			{
				completion.SetResult(wait());
			}
			catch (Exception exception)
			{
				completion.SetException(exception);
			}
		})
		{
			IsBackground = true,
		};
		thread.Start();
		SpinWait.SpinUntil(() => !thread.IsAlive || (thread.ThreadState & ThreadState.WaitSleepJoin) != 0);
		whenWaiting();
		using CancellationTokenSource cts = new();
		if (await Task.WhenAny(completion.Task, Task.Delay(DefaultTimeout, cts.Token)) != completion.Task)
		{
			throw new TimeoutException("The wait did not end.");
		}

		cts.Cancel();
		return await completion.Task;
	}

	public sealed class Tests
	{
		[Test]
		[Arguments(0, 0, true)]
		[Arguments(0, 1, false)]
		[Arguments(2, 0, true)]
		[Arguments(2, 1, true)]
		[Arguments(2, 2, true)]
		[Arguments(2, 3, false)]
		[Arguments(2, 5, false)]
		[Arguments(0, null, false)]
		[Arguments(1, null, true)]
		[Arguments(2, null, true)]
		public async Task IsSignaled_ShouldCompareToSignalCount(int signalCount, int? amount, bool expectedResult)
		{
			Signaler signaler = new();

			for (int i = 0; i < signalCount; i++)
			{
				signaler.Signal();
			}

			bool result = signaler.IsSignaled(amount?.Times());

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task Signal_AfterASuccessfulWait_ShouldNotThrow()
		{
			Signaler signaler = new();
			signaler.Signal();
			SignalerResult result = await WhileWaiting(
				() => signaler.Wait(2.Times(), 5.Seconds()),
				() => signaler.Signal());

			void Act()
				=> signaler.Signal();

			await That(result.IsSuccess).IsTrue();
			await That(Act).DoesNotThrow()
				.Because("nobody waits anymore, so the signal must not fail on the event of the ended wait");
			await That(signaler.IsSignaled(3.Times())).IsTrue();
		}

		[Test]
		public async Task Signal_AfterATimedOutWait_ShouldNotThrow()
		{
			Signaler signaler = new();
			signaler.Wait(2.Times(), 10.Milliseconds());

			void Act()
				=> signaler.Signal();

			await That(Act).DoesNotThrow()
				.Because("nobody waits anymore, so the signal must not fail on the event of the ended wait");
		}

		[Test]
		public async Task WaitAsync_InfiniteTimeout_ShouldWaitForTheSignal()
		{
			Signaler signaler = new();

			Task<SignalerResult> wait = signaler.WaitAsync(Timeout.InfiniteTimeSpan);
			signaler.Signal();
			SignalerResult result = await wait;

			await That(result.IsSuccess).IsTrue();
		}

		[Test]
		[Arguments(false)]
		[Arguments(true)]
		public async Task WaitAsync_NegativeTimeout_ShouldThrowArgumentOutOfRangeException(bool isAlreadySignaled)
		{
			Signaler signaler = new();
			if (isAlreadySignaled)
			{
				signaler.Signal();
				signaler.Signal();
			}

			void Act()
				=> _ = signaler.WaitAsync(-2.Milliseconds());

			void ActWithAmount()
				=> _ = signaler.WaitAsync(2.Times(), -2.Milliseconds());

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("timeout").And
				.WithMessage("The timeout must not be negative.").AsPrefix()
				.Because("the timeout is validated before the wait starts, also when the signals were already received");
			await That(ActWithAmount).Throws<ArgumentOutOfRangeException>()
				.WithParamName("timeout").And
				.WithMessage("The timeout must not be negative.").AsPrefix()
				.Because("the timeout is validated before the wait starts, also when the signals were already received");
		}

		[Test]
		public async Task WaitAsync_ShouldCompleteAsSoonAsEnoughSignalsWereRecorded()
		{
			Signaler signaler = new();
			signaler.Signal();

			Task<SignalerResult> wait = signaler.WaitAsync(2.Times(), 10.Seconds());
			bool isCompletedBeforeTheSecondSignal = wait.IsCompleted;
			signaler.Signal();
			SignalerResult result = await wait;

			await That(isCompletedBeforeTheSecondSignal).IsFalse();
			await That(result.IsSuccess).IsTrue();
			await That(result.Count).IsEqualTo(2)
				.Because("the signal before the wait counts as well");
		}

		[Test]
		public async Task WaitAsync_ShouldNotContinueOnTheThreadThatSignals()
		{
			Signaler signaler = new();
			using ManualResetEventSlim releaseTheContinuation = new();
			bool wasReleased = false;

			async Task WaitAndBlock()
			{
				await signaler.WaitAsync(10.Seconds());
				wasReleased = releaseTheContinuation.Wait(10.Seconds());
			}

			Task waiting = WaitAndBlock();
			signaler.Signal();
			releaseTheContinuation.Set();
			await waiting;

			await That(wasReleased).IsTrue()
				.Because("the code under test that signals must not run the continuation of the waiting expectation");
		}

		[Test]
		public async Task WaitAsync_ShouldReturnAtTheCancellationWithoutThrowing()
		{
			Signaler signaler = new();
			signaler.Signal();
			using CancellationTokenSource cts = new(30.Milliseconds());

			SignalerResult result = await signaler.WaitAsync(2.Times(), 10.Seconds(), cts.Token);

			await That(result.IsSuccess).IsFalse();
			await That(result.Count).IsEqualTo(1)
				.Because("the signals received until the cancellation are returned");
		}

		[Test]
		public async Task WaitAsync_ShouldUseTimeout()
		{
			Signaler signaler = new();
			Stopwatch sw = Stopwatch.StartNew();

			SignalerResult result = await signaler.WaitAsync(10.Milliseconds());

			sw.Stop();
			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(DefaultTimeout)
				.Because("the 10 ms timeout must end the wait long before the default signaler timeout of 30 s would");
		}

		[Test]
		[Arguments(0)]
		[Arguments(-1)]
		public async Task WaitAsync_ZeroOrNegativeAmount_ShouldThrowArgumentOutOfRangeException(int amount)
		{
			Signaler signaler = new();

			void Act()
				=> _ = signaler.WaitAsync(amount);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithMessage("The amount must be greater than zero*").AsWildcard().And
				.WithParamName("amount");
		}

		[Test]
		[Arguments(2)]
		[Arguments(3)]
		public async Task Wait_EnoughSignals_ShouldSucceed(int amount)
		{
			Signaler signaler = new();

			signaler.Signal();
			signaler.Signal();
			signaler.Signal();

			SignalerResult result = signaler.Wait(amount);

			await That(result.IsSuccess).IsTrue();
		}

		[Test]
		public async Task Wait_InfiniteTimeout_ShouldWaitForTheSignal()
		{
			Signaler signaler = new();

			SignalerResult result = await WhileWaiting(
				() => signaler.Wait(Timeout.InfiniteTimeSpan),
				() => signaler.Signal());

			await That(result.IsSuccess).IsTrue();
		}

		[Test]
		[Arguments(false)]
		[Arguments(true)]
		public async Task Wait_NegativeTimeout_ShouldThrowArgumentOutOfRangeException(bool isAlreadySignaled)
		{
			Signaler signaler = new();
			if (isAlreadySignaled)
			{
				signaler.Signal();
				signaler.Signal();
			}

			void Act()
				=> signaler.Wait(-2.Milliseconds());

			void ActWithAmount()
				=> signaler.Wait(2.Times(), -2.Milliseconds());

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("timeout").And
				.WithMessage("The timeout must not be negative.").AsPrefix()
				.Because("the timeout is validated before the wait starts, also when the signals were already received");
			await That(ActWithAmount).Throws<ArgumentOutOfRangeException>()
				.WithParamName("timeout").And
				.WithMessage("The timeout must not be negative.").AsPrefix()
				.Because("the timeout is validated before the wait starts, also when the signals were already received");
		}

		[Test]
		public async Task Wait_ShouldCatchOperationCanceledException()
		{
			Signaler signaler = new();
			TimeSpan timeout = 10.Seconds();
			using CancellationTokenSource cts = new();
			CancellationToken token = cts.Token;

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = await WhileWaiting(
				() => signaler.Wait(100.Times(), timeout, token),
				() =>
				{
					Parallel.For(0, 99, _ => signaler.Signal());
					// ReSharper disable once AccessToDisposedClosure
					cts.Cancel();
				});
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_ShouldReturnAsSoonAsEnoughSignalsWereRecorded()
		{
			Signaler signaler = new();
			TimeSpan timeout = 10.Seconds();

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = await WhileWaiting(
				() => signaler.Wait(100.Times(), timeout),
				() => Parallel.For(0, 100, _ => signaler.Signal()));
			sw.Stop();

			await That(result.IsSuccess).IsTrue();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_ShouldUseTimeout()
		{
			Signaler signaler = new();
			TimeSpan timeout = 10.Milliseconds();

			signaler.Signal();

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(2.Times(), timeout);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(DefaultTimeout)
				.Because("the 10 ms timeout must end the wait long before the default signaler timeout of 30 s would");
		}

		[Test]
		public async Task Wait_Single_AlreadySignaled_ShouldSucceed()
		{
			Signaler signaler = new();

			signaler.Signal();
			signaler.Signal();

			SignalerResult result = signaler.Wait();

			await That(result.IsSuccess).IsTrue();
		}

		[Test]
		public async Task Wait_Single_ShouldCatchOperationCanceledException()
		{
			Signaler signaler = new();
			TimeSpan timeout = 10.Seconds();
			using CancellationTokenSource cts = new();
			CancellationToken token = cts.Token;

			Stopwatch sw = new();
			sw.Start();
			// ReSharper disable once AccessToDisposedClosure
			SignalerResult result = await WhileWaiting(() => signaler.Wait(timeout, token), () => cts.Cancel());
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_Single_ShouldReturnAsSoonAsSignalWasRecorded()
		{
			Signaler signaler = new();
			TimeSpan timeout = 10.Seconds();

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = await WhileWaiting(() => signaler.Wait(timeout), () => signaler.Signal());
			sw.Stop();

			await That(result.IsSuccess).IsTrue();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_Single_ShouldUseTimeout()
		{
			Signaler signaler = new();
			TimeSpan timeout = 10.Milliseconds();

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(timeout);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(DefaultTimeout)
				.Because("the 10 ms timeout must end the wait long before the default signaler timeout of 30 s would");
		}

		[Test]
		public async Task Wait_Single_WhenTimeoutExceedsTheTimerRange_ShouldWaitForTheSignal()
		{
			Signaler signaler = new();
			using CancellationTokenSource cts = new(5.Seconds());
			CancellationToken token = cts.Token;

			SignalerResult result = await WhileWaiting(() => signaler.Wait(60.Days(), token), () => signaler.Signal());

			await That(result.IsSuccess).IsTrue()
				.Because("a timeout beyond the range of the wait handle must not throw");
		}

		[Test]
		public async Task Wait_WhenAnotherWaitIsPending_ShouldNotInterfereWithIt()
		{
			Signaler signaler = new();
			SignalerResult? result = null;

			SignalerResult pendingResult = await WhileWaiting(
				() => signaler.Wait(2.Times(), 5.Seconds()),
				() =>
				{
					signaler.Signal();
					result = signaler.Wait(2.Times(), TimeSpan.Zero);
					signaler.Signal();
				});

			await That(result?.IsSuccess).IsFalse();
			await That(pendingResult.IsSuccess).IsTrue()
				.Because("the other wait must neither replace nor remove the event of the pending wait");
			await That(pendingResult.Count).IsEqualTo(2);
		}

		[Test]
		public async Task Wait_WhenTimeoutExceedsTheTimerRange_ShouldWaitForTheSignals()
		{
			Signaler signaler = new();
			using CancellationTokenSource cts = new(5.Seconds());
			CancellationToken token = cts.Token;

			signaler.Signal();

			SignalerResult result = await WhileWaiting(
				() => signaler.Wait(2.Times(), 60.Days(), token),
				() => signaler.Signal());

			await That(result.IsSuccess).IsTrue()
				.Because("a timeout beyond the range of the wait handle must not throw");
			await That(result.Count).IsEqualTo(2);
		}

		[Test]
		[Arguments(0)]
		[Arguments(-1)]
		public async Task Wait_ZeroOrNegativeAmount_ShouldThrowArgumentOutOfRangeException(int amount)
		{
			Signaler signaler = new();

			void Act()
				=> signaler.Wait(amount);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithMessage("The amount must be greater than zero*").AsWildcard().And
				.WithParamName("amount");
		}
	}

	public sealed class WithParameterTests
	{
		[Test]
		[Arguments(0, 0, true)]
		[Arguments(0, 1, false)]
		[Arguments(2, 0, true)]
		[Arguments(2, 1, true)]
		[Arguments(2, 2, true)]
		[Arguments(2, 3, false)]
		[Arguments(2, 5, false)]
		[Arguments(0, null, false)]
		[Arguments(1, null, true)]
		[Arguments(2, null, true)]
		public async Task IsSignaled_ShouldCompareToSignalCount(int signalCount, int? amount, bool expectedResult)
		{
			Signaler<int> signaler = new();

			for (int i = 0; i < signalCount; i++)
			{
				signaler.Signal(i);
			}

			bool result = signaler.IsSignaled(amount?.Times());

			await That(result).IsEqualTo(expectedResult);
		}

		[Test]
		public async Task Signal_AfterASuccessfulWait_ShouldNotThrow()
		{
			Signaler<int> signaler = new();
			signaler.Signal(1);
			SignalerResult<int> result = await WhileWaiting(
				() => signaler.Wait(2.Times(), timeout: 5.Seconds()),
				() => signaler.Signal(2));

			void Act()
				=> signaler.Signal(3);

			await That(result.IsSuccess).IsTrue();
			await That(Act).DoesNotThrow()
				.Because("nobody waits anymore, so the signal must not fail on the event of the ended wait");
			await That(signaler.IsSignaled(3.Times())).IsTrue();
		}

		[Test]
		public async Task Signal_AfterATimedOutWait_ShouldNotThrow()
		{
			Signaler<int> signaler = new();
			signaler.Wait(2.Times(), timeout: 10.Milliseconds());

			void Act()
				=> signaler.Signal(1);

			await That(Act).DoesNotThrow()
				.Because("nobody waits anymore, so the signal must not fail on the event of the ended wait");
		}

		[Test]
		public async Task Signal_AfterATimedOutWait_WhenThePredicateOfTheWaitThrows_ShouldNotThrow()
		{
			Signaler<int> signaler = new();
			signaler.Wait(2.Times(), _ => throw new InvalidOperationException("predicate failed"),
				10.Milliseconds());

			void Act()
				=> signaler.Signal(1);

			await That(Act).DoesNotThrow()
				.Because("nobody waits anymore, so the signal must not fail on the event of the ended wait");
		}

		[Test]
		public async Task Signal_AfterAWaitEnded_ShouldNotInvokeItsPredicate()
		{
			Signaler<int> signaler = new();
			int invocations = 0;
			signaler.Wait(_ =>
			{
				invocations++;
				return true;
			}, TimeSpan.Zero);

			signaler.Signal(1);

			await That(invocations).IsEqualTo(0)
				.Because("the predicate belongs to a wait that already ended");
		}

		[Test]
		public async Task Signal_WhenThePredicateOfTheWaitThrows_ShouldNotThrow()
		{
			Signaler<int> signaler = new();
			signaler.Wait(_ => throw new InvalidOperationException("predicate failed"), TimeSpan.Zero);

			void Act()
				=> signaler.Signal(1);

			await That(Act).DoesNotThrow()
				.Because("the signal is sent on the thread of the code under test, which must not receive the exception");
			await That(signaler.IsSignaled()).IsTrue();
		}

		[Test]
		public async Task WaitAsync_InfiniteTimeout_ShouldWaitForTheSignal()
		{
			Signaler<int> signaler = new();

			Task<SignalerResult<int>> wait = signaler.WaitAsync(timeout: Timeout.InfiniteTimeSpan);
			signaler.Signal(1);
			SignalerResult<int> result = await wait;

			await That(result.IsSuccess).IsTrue();
		}

		[Test]
		[Arguments(false)]
		[Arguments(true)]
		public async Task WaitAsync_NegativeTimeout_ShouldThrowArgumentOutOfRangeException(bool isAlreadySignaled)
		{
			Signaler<int> signaler = new();
			if (isAlreadySignaled)
			{
				signaler.Signal(1);
				signaler.Signal(2);
			}

			void Act()
				=> _ = signaler.WaitAsync(timeout: -2.Milliseconds());

			void ActWithAmount()
				=> _ = signaler.WaitAsync(2.Times(), timeout: -2.Milliseconds());

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("timeout").And
				.WithMessage("The timeout must not be negative.").AsPrefix()
				.Because("the timeout is validated before the wait starts, also when the signals were already received");
			await That(ActWithAmount).Throws<ArgumentOutOfRangeException>()
				.WithParamName("timeout").And
				.WithMessage("The timeout must not be negative.").AsPrefix()
				.Because("the timeout is validated before the wait starts, also when the signals were already received");
		}

		[Test]
		public async Task WaitAsync_ShouldCompleteAsSoonAsEnoughSignalsWereRecorded()
		{
			Signaler<int> signaler = new();
			signaler.Signal(1);

			Task<SignalerResult<int>> wait = signaler.WaitAsync(2.Times(), timeout: 10.Seconds());
			bool isCompletedBeforeTheSecondSignal = wait.IsCompleted;
			signaler.Signal(2);
			SignalerResult<int> result = await wait;

			await That(isCompletedBeforeTheSecondSignal).IsFalse();
			await That(result.IsSuccess).IsTrue();
			await That(result.Parameters).IsEqualTo([1, 2,])
				.Because("the signal before the wait counts as well");
		}

		[Test]
		public async Task WaitAsync_ShouldReturnAtTheCancellationWithoutThrowing()
		{
			Signaler<int> signaler = new();
			signaler.Signal(1);
			using CancellationTokenSource cts = new(30.Milliseconds());

			SignalerResult<int> result = await signaler.WaitAsync(2.Times(), timeout: 10.Seconds(),
				cancellationToken: cts.Token);

			await That(result.IsSuccess).IsFalse();
			await That(result.Parameters).IsEqualTo([1,])
				.Because("the signals received until the cancellation are returned");
		}

		[Test]
		public async Task WaitAsync_WithPredicate_ShouldOnlyCountMatchingSignals()
		{
			Signaler<int> signaler = new();
			signaler.Signal(1);
			signaler.Signal(2);

			Task<SignalerResult<int>> wait = signaler.WaitAsync(2.Times(), p => p > 1, 10.Seconds());
			bool isCompletedBeforeTheMatchingSignal = wait.IsCompleted;
			signaler.Signal(3);
			SignalerResult<int> result = await wait;

			await That(isCompletedBeforeTheMatchingSignal).IsFalse()
				.Because("only one of the signals before the wait matches the predicate");
			await That(result.IsSuccess).IsTrue();
		}

		[Test]
		public async Task WaitAsync_WithPredicate_WhenThePredicateThrowsWhileSignaling_ShouldThrowItWithoutWaiting()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();
			InvalidOperationException exception = new("predicate failed");
			Stopwatch sw = Stopwatch.StartNew();
			Task<SignalerResult<int>> wait = signaler.WaitAsync(2.Times(), _ => throw exception, timeout);
			signaler.Signal(1);

			async Task Act()
				=> await wait;

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("predicate failed");
			sw.Stop();
			await That(sw.Elapsed).IsLessThan(timeout)
				.Because("the exception ends the wait instead of letting it run into the timeout");
		}

		[Test]
		[Arguments(2)]
		[Arguments(3)]
		public async Task Wait_EnoughSignals_ShouldSucceed(int amount)
		{
			Signaler<int> signaler = new();

			signaler.Signal(4);
			signaler.Signal(5);
			signaler.Signal(6);

			SignalerResult<int> result = signaler.Wait(amount);

			await That(result.IsSuccess).IsTrue();
			await That(result.Parameters).IsEqualTo([4, 5, 6,]).InAnyOrder();
		}

		[Test]
		public async Task Wait_InfiniteTimeout_ShouldWaitForTheSignal()
		{
			Signaler<int> signaler = new();

			SignalerResult<int> result = await WhileWaiting(
				() => signaler.Wait(timeout: Timeout.InfiniteTimeSpan),
				() => signaler.Signal(1));

			await That(result.IsSuccess).IsTrue();
		}

		[Test]
		[Arguments(false)]
		[Arguments(true)]
		public async Task Wait_NegativeTimeout_ShouldThrowArgumentOutOfRangeException(bool isAlreadySignaled)
		{
			Signaler<int> signaler = new();
			if (isAlreadySignaled)
			{
				signaler.Signal(1);
				signaler.Signal(2);
			}

			void Act()
				=> signaler.Wait(timeout: -2.Milliseconds());

			void ActWithAmount()
				=> signaler.Wait(2.Times(), timeout: -2.Milliseconds());

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("timeout").And
				.WithMessage("The timeout must not be negative.").AsPrefix()
				.Because("the timeout is validated before the wait starts, also when the signals were already received");
			await That(ActWithAmount).Throws<ArgumentOutOfRangeException>()
				.WithParamName("timeout").And
				.WithMessage("The timeout must not be negative.").AsPrefix()
				.Because("the timeout is validated before the wait starts, also when the signals were already received");
		}

		[Test]
		public async Task Wait_ShouldCatchOperationCanceledException()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();
			using CancellationTokenSource cts = new();
			CancellationToken token = cts.Token;

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = await WhileWaiting(
				() => signaler.Wait(100.Times(), timeout: timeout, cancellationToken: token),
				() =>
				{
					Parallel.For(0, 99, signaler.Signal);
					// ReSharper disable once AccessToDisposedClosure
					cts.Cancel();
				});
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_ShouldReturnAsSoonAsEnoughSignalsWereRecorded()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();

			Stopwatch sw = new();
			sw.Start();
			SignalerResult<int> result = await WhileWaiting(
				() => signaler.Wait(100.Times(), timeout: timeout),
				() => Parallel.For(0, 100, signaler.Signal));
			sw.Stop();

			await That(result.IsSuccess).IsTrue();
			await That(result.Parameters).IsEqualTo(Enumerable.Range(0, 100)).InAnyOrder();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_ShouldUseTimeout()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Milliseconds();

			signaler.Signal(1);

			Stopwatch sw = new();
			sw.Start();
			SignalerResult<int> result = signaler.Wait(2.Times(), timeout: timeout);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(DefaultTimeout)
				.Because("the 10 ms timeout must end the wait long before the default signaler timeout of 30 s would");
		}

		[Test]
		public async Task Wait_Single_AfterATimedOutWait_ShouldSucceedWithTheNewSignal()
		{
			Signaler<int> signaler = new();
			signaler.Wait(timeout: 10.Milliseconds());
			signaler.Signal(1);

			SignalerResult<int> result = signaler.Wait(timeout: 5.Seconds());

			await That(result.IsSuccess).IsTrue()
				.Because("the new wait must not wait on the disposed event of the ended wait");
			await That(result.Parameters).IsEqualTo([1,]);
		}

		[Test]
		public async Task Wait_Single_AlreadySignaled_ShouldSucceed()
		{
			Signaler<int> signaler = new();

			signaler.Signal(4);
			signaler.Signal(5);
			signaler.Signal(6);

			SignalerResult<int> result = signaler.Wait();

			await That(result.IsSuccess).IsTrue();
			await That(result.Parameters).IsEqualTo([4, 5, 6,]).InAnyOrder();
		}

		[Test]
		public async Task Wait_Single_ShouldCatchOperationCanceledException()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();
			using CancellationTokenSource cts = new();
			CancellationToken token = cts.Token;

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = await WhileWaiting(
				() => signaler.Wait(timeout: timeout, cancellationToken: token),
				// ReSharper disable once AccessToDisposedClosure
				() => cts.Cancel());
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_Single_ShouldReturnAsSoonAsSignalWasRecorded()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = await WhileWaiting(
				() => signaler.Wait(timeout: timeout),
				() => signaler.Signal(10));
			sw.Stop();

			await That(result.IsSuccess).IsTrue();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_Single_ShouldUseTimeout()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Milliseconds();

			Stopwatch sw = new();
			sw.Start();
			SignalerResult<int> result = signaler.Wait(timeout: timeout);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(DefaultTimeout)
				.Because("the 10 ms timeout must end the wait long before the default signaler timeout of 30 s would");
		}

		[Test]
		public async Task Wait_Single_WhenTimeoutExceedsTheTimerRange_ShouldWaitForTheSignal()
		{
			Signaler<int> signaler = new();
			using CancellationTokenSource cts = new(5.Seconds());
			CancellationToken token = cts.Token;

			SignalerResult<int> result = await WhileWaiting(
				() => signaler.Wait(timeout: 60.Days(), cancellationToken: token),
				() => signaler.Signal(1));

			await That(result.IsSuccess).IsTrue()
				.Because("a timeout beyond the range of the wait handle must not throw");
			await That(result.Parameters).IsEqualTo([1,]);
		}

		[Test]
		public async Task Wait_WhenTimeoutExceedsTheTimerRange_ShouldWaitForTheSignals()
		{
			Signaler<int> signaler = new();
			using CancellationTokenSource cts = new(5.Seconds());
			CancellationToken token = cts.Token;

			signaler.Signal(1);

			SignalerResult<int> result = await WhileWaiting(
				() => signaler.Wait(2.Times(), timeout: 60.Days(), cancellationToken: token),
				() => signaler.Signal(2));

			await That(result.IsSuccess).IsTrue()
				.Because("a timeout beyond the range of the wait handle must not throw");
			await That(result.Parameters).IsEqualTo([1, 2,]);
		}

		[Test]
		public async Task Wait_WithPredicate_ShouldCatchOperationCanceledException()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();
			using CancellationTokenSource cts = new();
			CancellationToken token = cts.Token;

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = await WhileWaiting(
				() => signaler.Wait(100.Times(), x => x != 50, timeout, token),
				() =>
				{
					Parallel.For(0, 100, signaler.Signal);
					// ReSharper disable once AccessToDisposedClosure
					cts.Cancel();
				});
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_WithPredicate_ShouldReturnAsSoonAsEnoughSignalsWereRecorded()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();

			Stopwatch sw = new();
			sw.Start();
			SignalerResult<int> result = await WhileWaiting(
				() => signaler.Wait(100.Times(), x => x >= 10, timeout),
				() => Parallel.For(0, 110, signaler.Signal));
			sw.Stop();

			await That(result.IsSuccess).IsTrue();
			await That(result.Parameters).Contains(Enumerable.Range(10, 100)).InAnyOrder();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_WithPredicate_Single_ShouldCatchOperationCanceledException()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();
			using CancellationTokenSource cts = new();
			CancellationToken token = cts.Token;

			signaler.Signal(50);

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = await WhileWaiting(
				() => signaler.Wait(x => x != 50, timeout, token),
				// ReSharper disable once AccessToDisposedClosure
				() => cts.Cancel());
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_WithPredicate_Single_ShouldReturnAsSoonAsSignalWasRecorded()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();
			Stopwatch sw = new();

			SignalerResult result = await WhileWaiting(
				() =>
				{
					sw.Start();
					SignalerResult<int> waitResult = signaler.Wait(x => x > 10, timeout);
					sw.Stop();
					return waitResult;
				},
				() =>
				{
					Thread.Sleep(20.Milliseconds());
					signaler.Signal(10);
					signaler.Signal(11);
				});

			await That(result.IsSuccess).IsTrue();
			await That(sw.Elapsed).IsLessThan(timeout)
				.And.IsGreaterThanOrEqualTo(10.Milliseconds());
		}

		[Test]
		public async Task Wait_WithPredicate_Single_WhenThePredicateSignals_ShouldCountTheNewSignal()
		{
			Signaler<int> signaler = new();
			signaler.Signal(1);

			SignalerResult<int> result = signaler.Wait(x =>
			{
				if (x == 1)
				{
					signaler.Signal(2);
				}

				return x == 2;
			}, 5.Seconds());

			await That(result.IsSuccess).IsTrue()
				.Because("a signal while the recorded parameters are evaluated must not corrupt the evaluation");
			await That(result.Parameters).IsEqualTo([1, 2,]);
		}

		[Test]
		public async Task Wait_WithPredicate_Single_WhenThePredicateThrowsWhileSignaling_ShouldThrowItWithoutWaiting()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();
			InvalidOperationException exception = new("predicate failed");

			Stopwatch sw = new();
			sw.Start();

			async Task Act()
				=> await WhileWaiting(() => signaler.Wait(_ => throw exception, timeout), () => signaler.Signal(1));

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("predicate failed");
			sw.Stop();
			await That(sw.Elapsed).IsLessThan(timeout)
				.Because("the exception ends the wait instead of letting it run into the timeout");
		}

		[Test]
		public async Task Wait_WithPredicate_WhenAnotherWaitIsPending_ShouldKeepItsOwnPredicate()
		{
			Signaler<int> signaler = new();
			SignalerResult<int>? result = null;
			signaler.Signal(0);

			SignalerResult<int> pendingResult = await WhileWaiting(
				() => signaler.Wait(2.Times(), x => x < 10, 5.Seconds()),
				() =>
				{
					result = signaler.Wait(x => x == 10, TimeSpan.Zero);
					signaler.Signal(1);
				});

			await That(result?.IsSuccess).IsFalse();
			await That(pendingResult.IsSuccess).IsTrue()
				.Because("the other wait must not replace the predicate of the pending wait");
			await That(pendingResult.Parameters).IsEqualTo([0, 1,]);
		}

		[Test]
		public async Task Wait_WithPredicate_WhenThePredicateThrowsWhileSignaling_ShouldThrowItWithoutWaiting()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();
			InvalidOperationException exception = new("predicate failed");

			Stopwatch sw = new();
			sw.Start();

			async Task Act()
				=> await WhileWaiting(
					() => signaler.Wait(2.Times(), _ => throw exception, timeout),
					() => signaler.Signal(1));

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("predicate failed");
			sw.Stop();
			await That(sw.Elapsed).IsLessThan(timeout)
				.Because("the exception ends the wait instead of letting it run into the timeout");
		}

		[Test]
		[Arguments(0)]
		[Arguments(-1)]
		public async Task Wait_ZeroOrNegativeAmount_ShouldThrowArgumentOutOfRangeException(int amount)
		{
			Signaler<int> signaler = new();

			void Act()
				=> signaler.Wait(amount);

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithMessage("The amount must be greater than zero*").AsWildcard().And
				.WithParamName("amount");
		}
	}
}
