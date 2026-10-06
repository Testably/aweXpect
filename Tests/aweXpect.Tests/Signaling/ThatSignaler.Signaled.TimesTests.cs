using aweXpect.Core;
using aweXpect.Signaling;

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class Signaled
	{
		public sealed class TimesTests
		{
			[Test]
			public async Task WhenFilteredWith_ShouldOnlyCountAndContainTheMatchingParameters()
			{
				Signaler<int> signaler = new();
				signaler.Signal(-1);
				signaler.Signal(1);
				signaler.Signal(2);

				async Task Act() =>
					await That(signaler).Signaled(2.Times()).With(x => x > 0)
						.WhoseParameters.IsEqualTo([1, 2,]);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenNotTriggeredOftenEnough_ShouldFail()
			{
				Signaler signaler = new();

				signaler.Signal();
				signaler.Signal();

				async Task Act() =>
					await That(signaler).Signaled(3.Times()).Within(50.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least 3 times within 0:00.050,
					             but it was only recorded twice within *
					             """).AsWildcard()
					.Because("the waited time is wall-clock time, which a busy machine can stretch beyond a second");
			}

			[Test]
			public async Task WhenNotTriggeredWithParameter_ShouldFail()
			{
				Signaler<int> signaler = new();

				signaler.Signal(1);
				signaler.Signal(2);

				async Task Act() =>
					await That(signaler).Signaled(3.Times()).Within(50.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least 3 times within 0:00.050,
					             but it was only recorded twice in [
					               1,
					               2
					             ] within *
					             """).AsWildcard()
					.Because("the waited time is wall-clock time, which a busy machine can stretch beyond a second");
			}

			[Test]
			[Arguments(0)]
			[Arguments(-1)]
			public async Task WhenTimesIsNotPositive_ShouldThrowArgumentOutOfRangeException(int times)
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).Signaled(times.Times()).Within(10.Milliseconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("times").And
					.WithMessage("The times must be greater than zero.").AsPrefix()
					.Because("being signaled at least zero or a negative number of times could never fail");
			}

			[Test]
			[Arguments(0)]
			[Arguments(-1)]
			public async Task WhenTimesWithParameterIsNotPositive_ShouldThrowArgumentOutOfRangeException(int times)
			{
				Signaler<int> signaler = new();

				async Task Act() =>
					await That(signaler).Signaled(times.Times()).Within(10.Milliseconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("times").And
					.WithMessage("The times must be greater than zero.").AsPrefix()
					.Because("being signaled at least zero or a negative number of times could never fail");
			}

			[Test]
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

			[Test]
			public async Task WhenWithinIsSpecifiedTwice_ShouldThrowInvalidOperationException()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).Signaled(2.Times()).Within(1.Seconds()).Within(50.Milliseconds());

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Within cannot be specified more than once.");
			}
		}
	}
}
