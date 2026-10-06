using System.Threading;
using aweXpect.Customization;
using aweXpect.Signaling;

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class Signaled
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenCanceled_ShouldBeInconclusive()
			{
				Signaler signaler = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());
				CancellationToken token = cts.Token;

				async Task Act() =>
					await That(signaler).Signaled().WithCancellation(token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:30,
					             but it could not be verified, because the evaluation was already canceled
					             """);
			}

			[Test]
			public async Task WhenDefaultTimeoutElapses_ShouldNameTheDefaultTimeout()
			{
				Signaler signaler = new();

				using (IDisposable __ = Customize.aweXpect.Settings().DefaultSignalerTimeout.Set(50.Milliseconds()))
				{
					async Task Act() =>
						await That(signaler).Signaled();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that signaler
						             has recorded the callback at least once within 0:00.050,
						             but it was never recorded within *
						             """).AsWildcard()
						.Because("the failure must show that the default timeout applied");
				}
			}

			[Test]
			public async Task WhenDefaultTimeoutIsInfinite_ShouldNotNameIt()
			{
				Signaler signaler = new();
				using CancellationTokenSource cts = new(50.Milliseconds());
				CancellationToken token = cts.Token;

				using (IDisposable __ = Customize.aweXpect.Settings().DefaultSignalerTimeout
					       .Set(Timeout.InfiniteTimeSpan))
				{
					async Task Act() =>
						await That(signaler).Signaled().WithCancellation(token);

					await That(Act).Throws<InconclusiveTestException>()
						.WithMessage("""
						             Expected that signaler
						             has recorded the callback at least once,
						             but it could not be verified, because the evaluation was already canceled
						             """)
						.Because("an infinite timeout does not add any information to the expectation");
				}
			}

			[Test]
			public async Task WhenNotTriggered_ShouldFail()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).Signaled().Within(50.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:00.050,
					             but it was never recorded within *
					             """).AsWildcard()
					.Because("the waited time is wall-clock time, which a busy machine can stretch beyond a second");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Signaler? subject = null;

				async Task Act()
					=> await That(subject!).Signaled();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has recorded the callback at least once,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTimeoutElapses_ShouldFail()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).Signaled().WithTimeout(50.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:30,
					             but it did not finish within 0:00.050
					             """).And
					.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."));
			}

			[Test]
			public async Task WhenTriggered_ShouldSucceed()
			{
				Signaler signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal());

				async Task Act() =>
					await That(signaler).Signaled();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class WithParameterTests
		{
			[Test]
			public async Task WhenCanceled_ShouldBeInconclusive()
			{
				Signaler<int> signaler = new();
				using CancellationTokenSource cts = new(50.Milliseconds());
				CancellationToken token = cts.Token;

				async Task Act() =>
					await That(signaler).Signaled().WithCancellation(token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:30,
					             but it could not be verified, because the evaluation was already canceled
					             """);
			}

			[Test]
			public async Task WhenNotTriggered_ShouldFail()
			{
				Signaler<int> signaler = new();

				async Task Act() =>
					await That(signaler).Signaled().Within(50.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:00.050,
					             but it was never recorded within *
					             """).AsWildcard()
					.Because("the waited time is wall-clock time, which a busy machine can stretch beyond a second");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Signaler<int>? subject = null;

				async Task Act()
					=> await That(subject!).Signaled();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has recorded the callback at least once,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTriggered_ShouldSucceed()
			{
				Signaler<int> signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ => signaler.Signal(1));

				async Task Act() =>
					await That(signaler).Signaled();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
