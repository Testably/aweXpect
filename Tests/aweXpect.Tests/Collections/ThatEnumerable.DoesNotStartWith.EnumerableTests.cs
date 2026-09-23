using System.Collections;
using System.Collections.Generic;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotStartWith
	{
		public sealed class EnumerableTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).DoesNotStartWith(0)
						.And.DoesNotStartWith(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).DoesNotStartWith(1, 1, 2, 3, 4);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldSupportEquivalent()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(x => new MyClass(x), 20);
				IEnumerable<MyClass> unexpected = [new(1), new(1), new(2),];

				async Task Act()
					=> await That(subject).DoesNotStartWith(unexpected).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with [MyClass { StringValue = "", Value = 1 }, MyClass { StringValue = "", Value = 1 }, MyClass { StringValue = "", Value = 2 }] using equivalency,
					             but it did start with [
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 1
					               },
					               MyClass {
					                 StringValue = "",
					                 Value = 2
					               }
					             ]
					             """);
			}

			[Fact]
			public async Task WhenCollectionsAreIdentical_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).DoesNotStartWith(1, 2, 3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with [1, 2, 3],
					             but it did start with [
					               1,
					               2,
					               3
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableHasDifferentStartingElements_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);
				IEnumerable<int> unexpected = [1, 3,];

				async Task Act()
					=> await That(subject).DoesNotStartWith(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject)!.DoesNotStartWith(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with [0],
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectStartsWithNull_ShouldFail()
			{
				IEnumerable subject = ToEnumerable<string?>(null, "a");
				string?[] unexpected = [null,];

				async Task Act()
					=> await That(subject).DoesNotStartWith(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with [<null>],
					             but it did start with [
					               <null>
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSubjectStartsWithUnexpectedValues_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["foo", "bar", "baz",]);
				IEnumerable<string> unexpected = ["foo", "bar",];

				async Task Act()
					=> await That(subject).DoesNotStartWith(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with ["foo", "bar"],
					             but it did start with [
					               "foo",
					               "bar"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedContainsAdditionalElements_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).DoesNotStartWith(1, 2, 3, 4);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnexpectedIsAList_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);
				List<int> unexpected = [1, 2,];

				async Task Act()
					=> await That(subject).DoesNotStartWith(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with [1, 2],
					             but it did start with [
					               1,
					               2
					             ]
					             """)
					.Because("treating the collection as a single unexpected item would let the expectation pass vacuously");
			}

			[Fact]
			public async Task WhenUnexpectedIsANonGenericCollection_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);
				ArrayList unexpected = new() { 1, 2, };

				async Task Act()
					=> await That(subject).DoesNotStartWith(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with [1, 2],
					             but it did start with [
					               1,
					               2
					             ]
					             """)
					.Because("treating the collection as a single unexpected item would let the expectation pass vacuously");
			}

			[Fact]
			public async Task WhenUnexpectedIsANonGenericCollectionThatDiffers_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);
				ArrayList unexpected = new() { 1, 3, };

				async Task Act()
					=> await That(subject).DoesNotStartWith(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnexpectedIsAString_ShouldFail()
			{
				IEnumerable subject = ToEnumerable(["foo", "bar",]);

				async Task Act()
					=> await That(subject).DoesNotStartWith("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with ["foo"],
					             but it did start with [
					               "foo"
					             ]
					             """)
					.Because("treating the string as a sequence of characters would let the expectation pass vacuously");
			}

			[Fact]
			public async Task WhenUnexpectedIsEmpty_ShouldThrowArgumentException()
			{
				IEnumerable subject = ToEnumerable([1, 2,]);

				async Task Act()
					=> await That(subject).DoesNotStartWith(Array.Empty<int>());

				await That(Act).Throws<ArgumentException>()
					.WithParamName("unexpected").And
					.WithMessage("The 'unexpected' collection cannot be empty.").AsPrefix();
			}
		}
	}
}
