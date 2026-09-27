using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsContainedIn
	{
		public sealed class SetTests
		{
			[Fact]
			public async Task InAnyOrder_ShouldUseTheComparerOfTheSet()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).IsContainedIn(["C", "B", "A",]).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", };

				async Task Act()
					=> await That(subject).IsContainedIn(["A", "B",]).InAnyOrder().Using(new AllDifferentComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection ["A", "B",] using AllDifferentComparer in any order,
					             but it contained item "a" at index 0 that was not expected

					             Collection:
					             [
					               "a"
					             ]

					             Expected:
					             [
					               "A",
					               "B"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenOnlyTheExpectedCollectionIsASetWithACustomComparer_ShouldUseTheDefaultEquality()
			{
				List<string> subject = ["a",];
				HashSet<string> expected = new(StringComparer.OrdinalIgnoreCase) { "A", "B", };

				async Task Act()
					=> await That(subject).IsContainedIn(expected).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in any order,
					             but it contained item "a" at index 0 that was not expected

					             Collection:
					             [
					               "a"
					             ]

					             Expected:
					             [
					               "A",
					               "B"
					             ]
					             """)
					.Because("only the comparer of the subject decides which items are the same");
			}
		}
	}
}
