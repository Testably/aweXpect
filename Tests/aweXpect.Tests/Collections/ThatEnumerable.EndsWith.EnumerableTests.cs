using System.Collections;
using System.Collections.Generic;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class EndsWith
	{
		public sealed class EnumerableTests
		{
			[Test]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).EndsWith(1)
						.And.EndsWith(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
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

			[Test]
			public async Task WhenCollectionsAreIdentical_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).EndsWith(1, 2, 3);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableHasDifferentEndingElements_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([0, 0, 1, 2, 3,]);
				IEnumerable<int> expected = [1, 3,];

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected,
					             but it contained item 2 at index 3 instead of 1

					             Collection:
					             [0, 0, 1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenExpectedContainsAdditionalElements_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).EndsWith(0, 0, 1, 2, 3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with [0, 0, 1, 2, 3],
					             but it contained only 3 items and lacked 2 items: [
					               0,
					               0
					             ]

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenExpectedIsAList_ShouldUseItsItemsAsExpectedSequence()
			{
				IEnumerable subject = ToEnumerable([0, 0, 1, 2, 3,]);
				List<int> expected = [1, 3,];

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected,
					             but it contained item 2 at index 3 instead of 1

					             Collection:
					             [0, 0, 1, 2, 3]
					             """)
					.Because("a collection argument is the expected sequence and not a single expected item");
			}

			[Test]
			public async Task WhenExpectedIsAnEmptyNonGenericCollection_ShouldThrowArgumentException()
			{
				IEnumerable subject = ToEnumerable([1, 2,]);

				async Task Act()
					=> await That(subject).EndsWith(new ArrayList());

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedIsANonGenericCollection_ShouldUseItsItemsAsExpectedSequence()
			{
				IEnumerable subject = ToEnumerable([0, 0, 1, 2, 3,]);
				ArrayList expected = new()
				{
					1,
					3,
				};

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected,
					             but it contained item 2 at index 3 instead of 1

					             Collection:
					             [0, 0, 1, 2, 3]
					             """)
					.Because("a collection argument without an item type is the expected sequence and not a single expected item");
			}

			[Test]
			public async Task WhenExpectedIsANonGenericCollectionThatMatches_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);
				ArrayList expected = new()
				{
					2,
					3,
				};

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsAString_ShouldUseItAsSingleExpectedItem()
			{
				IEnumerable subject = ToEnumerable(["bar", "baz",]);

				async Task Act()
					=> await That(subject).EndsWith("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with ["foo"],
					             but it contained item "baz" at index 1 instead of "foo"

					             Collection:
					             [
					               "bar",
					               "baz"
					             ]
					             """)
					.Because("a string argument is a single expected item and not a sequence of characters");
			}

			[Test]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				IEnumerable subject = ToEnumerable(1, 2);

				async Task Act()
					=> await That(subject).EndsWith(Array.Empty<int>());

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenRetriedAfterAMismatch_ShouldDescribeTheLastAttempt()
			{
				int attempts = 0;

				IEnumerable GetSubject()
					=> attempts++ == 0
						? new[]
						{
							3,
						}
						: new[]
						{
							2,
						};

				async Task Act()
					=> await That(GetSubject).Eventually().WithinTwoAttempts(5.Seconds())
						.EndsWith(1, 2).WithTimeSystem(new VirtualTimeSystem());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that GetSubject
					             eventually ends with [1, 2] within 0:05,
					             but it contained only 1 item and lacked 1 item: [
					               1
					             ]

					             Collection:
					             [2]
					             """)
					.Because("the mismatch of the first attempt does not apply to the later ones");
			}

			[Test]
			public async Task WhenSubjectEndsWithNull_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable("a", null);
				string?[] expected = [null,];

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectHasOnlyOneItemAndMissesOne_ShouldUseSingular()
			{
				IEnumerable subject = ToEnumerable([2,]);

				async Task Act()
					=> await That(subject).EndsWith(1, 2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with [1, 2],
					             but it contained only 1 item and lacked 1 item: [
					               1
					             ]

					             Collection:
					             [2]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject)!.EndsWith(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with [0],
					             but it was <null>
					             """);
			}
		}
	}
}
