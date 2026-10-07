using System.Linq.Expressions;
using aweXpect.Core;
using aweXpect.Recording;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	public sealed partial class TriggeredPropertyChangedFor
	{
		public sealed class Tests
		{
			[Test]
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

			[Test]
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

			[Test]
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

			[Test]
			public async Task WhenEventArgsAreNull_ShouldNotMatchAllProperties()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChangedWithoutEventArgs();

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut for all properties at least once,
					             but it was never recorded in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 0
					                 }, <null>)
					             ]
					             """)
					.Because("an event without event args names no property, so it is no notification for all properties");
			}

			[Test]
			public async Task WhenEventArgsAreNull_ShouldNotMatchTheProperty()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChangedWithoutEventArgs();

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.MyValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut for property MyValue at least once,
					             but it was never recorded in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 0
					                 }, <null>)
					             ]
					             """)
					.Because("an event without event args names no property");
			}

			[Test]
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

			[Test]
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

			[Test]
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

			[Test]
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

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenExpressionIsAsCastOfPropertyAccess_ShouldUsePropertyName()
			{
				PropertyChangedBaseClass sut = new();
				IEventRecording<PropertyChangedBaseClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedBaseClass.Name));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.Name as object);

				await That(Act).DoesNotThrow()
					.Because("a cast of the property value still names the property");
			}

			[Test]
			public async Task WhenExpressionIsCheckedConversionOfPropertyAccess_ShouldUsePropertyName()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedWithMembersClass.MyValue));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => checked((long)x.MyValue));

				await That(Act).DoesNotThrow()
					.Because("a conversion in a checked context still wraps a property access");
			}

			[Test]
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

			[Test]
			public async Task WhenExpressionIsDowncastOfParameter_ShouldUsePropertyName()
			{
				PropertyChangedBaseClass sut = new PropertyChangedDerivedClass();
				IEventRecording<PropertyChangedBaseClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedDerivedClass.Extra));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => ((PropertyChangedDerivedClass)x).Extra);

				await That(Act).DoesNotThrow()
					.Because("a cast of the subject is still the subject");
			}

			[Test]
			public async Task WhenExpressionIsFieldAccess_ShouldThrowArgumentException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(null);

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.MyField);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but it was x.MyField.").AsPrefix()
					.Because("a field is no property and must not silently become the null property name");
			}

			[Test]
			public async Task WhenExpressionIsForeignPropertyAccess_ShouldThrowArgumentException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();
				PropertyChangedWithMembersClass other = new();

				sut.NotifyPropertyChanged(nameof(PropertyChangedWithMembersClass.MyValue));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(_ => other.MyValue);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but it was value(")
					.AsPrefix()
					.Because("a property of another object is no property of the subject");
			}

			[Test]
			public async Task WhenExpressionIsImplicitlyBoxedPropertyAccess_ShouldUsePropertyName()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();
				Expression<Func<PropertyChangedWithMembersClass, object>> propertyExpression = x => x.MyValue;

				sut.NotifyPropertyChanged(nameof(PropertyChangedWithMembersClass.MyValue));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(propertyExpression);

				await That(Act).DoesNotThrow()
					.Because("the compiler-inserted boxing still wraps a property access");
			}

			[Test]
			public async Task WhenExpressionIsImplicitlyConvertedPropertyAccess_ShouldUsePropertyName()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();
				Expression<Func<PropertyChangedWithMembersClass, decimal>> propertyExpression = x => x.MyValue;

				sut.NotifyPropertyChanged(nameof(PropertyChangedWithMembersClass.MyValue));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(propertyExpression);

				await That(Act).DoesNotThrow()
					.Because("the compiler-inserted conversion operator still wraps a property access");
			}

			[Test]
			public async Task WhenExpressionIsIndexerAccess_ShouldThrowArgumentException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(null);

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x[1]);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but it was x.get_Item(1).")
					.AsPrefix()
					.Because("an indexer access does not name a property that the PropertyChanged event could report");
			}

			[Test]
			public async Task WhenExpressionIsInheritedPropertyAccess_ShouldUsePropertyName()
			{
				PropertyChangedDerivedClass sut = new();
				IEventRecording<PropertyChangedDerivedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedBaseClass.Name));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.Name);

				await That(Act).DoesNotThrow()
					.Because("a property of a base class is a property of the subject");
			}

			[Test]
			public async Task WhenExpressionIsMethodCall_ShouldThrowArgumentException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(null);

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.GetMyValue());

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but it was x.GetMyValue().")
					.AsPrefix()
					.Because("a method call is no property and must not silently become the null property name");
			}

			[Test]
			public async Task WhenExpressionIsNegatedPropertyAccess_ShouldThrowArgumentException()
			{
				PropertyChangedBaseClass sut = new();
				IEventRecording<PropertyChangedBaseClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedBaseClass.IsActive));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => !x.IsActive);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but it was Not(x.IsActive).")
					.AsPrefix()
					.Because("an operator computes a new value and does not name a property");
			}

			[Test]
			public async Task WhenExpressionIsNestedPropertyAccess_ShouldThrowArgumentException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedWithMembersClass.MyValue));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.Inner!.MyValue);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but it was x.Inner.MyValue.")
					.AsPrefix()
					.Because("the subject does not report the changes of a property of another object");
			}

			[Test]
			public async Task WhenExpressionIsNull_ShouldThrowArgumentNullException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();
				Expression<Func<PropertyChangedWithMembersClass, int>> propertyExpression = null!;

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(propertyExpression);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' cannot be null.").AsPrefix()
					.Because("a missing expression names no property and is rejected like any other null argument");
			}

			[Test]
			public async Task WhenExpressionIsParameterItself_ShouldThrowArgumentException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(null);

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but it was x.").AsPrefix()
					.Because("it would otherwise match the event that was raised without a property name");
			}

			[Test]
			public async Task WhenExpressionIsStaticPropertyAccess_ShouldThrowArgumentException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(DateTime.Now));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(_ => DateTime.Now);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but it was DateTime.Now.")
					.AsPrefix()
					.Because("a static property is no property of the subject");
			}

			[Test]
			public async Task WhenExpressionIsUserDefinedConversionOfParameter_ShouldThrowArgumentException()
			{
				PropertyChangedBaseClass sut = new();
				IEventRecording<PropertyChangedBaseClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedConversionTarget.Extra));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => ((PropertyChangedConversionTarget)x).Extra);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but it was Convert(x")
					.AsPrefix()
					.Because("a conversion operator creates another object than the subject");
			}

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenPropertyNameIsNull_ShouldMatchEventWithEmptyPropertyName()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged("");

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(null);

				await That(Act).DoesNotThrow()
					.Because("the contract makes no difference between the two spellings of the all-properties notification");
			}

			[Test]
			public async Task WhenPropertyNameIsNull_ShouldMatchEventWithoutPropertyName()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(sut, null);

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(null);

				await That(Act).DoesNotThrow()
					.Because("the explicit string overload remains the way to assert the null property name");
			}

			[Test]
			public async Task WhenPropertyNameIsNull_ShouldNotMatchNamedEvent()
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 5,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut for all properties at least once,
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

			[Test]
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

			[Test]
			public async Task WhenSubjectIsGenericTypeParameter_ShouldUsePropertyName()
			{
				PropertyChangedBaseClass sut = new();
				IEventRecording<PropertyChangedBaseClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedBaseClass.Name));

				async Task Act() =>
					await ForName(recording);

				await That(Act).DoesNotThrow()
					.Because("the compiler-inserted cast to the constraint is still the subject");
			}

			[Test]
			public async Task WhenSubjectIsInterface_ShouldUsePropertyName()
			{
				IPropertyChangedWithName sut = new PropertyChangedBaseClass();
				IEventRecording<IPropertyChangedWithName> recording = sut.Record().Events();

				((PropertyChangedBaseClass)sut).NotifyPropertyChanged(nameof(IPropertyChangedWithName.Name));

				async Task Act() =>
					await That(recording).TriggeredPropertyChangedFor(x => x.Name);

				await That(Act).DoesNotThrow()
					.Because("a property of the interface is a property of the subject");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEventRecording<PropertyChangedClass>? subject = null;

				async Task Act()
					=> await That(subject!).TriggeredPropertyChangedFor(x => x.MyValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has recorded the PropertyChanged event for property MyValue at least once,
					             but it was <null>
					             """);
			}

			private static async Task ForName<T>(IEventRecording<T> recording)
				where T : IPropertyChangedWithName
				=> await That(recording).TriggeredPropertyChangedFor(x => x.Name);
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenEventIsNotTriggered_ShouldSucceed()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				async Task Act() =>
					await That(recording).DoesNotComplyWith(n => n.TriggeredPropertyChangedFor(x => x.MyValue));

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

				sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));

				async Task Act() =>
					await That(recording).DoesNotComplyWith(n => n.TriggeredPropertyChangedFor(x => x.MyValue));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that recording
					             has never recorded the PropertyChanged event on sut for property MyValue,
					             but it was recorded once in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 422
					                 }, PropertyChangedEventArgs {
					                   PropertyName = "MyValue"
					                 })
					             ]
					             """);
			}

			[Test]
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

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenEventIsTriggeredForOtherProperty_ShouldSucceed()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged("SomeArbitraryProperty");

				async Task Act() =>
					await That(recording).DoesNotComplyWith(n => n.TriggeredPropertyChangedFor(x => x.MyValue));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
