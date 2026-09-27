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

#pragma warning disable aweXpect0006
				async Task Act()
					=> await That(subject).IsEqualTo([11,]).Equivalent();
#pragma warning restore aweXpect0006

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

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForADateOnlySet_ShouldUseTheComparerOfTheSet()
			{
				HashSet<DateOnly> subject = new(new SameMonthComparer()) { new DateOnly(2024, 1, 1), };

				async Task Act()
					=> await That(subject).IsEqualTo([new DateOnly(2024, 1, 31),]);

				await That(Act).DoesNotThrow()
					.Because("an element type that allows a tolerance keeps the comparer of the set until one is specified");
			}
#endif

			[Fact]
			public async Task ForADoubleSet_ShouldUseTheComparerOfTheSet()
			{
				HashSet<double> subject = new(new RoundingComparer()) { 1.0, 2.0, };

				async Task Act()
					=> await That(subject).IsEqualTo([1.2, 1.8,]);

				await That(Act).DoesNotThrow()
					.Because("an element type that allows a tolerance keeps the comparer of the set until one is specified");
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

#pragma warning disable aweXpect0006
				async Task Act()
					=> await That(subject).IsEqualTo(["A",]).IgnoringCase();
#pragma warning restore aweXpect0006

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

#pragma warning disable aweXpect0006
				async Task Act()
					=> await That(subject).IsEqualTo(["A",]).Using(new AllDifferentComparer());
#pragma warning restore aweXpect0006

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

#pragma warning disable aweXpect0006
				async Task Act()
					=> await That(subject).IsEqualTo([1L,]);
#pragma warning restore aweXpect0006

				await That(Act).DoesNotThrow()
					.Because("a set with the default comparer keeps the default equality, which compares numbers by value");
			}

			[Fact]
			public async Task Within_ShouldIgnoreTheComparerOfTheSet()
			{
				HashSet<double> subject = new(new RoundingComparer()) { 1.0, 2.0, };

				async Task Act()
					=> await That(subject).IsEqualTo([1.25, 1.75,]).Within(0.125);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to collection [1.25, 1.75,] ± 0.125 in order,
					             but it
					               contained item 1.0 at index 0 instead of 1.25 and
					               contained item 2.0 at index 1 instead of 1.75

					             Collection:
					             [1.0, 2.0]

					             Expected:
					             [1.25, 1.75]
					             """)
					.Because("the tolerance replaces the comparer of the set");
			}

			private sealed class ModuloComparer(int modulus) : IEqualityComparer<int>
			{
				public bool Equals(int x, int y) => x % modulus == y % modulus;

				public int GetHashCode(int obj) => obj % modulus;
			}

			private sealed class RoundingComparer : IEqualityComparer<double>
			{
				public bool Equals(double x, double y) => Math.Round(x) == Math.Round(y);

				public int GetHashCode(double obj) => Math.Round(obj).GetHashCode();
			}

#if NET8_0_OR_GREATER
			private sealed class SameMonthComparer : IEqualityComparer<DateOnly>
			{
				public bool Equals(DateOnly x, DateOnly y) => x.Year == y.Year && x.Month == y.Month;

				public int GetHashCode(DateOnly obj) => (obj.Year * 12) + obj.Month;
			}
#endif

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
