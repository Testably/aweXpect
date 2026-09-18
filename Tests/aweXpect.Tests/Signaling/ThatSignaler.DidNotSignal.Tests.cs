using System.Threading;
using aweXpect.Signaling;

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class DidNotSignal
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenNotTriggered_ShouldSucceed()
			{
				Signaler signaler = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());
				CancellationToken token = cts.Token;

				async Task Act() =>
					await That(signaler).DidNotSignal().WithCancellation(token);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Signaler? subject = null;

				async Task Act()
					=> await That(subject!).DidNotSignal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has never recorded the callback,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTriggered_ShouldFail()
			{
				Signaler signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal());

				async Task Act() =>
					await That(signaler).DidNotSignal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has never recorded the callback,
					             but it was recorded once
					             """);
			}
		}

		public sealed class WithParameterTests
		{
			[Fact]
			public async Task WhenNotTriggered_ShouldSucceed()
			{
				Signaler<int> signaler = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());
				CancellationToken token = cts.Token;

				async Task Act() =>
					await That(signaler).DidNotSignal().WithCancellation(token);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Signaler<int>? subject = null;

				async Task Act()
					=> await That(subject!).DidNotSignal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has never recorded the callback,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTriggered_ShouldFail()
			{
				Signaler<int> signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal(42));

				async Task Act() =>
					await That(signaler).DidNotSignal();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has never recorded the callback,
					             but it was recorded once in [
					               42
					             ]
					             """);
			}
		}
	}
}
