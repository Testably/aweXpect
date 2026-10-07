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
			_ = Task.Run(async () =>
			{
				await Task.Delay(50.Milliseconds());
				signaler.Signal();
			});
			SignalerResult result = signaler.Wait(2.Times(), 5.Seconds());

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
			_ = Task.Run(async () =>
			{
				await Task.Delay(50.Milliseconds());
				signaler.Signal();
			});

			SignalerResult result = signaler.Wait(Timeout.InfiniteTimeSpan);

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
			using CancellationTokenSource cts = new(30.Milliseconds());
			CancellationToken token = cts.Token;

			for (int i = 0; i < 99; i++)
			{
				_ = Task.Run(() => signaler.Signal(), token);
			}

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(100.Times(), timeout, token);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_ShouldReturnAsSoonAsEnoughSignalsWereRecorded()
		{
			Signaler signaler = new();
			TimeSpan timeout = 10.Seconds();

			for (int i = 0; i < 100; i++)
			{
				_ = Task.Run(() => signaler.Signal());
			}

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(100.Times(), timeout);
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
			using CancellationTokenSource cts = new(30.Milliseconds());
			CancellationToken token = cts.Token;

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(timeout, token);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_Single_ShouldReturnAsSoonAsSignalWasRecorded()
		{
			Signaler signaler = new();
			TimeSpan timeout = 10.Seconds();
			using ManualResetEventSlim ms = new();

			_ = Task.Run(async () =>
			{
				for (int i = 10; i < 1000; i++)
				{
					// ReSharper disable once AccessToDisposedClosure
					if (ms.IsSet)
					{
						break;
					}

					await Task.Delay(i.Milliseconds());
					signaler.Signal();
				}
			});

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(timeout);
			sw.Stop();

			ms.Set();
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

			_ = Task.Run(async () =>
			{
				await Task.Delay(50.Milliseconds());
				signaler.Signal();
			});

			SignalerResult result = signaler.Wait(60.Days(), cts.Token);

			await That(result.IsSuccess).IsTrue()
				.Because("a timeout beyond the range of the wait handle must not throw");
		}

		[Test]
		public async Task Wait_WhenAnotherWaitIsPending_ShouldNotInterfereWithIt()
		{
			Signaler signaler = new();
			SignalerResult? pendingResult = null;
			Thread pendingWait = new(() => pendingResult = signaler.Wait(2.Times(), 5.Seconds()));
			pendingWait.Start();
			while ((pendingWait.ThreadState & ThreadState.WaitSleepJoin) == 0)
			{
				await Task.Delay(1.Milliseconds());
			}

			signaler.Signal();
			SignalerResult result = signaler.Wait(2.Times(), TimeSpan.Zero);
			signaler.Signal();
			pendingWait.Join();

			await That(result.IsSuccess).IsFalse();
			await That(pendingResult?.IsSuccess).IsTrue()
				.Because("the other wait must neither replace nor remove the event of the pending wait");
			await That(pendingResult?.Count).IsEqualTo(2);
		}

		[Test]
		public async Task Wait_WhenTimeoutExceedsTheTimerRange_ShouldWaitForTheSignals()
		{
			Signaler signaler = new();
			using CancellationTokenSource cts = new(5.Seconds());

			signaler.Signal();
			_ = Task.Run(async () =>
			{
				await Task.Delay(50.Milliseconds());
				signaler.Signal();
			});

			SignalerResult result = signaler.Wait(2.Times(), 60.Days(), cts.Token);

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
			_ = Task.Run(async () =>
			{
				await Task.Delay(50.Milliseconds());
				signaler.Signal(2);
			});
			SignalerResult<int> result = signaler.Wait(2.Times(), timeout: 5.Seconds());

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
			_ = Task.Run(async () =>
			{
				await Task.Delay(50.Milliseconds());
				signaler.Signal(1);
			});
			Stopwatch sw = Stopwatch.StartNew();

			async Task Act()
				=> await signaler.WaitAsync(2.Times(), _ => throw exception, timeout);

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
			_ = Task.Run(async () =>
			{
				await Task.Delay(50.Milliseconds());
				signaler.Signal(1);
			});

			SignalerResult<int> result = signaler.Wait(timeout: Timeout.InfiniteTimeSpan);

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
			using CancellationTokenSource cts = new(30.Milliseconds());
			CancellationToken token = cts.Token;

			for (int i = 0; i < 99; i++)
			{
				int value = i;
				_ = Task.Run(() => signaler.Signal(value), token);
			}

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(100.Times(), timeout: timeout, cancellationToken: token);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_ShouldReturnAsSoonAsEnoughSignalsWereRecorded()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();

			for (int i = 0; i < 100; i++)
			{
				int value = i;
				_ = Task.Run(() => signaler.Signal(value));
			}

			Stopwatch sw = new();
			sw.Start();
			SignalerResult<int> result = signaler.Wait(100.Times(), timeout: timeout);
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
			using CancellationTokenSource cts = new(30.Milliseconds());
			CancellationToken token = cts.Token;

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(timeout: timeout, cancellationToken: token);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_Single_ShouldReturnAsSoonAsSignalWasRecorded()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();
			using ManualResetEventSlim ms = new();

			_ = Task.Run(async () =>
			{
				for (int i = 10; i < 1000; i++)
				{
					// ReSharper disable once AccessToDisposedClosure
					if (ms.IsSet)
					{
						break;
					}

					int value = i;
					await Task.Delay(i.Milliseconds());
					signaler.Signal(value);
				}
			});

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(timeout: timeout);
			sw.Stop();

			ms.Set();
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

			_ = Task.Run(async () =>
			{
				await Task.Delay(50.Milliseconds());
				signaler.Signal(1);
			});

			SignalerResult<int> result = signaler.Wait(timeout: 60.Days(), cancellationToken: cts.Token);

			await That(result.IsSuccess).IsTrue()
				.Because("a timeout beyond the range of the wait handle must not throw");
			await That(result.Parameters).IsEqualTo([1,]);
		}

		[Test]
		public async Task Wait_WhenTimeoutExceedsTheTimerRange_ShouldWaitForTheSignals()
		{
			Signaler<int> signaler = new();
			using CancellationTokenSource cts = new(5.Seconds());

			signaler.Signal(1);
			_ = Task.Run(async () =>
			{
				await Task.Delay(50.Milliseconds());
				signaler.Signal(2);
			});

			SignalerResult<int> result = signaler.Wait(2.Times(), timeout: 60.Days(), cancellationToken: cts.Token);

			await That(result.IsSuccess).IsTrue()
				.Because("a timeout beyond the range of the wait handle must not throw");
			await That(result.Parameters).IsEqualTo([1, 2,]);
		}

		[Test]
		public async Task Wait_WithPredicate_ShouldCatchOperationCanceledException()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();
			using CancellationTokenSource cts = new(30.Milliseconds());
			CancellationToken token = cts.Token;

			for (int i = 0; i < 100; i++)
			{
				int value = i;
				_ = Task.Run(() => signaler.Signal(value), token);
			}

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(100.Times(), x => x != 50, timeout, token);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_WithPredicate_ShouldReturnAsSoonAsEnoughSignalsWereRecorded()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();

			for (int i = 0; i < 110; i++)
			{
				int value = i;
				_ = Task.Run(() => signaler.Signal(value));
			}

			Stopwatch sw = new();
			sw.Start();
			SignalerResult<int> result = signaler.Wait(100.Times(), x => x >= 10, timeout);
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
			using CancellationTokenSource cts = new(30.Milliseconds());
			CancellationToken token = cts.Token;

			signaler.Signal(50);

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(x => x != 50, timeout, token);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(timeout);
		}

		[Test]
		public async Task Wait_WithPredicate_Single_ShouldReturnAsSoonAsSignalWasRecorded()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Seconds();
			using ManualResetEventSlim ms = new();

			_ = Task.Run(async () =>
			{
				for (int i = 10; i < 1000; i++)
				{
					// ReSharper disable once AccessToDisposedClosure
					if (ms.IsSet)
					{
						break;
					}

					int value = i;
					await Task.Delay(i.Milliseconds());
					signaler.Signal(value);
				}
			});

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(x => x > 10, timeout);
			sw.Stop();

			ms.Set();
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
			_ = Task.Run(async () =>
			{
				await Task.Delay(50.Milliseconds());
				signaler.Signal(1);
			});

			Stopwatch sw = new();
			sw.Start();

			void Act()
				=> signaler.Wait(_ => throw exception, timeout);

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
			using ManualResetEventSlim isWaiting = new();
			signaler.Signal(0);
			Task<SignalerResult<int>> pendingWait = Task.Run(() => signaler.Wait(2.Times(), x =>
			{
				// ReSharper disable once AccessToDisposedClosure
				isWaiting.Set();
				return x < 10;
			}, 5.Seconds()));
			isWaiting.Wait(5.Seconds());

			SignalerResult<int> result = signaler.Wait(x => x == 10, TimeSpan.Zero);
			signaler.Signal(1);
			SignalerResult<int> pendingResult = await pendingWait;

			await That(result.IsSuccess).IsFalse();
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
			_ = Task.Run(async () =>
			{
				await Task.Delay(50.Milliseconds());
				signaler.Signal(1);
			});

			Stopwatch sw = new();
			sw.Start();

			void Act()
				=> signaler.Wait(2.Times(), _ => throw exception, timeout);

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
