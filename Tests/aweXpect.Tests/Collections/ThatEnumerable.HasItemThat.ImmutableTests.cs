#if NET8_0_OR_GREATER
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasItemThat
	{
		public sealed class ImmutableTests
		{
			[Fact]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(1)).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has item that is equal to 1 at index 2,
					              but it had item 2 at index 2

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldSucceed()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(2)).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsNoItemAtGivenIndex_ShouldFail()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(3)).AtIndex(3);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has item that is equal to 3 at index 3,
					              but it did not contain any item at index 3

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenEnumerableContainsNullItemAtGivenIndex_ShouldFail()
			{
				ImmutableArray<string?> subject = ["a", null,];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotNull()).AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is not null at index 1,
					             but it had item <null> at index 1

					             Collection:
					             [
					               "a",
					               <null>
					             ]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableIsEmpty_ShouldFail()
			{
				ImmutableArray<int> subject = [];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsNotEqualTo(0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is not equal to 0,
					             but it did not contain any item

					             Collection:
					             []
					             """);
			}

			[Fact]
			public async Task WithInvalidMatch_ShouldNotMatch()
			{
				ImmutableArray<int> subject = [0, 1, 2, 3, 4,];

				async Task Act()
					=> await That(subject).HasItemThat(it => it.IsEqualTo(2)).WithInvalidMatch();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that is equal to 2 with invalid match,
					             but it did not contain any item with invalid match

					             Collection:
					             [0, 1, 2, 3, 4]
					             """);
			}

			[Fact]
			public async Task WithMultipleFailures_ShouldIncludeCollectionOnlyOnce()
			{
				ImmutableArray<string> subject = ["a", "b", "c",];

				async Task Act()
					=> await That(subject).HasItemThat(x => x.StartsWith("a").And.EndsWith("b")).AtIndex(0).And
						.HasItemThat(x => x.Contains("c").IgnoringCase()).AtIndex(1);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has item that starts with "a" and ends with "b" at index 0 and has item that contains "c" at least once ignoring case at index 1,
					             but it had item "a" at index 0 and it had item "b" at index 1

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}
		}

		public sealed class ImmutableNegatedTests
		{
			[Fact]
			public async Task WhenEnumerableContainsDifferentItemAtGivenIndex_ShouldSucceed()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasItemThat(x => x.IsEqualTo(1)).AtIndex(2));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsExpectedItemAtGivenIndex_ShouldFail()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.HasItemThat(x => x.IsEqualTo(2)).AtIndex(2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have item that is equal to 2 at index 2,
					             but it had item 2 at index 2

					             Collection:
					             [0, 1, 2]
					             """);
			}
		}
	}
}
#endif
