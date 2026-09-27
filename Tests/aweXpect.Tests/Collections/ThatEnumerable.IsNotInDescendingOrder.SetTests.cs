using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotInDescendingOrder
	{
		public sealed class SetTests
		{
			[Fact]
			public async Task ShouldUseTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(new ReverseComparer()) { "a", "b", "c", };

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(new ReverseComparer()) { "a", "b", "c", };

				async Task Act()
					=> await That(subject).IsNotInDescendingOrder().Using(Comparer<string>.Default);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in descending order using GenericComparer<string>,
					             but it was

					             Collection:
					             [
					               "c",
					               "b",
					               "a"
					             ]
					             """);
			}
		}
	}
}