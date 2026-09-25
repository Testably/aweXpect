using System.Threading;
using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class Triggered
	{
		public sealed class WithinTests
		{
			[Fact]
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

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at most once within 0:05,
					             but it was recorded twice in [
					               CustomEvent(),
					               CustomEvent()
					             ] after 0:*
					             """).AsWildcard();
			}

			[Fact]
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

			[Fact]
			public async Task Never_WhenEventIsNotTriggeredWithinTimeout_ShouldSucceed()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording =
					sut.Record().Events();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(2000.Milliseconds(), token)
					.ContinueWith(_ => sut.NotifyCustomEvent(), token);

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(10.Milliseconds())
						.Never();

				await That(Act).DoesNotThrow();
				cts.Cancel();
			}

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the CustomEvent event on sut within 0:05,
					             but it was recorded once in [
					               CustomEvent()
					             ] after 0:*
					             """).AsWildcard();
			}

			[Fact]
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

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the CustomEvent event on sut within 0:05,
					             but it was recorded once in [
					               CustomEvent()
					             ] after 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task WhenEventWith1ParameterIsNotTriggeredWithinTimeout_ShouldFail()
			{
				CustomEventWithParametersClass<string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string>> recording =
					sut.Record().Events();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(2000.Milliseconds(), token)
					.ContinueWith(_ => sut.NotifyCustomEvent("foo"), token);

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent))
						.Within(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once within 0:00.010,
					             but it was never recorded in [] within 0:*
					             """).AsWildcard();
				cts.Cancel();
			}

			[Fact]
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

			[Fact]
			public async Task WhenEventWith2ParametersIsNotTriggeredWithinTimeout_ShouldFail()
			{
				CustomEventWithParametersClass<string, int> sut = new();
				IEventRecording<CustomEventWithParametersClass<string, int>> recording =
					sut.Record().Events();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(2000.Milliseconds(), token)
					.ContinueWith(_ => sut.NotifyCustomEvent("foo", 1), token);

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string, int>.CustomEvent))
						.Within(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once within 0:00.010,
					             but it was never recorded in [] within 0:*
					             """).AsWildcard();
				cts.Cancel();
			}

			[Fact]
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

			[Fact]
			public async Task WhenEventWith3ParametersIsNotTriggeredWithinTimeout_ShouldFail()
			{
				CustomEventWithParametersClass<string, int, bool> sut = new();
				IEventRecording<CustomEventWithParametersClass<string, int, bool>> recording =
					sut.Record().Events();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(2000.Milliseconds(), token)
					.ContinueWith(_ => sut.NotifyCustomEvent("foo", 1, true), token);

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string, int, bool>.CustomEvent))
						.Within(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once within 0:00.010,
					             but it was never recorded in [] within 0:*
					             """).AsWildcard();
				cts.Cancel();
			}

			[Fact]
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

			[Fact]
			public async Task WhenEventWith4ParametersIsNotTriggeredWithinTimeout_ShouldFail()
			{
				CustomEventWithParametersClass<string, int, bool, DateTime> sut = new();
				IEventRecording<CustomEventWithParametersClass<string, int, bool, DateTime>> recording =
					sut.Record().Events();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(2000.Milliseconds(), token)
					.ContinueWith(_ => sut.NotifyCustomEvent("foo", 1, true, DateTime.Now), token);

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string, int, bool, DateTime>.CustomEvent))
						.Within(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once within 0:00.010,
					             but it was never recorded in [] within 0:*
					             """).AsWildcard();
				cts.Cancel();
			}

			[Fact]
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

			[Fact]
			public async Task WhenEventWithoutParametersIsNotTriggeredWithinTimeout_ShouldFail()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording =
					sut.Record().Events();
				using CancellationTokenSource cts = new();
				CancellationToken token = cts.Token;

				_ = Task.Delay(2000.Milliseconds(), token)
					.ContinueWith(_ => sut.NotifyCustomEvent(), token);

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once within 0:00.010,
					             but it was never recorded in [] within 0:*
					             """).AsWildcard();
				cts.Cancel();
			}

			[Fact]
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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEventRecording<CustomEventWithoutParametersClass>? subject = null;

				async Task Act()
					=> await That(subject!).Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(4.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recorded the CustomEvent event at least once within 0:04,
					             but it was <null>
					             """);
			}

			[Fact]
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
						.Within(System.Threading.Timeout.InfiniteTimeSpan)
						.WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once,
					             but it could not be verified, because it was already canceled
					             """).WithTimeout(10.Seconds())
					.Because("an infinite timeout imposes no limit, so only the cancellation ends the wait");
			}

			[Fact]
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
						.Within(System.Threading.Timeout.InfiniteTimeSpan);

				await That(Act).DoesNotThrow().WithTimeout(10.Seconds());
			}

			[Fact]
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

			[Theory]
			[InlineData(false)]
			[InlineData(true)]
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

			[Fact]
			public async Task WhenTimeoutIsZero_ShouldMentionTheTimeout()
			{
				IEventRecording<CustomEventWithoutParametersClass>? subject = null;

				async Task Act()
					=> await That(subject!).Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(TimeSpan.Zero);

				await That(Act).Throws<XunitException>()
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
