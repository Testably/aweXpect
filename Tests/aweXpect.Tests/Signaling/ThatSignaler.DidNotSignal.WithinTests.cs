using System.Threading;
using aweXpect.Signaling;

// ReSharper disable MethodHasAsyncOverload

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class DidNotSignal
	{
		public sealed class WithinTests
		{
			[Fact]
			public async Task WhenNotTriggeredWithinTheGivenTimeout_ShouldSucceed()
			{
				Signaler signaler = new();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(5.Seconds(), token)
					.ContinueWith(_ => signaler.Signal(), token);

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(40.Milliseconds());

				await That(Act).DoesNotThrow();
				cts.Cancel();
			}

			[Fact]
			public async Task WhenNotTriggeredWithParameterWithinTheGivenTimeout_ShouldSucceed()
			{
				Signaler<string> signaler = new();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(5.Seconds(), token)
					.ContinueWith(_ => signaler.Signal("foo"), token);

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(40.Milliseconds());

				await That(Act).DoesNotThrow();
				cts.Cancel();
			}

			[Fact]
			public async Task WhenTimeoutIsInfinite_ShouldNotMentionTheTimeout()
			{
				Signaler signaler = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(System.Threading.Timeout.InfiniteTimeSpan).WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that signaler
					             has never recorded the callback,
					             but it could not be verified, because it was already canceled
					             """).WithTimeout(10.Seconds())
					.Because("an infinite timeout imposes no limit, so only the cancellation ends the wait");
			}

			[Fact]
			public async Task WhenTimeoutIsInfinite_WithParameter_ShouldNotMentionTheTimeout()
			{
				Signaler<string> signaler = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(System.Threading.Timeout.InfiniteTimeSpan).WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that signaler
					             has never recorded the callback,
					             but it could not be verified, because it was already canceled
					             """).WithTimeout(10.Seconds())
					.Because("an infinite timeout imposes no limit, so only the cancellation ends the wait");
			}

			[Fact]
			public async Task WhenTimeoutIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(-5.Milliseconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("timeout").And
					.WithMessage("The timeout must not be negative.").AsPrefix();
			}

			[Fact]
			public async Task WhenTimeoutIsNegative_WithParameter_ShouldThrowArgumentOutOfRangeException()
			{
				Signaler<string> signaler = new();

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(-5.Milliseconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("timeout").And
					.WithMessage("The timeout must not be negative.").AsPrefix();
			}

			[Fact]
			public async Task WhenTriggeredWithinTheGivenTimeout_ShouldFail()
			{
				Signaler signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal());

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(10.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has never recorded the callback within 0:10,
					             but it was recorded once after 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenTriggeredWithParameterWithinTheGivenTimeout_ShouldFail()
			{
				Signaler<string> signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal("foo"));

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(10.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has never recorded the callback within 0:10,
					             but it was recorded once in [
					               "foo"
					             ] after 0:*
					             """).AsWildcard();
			}
		}
	}
}
