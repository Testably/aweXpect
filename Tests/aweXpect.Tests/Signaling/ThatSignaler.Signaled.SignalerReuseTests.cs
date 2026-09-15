using aweXpect.Signaling;

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class Signaled
	{
		public sealed class SignalerReuseTests
		{
			[Fact]
			public async Task WhenSignalingAfterAnAwaitedExpectation_ShouldNotThrow()
			{
				Signaler signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal());

				await That(signaler).Signaled();

				void Act() => signaler.Signal();

				await That(Act).DoesNotThrow()
					.Because("the expectation must not leave the signaler in a state where signaling fails");
			}

			[Fact]
			public async Task WhenSignalingAfterAnAwaitedExpectationWithParameter_ShouldNotThrow()
			{
				Signaler<int> signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal(1));

				await That(signaler).Signaled();

				void Act() => signaler.Signal(2);

				await That(Act).DoesNotThrow()
					.Because("the expectation must not leave the signaler in a state where signaling fails");
			}

			[Fact]
			public async Task WhenSignalingAfterAnExpectationThatNeverWaited_ShouldNotThrow()
			{
				Signaler signaler = new();
				signaler.Signal();

				await That(signaler).Signaled();

				void Act() => signaler.Signal();

				await That(Act).DoesNotThrow()
					.Because("the expectation must not leave the signaler in a state where signaling fails");
			}

			[Fact]
			public async Task WhenSignalingAfterAnUnmetExpectation_ShouldNotThrow()
			{
				Signaler signaler = new();

				await That(signaler).DidNotSignal().Within(20.Milliseconds());

				void Act() => signaler.Signal();

				await That(Act).DoesNotThrow()
					.Because("the expectation must not leave the signaler in a state where signaling fails");
			}
		}
	}
}
