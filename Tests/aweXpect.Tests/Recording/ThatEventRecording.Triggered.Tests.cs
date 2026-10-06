using aweXpect.Core;
using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class Triggered
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldSupportEventsWith4Parameters()
			{
				CustomEventWithParametersClass<string, int, bool, DateTime> sut = new();
				IEventRecording<CustomEventWithParametersClass<string, int, bool, DateTime>> recording =
					sut.Record().Events();

				sut.NotifyCustomEvent("foo", 1, true, DateTime.Now);

				async Task Act() =>
					await That(recording)
						.Triggered(nameof(CustomEventWithParametersClass<string, int, bool, DateTime>.CustomEvent));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCustomEventWithoutParametersIsTriggeredOftenEnough_ShouldSucceed()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvent();
				sut.NotifyCustomEvent();
				sut.NotifyCustomEvent();

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.AtLeast(3.Times());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenCustomEventWithoutParametersIsTriggeredTooFewTimes_ShouldFail()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvent();

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.AtLeast(3.Times());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least 3 times,
					             but it was only recorded once in [
					               CustomEvent()
					             ]
					             """);
			}

			[Test]
			public async Task WhenCustomEventWithParametersIsTriggeredTooFewTimes_ShouldFail()
			{
				CustomEventWithParametersClass<string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

				sut.NotifyCustomEvent("foo");
				sut.NotifyCustomEvent("bar");

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithParametersClass<string>.CustomEvent))
						.AtLeast(3.Times());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least 3 times,
					             but it was only recorded twice in [
					               CustomEvent("foo"),
					               CustomEvent("bar")
					             ]
					             """);
			}

			[Test]
			public async Task WhenEventNameIsNull_ShouldThrowArgumentNullException()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				async Task Act() =>
					await That(recording).Triggered(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("eventName").And
					.WithMessage("The 'eventName' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEventRecording<CustomEventWithoutParametersClass>? subject = null;

				async Task Act()
					=> await That(subject!).Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has recorded the CustomEvent event at least once,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenUsingEventWith5Parameters_ShouldFailWithTheReasonItWasNotRecorded()
			{
				CustomEventWithParametersClass<string, int?, bool, DateTime, int> sut = new();
				IEventRecording<CustomEventWithParametersClass<string, int?, bool, DateTime, int>> recording =
					sut.Record().Events();

				async Task Act() =>
					await That(recording).Triggered(
						nameof(CustomEventWithParametersClass<string, int?, bool, DateTime, int>.CustomEvent));

				await That(Act).Throws<NotSupportedException>()
					.WithMessage("The CustomEvent event contains too many parameters (5): [string, int?, bool, DateTime, int]")
					.Because("the recording skips an event it cannot attach to, so the reason surfaces on the expectation that asks for it");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments(3, false)]
			[Arguments(2, true)]
			public async Task AtLeast3_WhenNotificationCountIsEnough_ShouldFail(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				for (int i = 0; i < count; i++)
				{
					sut.NotifyCustomEvent();
				}

				async Task Act() =>
					await That(recording).DoesNotComplyWith(r => r
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.AtLeast(3.Times()));

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut fewer than 3 times,
					             but it was recorded 3 times in [
					               CustomEvent(),
					               CustomEvent(),
					               CustomEvent()
					             ]
					             """);
			}

			[Test]
			[Arguments(1, false)]
			[Arguments(0, true)]
			public async Task AtLeastOnce_WhenNotificationCountIsEnough_ShouldFail(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				for (int i = 0; i < count; i++)
				{
					sut.NotifyCustomEvent();
				}

				async Task Act() =>
					await That(recording).DoesNotComplyWith(r => r
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent)));

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has never recorded the CustomEvent event on sut,
					             but it was recorded once in [
					               CustomEvent()
					             ]
					             """);
			}

			[Test]
			[Arguments(3, false)]
			[Arguments(4, true)]
			public async Task AtMost3_WhenNotificationCountIsTooFew_ShouldFail(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				for (int i = 0; i < count; i++)
				{
					sut.NotifyCustomEvent();
				}

				async Task Act() =>
					await That(recording).DoesNotComplyWith(r => r
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.AtMost(3.Times()));

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut more than 3 times,
					             but it was only recorded 3 times in [
					               CustomEvent(),
					               CustomEvent(),
					               CustomEvent()
					             ]
					             """);
			}


			[Test]
			[Arguments(1, false)]
			[Arguments(2, true)]
			public async Task AtMostOnce_WhenNotificationCountIsEnough_ShouldFail(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				for (int i = 0; i < count; i++)
				{
					sut.NotifyCustomEvent();
				}

				async Task Act() =>
					await That(recording).DoesNotComplyWith(r => r
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.AtMost(1));

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut more than once,
					             but it was only recorded once in [
					               CustomEvent()
					             ]
					             """);
			}

			[Test]
			[Arguments(2, true)]
			[Arguments(3, false)]
			[Arguments(4, false)]
			[Arguments(5, false)]
			[Arguments(6, true)]
			public async Task Between3And5_WhenNotificationCountIsBetween_ShouldFail(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				for (int i = 0; i < count; i++)
				{
					sut.NotifyCustomEvent();
				}

				async Task Act() =>
					await That(recording).DoesNotComplyWith(r => r
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Between(3).And(5.Times()));

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that recording
					              has recorded the CustomEvent event on sut not between 3 and 5 times,
					              but it was recorded {count} times in [*
					              """).AsWildcard();
			}

			[Test]
			[Arguments(3, false)]
			[Arguments(2, true)]
			[Arguments(4, true)]
			public async Task Exactly3_WhenNotificationCountIsExact_ShouldFail(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				for (int i = 0; i < count; i++)
				{
					sut.NotifyCustomEvent();
				}

				async Task Act() =>
					await That(recording).DoesNotComplyWith(r => r
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Exactly(3.Times()));

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut not exactly 3 times,
					             but it was recorded 3 times in [
					               CustomEvent(),
					               CustomEvent(),
					               CustomEvent()
					             ]
					             """);
			}


			[Test]
			[Arguments(1, false)]
			[Arguments(0, true)]
			public async Task ExactlyOnce_WhenNotificationCountIsEnough_ShouldFail(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				for (int i = 0; i < count; i++)
				{
					sut.NotifyCustomEvent();
				}

				async Task Act() =>
					await That(recording).DoesNotComplyWith(r => r
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Exactly(1));

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut not exactly once,
					             but it was recorded once in [
					               CustomEvent()
					             ]
					             """);
			}

			[Test]
			[Arguments(2, false)]
			[Arguments(1, true)]
			public async Task ExactlyTwiceWithin_WhenNotificationCountIsExact_ShouldFail(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				for (int i = 0; i < count; i++)
				{
					sut.NotifyCustomEvent();
				}

				async Task Act() =>
					await That(recording).DoesNotComplyWith(r => r
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Within(40.Milliseconds())
						.Twice());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut not exactly twice within 0:00.040,
					             but it was recorded twice in [
					               CustomEvent(),
					               CustomEvent()
					             ] within 0:*
					             """).AsWildcard();
			}

			[Test]
			[Arguments(3, false)]
			[Arguments(4, true)]
			public async Task LessThan4_WhenNotificationCountIsTooFew_ShouldFail(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				for (int i = 0; i < count; i++)
				{
					sut.NotifyCustomEvent();
				}

				async Task Act() =>
					await That(recording).DoesNotComplyWith(r => r
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.LessThan(4.Times()));

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least 4 times,
					             but it was only recorded 3 times in [
					               CustomEvent(),
					               CustomEvent(),
					               CustomEvent()
					             ]
					             """);
			}

			[Test]
			[Arguments(3, false)]
			[Arguments(2, true)]
			public async Task MoreThan2_WhenNotificationCountIsEnough_ShouldFail(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				for (int i = 0; i < count; i++)
				{
					sut.NotifyCustomEvent();
				}

				async Task Act() =>
					await That(recording).DoesNotComplyWith(r => r
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.MoreThan(2.Times()));

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at most twice,
					             but it was recorded 3 times in [
					               CustomEvent(),
					               CustomEvent(),
					               CustomEvent()
					             ]
					             """);
			}


			[Test]
			[Arguments(1, true)]
			[Arguments(0, false)]
			public async Task Never_WhenNotificationCountIsEnough_ShouldFail(int count, bool expectSuccess)
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				for (int i = 0; i < count; i++)
				{
					sut.NotifyCustomEvent();
				}

				async Task Act() =>
					await That(recording).DoesNotComplyWith(r => r
						.Triggered(nameof(CustomEventWithoutParametersClass.CustomEvent))
						.Never());

				await That(Act).Throws<FailException>().OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once,
					             but it was never recorded
					             """);
			}
		}
	}
}
