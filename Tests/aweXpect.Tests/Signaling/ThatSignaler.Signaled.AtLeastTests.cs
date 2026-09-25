using aweXpect.Core;
using aweXpect.Signaling;

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class Signaled
	{
		public sealed class AtLeastTests
		{
			[Fact]
			public async Task WhenNotTriggeredOftenEnough_ShouldFail()
			{
				Signaler signaler = new();

				signaler.Signal();
				signaler.Signal();

				async Task Act() =>
					await That(signaler).Signaled().AtLeast(3.Times()).Within(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least 3 times within 0:00.050,
					             but it was only recorded twice within 0:00.*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenNotTriggeredWithParameter_ShouldFail()
			{
				Signaler<int> signaler = new();

				signaler.Signal(1);
				signaler.Signal(2);

				async Task Act() =>
					await That(signaler).Signaled().AtLeast(3.Times()).Within(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least 3 times within 0:00.050,
					             but it was only recorded twice in [
					               1,
					               2
					             ] within 0:00.*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenTriggeredMoreOften_ShouldSucceed()
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
					await That(signaler).Signaled().AtLeast(2.Times());

				await That(Act).DoesNotThrow();
			}

			[Fact]
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
					await That(signaler).Signaled().AtLeast(2.Times());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTriggeredOnlyOnce_ShouldFail()
			{
				Signaler signaler = new();

				signaler.Signal();

				async Task Act() =>
					await That(signaler).Signaled().AtLeast(2.Times()).Within(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback at least twice within 0:00.050,
					             but it was only recorded once within 0:00.*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenTriggeredWithParameterMoreOften_ShouldSucceed()
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
					await That(signaler).Signaled().AtLeast(3.Times());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenTriggeredWithParameterOftenEnough_ShouldSucceed()
			{
				Signaler<int> signaler = new();

				_ = Task.Delay(10.Milliseconds())
					.ContinueWith(_ =>
					{
						signaler.Signal(1);
						signaler.Signal(2);
						signaler.Signal(3);
					});

				async Task Act() =>
					await That(signaler).Signaled().AtLeast(3.Times());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
