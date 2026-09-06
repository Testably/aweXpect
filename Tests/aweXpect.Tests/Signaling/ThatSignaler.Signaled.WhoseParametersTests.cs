using System.Threading;
using aweXpect.Core;
using aweXpect.Signaling;

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class Signaled
	{
		public sealed class WhoseParametersTests
		{
			[Fact]
			public async Task WhenNotTriggered_ShouldFail()
			{
				Signaler<int> signaler = new();
				using CancellationTokenSource cts = new(50.Milliseconds());
				CancellationToken token = cts.Token;

				async Task Act() =>
					await That(signaler).Signaled(2.Times()).With(x => x > 0)
						.WhoseParameters.AreAllUnique().WithCancellation(token);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least twice with x => x > 0 and whose parameters only have unique items,
					             but it was never recorded
					             
					             Collection:
					             []
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Signaler<int>? subject = null;

				async Task Act()
					=> await That(subject!).Signaled().WhoseParameters.HasCount().EqualTo(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recorded the callback at least once and whose parameters have exactly 0 items,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTriggered_AndParametersDoNotMatch_ShouldFail()
			{
				Signaler<int> signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal(1));

				async Task Act() =>
					await That(signaler).Signaled().WhoseParameters.All().Satisfy(x => x < 1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once and whose parameters all satisfy x => x < 1,
					             but none of 1 did
					             
					             Not matching items:
					             [1]
					             
					             Collection:
					             [1]
					             """);
			}

			[Fact]
			public async Task WhenTriggered_AndParametersMatch_ShouldSucceed()
			{
				Signaler<int> signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal(1));

				async Task Act() =>
					await That(signaler).Signaled().WhoseParameters.All().ComplyWith(x => x.IsGreaterThan(0));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
