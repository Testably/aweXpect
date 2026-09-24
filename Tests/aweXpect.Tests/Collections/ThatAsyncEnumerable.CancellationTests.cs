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

			await That(Act).Throws<InvalidOperationException>()
				.WithMessage(
					$"Error evaluating ThatAsyncEnumerable.AsyncContainConstraint<int> constraint with value *: {new TaskCanceledException().Message}")
				.AsWildcard().And
				.WithInner<OperationCanceledException>()
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
		public async Task WhenTimeoutElapsesWhileTheSourceHangs_ShouldReportACountAsNotVerified()
		{
			IAsyncEnumerable<int> subject = HangAfter(1, 2);

			async Task Act()
				=> await That(subject).HasCount(3).WithTimeout(50.Milliseconds());

			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that subject
				             has exactly 3 items,
				             but it could not be verified, because it was already canceled

				             Collection:
				             [1, 2, (… and maybe more)]
				             """)
				.Because("an expectation that reports a cancellation as not verified does so for a pending item as well");
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
		///     The hang ends after half a minute, so that a regression which waits for it fails the test instead of
		///     hanging the test run.
		/// </remarks>
		private static async IAsyncEnumerable<int> HangAfter(params int[] items)
		{
			foreach (int item in items)
			{
				await Task.Yield();
				yield return item;
			}

			await Task.Delay(30.Seconds());
		}
	}
}
#endif
