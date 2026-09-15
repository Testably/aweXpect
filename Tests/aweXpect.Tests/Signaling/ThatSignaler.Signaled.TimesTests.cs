using System.Threading;
using aweXpect.Core;
using aweXpect.Signaling;

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class Signaled
	{
		public sealed class TimesTests
		{
			[Fact]
			public async Task WhenNotTriggeredOftenEnough_ShouldFail()
			{
				Signaler signaler = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());
				CancellationToken token = cts.Token;

				signaler.Signal();
				signaler.Signal();

				async Task Act() =>
					await That(signaler).Signaled(3.Times()).WithCancellation(token);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least 3 times,
					             but it was only recorded twice
					             """);
			}

			[Fact]
			public async Task WhenNotTriggeredWithParameter_ShouldFail()
			{
				Signaler<int> signaler = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());
				CancellationToken token = cts.Token;

				signaler.Signal(1);
				signaler.Signal(2);

				async Task Act() =>
					await That(signaler).Signaled(3.Times()).WithCancellation(token);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least 3 times,
					             but it was only recorded twice in [
					               1,
					               2
					             ]
					             """);
			}

			[Fact]
			public async Task WhenTriggeredOftenEnough_ShouldSucceed()
			{
				Signaler signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ =>
					{
						signaler.Signal();
						signaler.Signal();
					});

				async Task Act() =>
					await That(signaler).Signaled(2.Times());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
