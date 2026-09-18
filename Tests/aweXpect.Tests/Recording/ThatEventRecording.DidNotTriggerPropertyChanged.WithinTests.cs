using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class DidNotTriggerPropertyChanged
	{
		public sealed class WithinTests
		{
			[Fact]
			public async Task WhenEventIsNotTriggeredWithinTimeout_ShouldSucceed()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				_ = Task.Delay(2000.Milliseconds())
					.ContinueWith(_ => sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue)));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged()
						.Within(10.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventIsTriggeredWithinTimeout_ShouldFail()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				_ = Task.Delay(20.Milliseconds())
					.ContinueWith(_ => sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue)));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChanged()
						.Within(5.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the PropertyChanged event on sut within 0:05,
					             but it was recorded once in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 0
					                 }, PropertyChangedEventArgs {
					                   PropertyName = "MyValue"
					                 })
					             ] after 0:*
					             """).AsWildcard();
			}
		}
	}
}
