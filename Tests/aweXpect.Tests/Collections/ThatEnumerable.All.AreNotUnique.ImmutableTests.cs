#if NET8_0_OR_GREATER
using System.Collections.Immutable;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreNotUnique
		{
			public sealed class ImmutableArrayTests
			{
				[Test]
				public async Task ShouldUseCustomComparer()
				{
					ImmutableArray<int> subject = [1, 1, 1,];

					async Task Act()
						=> await That(subject).All().AreNotUnique().Using(new AllDifferentComparer());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not unique using AllDifferentComparer for all items,
						             but none of 3 were

						             Not matching items:
						             [1, 1, 1]

						             Collection:
						             [1, 1, 1]
						             """);
				}

				[Test]
				public async Task WhenAllItemsAreDuplicated_ShouldSucceed()
				{
					ImmutableArray<int> subject = [1, 2, 1, 2,];

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSomeItemsAreUnique_ShouldFail()
				{
					ImmutableArray<int> subject = [1, 2, 1, 3,];

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not unique for all items,
						             but only 2 of 4 were

						             Not matching items:
						             [2, 3]

						             Collection:
						             [1, 2, 1, 3]
						             """);
				}
			}

			public sealed class ImmutableArrayMemberTests
			{
				[Test]
				public async Task WhenAllMembersAreDuplicated_ShouldSucceed()
				{
					ImmutableArray<MyClass> subject =
					[
						new(1), new(2), new(1),
						new(2),
					];

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllStringMembersAreDuplicated_ShouldSucceed()
				{
					ImmutableArray<MyClass> subject = [new(1, "a"), new(2, "a"),];

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x.StringValue);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ImmutableArrayStringTests
			{
				[Test]
				public async Task ShouldUseCustomComparer()
				{
					ImmutableArray<string?> subject = ["a", "b", "c",];

					async Task Act()
						=> await That(subject).All().AreNotUnique().Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllItemsAreDuplicated_ShouldSucceed()
				{
					ImmutableArray<string?> subject = ["a", "b", "a", "b",];

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllMembersAreDuplicated_ShouldSucceed()
				{
					ImmutableArray<string?> subject = ["a", "b", "cc", "dd",];

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x!.Length);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenAllStringMembersAreDuplicated_ShouldSucceed()
				{
					ImmutableArray<string?> subject = ["a", "A",];

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x!).IgnoringCase();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldSucceed()
				{
					ImmutableArray<string?> subject = ["a", "A",];

					async Task Act()
						=> await That(subject).All().AreNotUnique().IgnoringCase();

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
