using System.Threading;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Signaling;

// ReSharper disable MethodHasAsyncOverload

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class Signaled
	{
		public sealed class WithinTests
		{
			[Test]
			public async Task WhenNotTriggeredWithinTheGivenTimeout_ShouldFail()
			{
				Signaler signaler = new();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(5.Seconds(), token)
					.ContinueWith(_ => signaler.Signal(), token);

				async Task Act() =>
					await That(signaler).Signaled().Within(40.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:00.040,
					             but it was never recorded within 0:*
					             """).AsWildcard();
				cts.Cancel();
			}

			[Test]
			public async Task WhenNotTriggeredWithParameterWithinTheGivenTimeout_ShouldFail()
			{
				Signaler<string> signaler = new();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(5.Seconds(), token)
					.ContinueWith(_ => signaler.Signal("foo"), token);

				async Task Act() =>
					await That(signaler).Signaled().Within(40.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:00.040,
					             but it was never recorded within 0:*
					             """).AsWildcard();
				cts.Cancel();
			}

			[Test]
			public async Task WhenTheOuterTimeoutIsAsLong_ShouldDecideByTheSignalsWithinTheTimeout()
			{
				Signaler signaler = new();
				signaler.Signal();

				async Task Act() =>
					await That(signaler).Signaled().AtMost(1.Times()).Within(200.Milliseconds())
						.WithTimeout(200.Milliseconds());

				await That(Act).DoesNotThrow()
					.Because("an outer timeout that is not shorter than Within must not decide the outcome");
			}

			[Test]
			public async Task WhenTheOuterTimeoutIsAsLong_WhenNotTriggered_ShouldFailWithTheSignalsWithinTheTimeout()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).Signaled().Within(200.Milliseconds()).WithTimeout(200.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:00.200,
					             but it was never recorded within 0:*
					             """).AsWildcard()
					.Because("an outer timeout that is not shorter than Within must not decide the outcome");
			}

			[Test]
			public async Task WhenTheOuterTimeoutIsAsLong_WithParameter_ShouldDecideByTheSignalsWithinTheTimeout()
			{
				Signaler<string> signaler = new();
				signaler.Signal("foo");

				async Task Act() =>
					await That(signaler).Signaled().AtMost(1.Times()).Within(200.Milliseconds())
						.WithTimeout(200.Milliseconds());

				await That(Act).DoesNotThrow()
					.Because("an outer timeout that is not shorter than Within must not decide the outcome");
			}

			[Test]
			public async Task
				WhenTheTestCancellationTimeoutIsShorterAndWithTimeoutIsLonger_ShouldFailWithTheTestCancellationTimeout()
			{
				Signaler signaler = new();
				Exception? exception;
				using (IDisposable _ = Customize.aweXpect.Settings().TestCancellation
					       .Set(TestCancellation.FromTimeout(300.Milliseconds())))
				{
					async Task Act() =>
						await That(signaler).Signaled().Within(2.Seconds()).WithTimeout(10.Seconds());

					exception = await Catch.ExceptionAsync(Act);
				}

				await That(exception).IsExactly<FailException>().And
					.HasMessage("""
					            Expected that signaler
					            has recorded the callback at least once within 0:02,
					            but it did not finish within 0:00.300
					            """).And
					.HasInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.300."))
					.Because("the effective timeout is the tighter of WithTimeout and TestCancellation");
			}

			[Test]
			public async Task WhenTimeoutIsInfinite_ShouldNotMentionTheTimeout()
			{
				Signaler signaler = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act() =>
					await That(signaler).Signaled().Within(System.Threading.Timeout.InfiniteTimeSpan).WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once,
					             but it could not be verified, because the evaluation was already canceled
					             """).WithTimeout(10.Seconds())
					.Because("an infinite timeout imposes no limit, so only the cancellation ends the wait");
			}

			[Test]
			public async Task WhenTimeoutIsInfinite_WithParameter_ShouldNotMentionTheTimeout()
			{
				Signaler<string> signaler = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act() =>
					await That(signaler).Signaled().Within(System.Threading.Timeout.InfiniteTimeSpan).WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once,
					             but it could not be verified, because the evaluation was already canceled
					             """).WithTimeout(10.Seconds())
					.Because("an infinite timeout imposes no limit, so only the cancellation ends the wait");
			}

			[Test]
			public async Task WhenTimeoutIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).Signaled().Within(-5.Milliseconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("timeout").And
					.WithMessage("The timeout must not be negative.").AsPrefix();
			}

			[Test]
			public async Task WhenTimeoutIsNegative_WithParameter_ShouldThrowArgumentOutOfRangeException()
			{
				Signaler<string> signaler = new();

				async Task Act() =>
					await That(signaler).Signaled().Within(-5.Milliseconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("timeout").And
					.WithMessage("The timeout must not be negative.").AsPrefix();
			}

			[Test]
			public async Task WhenTimeoutIsSpecifiedTwice_ShouldThrowInvalidOperationException()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).Signaled().Within(1.Seconds()).Within(50.Milliseconds());

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Within cannot be specified more than once.")
					.Because("the second timeout would silently replace the first one");
			}

			[Test]
			public async Task WhenTimeoutIsSpecifiedTwice_WithParameter_ShouldThrowInvalidOperationException()
			{
				Signaler<string> signaler = new();

				async Task Act() =>
					await That(signaler).Signaled().Within(1.Seconds()).Within(50.Milliseconds());

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Within cannot be specified more than once.")
					.Because("the second timeout would silently replace the first one");
			}

			[Test]
			public async Task WhenTriggeredWithinTheGivenTimeout_ShouldSucceed()
			{
				Signaler signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal());

				async Task Act() =>
					await That(signaler).Signaled().Within(10.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenTriggeredWithParameterWithinTheGivenTimeout_ShouldSucceed()
			{
				Signaler<string> signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal("foo"));

				async Task Act() =>
					await That(signaler).Signaled().Within(10.Seconds());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
