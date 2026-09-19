#if NET8_0_OR_GREATER
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class AtMost
	{
		public sealed class ImmutableItemsTests
		{
			[Fact]
			public async Task WhenEnumerableContainsSufficientlyFewEqualItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 1, 1, 1, 2, 2, 3,];

				async Task Act()
					=> await That(subject).AtMost(3).AreEqualTo(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsTooManyEqualItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 1, 1, 1, 2, 2, 3,];

				async Task Act()
					=> await That(subject).AtMost(3).AreEqualTo(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for at most 3 items,
					             but 4 of 7 were

					             Matching items:
					             [1, 1, 1, 1]

					             Collection:
					             [1, 1, 1, 1, 2, 2, 3]
					             """);
			}
		}

		public sealed class ImmutableStringTests
		{
			[Fact]
			public async Task ShouldSupportIgnoringCase()
			{
				ImmutableArray<string?> subject = ["foo", "FOO", "bar",];

				async Task Act()
					=> await That(subject).AtMost(1).AreEqualTo("foo").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" ignoring case for at most one item,
					             but 2 of 3 were

					             Matching items:
					             [
					               "foo",
					               "FOO"
					             ]

					             Collection:
					             [
					               "foo",
					               "FOO",
					               "bar"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedNumberOfEqualItems_ShouldSucceed()
			{
				ImmutableArray<string> subject = ["foo", "foo", "bar",];

				async Task Act()
					=> await That(subject)!.AtMost(2).AreEqualTo("foo");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsTooManyEqualItems_ShouldFail()
			{
				ImmutableArray<string> subject = ["foo", "foo", "bar",];

				async Task Act()
					=> await That(subject)!.AtMost(1).AreEqualTo("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" for at most one item,
					             but 2 of 3 were

					             Matching items:
					             [
					               "foo",
					               "foo"
					             ]

					             Collection:
					             [
					               "foo",
					               "foo",
					               "bar"
					             ]
					             """);
			}
		}
	}
}
#endif
