using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Customization;
using aweXpect.Signaling;

namespace aweXpect.Core.Tests.Customization;

public sealed class CustomizeSettingsTests
{
	/// <summary>
	///     The minimal timeout for tests, that have to await this long.
	/// </summary>
	/// <remarks>
	///     It should be as low as possible to have fast tests.
	/// </remarks>
	private TimeSpan LowTimeout { get; } = TimeSpan.FromMilliseconds(100);

	[Test]
	public async Task DefaultCheckInterval_ShouldBeUsedInTimeComparisons()
	{
		TimeSpan timeout = 2.Seconds();
		VirtualTimeSystem time = new();

		ChangingClass sut1 = new(time);
		ChangingClass sut2 = new(time);

		await That(Customize.aweXpect.Settings().DefaultCheckInterval.Get()).IsEqualTo(100.Milliseconds());
		await That(sut1).Satisfies(x => x.HasMeasuredInterval).Within(30.Seconds()).WithTimeSystem(time);
		await That(sut1.Interval).IsEqualTo(100.Milliseconds());
		using (IDisposable __ = Customize.aweXpect.Settings().DefaultCheckInterval.Set(timeout))
		{
			await That(sut2).Satisfies(x => x.HasMeasuredInterval).Within(30.Seconds()).WithTimeSystem(time);
			await That(sut2.Interval).IsEqualTo(timeout);
		}
	}

	[Test]
	[Arguments(0)]
	[Arguments(-1)]
	[Arguments(-5)]
	public async Task DefaultCheckInterval_WhenNotPositive_ShouldThrowArgumentOutOfRangeException(int milliseconds)
	{
		void Act() => Customize.aweXpect.Settings().DefaultCheckInterval.Set(milliseconds.Milliseconds());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("interval").And
			.WithMessage("The interval must be positive.").AsPrefix()
			.Because("an interval is validated like CheckEvery, which rejects zero, negative and infinite intervals");
		await That(Customize.aweXpect.Settings().DefaultCheckInterval.Get()).IsEqualTo(100.Milliseconds());
	}

	[Test]
	public async Task DefaultEventuallyTimeout_ShouldBeUsedInEventually()
	{
		await That(Customize.aweXpect.Settings().DefaultEventuallyTimeout.Get()).IsEqualTo(30.Seconds());
		VirtualTimeSystem time = new();
		using (IDisposable __ = Customize.aweXpect.Settings().DefaultEventuallyTimeout.Set(LowTimeout))
		{
			await That(Customize.aweXpect.Settings().DefaultEventuallyTimeout.Get()).IsEqualTo(LowTimeout);
			async Task Act() => await That(() => 1).Eventually().OnVirtualTime(time).IsEqualTo(2);
			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that () => 1
				             eventually is equal to 2 within 0:00.100,
				             but it was 1, which differs by -1
				             """);
		}

		await That(time.Now).IsEqualTo(LowTimeout);
		await That(Customize.aweXpect.Settings().DefaultEventuallyTimeout.Get()).IsEqualTo(30.Seconds());
	}

	[Test]
	public async Task DefaultEventuallyTimeout_WhenNegative_ShouldThrowArgumentOutOfRangeException()
	{
		void Act() => Customize.aweXpect.Settings().DefaultEventuallyTimeout.Set(-5.Milliseconds());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("timeout").And
			.WithMessage("The timeout must not be negative.").AsPrefix();
		await That(Customize.aweXpect.Settings().DefaultEventuallyTimeout.Get()).IsEqualTo(30.Seconds());
	}

	[Test]
	[Arguments(0)]
	[Arguments(-1)]
	public async Task DefaultEventuallyTimeout_WhenZeroOrInfinite_ShouldBeAccepted(int milliseconds)
	{
		TimeSpan timeout = milliseconds.Milliseconds();

		using (IDisposable __ = Customize.aweXpect.Settings().DefaultEventuallyTimeout.Set(timeout))
		{
			await That(Customize.aweXpect.Settings().DefaultEventuallyTimeout.Get()).IsEqualTo(timeout)
				.Because("-1 ms is the infinite timeout, which imposes no limit");
		}
	}

	[Test]
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

	[Test]
	public async Task DefaultSignalerTimeout_WhenNegative_ShouldThrowArgumentOutOfRangeException()
	{
		void Act() => Customize.aweXpect.Settings().DefaultSignalerTimeout.Set(-5.Milliseconds());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("timeout").And
			.WithMessage("The timeout must not be negative.").AsPrefix();
		await That(Customize.aweXpect.Settings().DefaultSignalerTimeout.Get()).IsEqualTo(30.Seconds());
	}

	[Test]
	[Arguments(0)]
	[Arguments(-1)]
	public async Task DefaultSignalerTimeout_WhenZeroOrInfinite_ShouldBeAccepted(int milliseconds)
	{
		TimeSpan timeout = milliseconds.Milliseconds();

		using (IDisposable __ = Customize.aweXpect.Settings().DefaultSignalerTimeout.Set(timeout))
		{
			await That(Customize.aweXpect.Settings().DefaultSignalerTimeout.Get()).IsEqualTo(timeout)
				.Because("-1 ms is the infinite timeout, which imposes no limit");
		}
	}

	[Test]
	public async Task DefaultTimeComparisonTimeout_ShouldBeUsedInTimeComparisons()
	{
		DateTime time = DateTime.UtcNow;
		DateTime otherTime = time.AddMilliseconds(10);
		await That(Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get()).IsEqualTo(TimeSpan.Zero);
		async Task Act() => await That(time).IsEqualTo(otherTime);
		await That(Act).Throws<FailException>()
			.WithMessage($"""
			              Expected that time
			              is equal to {Formatter.Format(otherTime)},
			              but it was {Formatter.Format(time)}, which differs by -0:00.010
			              """);
		using (IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(10.Milliseconds()))
		{
			await That(Act).DoesNotThrow();
		}

		await That(Act).Throws<FailException>()
			.WithMessage($"""
			              Expected that time
			              is equal to {Formatter.Format(otherTime)},
			              but it was {Formatter.Format(time)}, which differs by -0:00.010
			              """)
			.Because("the default tolerance must be restored once the customization is disposed");
	}

	[Test]
	[Arguments(-1)]
	[Arguments(-5)]
	public async Task DefaultTimeComparisonTolerance_WhenNegative_ShouldThrowArgumentOutOfRangeException(
		int milliseconds)
	{
		void Act() => Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(milliseconds.Milliseconds());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("tolerance").And
			.WithMessage("The tolerance must not be negative.").AsPrefix()
			.Because("a negative default tolerance tightens every time comparison instead of widening it");
	}

	[Test]
	public async Task DefaultTimeComparisonTolerance_WhenZero_ShouldBeAccepted()
	{
		using (IDisposable __ = Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Set(TimeSpan.Zero))
		{
			await That(Customize.aweXpect.Settings().DefaultTimeComparisonTolerance.Get()).IsEqualTo(TimeSpan.Zero);
		}
	}

	[Test]
	public async Task Settings_ShouldReturnSameInstance()
	{
		AwexpectCustomization.SettingsCustomization settings1 = Customize.aweXpect.Settings();
		AwexpectCustomization.SettingsCustomization settings2 = Customize.aweXpect.Settings();

		await That(settings1).IsSameAs(settings2);
	}

	[Test]
	public async Task TestCancellation_FromCancellationToken_ShouldBeApplied()
	{
		TimeSpan delay = 30.Seconds();
		VirtualTimeSystem time = new();
		CancellationToken cancelledToken = new(true);
		using (IDisposable __ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromCancellationToken(() => cancelledToken)))
		{
			async Task Act()
				=> await That(cancellationToken => time.Delay(delay, cancellationToken))
					.DoesNotThrow().WithTimeSystem(time);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that cancellationToken => time.Delay(delay, cancellationToken)
				             does not throw any exception,
				             but it could not be verified, because the evaluation was already canceled
				             """);
		}

		await That(time.Now).IsEqualTo(TimeSpan.Zero)
			.Because("the canceled evaluation must not wait for the subject");
	}

	[Test]
	public async Task TestCancellation_FromCancellationToken_WhenTheFactoryThrows_ShouldThrowTheException()
	{
		MyException exception = new("the factory failed");
		Exception? thrown;
		using (IDisposable __ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromCancellationToken(() => throw exception)))
		{
			async Task Act()
				=> await That(true).IsTrue();

			thrown = await Catch.ExceptionAsync(Act);
		}

		await That(thrown).IsSameAs(exception)
			.Because("a broken customization is no failure of the expectation");
	}

	[Test]
	public async Task TestCancellation_FromTimeout_ShouldBeApplied()
	{
		TimeSpan delay = 30.Seconds();
		VirtualTimeSystem time = new();
		Exception? exception;
		using (IDisposable __ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromTimeout(LowTimeout)))
		{
			async Task Act()
				=> await That(cancellationToken => time.Delay(delay, cancellationToken))
					.Throws<TaskCanceledException>().WithTimeSystem(time);

			exception = await Catch.ExceptionAsync(Act);
		}

		await That(exception).IsExactly<FailException>().And
			.HasMessage("""
			            Expected that cancellationToken => time.Delay(delay, cancellationToken)
			            throws a TaskCanceledException,
			            but it did not finish within 0:00.100
			            """);
		await That(time.Now).IsEqualTo(LowTimeout)
			.Because("the timeout of the test cancellation ends the wait for the subject");
	}

	[Test]
	public async Task TestCancellation_FromTimeout_WhenInfinite_ShouldBeAccepted()
	{
		using (IDisposable __ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromTimeout(Timeout.InfiniteTimeSpan)))
		{
			async Task Act()
				=> await That(1).IsEqualTo(1);

			await That(Act).DoesNotThrow()
				.Because("the infinite timeout imposes no limit");
		}
	}

	[Test]
	public async Task TestCancellation_FromTimeout_WhenNegative_ShouldThrowArgumentOutOfRangeException()
	{
		void Act() => TestCancellation.FromTimeout(-5.Seconds());

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithParamName("timeout").And
			.WithMessage("The timeout must not be negative.").AsPrefix();
	}

	[Test]
	public async Task TestCancellation_FromTimeout_WhenWithTimeoutIsLonger_ShouldApplyTheTestCancellationTimeout()
	{
		VirtualTimeSystem time = new();
		Exception? exception;
		using (IDisposable __ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromTimeout(LowTimeout)))
		{
			async Task Act()
				=> await That(cancellationToken => time.Delay(30.Seconds(), cancellationToken))
					.Throws<TaskCanceledException>()
					.WithTimeout(20.Seconds()).WithTimeSystem(time);

			exception = await Catch.ExceptionAsync(Act);
		}

		await That(exception).IsExactly<FailException>().And
			.HasMessage("""
			            Expected that cancellationToken => time.Delay(30.Seconds(), cancellationToken)
			            throws a TaskCanceledException,
			            but it did not finish within 0:00.100
			            """)
			.Because("the tighter limit wins, so a longer local timeout must not loosen the global one");
	}

	[Test]
	public async Task TestCancellation_None_ShouldBeApplied()
	{
		VirtualTimeSystem time = new();
		CancellationToken cancelledToken = new(true);
		using (IDisposable _ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromCancellationToken(() => cancelledToken)))
		{
			using (IDisposable __ = Customize.aweXpect.Settings().TestCancellation
				       .Set(TestCancellation.None()))
			{
				await That(cancellationToken => time.Delay(LowTimeout, cancellationToken))
					.DoesNotThrow().WithTimeSystem(time);
			}
		}

		await That(time.Now).IsEqualTo(LowTimeout)
			.Because("without a test cancellation the subject runs to its end");
	}

	[Test]
	public async Task TestCancellation_PerDefault_ShouldNotCancel()
	{
		VirtualTimeSystem time = new();

		await That(cancellationToken => time.Delay(LowTimeout, cancellationToken)).DoesNotThrow()
			.WithTimeSystem(time);

		await That(time.Now).IsEqualTo(LowTimeout)
			.Because("without a test cancellation the subject runs to its end");
	}

	[Test]
	public async Task WithCancellation_OverwritesTheCancellationToken()
	{
		TimeSpan delay = 30.Seconds();
		VirtualTimeSystem time = new();
		using CancellationTokenSource cts = new();
		CancellationToken cancelledToken = new(true);
		using (IDisposable _ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromCancellationToken(() => cts.Token)))
		{
			async Task Act()
				=> await That(cancellationToken => time.Delay(delay, cancellationToken))
					.Throws<TaskCanceledException>()
					.WithCancellation(cancelledToken).WithTimeSystem(time);

			await That(Act).Throws<InconclusiveTestException>()
				.WithMessage("""
				             Expected that cancellationToken => time.Delay(delay, cancellationToken)
				             throws a TaskCanceledException,
				             but it could not be verified, because the evaluation was already canceled
				             """);
		}

		await That(time.Now).IsEqualTo(TimeSpan.Zero)
			.Because("the canceled evaluation must not wait for the subject");
	}

	[Test]
	public async Task WithTimeout_WhenShorterThanTheTestCancellationTimeout_ShouldBeApplied()
	{
		TimeSpan delay = 30.Seconds();
		VirtualTimeSystem time = new();
		using (IDisposable _ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromTimeout(20.Seconds())))
		{
			async Task Act()
				=> await That(cancellationToken => time.Delay(delay, cancellationToken))
					.Throws<TaskCanceledException>()
					.WithTimeout(20.Milliseconds()).WithTimeSystem(time);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that cancellationToken => time.Delay(delay, cancellationToken)
				             throws a TaskCanceledException,
				             but it did not finish within 0:00.020
				             """);
		}

		await That(time.Now).IsEqualTo(20.Milliseconds())
			.Because("the shorter timeout of WithTimeout ends the wait, not the test cancellation");
	}

	private sealed class ChangingClass(VirtualTimeSystem time)
	{
		private long? _startTimestamp;

		public bool HasMeasuredInterval
		{
			get
			{
				if (_startTimestamp is null)
				{
					_startTimestamp = time.GetTimestamp();
					return false;
				}

				Interval = time.GetElapsedTime(_startTimestamp.Value);
				return true;
			}
		}

		public TimeSpan Interval { get; private set; }
	}
}
