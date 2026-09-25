using System.Diagnostics;
using System.Linq;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Signaling;

namespace aweXpect.Core.Tests.Signaling;

public sealed class SignalerTests
{
	public sealed class Tests
	{
		[Theory]
		[InlineData(0, 0, true)]
		[InlineData(0, 1, false)]
		[InlineData(2, 0, true)]
		[InlineData(2, 1, true)]
		[InlineData(2, 2, true)]
		[InlineData(2, 3, false)]
		[InlineData(2, 5, false)]
		[InlineData(0, null, false)]
		[InlineData(1, null, true)]
		[InlineData(2, null, true)]
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

		[Theory]
		[InlineData(2)]
		[InlineData(3)]
		public async Task Wait_EnoughSignals_ShouldSucceed(int amount)
		{
			Signaler signaler = new();

			signaler.Signal();
			signaler.Signal();
			signaler.Signal();

			SignalerResult result = signaler.Wait(amount);

			await That(result.IsSuccess).IsTrue();
		}

		[Fact]
		public async Task Wait_ShouldCatchOperationCanceledException()
		{
			Signaler signaler = new();
			using CancellationTokenSource cts = new(30.Milliseconds());
			CancellationToken token = cts.Token;

			for (int i = 0; i < 99; i++)
			{
				_ = Task.Run(() => signaler.Signal(), token);
			}

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(100.Times(), 10.Seconds(), token);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds());
		}

		[Fact]
		public async Task Wait_ShouldReturnAsSoonAsEnoughSignalsWereRecorded()
		{
			Signaler signaler = new();

			for (int i = 0; i < 100; i++)
			{
				_ = Task.Run(() => signaler.Signal());
			}

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(100.Times(), 10.Seconds());
			sw.Stop();

			await That(result.IsSuccess).IsTrue();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds());
		}

		[Fact]
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
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds())
				.Because("the 10 ms timeout must end the wait long before the default signaler timeout of 30 s would");
		}

		[Fact]
		public async Task Wait_Single_AlreadySignaled_ShouldSucceed()
		{
			Signaler signaler = new();

			signaler.Signal();
			signaler.Signal();

			SignalerResult result = signaler.Wait();

			await That(result.IsSuccess).IsTrue();
		}

		[Fact]
		public async Task Wait_Single_ShouldCatchOperationCanceledException()
		{
			Signaler signaler = new();
			using CancellationTokenSource cts = new(30.Milliseconds());
			CancellationToken token = cts.Token;

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(10.Seconds(), token);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds());
		}

		[Fact]
		public async Task Wait_Single_ShouldReturnAsSoonAsSignalWasRecorded()
		{
			Signaler signaler = new();
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
			SignalerResult result = signaler.Wait(10.Seconds());
			sw.Stop();

			ms.Set();
			await That(result.IsSuccess).IsTrue();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds());
		}

		[Fact]
		public async Task Wait_Single_ShouldUseTimeout()
		{
			Signaler signaler = new();
			TimeSpan timeout = 10.Milliseconds();

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(timeout);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds())
				.Because("the 10 ms timeout must end the wait long before the default signaler timeout of 30 s would");
		}

		[Fact]
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

		[Fact]
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

		[Theory]
		[InlineData(0)]
		[InlineData(-1)]
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
		[Theory]
		[InlineData(0, 0, true)]
		[InlineData(0, 1, false)]
		[InlineData(2, 0, true)]
		[InlineData(2, 1, true)]
		[InlineData(2, 2, true)]
		[InlineData(2, 3, false)]
		[InlineData(2, 5, false)]
		[InlineData(0, null, false)]
		[InlineData(1, null, true)]
		[InlineData(2, null, true)]
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

		[Fact]
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

		[Fact]
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

		[Theory]
		[InlineData(2)]
		[InlineData(3)]
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

		[Fact]
		public async Task Wait_ShouldCatchOperationCanceledException()
		{
			Signaler<int> signaler = new();
			using CancellationTokenSource cts = new(30.Milliseconds());
			CancellationToken token = cts.Token;

			for (int i = 0; i < 99; i++)
			{
				int value = i;
				_ = Task.Run(() => signaler.Signal(value), token);
			}

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(100.Times(), timeout: 10.Seconds(), cancellationToken: token);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds());
		}

		[Fact]
		public async Task Wait_ShouldReturnAsSoonAsEnoughSignalsWereRecorded()
		{
			Signaler<int> signaler = new();

			for (int i = 0; i < 100; i++)
			{
				int value = i;
				_ = Task.Run(() => signaler.Signal(value));
			}

			Stopwatch sw = new();
			sw.Start();
			SignalerResult<int> result = signaler.Wait(100.Times(), timeout: 10.Seconds());
			sw.Stop();

			await That(result.IsSuccess).IsTrue();
			await That(result.Parameters).IsEqualTo(Enumerable.Range(0, 100)).InAnyOrder();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds());
		}

		[Fact]
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
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds())
				.Because("the 10 ms timeout must end the wait long before the default signaler timeout of 30 s would");
		}

		[Fact]
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

		[Fact]
		public async Task Wait_Single_ShouldCatchOperationCanceledException()
		{
			Signaler<int> signaler = new();
			using CancellationTokenSource cts = new(30.Milliseconds());
			CancellationToken token = cts.Token;

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(timeout: 10.Seconds(), cancellationToken: token);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds());
		}

		[Fact]
		public async Task Wait_Single_ShouldReturnAsSoonAsSignalWasRecorded()
		{
			Signaler<int> signaler = new();
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
			SignalerResult result = signaler.Wait(timeout: 10.Seconds());
			sw.Stop();

			ms.Set();
			await That(result.IsSuccess).IsTrue();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds());
		}

		[Fact]
		public async Task Wait_Single_ShouldUseTimeout()
		{
			Signaler<int> signaler = new();
			TimeSpan timeout = 10.Milliseconds();

			Stopwatch sw = new();
			sw.Start();
			SignalerResult<int> result = signaler.Wait(timeout: timeout);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds())
				.Because("the 10 ms timeout must end the wait long before the default signaler timeout of 30 s would");
		}

		[Fact]
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

		[Fact]
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

		[Fact]
		public async Task Wait_WithPredicate_ShouldCatchOperationCanceledException()
		{
			Signaler<int> signaler = new();
			using CancellationTokenSource cts = new(30.Milliseconds());
			CancellationToken token = cts.Token;

			for (int i = 0; i < 100; i++)
			{
				int value = i;
				_ = Task.Run(() => signaler.Signal(value), token);
			}

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(100.Times(), x => x != 50, 10.Seconds(), token);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds());
		}

		[Fact]
		public async Task Wait_WithPredicate_ShouldReturnAsSoonAsEnoughSignalsWereRecorded()
		{
			Signaler<int> signaler = new();

			for (int i = 0; i < 110; i++)
			{
				int value = i;
				_ = Task.Run(() => signaler.Signal(value));
			}

			Stopwatch sw = new();
			sw.Start();
			SignalerResult<int> result = signaler.Wait(100.Times(), x => x >= 10, 10.Seconds());
			sw.Stop();

			await That(result.IsSuccess).IsTrue();
			await That(result.Parameters).Contains(Enumerable.Range(10, 100)).InAnyOrder();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds());
		}

		[Fact]
		public async Task Wait_WithPredicate_Single_ShouldCatchOperationCanceledException()
		{
			Signaler<int> signaler = new();
			using CancellationTokenSource cts = new(30.Milliseconds());
			CancellationToken token = cts.Token;

			signaler.Signal(50);

			Stopwatch sw = new();
			sw.Start();
			SignalerResult result = signaler.Wait(x => x != 50, 10.Seconds(), token);
			sw.Stop();

			await That(result.IsSuccess).IsFalse();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds());
		}

		[Fact]
		public async Task Wait_WithPredicate_Single_ShouldReturnAsSoonAsSignalWasRecorded()
		{
			Signaler<int> signaler = new();
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
			SignalerResult result = signaler.Wait(x => x > 10, 10.Seconds());
			sw.Stop();

			ms.Set();
			await That(result.IsSuccess).IsTrue();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds())
				.And.IsGreaterThanOrEqualTo(10.Milliseconds());
		}

		[Fact]
		public async Task Wait_WithPredicate_Single_WhenThePredicateThrowsWhileSignaling_ShouldThrowItWithoutWaiting()
		{
			Signaler<int> signaler = new();
			InvalidOperationException exception = new("predicate failed");
			_ = Task.Run(async () =>
			{
				await Task.Delay(50.Milliseconds());
				signaler.Signal(1);
			});

			Stopwatch sw = new();
			sw.Start();

			void Act()
				=> signaler.Wait(_ => throw exception, 10.Seconds());

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("predicate failed");
			sw.Stop();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds())
				.Because("the exception ends the wait instead of letting it run into the timeout");
		}

		[Fact]
		public async Task Wait_WithPredicate_WhenThePredicateThrowsWhileSignaling_ShouldThrowItWithoutWaiting()
		{
			Signaler<int> signaler = new();
			InvalidOperationException exception = new("predicate failed");
			_ = Task.Run(async () =>
			{
				await Task.Delay(50.Milliseconds());
				signaler.Signal(1);
			});

			Stopwatch sw = new();
			sw.Start();

			void Act()
				=> signaler.Wait(2.Times(), _ => throw exception, 10.Seconds());

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage("predicate failed");
			sw.Stop();
			await That(sw.Elapsed).IsLessThan(5000.Milliseconds())
				.Because("the exception ends the wait instead of letting it run into the timeout");
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-1)]
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
