using System.ComponentModel;
using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class Triggered
	{
		public sealed class WithSenderTests
		{
			[Test]
			public async Task WhenEventIsTriggeredBySomethingElse_ShouldFail()
			{
				PropertyChangedClass sender = new()
				{
					MyValue = 1,
				};
				PropertyChangedClass sut = new()
				{
					MyValue = 2,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(sut, nameof(PropertyChangedClass.MyValue));

				async Task Act() =>
					await That(recording).Triggered(nameof(INotifyPropertyChanged.PropertyChanged))
						.WithSender(s => s == sender);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut with sender s => s == sender at least once,
					             but it was never recorded in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 2
					                 }, PropertyChangedEventArgs {
					                   PropertyName = "MyValue"
					                 })
					             ]
					             """);
			}

			[Test]
			public async Task WhenEventIsTriggeredByTheExpectedSender_ShouldSucceed()
			{
				PropertyChangedClass sender = new()
				{
					MyValue = 1,
				};
				PropertyChangedClass sut = new()
				{
					MyValue = 2,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(sender, nameof(PropertyChangedClass.MyValue));

				async Task Act() =>
					await That(recording).Triggered(nameof(INotifyPropertyChanged.PropertyChanged))
						.WithSender(s => s == sender);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("predicate failed");
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(sut, nameof(PropertyChangedClass.MyValue));

				async Task Act() =>
					await That(recording).Triggered(nameof(INotifyPropertyChanged.PropertyChanged))
						.WithSender(_ => throw exception);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut with sender _ => throw exception at least once,
					             but the predicate did throw an InvalidOperationException:
					               predicate failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Test]
			public async Task WhenPredicateThrows_WhenNegated_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("predicate failed");
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(sut, nameof(PropertyChangedClass.MyValue));

				async Task Act() =>
					await That(recording).DidNotTrigger(nameof(INotifyPropertyChanged.PropertyChanged))
						.WithSender(_ => throw exception);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the PropertyChanged event on sut with sender _ => throw exception,
					             but the predicate did throw an InvalidOperationException:
					               predicate failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception))
					.Because("a predicate that threw answered nothing, so the negation fails as well");
			}

			[Test]
			public async Task WithCustomEvent_WhenSenderIsTheOnlyParameter_ShouldSucceed()
			{
				CustomEventWithParametersClass<EventArgs> sut = new();
				IEventRecording<CustomEventWithParametersClass<EventArgs>> recording = sut.Record().Events();

				sut.NotifyCustomEvent(new PropertyChangedEventArgs("foo"));

				async Task Act() =>
					await That(recording).Triggered(nameof(CustomEventWithParametersClass<EventArgs>.CustomEvent))
						.WithSender(_ => true);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
