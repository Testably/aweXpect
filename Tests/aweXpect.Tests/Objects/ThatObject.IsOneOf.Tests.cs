using System.Collections.Generic;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed class IsOneOf
	{
		public sealed class Tests
		{
			[Fact]
			public async Task SubjectToItself_ShouldSucceed()
			{
				object subject = new MyClass();
				IEnumerable<object> expected = [new MyClass(), subject,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task SubjectToSomeOtherValue_ShouldFail()
			{
				object subject = new MyClass();
				object[] expected = [new MyClass(),];

				async Task Act()
					=> await That(subject).IsOneOf(expected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [ThatObject.MyClass { Value = 0 }], because we want to test the failure,
					             but it was ThatObject.MyClass {
					                 Value = 0
					               }
					             """);
			}

			[Fact]
			public async Task WhenComparingWithCustomComparer_ShouldFail()
			{
				object subject = new MyClass();
				object[] expected = [subject,];

				async Task Act()
					=> await That(subject).IsOneOf(expected).Using(new AllDifferentComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [ThatObject.MyClass { Value = 0 }] using AllDifferentComparer,
					             but it was ThatObject.MyClass {
					                 Value = 0
					               }
					             """)
					.Because("a custom comparer still adds information, while the default equality does not");
			}

			[Fact]
			public async Task WhenComparingWithEquivalence_ShouldFail()
			{
				object subject = new MyClass
				{
					Value = 1,
				};
				object[] expected = [new MyClass(),];

				async Task Act()
					=> await That(subject).IsOneOf(expected).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to one of [ThatObject.MyClass { Value = 0 }],
					             but it was not:
					               Property Value differed:
					                   Actual: 1
					                 Expected: 0

					             Equivalency options:
					              - include public fields and properties
					             """)
					.Because("the equivalency keeps naming the comparison it performs and lists the differences to the only candidate");
			}

			[Fact]
			public async Task WhenComparingWithEquivalenceAgainstSeveralCandidates_ShouldDescribeTheSubject()
			{
				object subject = new MyClass
				{
					Value = 1,
				};

				async Task Act()
					=> await That(subject).IsOneOf(new MyClass { Value = 2, }, new MyClass { Value = 3, }).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to one of [ThatObject.MyClass { Value = 2 }, ThatObject.MyClass { Value = 3 }],
					             but it was ThatObject.MyClass {
					                 Value = 1
					               }

					             Equivalency options:
					              - include public fields and properties
					             """)
					.Because("the differences to the last candidate would read as if it were the only expectation");
			}

			[Fact]
			public async Task WhenComparingWithEquivalence_ShouldSucceed()
			{
				object subject = new MyClass();
				object[] expected = [new MyClass(),];

				async Task Act()
					=> await That(subject).IsOneOf(expected).Equivalent();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				object subject = "foo";
				object[] values = ["bar", "baz",];
				IEnumerable<object?> expected = Factory.GetSingleUseEnumerable<object?>(values);

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is one of expected,
					              but it was {Formatter.Format(subject)}

					              Expected values:
					              ["bar", "baz"]
					              """)
					.Because("the values are cached while they are enumerated, so the comparison and the message share one enumeration");
			}

			[Fact]
			public async Task WhenExpectedIsAnEnumerable_ShouldNameItsExpression()
			{
				MyClass subject = new();
				IEnumerable<MyClass> expected = [new MyClass { Value = 1, }, new MyClass { Value = 2, },];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of expected,
					             but it was ThatObject.MyClass {
					                 Value = 0
					               }

					             Expected values:
					             [ThatObject.MyClass { Value = 1 }, ThatObject.MyClass { Value = 2 }]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				object subject = new MyClass();
				object[] expected = [];

				object Act()
					=> That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Fact]
			public async Task WhenExpectedIsInfiniteAndContainsTheSubject_ShouldSucceed()
			{
				object subject = 8;
				IEnumerable<object?> expected = Factory.GetFibonacciNumbers<object?>(i => i);

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow()
					.Because("the values are only enumerated until the subject is found");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				object subject = new MyClass();
				object[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectedIsPassedAsParams_ShouldFormatTheValues()
			{
				MyClass subject = new();

				async Task Act()
					=> await That(subject).IsOneOf(new MyClass { Value = 1, }, new MyClass { Value = 2, });

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [ThatObject.MyClass { Value = 1 }, ThatObject.MyClass { Value = 2 }],
					             but it was ThatObject.MyClass {
					                 Value = 0
					               }
					             """)
					.Because("the separate arguments have no single expression to name");
			}

			[Fact]
			public async Task WhenExpectedOnlyContainsNullValues_ShouldFail()
			{
				MyClass subject = new();
				IEnumerable<object?> expected = [null,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of expected,
					             but it was ThatObject.MyClass {
					                 Value = 0
					               }

					             Expected values:
					             [<null>]
					             """);
			}

			[Fact]
			public async Task WhenNullableExpectedIsEmpty_ShouldThrowArgumentException()
			{
				object subject = new MyClass();
				object?[] expected = [];

				object Act()
					=> That(subject).IsOneOf(expected);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix()
					.Because("an empty set is rejected when the expectation is built, before it is awaited");
			}

			[Fact]
			public async Task WhenNullableExpectedIsNull_ShouldThrowArgumentNullException()
			{
				object subject = new MyClass();
				object?[]? expected = null;

				async Task Act()
					=> await That(subject).IsOneOf(expected!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' value cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenOnlyACombinedExpectationFails_ShouldNotListTheExpectedValues()
			{
				MyClass subject = new();
				IEnumerable<MyClass> expected = [subject,];

				async Task Act()
					=> await That(subject).IsOneOf(expected).And.IsNull();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of expected and is null,
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

				MyClass result = await That(subject).IsOneOf(new MyClass(), subject);

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task WhenSubjectIsAReferenceType_WithEnumerable_ShouldReturnTheTypedSubject()
			{
				MyClass subject = new();
				IEnumerable<MyClass> expected = [new MyClass(), subject,];

				MyClass result = await That(subject).IsOneOf(expected);

				await That(result).IsSameAs(subject);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				MyClass? subject = null;

				async Task Act()
					=> await That(subject).IsOneOf(new MyClass());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of [ThatObject.MyClass { Value = 0 }],
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNullAndExpectedContainsNull_ShouldSucceed()
			{
				object? subject = null;
				IEnumerable<object?> expected = [new MyClass(), null,];

				async Task Act()
					=> await That(subject).IsOneOf(expected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
