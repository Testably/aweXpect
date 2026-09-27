#if NET8_0_OR_GREATER
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class None
	{
		public sealed partial class AreEqualTo
		{
			public sealed class ImmutableItemsTests
			{
				[Fact]
				public async Task WhenEnumerableContainsEqualValues_ShouldFail()
				{
					ImmutableArray<int> subject = [1, 1, 1, 1, 2, 2, 3,];

					async Task Act()
						=> await That(subject).None().AreEqualTo(1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for no items,
						             but 4 of 7 were

						             Matching items:
						             [1, 1, 1, 1]

						             Collection:
						             [1, 1, 1, 1, 2, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldSucceed()
				{
					ImmutableArray<int> subject = [];

					async Task Act()
						=> await That(subject).None().AreEqualTo(0);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsDifferentValues_ShouldSucceed()
				{
					ImmutableArray<int> subject = [1, 1, 1, 1, 2, 2, 3,];

					async Task Act()
						=> await That(subject).None().AreEqualTo(42);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ImmutableStringTests
			{
				[Fact]
				public async Task AsPrefix_WhenAnItemMatchesThePattern_ShouldFail()
				{
					ImmutableArray<string?> subject = ["text", "# Title", "## Intro",];

					async Task Act()
						=> await That(subject).None().AreEqualTo("#").AsPrefix();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             starts with "#" for no items,
						             but 2 of 3 were

						             Matching items:
						             [
						               "# Title",
						               "## Intro"
						             ]

						             Collection:
						             [
						               "text",
						               "# Title",
						               "## Intro"
						             ]
						             """);
				}

				[Fact]
				public async Task AsRegex_WhenAnItemMatchesThePattern_ShouldFail()
				{
					ImmutableArray<string?> subject = ["text", "# Title", "## Intro",];

					async Task Act()
						=> await That(subject).None().AreEqualTo("^#").AsRegex();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             matches regex "^#" for no items,
						             but 2 of 3 were

						             Matching items:
						             [
						               "# Title",
						               "## Intro"
						             ]

						             Collection:
						             [
						               "text",
						               "# Title",
						               "## Intro"
						             ]
						             """);
				}

				[Fact]
				public async Task AsSuffix_WhenAnItemMatchesThePattern_ShouldFail()
				{
					ImmutableArray<string?> subject = ["text", "# Title", "## Intro",];

					async Task Act()
						=> await That(subject).None().AreEqualTo("le").AsSuffix();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             ends with "le" for no items,
						             but 1 of 3 were

						             Matching items:
						             [
						               "# Title"
						             ]

						             Collection:
						             [
						               "text",
						               "# Title",
						               "## Intro"
						             ]
						             """);
				}

				[Fact]
				public async Task AsWildcard_WhenAnItemMatchesThePattern_ShouldFail()
				{
					ImmutableArray<string?> subject = ["text", "# Title", "## Intro",];

					async Task Act()
						=> await That(subject).None().AreEqualTo("#*").AsWildcard();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             matches "#*" for no items,
						             but 2 of 3 were

						             Matching items:
						             [
						               "# Title",
						               "## Intro"
						             ]

						             Collection:
						             [
						               "text",
						               "# Title",
						               "## Intro"
						             ]
						             """);
				}

				[Fact]
				public async Task ShouldSupportIgnoringCase()
				{
					ImmutableArray<string?> subject = ["FOO", "BAR", "BAZ",];

					async Task Act()
						=> await That(subject).None().AreEqualTo("bar").IgnoringCase();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "bar" ignoring case for no items,
						             but 1 of 3 were

						             Matching items:
						             [
						               "BAR"
						             ]

						             Collection:
						             [
						               "FOO",
						               "BAR",
						               "BAZ"
						             ]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableContainsEqualValues_ShouldFail()
				{
					ImmutableArray<string> subject = ["foo", "bar", "baz",];

					async Task Act()
						=> await That(subject)!.None().AreEqualTo("bar");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "bar" for no items,
						             but 1 of 3 were

						             Matching items:
						             [
						               "bar"
						             ]

						             Collection:
						             [
						               "foo",
						               "bar",
						               "baz"
						             ]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldSucceed()
				{
					ImmutableArray<string?> subject = [];

					async Task Act()
						=> await That(subject).None().AreEqualTo("foo");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsDifferentValues_ShouldSucceed()
				{
					ImmutableArray<string> subject = ["FOO", "BAR", "BAZ",];

					async Task Act()
						=> await That(subject)!.None().AreEqualTo("bar");

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
