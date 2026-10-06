using aweXpect.Core;
using aweXpect.Signaling;

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class DidNotSignal
	{
		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenNotTriggered_ShouldFail()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).DoesNotComplyWith(it => it.DidNotSignal().Within(50.Milliseconds()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:00.050,
					             but it was never recorded within 0:*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenNotTriggeredOftenEnough_ShouldFail()
			{
				Signaler signaler = new();

				signaler.Signal();

				async Task Act() =>
					await That(signaler).DoesNotComplyWith(it => it.DidNotSignal(2.Times()).Within(50.Milliseconds()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least twice within 0:00.050,
					             but it was only recorded once within 0:*
					             """).AsWildcard();
			}

			[Test]
			public async Task WithParameter_WhenTriggeredWithOtherParameter_ShouldFail()
			{
				Signaler<int> signaler = new();

				signaler.Signal(1);

				async Task Act() =>
					await That(signaler).DoesNotComplyWith(it
						=> it.DidNotSignal().With(p => p == 42).Within(50.Milliseconds()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once with p => p == 42 within 0:00.050,
					             but it was never recorded in [
					               1
					             ] within 0:*
					             """).AsWildcard();
			}
		}
	}
}
