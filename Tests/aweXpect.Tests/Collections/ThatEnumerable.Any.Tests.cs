using System.Collections;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections.Immutable;
#endif

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed class Any
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAtLeastOneItemMatches_ShouldSucceed()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.IsEqualTo(3));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenMultipleItemsMatch_ShouldSucceed()
			{
				int[] subject = [1, 2, 2, 3, 2,];

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.IsEqualTo(2));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoItemsMatch_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.IsEqualTo(99));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 99 for at least one item,
					             but none of 5 were

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsEmpty_ShouldFail()
			{
				IEnumerable<int> subject = [];

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.IsEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for at least one item,
					             but none of 0 were

					             Collection:
					             []
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable<int>? subject = null;

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.IsEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1 for at least one item,
					             but it was <null>
					             """);
			}
		}

		public sealed class EnumerableTests
		{
			[Fact]
			public async Task WhenAtLeastOneItemMatches_ShouldSucceed()
			{
				IEnumerable subject = new object[] { "a", 1, "b", };

				async Task Act()
					=> await That(subject).Any().Satisfy(x => x is int);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoItemsMatch_ShouldFail()
			{
				IEnumerable subject = new object[] { "a", "b", "c", };

				async Task Act()
					=> await That(subject).Any().Satisfy(x => x is int);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             satisfies x => x is int for at least one item,
					             but none of 3 did

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """);
			}
		}

		public sealed class StringTests
		{
			[Fact]
			public async Task WhenAtLeastOneItemMatches_ShouldSucceed()
			{
				IEnumerable<string?> subject = ["apple", "banana", "cherry",];

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.StartsWith("b"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoItemsMatch_ShouldFail()
			{
				IEnumerable<string?> subject = ["apple", "cherry",];

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.StartsWith("b"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with "b" for at least one item,
					             but none of 2 were

					             Collection:
					             [
					               "apple",
					               "cherry"
					             ]
					             """);
			}
		}

#if NET8_0_OR_GREATER
		public sealed class ImmutableTests
		{
			[Fact]
			public async Task WhenAtLeastOneItemMatches_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.IsEqualTo(3));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenNoItemsMatch_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).Any().ComplyWith(it => it.IsEqualTo(99));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 99 for at least one item,
					             but none of 3 were

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsImmutableArrayOfStrings_ShouldSucceed()
			{
				ImmutableArray<string?> subject = ["apple", "banana",];

				async Task Act()
					=> await That(subject).Any().Satisfy(x => x == "banana");

				await That(Act).DoesNotThrow();
			}
		}
#endif

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAnyItemComplies_ShouldFail()
			{
				int[] subject = [1, 2, 3, 4, 5,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.Any().ComplyWith(x => x.IsGreaterThan(2)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2 for not at least one item,
					             but 3 of 5 were

					             Collection:
					             [1, 2, 3, 4, 5]
					             """);
			}
		}
	}
}
