using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsContainedIn
	{
		public sealed class SetTests
		{
			[Fact]
			public async Task ForADoubleSet_ShouldUseTheComparerOfTheSet()
			{
				HashSet<double> subject = new(new RoundingComparer()) { 1.0, 2.0, };

#pragma warning disable aweXpect0006
				async Task Act()
					=> await That(subject).IsContainedIn([1.2, 1.8, 3.0,]);
#pragma warning restore aweXpect0006

				await That(Act).DoesNotThrow()
					.Because("an element type that allows a tolerance keeps the comparer of the set until one is specified");
			}

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

			[Fact]
			public async Task Within_ShouldIgnoreTheComparerOfTheSet()
			{
				HashSet<double> subject = new(new RoundingComparer()) { 1.0, 2.0, };

#pragma warning disable aweXpect0006
				async Task Act()
					=> await That(subject).IsContainedIn([1.25, 1.75, 3.0,]).Within(0.125);
#pragma warning restore aweXpect0006

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection [1.25, 1.75, 3.0,] ± 0.125 in order and contiguous,
					             but it
					               contained item 1.0 at index 0 that was not expected and
					               contained item 2.0 at index 1 that was not expected

					             Collection:
					             [1.0, 2.0]

					             Expected:
					             [1.25, 1.75, 3.0]
					             """)
					.Because("the tolerance replaces the comparer of the set");
			}

			private sealed class RoundingComparer : IEqualityComparer<double>
			{
				public bool Equals(double x, double y) => Math.Round(x) == Math.Round(y);

				public int GetHashCode(double obj) => Math.Round(obj).GetHashCode();
			}
		}
	}
}
