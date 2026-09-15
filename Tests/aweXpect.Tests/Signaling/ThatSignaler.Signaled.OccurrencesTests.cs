using aweXpect.Core;
using aweXpect.Signaling;

// ReSharper disable MethodHasAsyncOverload

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class Signaled
	{
		public sealed class OccurrencesTests
		{
			[Fact]
			public async Task WhenNeverRecorded_AtLeastZeroTimesShouldSucceedWithoutWaiting()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).Signaled().AtLeast(0.Times());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNeverRecorded_NeverShouldSucceed()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).Signaled().Never().Within(40.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenRecordedExactlyTwice_ExactlyShouldSucceed()
			{
				Signaler signaler = new();

				signaler.Signal();
				signaler.Signal();

				async Task Act() =>
					await That(signaler).Signaled().Exactly(2.Times()).Within(40.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenRecordedLessOftenThanTheMaximum_AtMostShouldSucceed()
			{
				Signaler signaler = new();

				signaler.Signal();

				async Task Act() =>
					await That(signaler).Signaled().AtMost(2.Times()).Within(40.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenRecordedMoreOftenThanExpected_ExactlyShouldFail()
			{
				Signaler signaler = new();

				signaler.Signal();
				signaler.Signal();
				signaler.Signal();

				async Task Act() =>
					await That(signaler).Signaled().Exactly(2.Times()).Within(40.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback exactly twice within 0:00.040,
					             but it was recorded 3 times
					             """);
			}

			[Fact]
			public async Task WhenRecordedMoreOftenThanTheMaximum_LessThanShouldFail()
			{
				Signaler signaler = new();

				signaler.Signal();
				signaler.Signal();
				signaler.Signal();

				async Task Act() =>
					await That(signaler).Signaled().LessThan(3.Times()).Within(40.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback less than 3 times within 0:00.040,
					             but it was recorded 3 times
					             """);
			}

			[Fact]
			public async Task WhenRecordedOnce_NeverShouldFail()
			{
				Signaler signaler = new();

				signaler.Signal();

				async Task Act() =>
					await That(signaler).Signaled().Never().Within(40.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has never recorded the callback within 0:00.040,
					             but it was recorded once
					             """);
			}

			[Fact]
			public async Task WhenRecordedOnce_OnceShouldSucceed()
			{
				Signaler signaler = new();

				signaler.Signal();

				async Task Act() =>
					await That(signaler).Signaled().Once().Within(40.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenRecordedTooOftenAfterAWhile_AtMostShouldFail()
			{
				Signaler signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ =>
					{
						signaler.Signal();
						signaler.Signal();
						signaler.Signal();
					});

				async Task Act() =>
					await That(signaler).Signaled().AtMost(2.Times()).Within(10.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at most twice within 0:10,
					             but it was recorded 3 times
					             """);
			}

			[Fact]
			public async Task WhenRecordedTwice_OnceShouldFail()
			{
				Signaler signaler = new();

				signaler.Signal();
				signaler.Signal();

				async Task Act() =>
					await That(signaler).Signaled().Once().Within(40.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback exactly once within 0:00.040,
					             but it was recorded twice
					             """);
			}

			[Fact]
			public async Task WhenRecordedTwice_TwiceShouldSucceed()
			{
				Signaler signaler = new();

				signaler.Signal();
				signaler.Signal();

				async Task Act() =>
					await That(signaler).Signaled().Twice().Within(40.Milliseconds());

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class OccurrencesWithParameterTests
		{
			[Fact]
			public async Task WhenNeverRecorded_AtLeastZeroTimesShouldSucceedWithoutWaiting()
			{
				Signaler<int> signaler = new();

				async Task Act() =>
					await That(signaler).Signaled().AtLeast(0.Times());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenRecordedExactlyTwice_ExactlyShouldSucceed()
			{
				Signaler<int> signaler = new();

				signaler.Signal(1);
				signaler.Signal(2);

				async Task Act() =>
					await That(signaler).Signaled().Exactly(2.Times()).Within(40.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenRecordedMoreOftenThanExpected_ExactlyShouldFail()
			{
				Signaler<int> signaler = new();

				signaler.Signal(1);
				signaler.Signal(2);
				signaler.Signal(3);

				async Task Act() =>
					await That(signaler).Signaled().Exactly(2.Times()).Within(40.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback exactly twice within 0:00.040,
					             but it was recorded 3 times in [
					               1,
					               2,
					               3
					             ]
					             """);
			}

			[Fact]
			public async Task WhenRecordedOnlyWithNotMatchingParameters_NeverShouldSucceed()
			{
				Signaler<int> signaler = new();

				signaler.Signal(1);

				async Task Act() =>
					await That(signaler).Signaled().Never().With(p => p > 1).Within(40.Milliseconds());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
