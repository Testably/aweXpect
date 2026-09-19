using aweXpect.Core;
using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class TriggeredPropertyChangedFor
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenBothAllPropertiesAndNamedEventAreRecorded_ShouldCountBothForThatProperty()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(sut, null);
				sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));
				sut.NotifyPropertyChanged("SomeOtherProperty");

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.MyValue).Exactly(2.Times());

				await That(Act).DoesNotThrow()
					.Because("the notification for all properties changed MyValue, too");
			}

			[Fact]
			public async Task WhenBothAllPropertiesAndNamedEventAreRecorded_ShouldCountOnlyAllPropertiesForOtherProperty()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(sut, null);
				sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor("SomeOtherProperty").Exactly(1.Times());

				await That(Act).DoesNotThrow()
					.Because("only the notification for all properties changed the other property");
			}

			[Fact]
			public async Task WhenEmptyPropertyNameIsExpected_ShouldMatchNullPropertyName()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(sut, null);

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor("");

				await That(Act).DoesNotThrow()
					.Because("the contract makes no difference between the two spellings of the all-properties notification");
			}

			[Fact]
			public async Task WhenEventIsRecordedWithEmptyPropertyName_ShouldMatchExpressionForAnyProperty()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged("");

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.MyValue);

				await That(Act).DoesNotThrow()
					.Because("an empty property name notifies that all properties changed");
			}

			[Fact]
			public async Task WhenEventIsRecordedWithEmptyPropertyName_ShouldMatchStringNameOfAnyProperty()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged("");

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor("SomeArbitraryProperty");

				await That(Act).DoesNotThrow()
					.Because("an empty property name notifies that all properties changed");
			}

			[Fact]
			public async Task WhenEventIsRecordedWithNullPropertyName_ShouldMatchExpressionForAnyProperty()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(sut, null);

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.MyValue);

				await That(Act).DoesNotThrow()
					.Because("a null property name notifies that all properties changed");
			}

			[Fact]
			public async Task WhenEventIsRecordedWithNullPropertyName_ShouldMatchStringNameOfAnyProperty()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(sut, null);

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor("SomeArbitraryProperty");

				await That(Act).DoesNotThrow()
					.Because("a null property name notifies that all properties changed");
			}

			[Fact]
			public async Task WhenEventIsRecordedWithWhitespacePropertyName_ShouldNotMatchOtherProperty()
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 2,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(" ");

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.MyValue);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut for property MyValue at least once,
					             but it was never recorded in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 2
					                 }, PropertyChangedEventArgs {
					                   PropertyName = " "
					                 })
					             ]
					             """)
					.Because("only a null or empty name notifies that all properties changed");
			}

			[Fact]
			public async Task WhenExpressionIsConvertedPropertyAccess_ShouldUsePropertyName()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedWithMembersClass.MyValue));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => (object)x.MyValue);

				await That(Act).DoesNotThrow()
					.Because("the compiler-inserted conversion still wraps a property access");
			}

			[Fact]
			public async Task WhenExpressionIsFieldAccess_ShouldThrowArgumentException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(null);

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.MyField);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but was 'x.MyField'.").AsPrefix()
					.Because("a field is no property and must not silently become the null property name");
			}

			[Fact]
			public async Task WhenExpressionIsIndexerAccess_ShouldThrowArgumentException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(null);

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x[1]);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but was 'x.get_Item(1)'.")
					.AsPrefix()
					.Because("an indexer access does not name a property that the PropertyChanged event could report");
			}

			[Fact]
			public async Task WhenExpressionIsMethodCall_ShouldThrowArgumentException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(null);

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.GetMyValue());

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but was 'x.GetMyValue()'.")
					.AsPrefix()
					.Because("a method call is no property and must not silently become the null property name");
			}

			[Fact]
			public async Task WhenExpressionIsNestedPropertyAccess_ShouldUseInnerPropertyName()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedWithMembersClass.MyValue));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.Inner!.MyValue);

				await That(Act).DoesNotThrow()
					.Because("the last segment of a nested access still unambiguously names a property");
			}

			[Fact]
			public async Task WhenExpressionIsParameterItself_ShouldThrowArgumentException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(null);

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but was 'x'.").AsPrefix()
					.Because("it would otherwise match the event that was raised without a property name");
			}

			[Fact]
			public async Task WhenPropertyNameDoesNotMatch_ShouldFail()
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 2,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged("foo");

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.MyValue);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut for property MyValue at least once,
					             but it was never recorded in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 2
					                 }, PropertyChangedEventArgs {
					                   PropertyName = "foo"
					                 })
					             ]
					             """);
			}

			[Fact]
			public async Task WhenPropertyNameIsNull_ShouldMatchEventWithEmptyPropertyName()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged("");

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor((string?)null);

				await That(Act).DoesNotThrow()
					.Because("the contract makes no difference between the two spellings of the all-properties notification");
			}

			[Fact]
			public async Task WhenPropertyNameIsNull_ShouldMatchEventWithoutPropertyName()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(sut, null);

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor((string?)null);

				await That(Act).DoesNotThrow()
					.Because("the explicit string overload remains the way to assert the null property name");
			}

			[Fact]
			public async Task WhenPropertyNameIsNull_ShouldNotMatchNamedEvent()
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 5,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor((string?)null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut for property  at least once,
					             but it was never recorded in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 5
					                 }, PropertyChangedEventArgs {
					                   PropertyName = "MyValue"
					                 })
					             ]
					             """)
					.Because("a notification for a single property is no notification for all properties");
			}

			[Fact]
			public async Task WhenPropertyNameMatches_ShouldSucceed()
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 2,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.MyValue);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEventRecording<PropertyChangedClass>? subject = null;

				async Task Act()
					=> await That(subject!).TriggeredPropertyChangedFor(x => x.MyValue);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has recorded the PropertyChanged event for property MyValue at least once,
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

			[Fact]
			public async Task WhenEventIsTriggeredForAllProperties_ShouldFail()
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 423,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(sut, null);

				async Task Act() =>
					await That(recording).DoesNotComplyWith(n => n.TriggeredPropertyChangedFor(x => x.MyValue));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the PropertyChanged event on sut for property MyValue,
					             but it was recorded once in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 423
					                 }, PropertyChangedEventArgs {
					                   PropertyName = <null>
					                 })
					             ]
					             """)
					.Because("the negation has to be the exact complement of the positive expectation");
			}
		}
	}
}
