using System.Collections;
using System.Collections.Generic;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotEndWith
	{
		public sealed class EnumerableTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).DoesNotEndWith(0)
						.And.DoesNotEndWith(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldSupportEquivalent()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(x => new MyClass(x), 6);

				async Task Act()
					=> await That(subject).DoesNotEndWith(
						new MyClass(3),
						new MyClass(5),
						new MyClass(8)
					).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with [MyClass { StringValue = "", Value = 3 }, MyClass { StringValue = "", Value = 5 }, MyClass { StringValue = "", Value = 8 }] using equivalency,
					             but it did end with [
					               MyClass {
					                 StringValue = "",
					                 Value = 3
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 5
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 8
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenCollectionsAreIdentical_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).DoesNotEndWith(1, 2, 3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with [1, 2, 3],
					             but it did end with [
					               1,
					               2,
					               3
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableHasDifferentEndingElements_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([0, 0, 1, 2, 3,]);
				IEnumerable<int> unexpected = [1, 3,];

				async Task Act()
					=> await That(subject).DoesNotEndWith(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectEndsWithNull_ShouldFail()
			{
				IEnumerable subject = ToEnumerable<string?>("a", null);
				string?[] unexpected = [null,];

				async Task Act()
					=> await That(subject).DoesNotEndWith(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with [<null>],
					             but it did end with [
					               <null>
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSubjectEndsWithUnexpectedValues_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["foo", "bar", "baz",]);
				IEnumerable<string> unexpected = ["bar", "baz",];

				async Task Act()
					=> await That(subject).DoesNotEndWith(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with ["bar", "baz"],
					             but it did end with [
					               "bar",
					               "baz"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject)!.DoesNotEndWith(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with [0],
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedContainsAdditionalElements_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).DoesNotEndWith(0, 0, 1, 2, 3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnexpectedIsAList_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);
				List<int> unexpected = [2, 3,];

				async Task Act()
					=> await That(subject).DoesNotEndWith(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with [2, 3],
					             but it did end with [
					               2,
					               3
					             ]
					             """)
					.Because("treating the collection as a single unexpected item would let the expectation pass vacuously");
			}

			[Fact]
			public async Task WhenUnexpectedIsAString_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["foo", "bar",]);

				async Task Act()
					=> await That(subject).DoesNotEndWith("bar");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with ["bar"],
					             but it did end with [
					               "bar"
					             ]
					             """)
					.Because("treating the string as a sequence of characters would let the expectation pass vacuously");
			}

			[Fact]
			public async Task WhenUnexpectedIsEmpty_ShouldThrowArgumentException()
			{
				IEnumerable subject = ToEnumerable([1, 2,]);

				async Task Act()
					=> await That(subject).DoesNotEndWith(Array.Empty<int>());

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix();
			}
		}
	}
}
