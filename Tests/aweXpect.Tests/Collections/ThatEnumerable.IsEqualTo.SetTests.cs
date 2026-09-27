using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsEqualTo
	{
		public sealed class SetTests
		{
			[Fact]
			public async Task Equivalent_ShouldIgnoreTheComparerOfTheSet()
			{
				HashSet<int> subject = new(new ModuloComparer(10)) { 1, };

				async Task Act()
					=> await That(subject).IsEqualTo([11,]).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection [11,] using equivalency in order,
					             but it contained item 1 at index 0 instead of 11

					             Collection:
					             [1]

					             Expected:
					             [11]
					             """);
			}

			[Fact]
			public async Task ForASortedSet_InSameOrder_ShouldUseTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).IsEqualTo(["A", "B",]);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task IgnoringCase_ShouldIgnoreTheComparerOfTheSet()
			{
				HashSet<string> subject = new(new AllDifferentComparer()) { "a", };

				async Task Act()
					=> await That(subject).IsEqualTo(["A",]).IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task InAnyOrder_ShouldUseTheComparerOfTheSet()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).IsEqualTo(["B", "A",]).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", };

				async Task Act()
					=> await That(subject).IsEqualTo(["A",]).Using(new AllDifferentComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection ["A",] using AllDifferentComparer in order,
					             but it contained item "a" at index 0 instead of "A"

					             Collection:
					             [
					               "a"
					             ]

					             Expected:
					             [
					               "A"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenExpectedContainsTwoItemsThatTheSetUnifies_ShouldFail()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", };

				async Task Act()
					=> await That(subject).IsEqualTo(["a", "A",]).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection ["a", "A",] in any order,
					             but it lacked 1 of 2 expected items: "A"

					             Collection:
					             [
					               "a"
					             ]

					             Expected:
					             [
					               "a",
					               "A"
					             ]
					             """)
					.Because("a set holds an item at most once, so one item cannot stand in for two");
			}

			[Fact]
			public async Task WhenItemsAreNull_ShouldNotAskTheComparerOfTheSet()
			{
				HashSet<string?> subject = new(new NullRejectingComparer()) { null, "a", };

				async Task Act()
					=> await That(subject).IsEqualTo([null, "A",]).InAnyOrder();

				await That(Act).DoesNotThrow()
					.Because("a comparer may reject null, which only equals null");
			}

			[Fact]
			public async Task WhenSetContainsOtherItemsAccordingToItsComparer_ShouldFail()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).IsEqualTo(["A", "C",]).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection ["A", "C",] in any order,
					             but it
					               contained item "b" at index 1 that was not expected and
					               lacked 1 of 2 expected items: "C"

					             Collection:
					             [
					               "a",
					               "b"
					             ]

					             Expected:
					             [
					               "A",
					               "C"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSetContainsTheItemsAccordingToItsComparer_ShouldSucceed()
			{
				HashSet<int> subject = new(new ModuloComparer(10)) { 1, 2, };

				async Task Act()
					=> await That(subject).IsEqualTo([12, 11,]).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDefaultComparer_ShouldCompareNumbersOfDifferentTypesByValue()
			{
				HashSet<object> subject = [1,];

				async Task Act()
					=> await That(subject).IsEqualTo([1L,]);

				await That(Act).DoesNotThrow()
					.Because("a set with the default comparer keeps the default equality, which compares numbers by value");
			}

			private sealed class ModuloComparer(int modulus) : IEqualityComparer<int>
			{
				public bool Equals(int x, int y) => x % modulus == y % modulus;

				public int GetHashCode(int obj) => obj % modulus;
			}

			private sealed class NullRejectingComparer : IEqualityComparer<string?>
			{
				public bool Equals(string? x, string? y)
					=> x is null || y is null
						? throw new ArgumentNullException(x is null ? nameof(x) : nameof(y))
						: StringComparer.OrdinalIgnoreCase.Equals(x, y);

				public int GetHashCode(string? obj) => obj!.Length;
			}
		}
	}
}
