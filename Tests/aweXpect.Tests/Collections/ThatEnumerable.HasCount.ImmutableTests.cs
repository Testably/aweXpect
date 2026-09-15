#if NET8_0_OR_GREATER
using System.Collections;
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasCount
	{
		public sealed class ImmutableArrayTests
		{
			[Fact]
			public async Task WhenImmutableArrayContainsMatchingItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenImmutableArrayContainsTooFewItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount(4);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 4 items,
					             but found only 3

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenImmutableArrayContainsTooManyItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but found at least 3

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}

		public sealed class ImmutableArrayEqualToTests
		{
			[Fact]
			public async Task WhenImmutableArrayContainsMatchingItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().EqualTo(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenImmutableArrayContainsTooFewItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().EqualTo(4);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 4 items,
					             but found only 3

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenImmutableArrayContainsTooManyItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().EqualTo(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but found at least 3

					             Collection:
					             [1, 2, 3]
					             """);
			}
		}

		public sealed class ImmutableArrayNotEqualToTests
		{
			[Fact]
			public async Task WhenImmutableArrayContainsMatchingItems_ShouldFail()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().NotEqualTo(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have exactly 3 items,
					             but it did

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenImmutableArrayContainsOtherNumberOfItems_ShouldSucceed()
			{
				ImmutableArray<int> subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).HasCount().NotEqualTo(4);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class EnumerableTests
		{
			[Fact]
			public async Task WhenEnumerableContainsMatchingItems_ShouldSucceed()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsTooFewItems_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount(4);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 4 items,
					             but found only 3

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsTooManyItems_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but found at least 3

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject)!.HasCount(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but it was <null>
					             """);
			}
		}

		public sealed class EnumerableEqualToTests
		{
			[Fact]
			public async Task WhenEnumerableContainsMatchingItems_ShouldSucceed()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount().EqualTo(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableContainsTooFewItems_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount().EqualTo(4);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 4 items,
					             but found only 3

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsTooManyItems_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount().EqualTo(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but found at least 3

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				IEnumerable? subject = null;

				async Task Act()
					=> await That(subject)!.HasCount().EqualTo(2);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has exactly 2 items,
					             but it was <null>
					             """);
			}
		}

		public sealed class EnumerableNotEqualToTests
		{
			[Fact]
			public async Task WhenEnumerableContainsMatchingItems_ShouldFail()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount().NotEqualTo(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have exactly 3 items,
					             but it did

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task WhenEnumerableContainsOtherNumberOfItems_ShouldSucceed()
			{
				IEnumerable subject = new[]
				{
					1, 2, 3,
				};

				async Task Act()
					=> await That(subject).HasCount().NotEqualTo(4);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
#endif
