#if NET8_0_OR_GREATER
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class Satisfy
		{
			public sealed class ImmutableItemTests
			{
				[Fact]
				public async Task WhenEnumerableContainsDifferentValues_ShouldFail()
				{
					ImmutableArray<int> subject = [1, 1, 1, 1, 2, 2, 3,];

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == 1);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x == 1 for all items,
						             but only 4 of 7 did

						             Not matching items:
						             [2, 2, 3]

						             Collection:
						             [1, 1, 1, 1, 2, 2, 3]
						             """);
				}

				[Fact]
				public async Task WhenEnumerableIsEmpty_ShouldSucceed()
				{
					ImmutableArray<int> subject = [];

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == 0);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsEqualValues_ShouldSucceed()
				{
					ImmutableArray<int> subject = [1, 1, 1, 1, 1, 1, 1,];

					async Task Act()
						=> await That(subject).All().Satisfy(x => x == 1);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
				{
					ImmutableArray<int> subject = [1, 1, 2, 3, 5, 8,];

					async Task Act()
						=> await That(subject).All().Satisfy(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("predicate").And
						.WithMessage("The 'predicate' cannot be null.").AsPrefix();
				}
			}

			public sealed class ImmutableStringTests
			{
				[Fact]
				public async Task WhenEnumerableContainsDifferentValues_ShouldFail()
				{
					ImmutableArray<string?> subject = ["foo", "bar", "baz",];

					async Task Act()
						=> await That(subject).All().Satisfy(x => x?.StartsWith("ba") == true);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             satisfies x => x?.StartsWith("ba") == true for all items,
						             but only 2 of 3 did

						             Not matching items:
						             [
						               "foo"
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
						=> await That(subject).All().Satisfy(x => x == "");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsMatchingValues_ShouldSucceed()
				{
					ImmutableArray<string> subject = ["foo", "bar", "baz",];

					async Task Act()
						=> await That(subject)!.All().Satisfy(x => x?.Length == 3);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
				{
					ImmutableArray<string> subject = ["foo", "bar", "baz",];

					async Task Act()
						=> await That(subject)!.All().Satisfy(null!);

					await That(Act).Throws<ArgumentNullException>()
						.WithParamName("predicate").And
						.WithMessage("The 'predicate' cannot be null.").AsPrefix();
				}
			}
		}
	}
}
#endif
