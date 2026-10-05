using System.Linq.Expressions;
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
					             has never recorded the PropertyChanged event on sut for all properties,
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
					             has never recorded the PropertyChanged event on sut for all properties,
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
			public async Task WhenExpressionIsAsCastOfPropertyAccess_ShouldUsePropertyName()
			{
				PropertyChangedBaseClass sut = new();
				IEventRecording<PropertyChangedBaseClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedBaseClass.Name));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(x => x.Name as object);

				await That(Act).Throws<XunitException>()
					.Because("a cast of the property value still names the property");
			}

			[Fact]
			public async Task WhenExpressionIsCheckedConversionOfPropertyAccess_ShouldUsePropertyName()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedWithMembersClass.MyValue));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(x => checked((long)x.MyValue));

				await That(Act).Throws<XunitException>()
					.Because("a conversion in a checked context still wraps a property access");
			}

			[Fact]
			public async Task WhenExpressionIsConvertedPropertyAccess_ShouldUsePropertyName()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedWithMembersClass.MyValue));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(x => (object)x.MyValue);

				await That(Act).Throws<XunitException>()
					.Because("the conversion still wraps a property access");
			}

			[Fact]
			public async Task WhenExpressionIsDowncastOfParameter_ShouldUsePropertyName()
			{
				PropertyChangedBaseClass sut = new PropertyChangedDerivedClass();
				IEventRecording<PropertyChangedBaseClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedDerivedClass.Extra));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(x => ((PropertyChangedDerivedClass)x).Extra);

				await That(Act).Throws<XunitException>()
					.Because("a cast of the subject is still the subject");
			}

			[Fact]
			public async Task WhenExpressionIsForeignPropertyAccess_ShouldThrowArgumentException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();
				PropertyChangedWithMembersClass other = new();

				sut.NotifyPropertyChanged(nameof(PropertyChangedWithMembersClass.MyValue));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(_ => other.MyValue);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but it was value(")
					.AsPrefix()
					.Because("a property of another object is no property of the subject");
			}

			[Fact]
			public async Task WhenExpressionIsImplicitlyBoxedPropertyAccess_ShouldUsePropertyName()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();
				Expression<Func<PropertyChangedWithMembersClass, object>> propertyExpression = x => x.MyValue;

				sut.NotifyPropertyChanged(nameof(PropertyChangedWithMembersClass.MyValue));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(propertyExpression);

				await That(Act).Throws<XunitException>()
					.Because("the compiler-inserted boxing still wraps a property access");
			}

			[Fact]
			public async Task WhenExpressionIsImplicitlyConvertedPropertyAccess_ShouldUsePropertyName()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();
				Expression<Func<PropertyChangedWithMembersClass, decimal>> propertyExpression = x => x.MyValue;

				sut.NotifyPropertyChanged(nameof(PropertyChangedWithMembersClass.MyValue));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(propertyExpression);

				await That(Act).Throws<XunitException>()
					.Because("the compiler-inserted conversion operator still wraps a property access");
			}

			[Fact]
			public async Task WhenExpressionIsInheritedPropertyAccess_ShouldUsePropertyName()
			{
				PropertyChangedDerivedClass sut = new();
				IEventRecording<PropertyChangedDerivedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedBaseClass.Name));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(x => x.Name);

				await That(Act).Throws<XunitException>()
					.Because("a property of a base class is a property of the subject");
			}

			[Fact]
			public async Task WhenExpressionIsNegatedPropertyAccess_ShouldThrowArgumentException()
			{
				PropertyChangedBaseClass sut = new();
				IEventRecording<PropertyChangedBaseClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedBaseClass.IsActive));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(x => !x.IsActive);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but it was Not(x.IsActive).")
					.AsPrefix()
					.Because("an operator computes a new value and does not name a property");
			}

			[Fact]
			public async Task WhenExpressionIsNestedPropertyAccess_ShouldThrowArgumentException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedWithMembersClass.MyValue));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(x => x.Inner!.MyValue);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but it was x.Inner.MyValue.")
					.AsPrefix()
					.Because("the subject does not report the changes of a property of another object");
			}

			[Fact]
			public async Task WhenExpressionIsNull_ShouldThrowArgumentNullException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();
				Expression<Func<PropertyChangedWithMembersClass, int>> propertyExpression = null!;

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(propertyExpression);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' cannot be null.").AsPrefix()
					.Because("a missing expression names no property and is rejected like any other null argument");
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
					.WithMessage("The 'propertyExpression' must refer to a property, but it was x.").AsPrefix()
					.Because("it would otherwise be checked against the event raised without a name");
			}

			[Fact]
			public async Task WhenExpressionIsStaticPropertyAccess_ShouldThrowArgumentException()
			{
				PropertyChangedWithMembersClass sut = new();
				IEventRecording<PropertyChangedWithMembersClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(DateTime.Now));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(_ => DateTime.Now);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but it was DateTime.Now.")
					.AsPrefix()
					.Because("a static property is no property of the subject");
			}

			[Fact]
			public async Task WhenExpressionIsUserDefinedConversionOfParameter_ShouldThrowArgumentException()
			{
				PropertyChangedBaseClass sut = new();
				IEventRecording<PropertyChangedBaseClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedConversionTarget.Extra));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(x => ((PropertyChangedConversionTarget)x).Extra);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("propertyExpression").And
					.WithMessage("The 'propertyExpression' must refer to a property, but it was Convert(x")
					.AsPrefix()
					.Because("a conversion operator creates another object than the subject");
			}

			[Fact]
			public async Task WhenSubjectIsGenericTypeParameter_ShouldUsePropertyName()
			{
				PropertyChangedBaseClass sut = new();
				IEventRecording<PropertyChangedBaseClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedBaseClass.Name));

				async Task Act() =>
					await ForName(recording);

				await That(Act).Throws<XunitException>()
					.Because("the compiler-inserted cast to the constraint is still the subject");
			}

			[Fact]
			public async Task WhenSubjectIsInterface_ShouldUsePropertyName()
			{
				IPropertyChangedWithName sut = new PropertyChangedBaseClass();
				IEventRecording<IPropertyChangedWithName> recording = sut.Record().Events();

				((PropertyChangedBaseClass)sut).NotifyPropertyChanged(nameof(IPropertyChangedWithName.Name));

				async Task Act() =>
					await That(recording).DidNotTriggerPropertyChangedFor(x => x.Name);

				await That(Act).Throws<XunitException>()
					.Because("a property of the interface is a property of the subject");
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

			private static async Task ForName<T>(IEventRecording<T> recording)
				where T : IPropertyChangedWithName
				=> await That(recording).DidNotTriggerPropertyChangedFor(x => x.Name);
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenEventIsNotTriggered_ShouldFail()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				async Task Act() =>
					await That(recording).DoesNotComplyWith(n => n.DidNotTriggerPropertyChangedFor(x => x.MyValue));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut for property MyValue at least once,
					             but it was never recorded
					             """);
			}

			[Fact]
			public async Task WhenEventIsTriggered_ShouldSucceed()
			{
				PropertyChangedClass sut = new();
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged(nameof(PropertyChangedClass.MyValue));

				async Task Act() =>
					await That(recording).DoesNotComplyWith(n => n.DidNotTriggerPropertyChangedFor(x => x.MyValue));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEventIsTriggeredForOtherProperty_ShouldFail()
			{
				PropertyChangedClass sut = new()
				{
					MyValue = 428,
				};
				IEventRecording<PropertyChangedClass> recording = sut.Record().Events();

				sut.NotifyPropertyChanged("SomeOtherProperty");

				async Task Act() =>
					await That(recording).DoesNotComplyWith(n => n.DidNotTriggerPropertyChangedFor(x => x.MyValue));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that recording
					             has recorded the PropertyChanged event on sut for property MyValue at least once,
					             but it was never recorded in [
					               PropertyChanged(ThatEventRecording.PropertyChangedClass {
					                   MyValue = 428
					                 }, PropertyChangedEventArgs {
					                   PropertyName = "SomeOtherProperty"
					                 })
					             ]
					             """);
			}
		}
	}
}
