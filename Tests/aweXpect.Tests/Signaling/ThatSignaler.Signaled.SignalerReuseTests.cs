using aweXpect.Signaling;

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class Signaled
	{
		public sealed class SignalerReuseTests
		{
			[Test]
			public async Task WhenAwaitingASignalWithParameterAfterAnUnmetExpectation_ShouldSucceed()
			{
				Signaler<int> signaler = new();
				await That(signaler).DidNotSignal().Within(10.Milliseconds());
				signaler.Signal(1);

				async Task Act()
					=> await That(signaler).Signaled().With(x => x == 1);

				await That(Act).DoesNotThrow()
					.Because("the unmet expectation must not leave a disposed event behind for the next wait");
			}

			[Test]
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

			[Test]
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

			[Test]
			public async Task WhenSignalingAfterAnExpectationThatNeverWaited_ShouldNotThrow()
			{
				Signaler signaler = new();
				signaler.Signal();

				await That(signaler).Signaled();

				void Act() => signaler.Signal();

				await That(Act).DoesNotThrow()
					.Because("the expectation must not leave the signaler in a state where signaling fails");
			}

			[Test]
			public async Task WhenSignalingAfterAnUnmetExpectation_ShouldNotThrow()
			{
				Signaler signaler = new();

				await That(signaler).DidNotSignal().Within(20.Milliseconds());

				void Act() => signaler.Signal();

				await That(Act).DoesNotThrow()
					.Because("the expectation must not leave the signaler in a state where signaling fails");
			}

			[Test]
			public async Task WhenSignalingAfterAnUnmetExpectationWithParameter_ShouldNotThrow()
			{
				Signaler<int> signaler = new();

				await That(signaler).DidNotSignal().Within(20.Milliseconds());

				void Act() => signaler.Signal(1);

				await That(Act).DoesNotThrow()
					.Because("the expectation must not leave the signaler in a state where signaling fails");
			}
		}
	}
}
