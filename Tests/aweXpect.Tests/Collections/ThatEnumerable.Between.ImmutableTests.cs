#if NET8_0_OR_GREATER
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Between
	{
		public sealed class ImmutableItemsTests
		{
			[Test]
			public async Task WhenEnumerableContainsSufficientlyEqualItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 1, 1, 1, 2, 2, 3,];

				async Task Act()
					=> await That(subject).Between(3).And(4).AreEqualTo(1);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsTooFewEqualItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 1, 1, 1, 2, 2, 3,];

				async Task Act()
					=> await That(subject).Between(3).And(4).AreEqualTo(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2 for between 3 and 4 items,
					             but only 2 of 7 were

					             Collection:
					             [1, 1, 1, 1, 2, 2, 3]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsTooManyEqualItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 1, 1, 1, 2, 2, 3,];

				async Task Act()
					=> await That(subject).Between(1).And(3).AreEqualTo(1);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for between 1 and 3 items,
					             but 4 of 7 were

					             Collection:
					             [1, 1, 1, 1, 2, 2, 3]
					             """);
			}
		}

		public sealed class ImmutableStringTests
		{
			[Test]
			public async Task ShouldSupportIgnoringCase()
			{
				ImmutableArray<string?> subject = ["foo", "FOO", "bar",];

				async Task Act()
					=> await That(subject).Between(0).And(1).AreEqualTo("foo").IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" ignoring case for between 0 and 1 items,
					             but 2 of 3 were

					             Collection:
					             [
					               "foo",
					               "FOO",
					               "bar"
					             ]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsExpectedNumberOfEqualItems_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["foo", "foo", "bar",];

				async Task Act()
					=> await That(subject).Between(2).And(3).AreEqualTo("foo");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenEnumerableContainsTooFewEqualItems_ShouldFail()
			{
				ImmutableArray<string> subject = ["foo", "FOO", "foo", "bar",];

				async Task Act()
					=> await That(subject)!.Between(3).And(4).AreEqualTo("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" for between 3 and 4 items,
					             but only 2 of 4 were

					             Collection:
					             [
					               "foo",
					               "FOO",
					               "foo",
					               "bar"
					             ]
					             """);
			}

			[Test]
			public async Task WhenEnumerableContainsTooManyEqualItems_ShouldFail()
			{
				ImmutableArray<string> subject = ["foo", "foo", "bar",];

				async Task Act()
					=> await That(subject)!.Between(0).And(1).AreEqualTo("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to "foo" for between 0 and 1 items,
					             but 2 of 3 were

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
