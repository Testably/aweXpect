using System.Collections.Generic;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed class IsNotOneOf
	{
		public sealed class Tests
		{
			[Fact]
			public async Task SubjectToItself_ShouldFail()
			{
				object subject = new MyClass();
				IEnumerable<object> unexpected = [new MyClass(), subject,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of unexpected, because we want to test the failure,
					             but it was ThatObject.MyClass {
					                 Value = 0
					               }

					             Unexpected values:
					             [ThatObject.MyClass { Value = 0 }, ThatObject.MyClass { Value = 0 }]
					             """);
			}

			[Fact]
			public async Task SubjectToSomeOtherValue_ShouldSucceed()
			{
				object subject = new MyClass();
				object[] unexpected = [new MyClass(),];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenComparingWithEquivalenceAndContainsEquivalentValue_ShouldFail()
			{
				object subject = new MyClass();
				object[] unexpected = [new MyClass(),];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equivalent to one of [ThatObject.MyClass { Value = 0 }],
					             but it was ThatObject.MyClass {
					                 Value = 0
					               }, which is considered equivalent
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				object subject = new MyClass();
				object[] expected = [];

				object Act()
					=> That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				object subject = new MyClass();
				object[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectedOnlyContainsNullValues_ShouldSucceed()
			{
				MyClass subject = new();
				IEnumerable<object?> unexpected = [null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				object subject = new MyClass();
				object?[] expected = [];

				object Act()
					=> That(subject).IsNotOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Fact]
			public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				object subject = new MyClass();
				object?[]? expected = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' value cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenOnlyACombinedExpectationFails_ShouldNotListTheUnexpectedValues()
			{
				MyClass subject = new();
				IEnumerable<MyClass> unexpected = [new MyClass(),];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected).And.IsNull();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of unexpected and is null,
					             but it was ThatObject.MyClass {
					                 Value = 0
					               }
					             """)
					.Because("the values explain only a failure of the expectation that names them");
			}

			[Fact]
			public async Task WhenSubjectIsAReferenceType_ShouldReturnTheTypedSubject()
			{
				MyClass subject = new();

				MyClass result = await That(subject).IsNotOneOf(new MyClass(), new MyClass());

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task WhenSubjectIsAReferenceType_WithEnumerable_ShouldReturnTheTypedSubject()
			{
				MyClass subject = new();
				IEnumerable<MyClass> unexpected = [new MyClass(), new MyClass(),];

				MyClass result = await That(subject).IsNotOneOf(unexpected);

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				MyClass? subject = null;

				async Task Act()
					=> await That(subject).IsNotOneOf(new MyClass());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNullAndUnexpectedContainsNull_ShouldFail()
			{
				object? subject = null;
				IEnumerable<object?> expected = [new MyClass(), null,];

				async Task Act()
					=> await That(subject).IsNotOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of expected,
					             but it was <null>

					             Unexpected values:
					             [ThatObject.MyClass { Value = 0 }, <null>]
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedIsAnEnumerable_ShouldNameItsExpression()
			{
				MyClass subject = new();
				IEnumerable<MyClass> unexpected = [new MyClass { Value = 1, }, subject,];

				async Task Act()
					=> await That(subject).IsNotOneOf(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of unexpected,
					             but it was ThatObject.MyClass {
					                 Value = 0
					               }

					             Unexpected values:
					             [ThatObject.MyClass { Value = 1 }, ThatObject.MyClass { Value = 0 }]
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedIsPassedAsParams_ShouldFormatTheValues()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).IsNotOneOf(new MyClass { Value = 1, }, subject);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not one of [ThatObject.MyClass { Value = 1 }, ThatObject.MyClass { Value = 0 }],
					             but it was ThatObject.MyClass {
					                 Value = 0
					               }
					             """)
					.Because("the separate arguments have no single expression to name");
			}
		}
	}
}
