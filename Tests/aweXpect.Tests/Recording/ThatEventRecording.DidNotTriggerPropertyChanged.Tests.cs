using aweXpect.Recording;

// ReSharper disable AccessToDisposedClosure

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class DidNotTriggerPropertyChanged
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventIsNotTriggered_ShouldSucceed()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged();

				await That(Act).DoesNotThrow();
			}

			[Fact]
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

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEventRecording<PropertyChangedClass>? subject = null;

				async Task Act()
					=> await That(subject!).DidNotTriggerPropertyChanged();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has never recorded the PropertyChanged event,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventIsNotTriggered_ShouldFail()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				async Task Act() =>
					await That(recording).DoesNotComplyWith(n => n.DidNotTriggerPropertyChanged());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut at least once,
					             but it was never recorded in []
					             """);
			}

			[Fact]
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
