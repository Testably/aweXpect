using aweXpect.Recording;

// ReSharper disable AccessToDisposedClosure

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class DidNotTriggerPropertyChanged
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenEventIsNotTriggered_ShouldSucceed()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEventIsTriggered_ShouldFail()
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 422,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged("SomeArbitraryProperty");

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the PropertyChanged event on sut,
					             but it was recorded once in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 422
					                 }, PropertyChangedEventArgs {
					                   PropertyName = "SomeArbitraryProperty"
					                 })
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEventRecording<PropertyChangedClass>? subject = null;

				async Task Act()
					=> await That(subject!).DidNotTriggerPropertyChanged();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has never recorded the PropertyChanged event,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventIsNotTriggered_ShouldFail()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				async Task Act() =>
					await That(recording).DoesNotComplyWith(n => n.DidNotTriggerPropertyChanged());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut at least once,
					             but it was never recorded
					             """);
			}

			[Test]
			public async Task WhenEventIsTriggered_ShouldSucceed()
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 422,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged("SomeArbitraryProperty");

				async Task Act() =>
					await That(recording).DoesNotComplyWith(n => n.DidNotTriggerPropertyChanged());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
