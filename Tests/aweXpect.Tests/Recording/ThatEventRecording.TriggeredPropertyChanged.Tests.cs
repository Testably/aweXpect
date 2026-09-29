using System.ComponentModel;
using aweXpect.Core;
using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class TriggeredPropertyChanged
	{
		public sealed class Tests
		{
			[Fact]
			public async Task TriggersPropertyChangedFor_WhenEventIsTriggeredOftenEnough_ShouldSucceed()
			{
				PropertyChangedClass sut = new();

				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));
				sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));
				sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));

				async Task Act() =>
					await That(recording).TriggeredPropertyChanged()
						.AtLeast(3.Times());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task TriggersPropertyChangedFor_WhenEventIsTriggeredTooFewTimes_ShouldFail()
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 42,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));

				async Task Act() =>
					await That(recording).TriggeredPropertyChanged()
						.AtLeast(2.Times());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut at least twice,
					             but it was recorded once in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 42
					                 }, PropertyChangedEventArgs {
					                   PropertyName = "MyValue"
					                 })
					             ]
					             """);
			}

			[Fact]
			public async Task WhenPropertyChangedEventIsTriggeredOftenEnough_ShouldSucceed()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged("foo");
				sut.NotifyPropertyChanged("foo");
				sut.NotifyPropertyChanged("foo");

				async Task Act() =>
					await That(recording).TriggeredPropertyChanged()
						.With<PropertyChangedEventArgs>(e => e.PropertyName == "foo")
						.AtLeast(3.Times());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPropertyChangedEventIsTriggeredTooFewTimes_ShouldFail()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged("foo");
				sut.NotifyPropertyChanged("bar");

				async Task Act() =>
					await That(recording).TriggeredPropertyChanged()
						.With<PropertyChangedEventArgs>(e => e.PropertyName == "foo")
						.AtLeast(2.Times());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut with PropertyChangedEventArgs e => e.PropertyName == "foo" at least twice,
					             but it was recorded once in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 0
					                 }, PropertyChangedEventArgs {
					                   PropertyName = "foo"
					                 }),
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 0
					                 }, PropertyChangedEventArgs {
					                   PropertyName = "bar"
					                 })
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEventRecording<PropertyChangedClass>? subject = null;

				async Task Act()
					=> await That(subject!).TriggeredPropertyChanged();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recorded the PropertyChanged event at least once,
					             but it was <null>
					             """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventIsNotTriggered_ShouldSucceed()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				async Task Act() =>
					await That(recording).DoesNotComplyWith(n => n.TriggeredPropertyChanged());

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
					await That(recording).DoesNotComplyWith(n => n.TriggeredPropertyChanged());

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
		}
	}
}
