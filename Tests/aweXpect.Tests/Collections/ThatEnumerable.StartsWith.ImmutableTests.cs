#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Collections.Immutable;
using aweXpect.Equivalency;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class StartsWith
	{
		public sealed class ImmutableTests
		{
			[Fact]
			public async Task ShouldSupportEquivalent()
			{
				ImmutableArray<MyClass> subject = [..Factory.GetFibonacciNumbers(x => new MyClass(x), 20),];

				async Task Act()
					=> await That(subject).StartsWith(new MyClass(1), new MyClass(1), new MyClass(2)).Equivalent();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenCollectionsAreIdentical_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).StartsWith(1, 2, 3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableHasDifferentStartingElements_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				IEnumerable<int> expected = [1, 3,];

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with expected,
					             but it contained 2 at index 1 instead of 3

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenExpectedContainsAdditionalElements_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];
				IEnumerable<int> expected = [1, 2, 3, 4,];

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with expected,
					             but it contained only 3 items and misses 1 items: [
					               4
					             ]

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsEmpty_ShouldThrowArgumentException()
			{
				ImmutableArray<int> subject = [1,];

				async Task Act()
					=> await That(subject).StartsWith();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' collection cannot be empty.").AsPrefix();
			}
		}

		public sealed class ImmutableStringTests
		{
			[Fact]
			public async Task ShouldIncludeOptionsInFailureMessage()
			{
				ImmutableArray<string?> subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject).StartsWith("FOO", "BAZ").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with ["FOO", "BAZ"] ignoring case,
					             but it contained "bar" at index 1 instead of "BAZ"

					             Collection:
					             [
					               "foo",
					               "bar",
					               "baz"
					             ]
					             """);
			}

			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				ImmutableArray<string> subject = ["foo", "bar", "baz",];

				async Task Act()
					=> await That(subject)!.StartsWith("FOO", "BAR").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectStartsWithExpectedValues_ShouldSucceed()
			{
				ImmutableArray<string> subject = ["foo", "bar", "baz",];
				IEnumerable<string> expected = ToEnumerable(["foo", "bar",]);

				async Task Act()
					=> await That(subject)!.StartsWith(expected);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
