#if NET8_0_OR_GREATER
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotHaveItemThat
	{
		public sealed class ImmutableTests
		{
			[Fact]
			public async Task WhenEnumerableContainsMatchingItemAtGivenIndex_ShouldFail()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(2)).AtIndex(2);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have item that is equal to 2 at index 2,
					              but it had item 2 at index 2

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenEnumerableContainsOtherItemAtGivenIndex_ShouldSucceed()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(1)).AtIndex(2);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenEnumerableHasFewerItemsThanTheGivenIndex_ShouldSucceed()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(2)).AtIndex(3);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WithAnyIndex_WhenAnItemMatches_ShouldFail()
			{
				ImmutableArray<int> subject = [0, 1, 2,];

				async Task Act()
					=> await That(subject).DoesNotHaveItemThat(it => it.IsEqualTo(1));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              does not have item that is equal to 1,
					              but it had item 1

					              Collection:
					              {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
#endif
