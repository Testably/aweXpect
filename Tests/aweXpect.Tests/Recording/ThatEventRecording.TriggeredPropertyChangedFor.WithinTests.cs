using aweXpect.Core;
using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class TriggeredPropertyChangedFor
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
					await That(recording).TriggeredPropertyChangedFor(x => x.MyValue)
						.Within(10.Milliseconds())
						.AtLeast(3.Times());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut for property MyValue at least 3 times within 0:00.010,
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
					await That(recording).TriggeredPropertyChangedFor(x => x.MyValue)
						.Within(5.Seconds())
						.AtLeast(3.Times());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEventRecording<PropertyChangedClass>? subject = null;

				async Task Act()
					=> await That(subject!).TriggeredPropertyChangedFor(x => x.MyValue).Within(5678.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recorded the PropertyChanged event for property MyValue at least once within 0:05.678,
					             but it was <null>
					             """);
			}
		}
	}
}
