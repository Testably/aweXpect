using System.Collections;
using System.Collections.Generic;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class EndsWith
	{
		public sealed class EnumerableTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).EndsWith(1)
						.And.EndsWith(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldSupportEquivalent()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(x => new MyClass(x), 6);

				async Task Act()
					=> await That(subject).EndsWith(
						new MyClass(3),
						new MyClass(5),
						new MyClass(8)
					).Equivalent();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenCollectionsAreIdentical_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).EndsWith(1, 2, 3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableHasDifferentEndingElements_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([0, 0, 1, 2, 3,]);
				IEnumerable<int> expected = [1, 3,];

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with [1, 3],
					             but it contained 2 at index 3 instead of 1

					             Collection:
					             [0, 0, 1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenExpectedContainsAdditionalElements_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).EndsWith(0, 0, 1, 2, 3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with [0, 0, 1, 2, 3],
					             but it contained only 3 items and misses 2 items: [
					               0,
					               0
					             ]

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsAList_ShouldUseItsItemsAsExpectedSequence()
			{
				IEnumerable subject = ToEnumerable([0, 0, 1, 2, 3,]);
				List<int> expected = [1, 3,];

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with [1, 3],
					             but it contained 2 at index 3 instead of 1

					             Collection:
					             [0, 0, 1, 2, 3]
					             """)
					.Because("a collection argument is the expected sequence and not a single expected item");
			}

			[Fact]
			public async Task WhenExpectedIsAnEmptyNonGenericCollection_ShouldThrowArgumentException()
			{
				IEnumerable subject = ToEnumerable([1, 2,]);

				async Task Act()
					=> await That(subject).EndsWith(new ArrayList());

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectedIsANonGenericCollection_ShouldUseItsItemsAsExpectedSequence()
			{
				IEnumerable subject = ToEnumerable([0, 0, 1, 2, 3,]);
				ArrayList expected = new() { 1, 3, };

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with [1, 3],
					             but it contained 2 at index 3 instead of 1

					             Collection:
					             [0, 0, 1, 2, 3]
					             """)
					.Because("a collection argument without an item type is the expected sequence and not a single expected item");
			}

			[Fact]
			public async Task WhenExpectedIsANonGenericCollectionThatMatches_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);
				ArrayList expected = new() { 2, 3, };

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectedIsAString_ShouldUseItAsSingleExpectedItem()
			{
				IEnumerable subject = ToEnumerable(["bar", "baz",]);

				async Task Act()
					=> await That(subject).EndsWith("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with ["foo"],
					             but it contained "baz" at index 1 instead of "foo"

					             Collection:
					             [
					               "bar",
					               "baz"
					             ]
					             """)
					.Because("a string argument is a single expected item and not a sequence of characters");
			}

			[Fact]
			public async Task WhenSubjectHasOnlyOneItemAndMissesOne_ShouldUseSingular()
			{
				IEnumerable subject = ToEnumerable([2,]);

				async Task Act()
					=> await That(subject).EndsWith(1, 2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with [1, 2],
					             but it contained only 1 item and misses 1 item: [
					               1
					             ]

					             Collection:
					             [2]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IEnumerable subject = ToEnumerable(1, 2);

				async Task Act()
					=> await That(subject).EndsWith(Array.Empty<int>());

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject)!.EndsWith(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with [0],
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectEndsWithNull_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable<string?>("a", null);
				string?[] expected = [null,];

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
