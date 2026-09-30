using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsInAscendingOrder
	{
		public sealed class SetTests
		{
			[Fact]
			public async Task ForAnImmutableSortedSet_ShouldUseTheComparerOfTheSet()
			{
				ImmutableSortedSet<string> subject = ImmutableSortedSet.Create(new ReverseComparer(), "a", "b", "c");

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForAnUntypedSortedSet_ShouldUseTheDefaultOrder()
			{
				IEnumerable subject = new SortedSet<string>(new ReverseComparer()) { "a", "b", "c", };

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order,
					             but it had "c" before "b", which is not in ascending order

					             Collection:
					             [
					               "c",
					               "b",
					               "a"
					             ]
					             """)
					.Because("the comparer of a set of another item type than the compared one cannot be read");
			}

			[Fact]
			public async Task ShouldUseTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(new ReverseComparer()) { "a", "b", "c", };

				async Task Act()
					=> await That(subject).IsInAscendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(new ReverseComparer()) { "a", "b", "c", };

				async Task Act()
					=> await That(subject).IsInAscendingOrder().Using(Comparer<string>.Default);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order using GenericComparer<string>,
					             but it had "c" before "b", which is not in ascending order

					             Collection:
					             [
					               "c",
					               "b",
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task WithAMember_ShouldIgnoreTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(new ReverseComparer()) { "a", "b", "c", };

				async Task Act()
					=> await That(subject).IsInAscendingOrder(x => x);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in ascending order by x => x,
					             but it had "c" before "b", which is not in ascending order

					             Collection:
					             [
					               "c",
					               "b",
					               "a"
					             ]
					             """)
					.Because("the comparer of the set orders the items, not their members");
			}
		}
	}
}