using System.Threading;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Signaling;

// ReSharper disable MethodHasAsyncOverload

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class DidNotSignal
	{
		public sealed class WithinTests
		{
			[Test]
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

			[Test]
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

			[Test]
			public async Task WhenTheOuterTimeoutIsAsLong_ShouldSucceed()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(200.Milliseconds()).WithTimeout(200.Milliseconds())
						.WithTimeSystem(new VirtualTimeSystem());

				await That(Act).DoesNotThrow()
					.Because("an outer timeout that is not shorter than Within must not decide the outcome");
			}

			[Test]
			public async Task WhenTheOuterTimeoutIsAsLong_WithParameter_ShouldSucceed()
			{
				Signaler<string> signaler = new();

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(200.Milliseconds()).WithTimeout(200.Milliseconds())
						.WithTimeSystem(new VirtualTimeSystem());

				await That(Act).DoesNotThrow()
					.Because("an outer timeout that is not shorter than Within must not decide the outcome");
			}

			[Test]
			public async Task WhenTimeoutIsInfinite_ShouldNotMentionTheTimeout()
			{
				Signaler signaler = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(Timeout.InfiniteTimeSpan).WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that signaler
					             has never recorded the callback,
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
					await That(signaler).DidNotSignal().Within(Timeout.InfiniteTimeSpan).WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that signaler
					             has never recorded the callback,
					             but it could not be verified, because the evaluation was already canceled
					             """).WithTimeout(10.Seconds())
					.Because("an infinite timeout imposes no limit, so only the cancellation ends the wait");
			}

			[Test]
			public async Task WhenTimeoutIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(-5.Milliseconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("timeout").And
					.WithMessage("The timeout must not be negative.").AsPrefix();
			}

			[Test]
			public async Task WhenTimeoutIsNegative_WithParameter_ShouldThrowArgumentOutOfRangeException()
			{
				Signaler<string> signaler = new();

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(-5.Milliseconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("timeout").And
					.WithMessage("The timeout must not be negative.").AsPrefix();
			}

			[Test]
			public async Task WhenTimeoutIsSpecifiedTwice_ShouldThrowInvalidOperationException()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(1.Seconds()).Within(50.Milliseconds());

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Within cannot be specified more than once.")
					.Because("the second timeout would silently replace the first one");
			}

			[Test]
			public async Task WhenTimeoutIsSpecifiedTwice_WithParameter_ShouldThrowInvalidOperationException()
			{
				Signaler<string> signaler = new();

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(1.Seconds()).Within(50.Milliseconds());

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Within cannot be specified more than once.")
					.Because("the second timeout would silently replace the first one");
			}

			[Test]
			public async Task WhenTriggeredWithinTheGivenTimeout_ShouldFail()
			{
				Signaler signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal());

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(10.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that signaler
					             has never recorded the callback within 0:10,
					             but it was recorded once after 0:*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenTriggeredWithParameterWithinTheGivenTimeout_ShouldFail()
			{
				Signaler<string> signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal("foo"));

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(10.Seconds());

				await That(Act).Throws<FailException>()
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
