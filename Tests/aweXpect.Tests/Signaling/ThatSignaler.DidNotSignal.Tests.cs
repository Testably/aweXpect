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
			public async Task WhenCanceled_ShouldBeInconclusive()
			{
				Signaler signaler = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());
				CancellationToken token = cts.Token;

				async Task Act() =>
					await That(signaler).DidNotSignal().WithCancellation(token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that signaler
					             has never recorded the callback,
					             but it could not be verified, because it was already canceled
					             """)
					.Because("a cancellation ends the wait before the timeout, so it must not pass early");
			}

			[Fact]
			public async Task WhenNotTriggered_ShouldSucceed()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(50.Milliseconds());

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
			public async Task WhenTimeoutElapses_ShouldFail()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).DidNotSignal().WithTimeout(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has never recorded the callback,
					             but it did not finish within 0:00.050
					             """).And
					.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
					.Because("the timeout of the expectation ends the wait before the signaler timeout, so it must not pass early");
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
			public async Task WhenCanceled_ShouldBeInconclusive()
			{
				Signaler<int> signaler = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());
				CancellationToken token = cts.Token;

				async Task Act() =>
					await That(signaler).DidNotSignal().WithCancellation(token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that signaler
					             has never recorded the callback,
					             but it could not be verified, because it was already canceled
					             """)
					.Because("a cancellation ends the wait before the timeout, so it must not pass early");
			}

			[Fact]
			public async Task WhenNotTriggered_ShouldSucceed()
			{
				Signaler<int> signaler = new();

				async Task Act() =>
					await That(signaler).DidNotSignal().Within(50.Milliseconds());

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
			public async Task WhenTimeoutElapses_ShouldFail()
			{
				Signaler<int> signaler = new();

				async Task Act() =>
					await That(signaler).DidNotSignal().WithTimeout(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has never recorded the callback,
					             but it did not finish within 0:00.050
					             """).And
					.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
					.Because("the timeout of the expectation ends the wait before the signaler timeout, so it must not pass early");
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
