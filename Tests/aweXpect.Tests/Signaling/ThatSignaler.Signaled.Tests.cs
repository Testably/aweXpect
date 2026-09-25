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
			[Fact]
			public async Task WhenCanceled_ShouldBeInconclusive()
			{
				Signaler signaler = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());
				CancellationToken token = cts.Token;

				async Task Act() =>
					await That(signaler).Signaled().WithCancellation(token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:30,
					             but it could not be verified, because it was already canceled
					             """);
			}

			[Fact]
			public async Task WhenDefaultTimeoutElapses_ShouldNameTheDefaultTimeout()
			{
				Signaler signaler = new();

				using (IDisposable __ = Customize.aweXpect.Settings().DefaultSignalerTimeout.Set(50.Milliseconds()))
				{
					async Task Act() =>
						await That(signaler).Signaled();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that signaler
						             has recorded the callback at least once within 0:00.050,
						             but it was never recorded within 0:00.*
						             """).AsWildcard()
						.Because("the failure must show that the default timeout applied");
				}
			}

			[Fact]
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

					await That(Act).Throws<InconclusiveException>()
						.WithMessage("""
						             Expected that signaler
						             has recorded the callback at least once,
						             but it could not be verified, because it was already canceled
						             """)
						.Because("an infinite timeout does not add any information to the expectation");
				}
			}

			[Fact]
			public async Task WhenNotTriggered_ShouldFail()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).Signaled().Within(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:00.050,
					             but it was never recorded within 0:00.*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Signaler? subject = null;

				async Task Act()
					=> await That(subject!).Signaled();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recorded the callback at least once,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenTimeoutElapses_ShouldFail()
			{
				Signaler signaler = new();

				async Task Act() =>
					await That(signaler).Signaled().WithTimeout(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:30,
					             but it did not finish within 0:00.050
					             """).And
					.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."));
			}

			[Fact]
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
			[Fact]
			public async Task WhenCanceled_ShouldBeInconclusive()
			{
				Signaler<int> signaler = new();
				using CancellationTokenSource cts = new(50.Milliseconds());
				CancellationToken token = cts.Token;

				async Task Act() =>
					await That(signaler).Signaled().WithCancellation(token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:30,
					             but it could not be verified, because it was already canceled
					             """);
			}

			[Fact]
			public async Task WhenNotTriggered_ShouldFail()
			{
				Signaler<int> signaler = new();

				async Task Act() =>
					await That(signaler).Signaled().Within(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least once within 0:00.050,
					             but it was never recorded within 0:00.*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Signaler<int>? subject = null;

				async Task Act()
					=> await That(subject!).Signaled();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recorded the callback at least once,
					             but it was <null>
					             """);
			}

			[Fact]
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
