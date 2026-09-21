#if NET8_0_OR_GREATER
using System.Collections.Immutable;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class ComplyWith
		{
			public sealed class ImmutableTests
			{
				[Fact]
				public async Task WhenEnumerableContainsDifferentValues_ShouldFail()
				{
					ImmutableArray<int> subject = [1, 1, 1, 1, 2, 2, 3,];

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(1));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 for all items,
						             but only 4 of 7 were

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
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(0));

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenEnumerableOnlyContainsEqualValues_ShouldSucceed()
				{
					ImmutableArray<int> subject = [1, 1, 1, 1, 1, 1, 1,];

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.IsEqualTo(1));

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ImmutableNegatedTests
			{
				[Fact]
				public async Task WhenEnumerableOnlyContainsEqualValues_ShouldFail()
				{
					ImmutableArray<int> subject = [1, 1, 1, 1, 1, 1, 1,];

					async Task Act()
						=> await That(subject).All().ComplyWith(x => x.DoesNotComplyWith(it => it.IsEqualTo(1)));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to 1 for all items,
						             but none of 7 were

						             Not matching items:
						             [1, 1, 1, 1, 1, 1, 1]

						             Collection:
						             [1, 1, 1, 1, 1, 1, 1]
						             """);
				}

				[Fact]
				public async Task WhenAllItemsComply_ShouldFail()
				{
					ImmutableArray<int> subject = [1, 1, 1,];

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it
							=> it.All().ComplyWith(x => x.IsEqualTo(1)));

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 1 not for all items,
						             but all 3 were

						             Collection:
						             [1, 1, 1]
						             """);
				}
			}
		}
	}
}
#endif
