using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed class DidNotTrigger
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAllRecordedEventsAreFilteredOut_ShouldSucceed()
			{
				CustomEventWithParametersClass<string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

				sut.NotifyCustomEvent("foo");

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithParametersClass<string>.CustomEvent))
						.WithParameter<string>(s => s == "bar");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventIsNotTriggered_ShouldSucceed()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventIsTriggered_ShouldFail()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvent();

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the CustomEvent event on sut,
					             but it was recorded once in [
					               CustomEvent()
					             ]
					             """);
			}

			[Fact]
			public async Task WhenMatchingEventIsTriggered_ShouldFail()
			{
				CustomEventWithParametersClass<string> sut = new();
				IEventRecording<CustomEventWithParametersClass<string>> recording = sut.Record().Events();

				sut.NotifyCustomEvent("foo");

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(CustomEventWithParametersClass<string>.CustomEvent))
						.WithParameter<string>(s => s == "foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the CustomEvent event on sut with string parameter s => s == "foo",
					             but it was recorded once in [
					               CustomEvent("foo")
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEventRecording<CustomEventWithoutParametersClass>? subject = null;

				async Task Act()
					=> await That(subject!).DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has never recorded the CustomEvent event,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventIsNotTriggered_ShouldFail()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				async Task Act() =>
					await That(recording).DoesNotComplyWith(r => r
						.DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the CustomEvent event on sut at least once,
					             but it was never recorded in []
					             """);
			}

			[Fact]
			public async Task WhenEventIsTriggered_ShouldSucceed()
			{
				CustomEventWithoutParametersClass sut = new();
				IEventRecording<CustomEventWithoutParametersClass> recording = sut.Record().Events();

				sut.NotifyCustomEvent();

				async Task Act() =>
					await That(recording).DoesNotComplyWith(r => r
						.DidNotTrigger(nameof(CustomEventWithoutParametersClass.CustomEvent)));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
