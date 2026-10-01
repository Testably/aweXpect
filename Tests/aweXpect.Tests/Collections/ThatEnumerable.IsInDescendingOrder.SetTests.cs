using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsInDescendingOrder
	{
		public sealed class SetTests
		{
			[Fact]
			public async Task ShouldUseTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(new ReverseComparer()) { "a", "b", "c", };

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order using the subject's ReverseComparer,
					             but it had "c" before "b", which is not in descending order

					             Collection:
					             [
					               "c",
					               "b",
					               "a"
					             ]
					             """)
					.Because("a sorted set is always in ascending order of its own comparer");
			}

			[Fact]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(new ReverseComparer()) { "a", "b", "c", };

				async Task Act()
					=> await That(subject).IsInDescendingOrder().Using(Comparer<string>.Default);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithAMember_ShouldIgnoreTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(new ReverseComparer()) { "a", "b", "c", };

				async Task Act()
					=> await That(subject).IsInDescendingOrder(x => x);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTheDefaultComparer_ShouldNameTheComparerOfTheSet()
			{
				SortedSet<string> subject = ["a", "b",];

				async Task Act()
					=> await That(subject).IsInDescendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is in descending order using the subject's GenericComparer<string>,
					             but it had "a" before "b", which is not in descending order

					             Collection:
					             [
					               "a",
					               "b"
					             ]
					             """)
					.Because("the default comparer of a string set differs from the ordinal default order");
			}
		}
	}
}