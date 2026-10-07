using System.Threading;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Customization;
using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class Triggered
	{
		public sealed class WithinTests
		{
			[Test]
			public async Task AtMost_WhenEventIsTriggeredFewEnoughTimesWithinTimeout_ShouldSucceed()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording =
					sut.Record().Events();

				sut.NotifyCustomEvent();

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(50.Milliseconds())
						.AtMost(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task AtMost_WhenEventIsTriggeredTooOftenWithinTimeout_ShouldFail()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording =
					sut.Record().Events();

				_ = Task.Delay(20.Milliseconds())
					.ContinueWith(_ => sut.NotifyCustomEvents(2));

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(5.Seconds())
						.AtMost(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at most once within 0:05,
					             but it was recorded twice in [
					               CustomEvent(),
					               CustomEvent()
					             ] after 0:*
					             """).AsWildcard();
			}

			[Test]
			public async Task Filter_WhenSpecifiedAfterTheOccurrenceConstraint_ShouldStillApply()
			{
				CustomEventWithParametersClass<string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string>> recording =
					sut.Record().Events();

				sut.NotifyCustomEvent("foo");

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent))
						.Never()
						.WithParameter<string>(s => s == "bar")
						.Within(50.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task Never_WhenEventIsNotTriggeredWithinTimeout_ShouldSucceed()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording =
					sut.Record().Events();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(30.Seconds(), token)
					.ContinueWith(_ => sut.NotifyCustomEvent(), token);

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(10.Milliseconds())
						.Never()
						.WithTimeSystem(new VirtualTimeSystem());

				await That(Act).DoesNotThrow();
				cts.Cancel();
			}

			[Test]
			public async Task Never_WhenEventIsTriggeredWithinTimeout_ShouldFail()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording =
					sut.Record().Events();

				_ = Task.Delay(20.Milliseconds())
					.ContinueWith(_ => sut.NotifyCustomEvent());

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(5.Seconds())
						.Never();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the CustomEvent event on sut within 0:05,
					             but it was recorded once in [
					               CustomEvent()
					             ] after 0:*
					             """).AsWildcard();
			}

			[Test]
			public async Task Never_WhenSpecifiedAfterTheOccurrenceConstraint_ShouldStillApplyTheTimeout()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording =
					sut.Record().Events();

				_ = Task.Delay(20.Milliseconds())
					.ContinueWith(_ => sut.NotifyCustomEvent());

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Never()
						.Within(5.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the CustomEvent event on sut within 0:05,
					             but it was recorded once in [
					               CustomEvent()
					             ] after 0:*
					             """).AsWildcard();
			}

			[Test]
			public async Task WhenEventWith1ParameterIsNotTriggeredWithinTimeout_ShouldFail()
			{
				CustomEventWithParametersClass<string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string>> recording =
					sut.Record().Events();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(30.Seconds(), token)
					.ContinueWith(_ => sut.NotifyCustomEvent("foo"), token);

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent))
						.Within(10.Milliseconds())
						.WithTimeSystem(new VirtualTimeSystem());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once within 0:00.010,
					             but it was never recorded within 0:00.010
					             """);
				cts.Cancel();
			}

			[Test]
			public async Task WhenEventWith1ParameterIsTriggeredWithinTimeout_ShouldSucceed()
			{
				CustomEventWithParametersClass<string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string>> recording =
					sut.Record().Events();

				_ = Task.Delay(20.Milliseconds())
					.ContinueWith(_ => sut.NotifyCustomEvent("foo"));

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent))
						.Within(5.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventWith2ParametersIsNotTriggeredWithinTimeout_ShouldFail()
			{
				CustomEventWithParametersClass<string, int> sut = new();
				IEventRecording<CustomEventWithParametersClass<string, int>> recording =
					sut.Record().Events();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(30.Seconds(), token)
					.ContinueWith(_ => sut.NotifyCustomEvent("foo", 1), token);

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string, int>.CustomEvent))
						.Within(10.Milliseconds())
						.WithTimeSystem(new VirtualTimeSystem());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once within 0:00.010,
					             but it was never recorded within 0:00.010
					             """);
				cts.Cancel();
			}

			[Test]
			public async Task WhenEventWith2ParametersIsTriggeredWithinTimeout_ShouldSucceed()
			{
				CustomEventWithParametersClass<string, int> sut = new();
				IEventRecording<CustomEventWithParametersClass<string, int>> recording =
					sut.Record().Events();

				_ = Task.Delay(20.Milliseconds())
					.ContinueWith(_ => sut.NotifyCustomEvent("foo", 1));

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string, int>.CustomEvent))
						.Within(5.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventWith3ParametersIsNotTriggeredWithinTimeout_ShouldFail()
			{
				CustomEventWithParametersClass<string, int, bool> sut = new();
				IEventRecording<CustomEventWithParametersClass<string, int, bool>> recording =
					sut.Record().Events();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(30.Seconds(), token)
					.ContinueWith(_ => sut.NotifyCustomEvent("foo", 1, true), token);

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string, int, bool>.CustomEvent))
						.Within(10.Milliseconds())
						.WithTimeSystem(new VirtualTimeSystem());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once within 0:00.010,
					             but it was never recorded within 0:00.010
					             """);
				cts.Cancel();
			}

			[Test]
			public async Task WhenEventWith3ParametersIsTriggeredWithinTimeout_ShouldSucceed()
			{
				CustomEventWithParametersClass<string, int, bool> sut = new();
				IEventRecording<CustomEventWithParametersClass<string, int, bool>> recording =
					sut.Record().Events();

				_ = Task.Delay(20.Milliseconds())
					.ContinueWith(_ => sut.NotifyCustomEvent("foo", 1, true));

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string, int, bool>.CustomEvent))
						.Within(5.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventWith4ParametersIsNotTriggeredWithinTimeout_ShouldFail()
			{
				CustomEventWithParametersClass<string, int, bool, DateTime> sut = new();
				IEventRecording<CustomEventWithParametersClass<string, int, bool, DateTime>> recording =
					sut.Record().Events();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(30.Seconds(), token)
					.ContinueWith(_ => sut.NotifyCustomEvent("foo", 1, true, DateTime.Now), token);

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string, int, bool, DateTime>.CustomEvent))
						.Within(10.Milliseconds())
						.WithTimeSystem(new VirtualTimeSystem());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once within 0:00.010,
					             but it was never recorded within 0:00.010
					             """);
				cts.Cancel();
			}

			[Test]
			public async Task WhenEventWith4ParametersIsTriggeredWithinTimeout_ShouldSucceed()
			{
				CustomEventWithParametersClass<string, int, bool, DateTime> sut = new();
				IEventRecording<CustomEventWithParametersClass<string, int, bool, DateTime>> recording =
					sut.Record().Events();

				_ = Task.Delay(20.Milliseconds())
					.ContinueWith(_ => sut.NotifyCustomEvent("foo", 1, true, DateTime.Now));

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string, int, bool, DateTime>.CustomEvent))
						.Within(5.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventWithoutParametersIsNotTriggeredWithinTimeout_ShouldFail()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording =
					sut.Record().Events();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(30.Seconds(), token)
					.ContinueWith(_ => sut.NotifyCustomEvent(), token);

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(10.Milliseconds())
						.WithTimeSystem(new VirtualTimeSystem());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once within 0:00.010,
					             but it was never recorded within 0:00.010
					             """);
				cts.Cancel();
			}

			[Test]
			public async Task WhenEventWithoutParametersIsTriggeredWithinTimeout_ShouldSucceed()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording =
					sut.Record().Events();

				_ = Task.Delay(20.Milliseconds())
					.ContinueWith(_ => sut.NotifyCustomEvent());

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(5.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEventRecording<CustomEventWithoutParametersClass>? subject = null;

				async Task Act()
					=> await That(subject!).Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(4.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has recorded the CustomEvent event at least once within 0:04,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenTheOuterTimeoutIsAsLong_WhenNotTriggered_ShouldFailWithTheEventsWithinTheTimeout()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording =
					sut.Record().Events();

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(200.Milliseconds()).WithTimeout(200.Milliseconds())
						.WithTimeSystem(new VirtualTimeSystem());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once within 0:00.200,
					             but it was never recorded within 0:00.200
					             """)
					.Because("an outer timeout that is not shorter than Within must not decide the outcome");
			}

			[Test]
			public async Task
				WhenTheTestCancellationTimeoutIsShorterAndWithTimeoutIsLonger_ShouldFailWithTheTestCancellationTimeout()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording =
					sut.Record().Events();
				Exception? exception;
				using (IDisposable _ = Customize.aweXpect.Settings().TestCancellation
					       .Set(TestCancellation.FromTimeout(300.Milliseconds())))
				{
					async Task Act() =>
						await That(recording)
							.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
							.Within(30.Seconds()).WithTimeout(60.Seconds())
							.WithTimeSystem(new VirtualTimeSystem());

					exception = await Catch.ExceptionAsync(Act);
				}

				await That(exception).IsExactly<FailException>().And
					.HasMessage("""
					            Expected that recording
					            has recorded the CustomEvent event on sut at least once within 0:30,
					            but it did not finish within 0:00.300
					            """).And
					.HasInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.300."))
					.Because("the effective timeout is the tighter of WithTimeout and TestCancellation");
			}

			[Test]
			public async Task WhenTimeoutIsInfinite_ShouldNotMentionTheTimeout()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording =
					sut.Record().Events();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(Timeout.InfiniteTimeSpan)
						.WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once,
					             but it could not be verified, because the evaluation was already canceled
					             """).WithTimeout(10.Seconds())
					.Because("an infinite timeout imposes no limit, so only the cancellation ends the wait");
			}

			[Test]
			public async Task WhenTimeoutIsInfinite_ShouldWaitUntilTheEventIsTriggered()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording =
					sut.Record().Events();

				_ = Task.Delay(20.Milliseconds())
					.ContinueWith(_ => sut.NotifyCustomEvent());

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(Timeout.InfiniteTimeSpan);

				await That(Act).DoesNotThrow().WithTimeout(10.Seconds());
			}

			[Test]
			public async Task WhenTimeoutIsNegative_ShouldThrowArgumentOutOfRangeException()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording =
					sut.Record().Events();

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(-5.Milliseconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.WithParamName("timeout").And
					.WithMessage("The timeout must not be negative.").AsPrefix();
			}

			[Test]
			[Arguments(false)]
			[Arguments(true)]
			public async Task WhenTimeoutIsSpecifiedTwice_ShouldThrowInvalidOperationException(bool never)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording =
					sut.Record().Events();

				async Task Act()
				{
					if (never)
					{
						await That(recording).Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
							.Never().Within(1.Seconds()).Within(2.Seconds());
					}
					else
					{
						await That(recording).Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
							.Within(1.Seconds()).Within(2.Seconds());
					}
				}

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Within cannot be specified more than once.")
					.Because("the second timeout would silently replace the first one");
			}

			[Test]
			public async Task WhenTimeoutIsZero_ShouldMentionTheTimeout()
			{
				IEventRecording<CustomEventWithoutParametersClass>? subject = null;

				async Task Act()
					=> await That(subject!).Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(TimeSpan.Zero);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has recorded the CustomEvent event at least once within 0:00,
					             but it was <null>
					             """)
					.Because("an explicit timeout is named like on a signaler, even when it is zero");
			}
		}
	}
}
