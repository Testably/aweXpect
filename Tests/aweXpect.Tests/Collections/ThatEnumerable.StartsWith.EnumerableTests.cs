using System.Collections;
using System.Collections.Generic;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class StartsWith
	{
		public sealed class EnumerableTests
		{
			[Fact]
			public async Task DoesNotEnumerateTwice()
			{
				IEnumerable subject = new ThrowWhenIteratingTwiceEnumerable();

				async Task Act()
					=> await That(subject).StartsWith(1)
						.And.StartsWith(1);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task DoesNotMaterializeEnumerable()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers();

				async Task Act()
					=> await That(subject).StartsWith(1, 1, 2, 3, 5);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldSupportEquivalent()
			{
				IEnumerable subject = Factory.GetFibonacciNumbers(x => new MyClass(x), 20);

				async Task Act()
					=> await That(subject).StartsWith(new MyClass(1), new MyClass(1), new MyClass(2)).Equivalent();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenCollectionsAreIdentical_ShouldSucceed()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).StartsWith(1, 2, 3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableHasDifferentStartingElements_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);
				IEnumerable<int> expected = [1, 3,];

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with [1, 3],
					             but it contained 2 at index 1 instead of 3

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenExpectedContainsAdditionalElements_ShouldFail()
			{
				IEnumerable subject = ToEnumerable([1, 2, 3,]);
				IEnumerable<int> expected = [1, 2, 3, 4,];

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with [1, 2, 3, 4],
					             but it contained only 3 items and misses 1 item: [
					               4
					             ]

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenSubjectHasOnlyOneItemAndMissesOne_ShouldUseSingular()
			{
				IEnumerable subject = ToEnumerable([1,]);

				async Task Act()
					=> await That(subject).StartsWith(1, 2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with [1, 2],
					             but it contained only 1 item and misses 1 item: [
					               2
					             ]

					             Collection:
					             [1]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject)!.StartsWith(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with [0],
					             but it was <null>
					             """);
			}
		}
	}
}
