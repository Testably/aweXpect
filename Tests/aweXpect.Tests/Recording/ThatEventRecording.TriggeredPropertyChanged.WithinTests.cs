using aweXpect.Core;
using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class TriggeredPropertyChanged
	{
		public sealed class WithinTests
		{
			[Fact]
			public async Task TriggersPropertyChangedFor_WhenEventIsNotTriggeredOftenEnoughWithinTimeout_ShouldFail()
			{
				PropertyChangedClass sut = new();

				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				_ = Task.Delay(2000.Milliseconds())
					.ContinueWith(_ =>
					{
						sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));
						sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));
						sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));
					});

				async Task Act() =>
					await That(recording).TriggeredPropertyChanged()
						.Within(10.Milliseconds())
						.AtLeast(3.Times());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut at least 3 times within 0:00.010,
					             but it was never recorded in [] within 0:*
					             """).AsWildcard();
			}

			[Fact]
			public async Task TriggersPropertyChangedFor_WhenEventIsTriggeredOftenEnoughWithinTimeout_ShouldSucceed()
			{
				PropertyChangedClass sut = new();

				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				_ = Task.Delay(20.Milliseconds())
					.ContinueWith(_ =>
					{
						sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));
						sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));
						sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));
					});

				async Task Act() =>
					await That(recording).TriggeredPropertyChanged()
						.Within(5.Seconds())
						.AtLeast(3.Times());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEventRecording<PropertyChangedClass>? subject = null;

				async Task Act()
					=> await That(subject!).TriggeredPropertyChanged().Within(5671.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recorded the PropertyChanged event at least once within 0:05.671,
					             but it was <null>
					             """);
			}
		}
	}
}
