#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using aweXpect.Customization;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class CancellationTests
	{
		[Fact]
		public async Task WhenAbandonedSourceFaultsLater_ShouldNotRaiseUnobservedTaskException()
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
				await AbandonSourceThatFaultsLater(exception);
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
			finally
			{
				TaskScheduler.UnobservedTaskException -= handler;
			}

			await That(isRaised).IsFalse()
				.Because("the exception of an abandoned source must be observed, as nobody else awaits it");
		}

		[Fact]
		public async Task WhenCancellationIsRequestedBetweenItems_ShouldAbortANegatedExpectation()
		{
			IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);
			using CancellationTokenSource cts = new();

			async Task Act()
				=> await That(subject).DoesNotContain(item => Cancel(cts, item == 3)).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item matching item => Cancel(cts, item == 3),
				             but it could not be verified, because it was already canceled
				             """)
				.Because("a cancellation between two items must not be mistaken for the end of the source");
		}

		[Fact]
		public async Task WhenCancellationIsRequestedBetweenItems_ShouldAbortContains()
		{
			IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);
			using CancellationTokenSource cts = new();

			async Task Act()
				=> await That(subject).Contains(item => Cancel(cts, item == 3)).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             contains an item matching item => Cancel(cts, item == 3) at least once,
				             but it could not be verified, because it was already canceled
				             """)
				.Because("a cancellation between two items must not be reported as a missing item");
		}

		[Fact]
		public async Task WhenCancellationIsRequestedBetweenItems_ShouldAbortIsEqualTo()
		{
			IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);
			using CancellationTokenSource cts = new();

			async Task Act()
				=> await That(subject).IsEqualTo([1, 2, 3]).Using(new CancellingComparer(cts))
					.WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [1, 2, 3] using ThatAsyncEnumerable.CancellationTests.CancellingComparer in order,
				             but it could not be verified, because it was already canceled

				             Expected:
				             [1, 2, 3]
				             """)
				.Because("a cancellation between two items must not be reported as missing items");
		}

		[Fact]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldReportACountAsNotVerified()
		{
			IAsyncEnumerable<int> subject = HangAfter(1, 2);
			using CancellationTokenSource cts = new();
			cts.CancelAfter(50.Milliseconds());

			async Task Act()
				=> await That(subject).HasCount(3).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 3 items,
				             but it could not be verified, because it was already canceled

				             Collection:
				             [1, 2, (… and maybe more)]
				             """);
		}

		[Fact]
		public async Task WhenCancellationIsRequestedWhileTheSourceHangs_ShouldStopWaiting()
		{
			IAsyncEnumerable<int> subject = HangAfter();
			using CancellationTokenSource cts = new();
			cts.CancelAfter(50.Milliseconds());

			async Task Act()
				=> await That(subject).Contains(1).WithCancellation(cts.Token);

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 1 at least once,
				             but it could not be verified, because it was already canceled
				             """)
				.Because("a requested cancellation aborts the evaluation, even if the source ignores it");
		}

		[Fact]
		public async Task WhenChainedExpectationsAreMet_ShouldEnumerateTheSourceOnce()
		{
			int enumerations = 0;

			async IAsyncEnumerable<int> Numbers()
			{
				enumerations++;
				foreach (int item in new[] { 1, 2, 3, })
				{
					await Task.Yield();
					yield return item;
				}
			}

			IAsyncEnumerable<int> subject = Numbers();

			async Task Act()
				=> await That(subject).Contains(1).And.Contains(3).And.HasCount(3).WithTimeout(30.Seconds());

			await That(Act).DoesNotThrow();
			await That(enumerations).IsEqualTo(1)
				.Because("the chained expectations continue the materialized source instead of enumerating it again");
		}

		[Fact]
		public async Task WhenExpectedItemPrecedesAHang_ShouldSucceed()
		{
			IAsyncEnumerable<int> subject = HangAfter(1);

			async Task Act()
				=> await That(subject).Contains(1).WithTimeout(30.Seconds());

			await That(Act).DoesNotThrow()
				.Because("the source is only enumerated as far as necessary");
		}

		[Fact]
		public async Task WhenGlobalTimeoutElapsesWhileTheSourceHangs_ShouldFail()
		{
			IAsyncEnumerable<int> subject = HangAfter();

			async Task Act()
			{
				using IDisposable _ = Customize.aweXpect.Settings().TestCancellation
					.Set(TestCancellation.FromTimeout(50.Milliseconds()));
				await That(subject).Contains(1);
			}

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 1 at least once,
				             but it did not finish within 0:00.050
				             """);
		}

		[Fact]
		public async Task WhenSourceHangsAfterSomeItems_ShouldFailAfterTheTimeout()
		{
			IAsyncEnumerable<int> subject = HangAfter(1, 2, 3);

			async Task Act()
				=> await That(subject).Contains(4).WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 4 at least once,
				             but it did not finish within 0:00.050
				             """).And
				.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."));
		}

		[Fact]
		public async Task WhenSourceHangsBeforeTheCollectionIsFormatted_ShouldFailAfterTheTimeout()
		{
			IAsyncEnumerable<int> subject = HangAfter(1, 2);

			async Task Act()
				=> await That(subject).IsInAscendingOrder().WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             is in ascending order,
				             but it did not finish within 0:00.050

				             Collection:
				             [1, 2, (… and maybe more)]
				             """);
		}

		[Fact]
		public async Task WhenSourceIgnoresTheToken_ShouldFailAfterTheTimeout()
		{
			IAsyncEnumerable<int> subject = HangAfter();

			async Task Act()
				=> await That(subject).Contains(1).WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 1 at least once,
				             but it did not finish within 0:00.050
				             """);
		}

		[Fact]
		public async Task WhenSourceIgnoresTheToken_ShouldFailANegatedExpectationAfterTheTimeout()
		{
			IAsyncEnumerable<int> subject = HangAfter();

			async Task Act()
				=> await That(subject).DoesNotContain(1).WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item equal to 1,
				             but it did not finish within 0:00.050
				             """)
				.Because("a timeout must not be mistaken for the end of the source");
		}

		[Fact]
		public async Task WhenSourceObservesTheToken_ShouldCancelTheSource()
		{
			CancellationToken sourceToken = CancellationToken.None;

			async IAsyncEnumerable<int> Numbers([EnumeratorCancellation] CancellationToken cancellationToken = default)
			{
				sourceToken = cancellationToken;
				await Task.Delay(30.Seconds(), cancellationToken);
				yield return 1;
			}

			IAsyncEnumerable<int> subject = Numbers();

			async Task Act()
				=> await That(subject).Contains(1).WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 1 at least once,
				             but it did not finish within 0:00.050
				             """);
			await That(sourceToken.IsCancellationRequested).IsTrue()
				.Because("the token of the evaluation must reach the source");
		}

		[Fact]
		public async Task WhenTimeoutElapsesBetweenItems_ShouldFailANegatedExpectation()
		{
			CancellationToken sourceToken = CancellationToken.None;

			async IAsyncEnumerable<int> Numbers([EnumeratorCancellation] CancellationToken cancellationToken = default)
			{
				sourceToken = cancellationToken;
				await Task.Yield();
				yield return 1;
				await Task.Yield();
				yield return 2;
			}

			IAsyncEnumerable<int> subject = Numbers();

			async Task Act()
				=> await That(subject).DoesNotContain(item => BlockUntilCancelled(item == 2, sourceToken))
					.WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item matching item => BlockUntilCancelled(item == 2, sourceToken),
				             but it did not finish within 0:00.050
				             """)
				.Because("a timeout between two items must not be mistaken for the end of the source");
		}

		[Fact]
		public async Task WhenTimeoutElapsesWhileTheSourceHangs_ShouldFailACount()
		{
			IAsyncEnumerable<int> subject = HangAfter(1, 2);

			async Task Act()
				=> await That(subject).HasCount(3).WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 3 items,
				             but it did not finish within 0:00.050

				             Collection:
				             [1, 2, (… and maybe more)]
				             """).And
				.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
				.Because("a timeout is reported the same way, whichever expectation was pending");
		}

		/// <remarks>
		///     The source is only reachable from within this method, so that it can be collected afterwards.
		/// </remarks>
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static async Task AbandonSourceThatFaultsLater(Exception exception)
		{
			TaskCompletionSource tcs = new();

			async IAsyncEnumerable<int> Numbers()
			{
				await tcs.Task;
				yield return 1;
			}

			IAsyncEnumerable<int> subject = Numbers();

			async Task Act()
				=> await That(subject).Contains(1).WithTimeout(50.Milliseconds());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 1 at least once,
				             but it did not finish within 0:00.050
				             """);
			tcs.SetException(exception);
		}

		/// <remarks>
		///     Blocks the evaluation between two items until it is cancelled, bounded by half a minute, so that a
		///     regression fails the test instead of hanging the test run.
		/// </remarks>
		private static bool BlockUntilCancelled(bool result, CancellationToken cancellationToken)
		{
			cancellationToken.WaitHandle.WaitOne(30.Seconds());
			return result;
		}

		private static bool Cancel(CancellationTokenSource cts, bool result)
		{
			cts.Cancel();
			return result;
		}

		/// <remarks>
		///     The items are provided synchronously, so that they are received before a short timeout elapses, even on
		///     a busy machine. The hang ends after half a minute, so that a regression which waits for it fails the test
		///     instead of hanging the test run.
		/// </remarks>
		private static async IAsyncEnumerable<int> HangAfter(params int[] items)
		{
			foreach (int item in items)
			{
				yield return item;
			}

			await Task.Delay(30.Seconds());
		}

		private sealed class CancellingComparer(CancellationTokenSource cts) : IEqualityComparer<object>
		{
			public new bool Equals(object? x, object? y) => Cancel(cts, object.Equals(x, y));

			public int GetHashCode(object obj) => obj.GetHashCode();
		}
	}
}
#endif
