using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotInAscendingOrder
	{
		public sealed class SetTests
		{
			[Fact]
			public async Task ShouldUseTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(new ReverseComparer()) { "a", "b", "c", };

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order using the subject's ReverseComparer,
					             but it was

					             Collection:
					             [
					               "c",
					               "b",
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(new ReverseComparer()) { "a", "b", "c", };

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder().Using(Comparer<string>.Default);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithTheOrdinalComparer_ShouldNotNameTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(StringComparer.Ordinal) { "a", "B", };

				async Task Act()
					=> await That(subject).IsNotInAscendingOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not in ascending order,
					             but it was

					             Collection:
					             [
					               "B",
					               "a"
					             ]
					             """)
					.Because("the ordinal comparer is the default order for strings");
			}
		}
	}
}