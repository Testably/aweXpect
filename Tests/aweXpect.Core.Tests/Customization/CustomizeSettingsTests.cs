using System.Diagnostics;
using System.Threading;
using aweXpect.Chronology;
using aweXpect.Customization;
using aweXpect.Signaling;

namespace aweXpect.Core.Tests.Customization;

public sealed class CustomizeSettingsTests
{
	[Fact]
	public async Task DefaultCheckInterval_ShouldBeUsedInTimeComparisons()
	{
		TimeSpan timeout = 2.Seconds();

		ChangingClass sut1 = new();
		ChangingClass sut2 = new();

		await That(Customize.aweXpect.Settings().DefaultCheckInterval.Get()).IsEqualTo(100.Milliseconds());
		await That(sut1).Satisfies(x => x.HasMeasuredInterval).Within(30.Seconds());
		await That(sut1.Interval).IsGreaterThanOrEqualTo(50.Milliseconds()).And.IsLessThan(10.Seconds());
		using (IDisposable __ = Customize.aweXpect.Settings().DefaultCheckInterval.Set(timeout))
		{
			await That(sut2).Satisfies(x => x.HasMeasuredInterval).Within(30.Seconds());
			await That(sut2.Interval).IsGreaterThanOrEqualTo(timeout).Within(50.Milliseconds()).And
				.IsLessThan(20.Seconds());
		}
	}

	[Fact]
	public async Task DefaultEventuallyTimeout_ShouldBeUsedInEventually()
	{
		await That(Customize.aweXpect.Settings().DefaultEventuallyTimeout.Get()).IsEqualTo(30.Seconds());
		Stopwatch stopwatch = new();
		using (IDisposable __ = Customize.aweXpect.Settings().DefaultEventuallyTimeout.Set(LowTimeout))
		{
			await That(Customize.aweXpect.Settings().DefaultEventuallyTimeout.Get()).IsEqualTo(LowTimeout);
			stopwatch.Start();
			async Task Act() => await That(() => 1).Eventually().IsEqualTo(2);
			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that () => 1
				             is equal to 2 within 0:00.100,
				             but it was 1 which differs by -1
				             """);
			stopwatch.Stop();
		}

		await That(stopwatch.Elapsed).IsGreaterThanOrEqualTo(LowTimeout).Within(50.Milliseconds()).And
			.IsLessThan(10.Seconds());
		await That(Customize.aweXpect.Settings().DefaultEventuallyTimeout.Get()).IsEqualTo(30.Seconds());
	}

	[Fact]
	public async Task DefaultSignalerTimeout_ShouldBeUsedInSignaler()
	{
		Signaler signaler = new();
		await That(Customize.aweXpect.Settings().DefaultSignalerTimeout.Get()).IsEqualTo(30000.Milliseconds());
		using (IDisposable __ = Customize.aweXpect.Settings().DefaultSignalerTimeout.Set(10.Milliseconds()))
		{
			using CancellationTokenSource cts = new();
			CancellationToken token = cts.Token;
			_ = Task.Delay(10.Seconds(), token).ContinueWith(_ => signaler.Signal(), token);
			SignalerResult result = signaler.Wait();
			cts.Cancel();
			await That(result.IsSuccess).IsFalse();
			await That(Customize.aweXpect.Settings().DefaultSignalerTimeout.Get()).IsEqualTo(10.Milliseconds());
		}

		{
			_ = Task.Delay(LowTimeout).ContinueWith(_ => signaler.Signal());
			SignalerResult result = signaler.Wait();
			await That(result.IsSuccess).IsTrue();
			await That(Customize.aweXpect.Settings().DefaultSignalerTimeout.Get()).IsEqualTo(30000.Milliseconds());
		}
	}

	[Fact]
	public async Task DefaultTimeComparisonTimeout_ShouldBeUsedInTimeComparisons()
	{
		DateTime time = DateTime.UtcNow;
		DateTime otherTime = time.AddMilliseconds(10);
		await That(Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get()).IsEqualTo(TimeSpan.Zero);
		async Task Act() => await That(time).IsEqualTo(otherTime);
		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that time
			              is equal to {Formatter.Format(otherTime)},
			              but it was {Formatter.Format(time)} which differs by -0:00.010
			              """);
		using (IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(10.Milliseconds()))
		{
			await That(Act).DoesNotThrow();
		}

		await That(Act).Throws<XunitException>()
			.WithMessage($"""
			              Expected that time
			              is equal to {Formatter.Format(otherTime)},
			              but it was {Formatter.Format(time)} which differs by -0:00.010
			              """)
			.Because("the default tolerance must be restored once the customization is disposed");
	}

	[Fact]
	public async Task DefaultTimeComparisonTolerance_WhenNegative_ShouldThrowArgumentOutOfRangeException()
	{
		void Act() => Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(-1.Milliseconds());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix()
			.Because("a negative default tolerance tightens every time comparison instead of widening it");
	}

	[Fact]
	public async Task Settings_ShouldReturnSameInstance()
	{
		AwexpectCustomization.SettingsCustomization settings1 = Customize.aweXpect.Settings();
		AwexpectCustomization.SettingsCustomization settings2 = Customize.aweXpect.Settings();

		await That(settings1).IsSameAs(settings2);
	}

	[Fact]
	public async Task TestCancellation_FromCancellationToken_ShouldBeApplied()
	{
		Stopwatch stopwatch = new();
		CancellationToken cancelledToken = new(true);
		using (IDisposable __ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromCancellationToken(() => cancelledToken)))
		{
			async Task Act()
				=> await That(cancellationToken => Task.Delay(30.Seconds(), cancellationToken))
					.DoesNotThrow();

			stopwatch.Start();
			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that cancellationToken => Task.Delay(30.Seconds(), cancellationToken)
				             does not throw any exception,
				             but it could not be verified, because it was already canceled
				             """);
			stopwatch.Stop();
		}

		await That(stopwatch.Elapsed).IsLessThan(10.Seconds());
	}

	[Fact]
	public async Task TestCancellation_FromTimeout_ShouldBeApplied()
	{
		Stopwatch stopwatch = new();
		using (IDisposable __ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromTimeout(LowTimeout)))
		{
			async Task Act()
				=> await That(cancellationToken => Task.Delay(30.Seconds(), cancellationToken))
					.Throws<TaskCanceledException>();

			stopwatch.Start();
			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that cancellationToken => Task.Delay(30.Seconds(), cancellationToken)
				             throws a TaskCanceledException,
				             but it did not finish within 0:00.100
				             """)
				.WithTimeout(30.Seconds());
			stopwatch.Stop();
		}

		await That(stopwatch.Elapsed).IsLessThanOrEqualTo(10.Seconds());
		await That(stopwatch.Elapsed).IsGreaterThanOrEqualTo(LowTimeout).Within(50.Milliseconds());
	}

	[Fact]
	public async Task TestCancellation_None_ShouldBeApplied()
	{
		Stopwatch stopwatch = new();
		CancellationToken cancelledToken = new(true);
		using (IDisposable _ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromCancellationToken(() => cancelledToken)))
		{
			using (IDisposable __ = Customize.aweXpect.Settings().TestCancellation
				       .Set(TestCancellation.None()))
			{
				stopwatch.Start();
				await That(cancellationToken => Task.Delay(LowTimeout, cancellationToken))
					.DoesNotThrow();
				stopwatch.Stop();
			}
		}

		await That(stopwatch.Elapsed).IsGreaterThanOrEqualTo(LowTimeout).Within(50.Milliseconds());
	}

	[Fact]
	public async Task TestCancellation_PerDefault_ShouldNotCancel()
	{
		Stopwatch stopwatch = new();
		stopwatch.Start();
		await That(cancellationToken => Task.Delay(LowTimeout, cancellationToken)).DoesNotThrow();
		stopwatch.Stop();

		await That(stopwatch.Elapsed).IsGreaterThanOrEqualTo(LowTimeout).Within(50.Milliseconds());
	}

	[Fact]
	public async Task WithCancellation_OverwritesTheCancellationToken()
	{
		TimeSpan delay = 30.Seconds();
		Stopwatch stopwatch = new();
		using CancellationTokenSource cts = new(20.Seconds());
		CancellationToken cancelledToken = new(true);
		using (IDisposable _ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromCancellationToken(() => cts.Token)))
		{
			async Task Act()
				=> await That(cancellationToken => Task.Delay(delay, cancellationToken))
					.Throws<TaskCanceledException>()
					.WithCancellation(cancelledToken);

			stopwatch.Start();
			await That(Act).Throws<InconclusiveException>()
				.WithMessage("""
				             Expected that cancellationToken => Task.Delay(delay, cancellationToken)
				             throws a TaskCanceledException,
				             but it could not be verified, because it was already canceled
				             """);
			stopwatch.Stop();
		}

		await That(stopwatch.Elapsed).IsLessThanOrEqualTo(10.Seconds());
	}

	[Fact]
	public async Task WithTimeout_OverwritesTheCancellationToken()
	{
		TimeSpan delay = 30.Seconds();
		Stopwatch stopwatch = new();
		using (IDisposable _ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromTimeout(20.Seconds())))
		{
			async Task Act()
				=> await That(cancellationToken => Task.Delay(delay, cancellationToken))
					.Throws<TaskCanceledException>()
					.WithTimeout(20.Milliseconds());

			stopwatch.Start();
			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that cancellationToken => Task.Delay(delay, cancellationToken)
				             throws a TaskCanceledException,
				             but it did not finish within 0:00.020
				             """);
			stopwatch.Stop();
		}

		await That(stopwatch.Elapsed).IsLessThanOrEqualTo(10.Seconds());
	}

	private sealed class ChangingClass
	{
		private readonly Stopwatch _stopwatch = new();

		public bool HasMeasuredInterval
		{
			get
			{
				if (!_stopwatch.IsRunning)
				{
					_stopwatch.Start();
					return false;
				}

				_stopwatch.Stop();
				return true;
			}
		}

		public TimeSpan Interval => _stopwatch.Elapsed;
	}

	/// <summary>
	///     The minimal timeout for tests, that have to await this long.
	/// </summary>
	/// <remarks>
	///     It should be as low as possible to have fast tests.
	/// </remarks>
	private TimeSpan LowTimeout { get; } = TimeSpan.FromMilliseconds(100);
}
