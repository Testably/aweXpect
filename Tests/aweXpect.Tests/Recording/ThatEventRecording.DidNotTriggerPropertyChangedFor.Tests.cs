using aweXpect.Recording;

// ReSharper disable AccessToDisposedClosure

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class DidNotTriggerPropertyChangedFor
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenEventIsNotTriggeredAtAll_ShouldSucceed()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(x => x.MyValue);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventIsTriggered_ShouldFail()
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 421,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(x => x.MyValue);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the PropertyChanged event on sut for property MyValue,
					             but it was recorded once in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 421
					                 }, PropertyChangedEventArgs {
					                   PropertyName = "MyValue"
					                 })
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEventIsTriggeredForEmptyPropertyName_ShouldFail()
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 424,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged("");

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(x => x.MyValue);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the PropertyChanged event on sut for property MyValue,
					             but it was recorded once in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 424
					                 }, PropertyChangedEventArgs {
					                   PropertyName = ""
					                 })
					             ]
					             """)
					.Because("an empty property name notifies that all properties changed");
			}

			[Fact]
			public async Task WhenEventIsTriggeredForEmptyPropertyName_ShouldFailForExpectedNullPropertyName()
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 426,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged("");

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor((string?)null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the PropertyChanged event on sut for property ,
					             but it was recorded once in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 426
					                 }, PropertyChangedEventArgs {
					                   PropertyName = ""
					                 })
					             ]
					             """)
					.Because("the all-properties notification must not slip through the negated expectation for its other spelling");
			}

			[Fact]
			public async Task WhenEventIsTriggeredForNullPropertyName_ShouldFail()
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 425,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(sut, null);

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor("MyValue");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the PropertyChanged event on sut for property MyValue,
					             but it was recorded once in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 425
					                 }, PropertyChangedEventArgs {
					                   PropertyName = <null>
					                 })
					             ]
					             """)
					.Because("a null property name notifies that all properties changed");
			}

			[Fact]
			public async Task WhenEventIsTriggeredForNullPropertyName_ShouldFailForExpectedEmptyPropertyName()
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 427,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(sut, null);

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor("");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the PropertyChanged event on sut for property ,
					             but it was recorded once in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 427
					                 }, PropertyChangedEventArgs {
					                   PropertyName = <null>
					                 })
					             ]
					             """)
					.Because("the all-properties notification must not slip through the negated expectation for its other spelling");
			}

			[Fact]
			public async Task WhenEventIsTriggeredForOtherPropertyName_ShouldSucceed()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged("SomeOtherProperty");

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(x => x.MyValue);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventIsTriggeredForWhitespacePropertyName_ShouldSucceed()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(" ");

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(x => x.MyValue);

				await That(Act).DoesNotThrow()
					.Because("only a null or empty name notifies that all properties changed");
			}

			[Fact]
			public async Task WhenExpressionIsParameterItself_ShouldThrowArgumentException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(null);

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(x => x);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but was 'x'.").AsPrefix()
					.Because("it would otherwise be checked against the event raised without a name");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEventRecording<PropertyChangedClass>? subject = null;

				async Task Act()
					=> await That(subject!).DidNotTriggerPropertyChangedFor(x => x.MyValue);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has never recorded the PropertyChanged event for property MyValue,
					             but it was <null>
					             """);
			}
		}
	}
}
