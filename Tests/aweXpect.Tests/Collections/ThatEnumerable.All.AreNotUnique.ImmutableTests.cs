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
				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					ImmutableArray<int> subject = [1, 1, 1,];

					async Task Act()
						=> await That(subject).All().AreNotUnique().Using(new AllDifferentComparer());

					await That(Act).Throws<XunitException>()
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

				[Fact]
				public async Task WhenAllItemsAreDuplicated_ShouldSucceed()
				{
					ImmutableArray<int> subject = [1, 2, 1, 2,];

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSomeItemsAreUnique_ShouldFail()
				{
					ImmutableArray<int> subject = [1, 2, 1, 3,];

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).Throws<XunitException>()
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
				[Fact]
				public async Task WhenAllMembersAreDuplicated_ShouldSucceed()
				{
					ImmutableArray<MyClass> subject = [new MyClass(1), new MyClass(2), new MyClass(1),
						new MyClass(2),
					];

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x.Value);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllStringMembersAreDuplicated_ShouldSucceed()
				{
					ImmutableArray<MyClass> subject = [new MyClass(1, "a"), new MyClass(2, "a"),];

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x.StringValue);

					await That(Act).DoesNotThrow();
				}
			}

			public sealed class ImmutableArrayStringTests
			{
				[Fact]
				public async Task ShouldUseCustomComparer()
				{
					ImmutableArray<string?> subject = ["a", "b", "c",];

					async Task Act()
						=> await That(subject).All().AreNotUnique().Using(new AllEqualComparer());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllItemsAreDuplicated_ShouldSucceed()
				{
					ImmutableArray<string?> subject = ["a", "b", "a", "b",];

					async Task Act()
						=> await That(subject).All().AreNotUnique();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenDiffersInCasingAndCasingIsIgnored_ShouldSucceed()
				{
					ImmutableArray<string?> subject = ["a", "A",];

					async Task Act()
						=> await That(subject).All().AreNotUnique().IgnoringCase();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllMembersAreDuplicated_ShouldSucceed()
				{
					ImmutableArray<string?> subject = ["a", "b", "cc", "dd",];

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x!.Length);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenAllStringMembersAreDuplicated_ShouldSucceed()
				{
					ImmutableArray<string?> subject = ["a", "A",];

					async Task Act()
						=> await That(subject).All().AreNotUnique(x => x!).IgnoringCase();

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
#endif
