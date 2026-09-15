using System.Threading;
using aweXpect.Core;
using aweXpect.Signaling;

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class Signaled
	{
		public sealed class WithTests
		{
			[Fact]
			public async Task WhenApplyingMultiplePredicates_ShouldVerifyAll()
			{
				Signaler<int> signaler = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());
				CancellationToken token = cts.Token;

				signaler.Signal(1);
				signaler.Signal(2);
				signaler.Signal(3);

				async Task Act() =>
					await That(signaler).Signaled().AtLeast(2.Times())
						.With(p => p > 1).With(p => p < 3)
						.WithCancellation(token);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least twice with p => p > 1 and with p => p < 3,
					             but it was only recorded once in [
					               1,
					               2,
					               3
					             ]
					             """);
			}

			[Fact]
			public async Task WhenNotTriggeredOftenEnoughMatchingPredicate_ShouldFail()
			{
				Signaler<int> signaler = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());
				CancellationToken token = cts.Token;

				signaler.Signal(1);
				signaler.Signal(2);

				async Task Act() =>
					await That(signaler).Signaled().AtLeast(2.Times())
						.With(p => p > 1)
						.WithCancellation(token);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least twice with p => p > 1,
					             but it was only recorded once in [
					               1,
					               2
					             ]
					             """);
			}

			[Fact]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				Signaler<int> signaler = new();

				async Task Act()
					=> await That(signaler).Signaled().With(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The predicate cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenTriggeredMoreOftenMatchingPredicate_ShouldSucceed()
			{
				Signaler<int> signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ =>
					{
						signaler.Signal(1);
						signaler.Signal(2);
						signaler.Signal(3);
						signaler.Signal(4);
					});

				async Task Act() =>
					await That(signaler).Signaled().AtLeast(2.Times())
						.With(p => p < 4);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTriggeredOftenEnoughMatchingPredicate_ShouldSucceed()
			{
				Signaler<int> signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ =>
					{
						signaler.Signal(1);
						signaler.Signal(2);
						signaler.Signal(3);
						signaler.Signal(4);
					});

				async Task Act() =>
					await That(signaler).Signaled().AtLeast(2.Times())
						.With(p => p < 3);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
