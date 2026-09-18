using System.Threading;
using aweXpect.Signaling;

// ReSharper disable MethodHasAsyncOverload

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class Signaled
	{
		public sealed class WithinTests
		{
			[Fact]
			public async Task WhenNotTriggeredWithinTheGivenTimeout_ShouldFail()
			{
				Signaler signaler = new();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(5.Seconds(), token)
					.ContinueWith(_ => signaler.Signal(), token);

				async Task Act() =>
					await That(signaler).Signaled().Within(40.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:00.040,
					             but it was never recorded within 0:*
					             """).AsWildcard();
				cts.Cancel();
			}

			[Fact]
			public async Task WhenNotTriggeredWithParameterWithinTheGivenTimeout_ShouldFail()
			{
				Signaler<string> signaler = new();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(5.Seconds(), token)
					.ContinueWith(_ => signaler.Signal("foo"), token);

				async Task Act() =>
					await That(signaler).Signaled().Within(40.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:00.040,
					             but it was never recorded within 0:*
					             """).AsWildcard();
				cts.Cancel();
			}

			[Fact]
			public async Task WhenTriggeredWithinTheGivenTimeout_ShouldSucceed()
			{
				Signaler signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal());

				async Task Act() =>
					await That(signaler).Signaled().Within(10.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
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
