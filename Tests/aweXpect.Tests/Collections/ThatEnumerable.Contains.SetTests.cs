using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
#endif
using aweXpect.Core;
using aweXpect.Equivalency;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Contains
	{
		public sealed class SetCollectionTests
		{
			[Fact]
			public async Task Equivalent_ShouldIgnoreTheComparerOfTheSet()
			{
				HashSet<int> subject = new(new ModuloComparer(10)) { 1, };

				async Task Act()
					=> await That(subject).Contains([11,]).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection [11,] in order and contiguous using equivalency,
					             but it lacked the one expected item

					             Collection:
					             [1]

					             Expected:
					             [11]
					             """);
			}

			[Fact]
			public async Task ForASortedSet_InSameOrder_ShouldUseTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", "c", };

				async Task Act()
					=> await That(subject).Contains(["B", "C",]);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task InAnyOrder_ShouldUseTheComparerOfTheSet()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).Contains(["B", "A",]).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", };

				async Task Act()
					=> await That(subject).Contains(["A",]).InAnyOrder().Using(new AllDifferentComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection ["A",] in any order using AllDifferentComparer,
					             but it lacked the one expected item

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
					=> await That(subject).Contains(["a", "A",]).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains collection ["a", "A",] in any order,
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
			public async Task WhenSetContainsTheItemsAccordingToItsComparer_ShouldSucceed()
			{
				HashSet<int> subject = new(new ModuloComparer(10)) { 1, 2, };

				async Task Act()
					=> await That(subject).Contains([12, 11,]).InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithDefaultComparer_ShouldCompareNumbersOfDifferentTypesByValue()
			{
				HashSet<object> subject = [1,];

				async Task Act()
					=> await That(subject).Contains([1L,]);

				await That(Act).DoesNotThrow()
					.Because("a set with the default comparer keeps the default equality, which compares numbers by value");
			}

			private sealed class ModuloComparer(int modulus) : IEqualityComparer<int>
			{
				public bool Equals(int x, int y) => x % modulus == y % modulus;

				public int GetHashCode(int obj) => obj % modulus;
			}
		}

		public sealed class SetItemTests
		{
			[Fact]
			public async Task Equivalent_ShouldIgnoreTheComparerOfTheSet()
			{
				HashSet<int> subject = new(new ModuloComparer(10)) { 1, };

				async Task Act()
					=> await That(subject).Contains(11).Equivalent();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains an item equivalent to 11 at least once,
					             but it did not contain it

					             Collection:
					             [1]
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task ForAFrozenSet_ShouldUseTheComparerOfTheSet()
			{
				FrozenSet<string> subject = new[] { "a", }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

				async Task Act()
					=> await That(subject).Contains("A");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForAnImmutableHashSet_ShouldUseTheComparerOfTheSet()
			{
				ImmutableHashSet<string> subject = ImmutableHashSet.Create(StringComparer.OrdinalIgnoreCase, "a");

				async Task Act()
					=> await That(subject).Contains("A");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForAnImmutableSortedSet_ShouldUseTheComparerOfTheSet()
			{
				ImmutableSortedSet<string> subject = ImmutableSortedSet.Create(StringComparer.OrdinalIgnoreCase, "a");

				async Task Act()
					=> await That(subject).Contains("A");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForASetWithoutAnExposedComparer_ShouldUseTheDefaultEquality()
			{
				ReadOnlySetWrapper<string> subject = new(new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "a", });

				async Task Act()
					=> await That(subject).Contains("A");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "A" at least once,
					             but it did not contain it

					             Collection:
					             [
					               "a"
					             ]
					             """)
					.Because("a set that does not expose its comparer cannot be told apart from one with the default comparer");
			}
#endif

			[Fact]
			public async Task ForASortedSet_ShouldUseTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", };

				async Task Act()
					=> await That(subject).Contains("A");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldCountTheItemOnce()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).Contains("A").AtLeast(2.Times());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "A" at least twice,
					             but it contained "A" once

					             Collection:
					             [
					               "a",
					               "b"
					             ]
					             """)
					.Because("a set holds an item at most once");
			}

			[Fact]
			public async Task ShouldSupportExactlyOnce()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).Contains("A").Exactly(1.Times());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				HashSet<int> subject = new(new ModuloComparer(10)) { 1, };

				async Task Act()
					=> await That(subject).Contains(11).Using(new AllDifferentComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains an item equal to 11 using AllDifferentComparer at least once,
					             but it did not contain it

					             Collection:
					             [1]
					             """);
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldNotAskTheSet()
			{
				HashSet<string?> subject = new(new ThrowingForNullComparer()) { "a", };

				async Task Act()
					=> await That(subject).Contains(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains <null> at least once,
					             but it did not contain it

					             Collection:
					             [
					               "a"
					             ]
					             """)
					.Because("some sets reject a lookup for a `null` item");
			}

			[Fact]
			public async Task WhenIgnoringCase_ShouldIgnoreTheComparerOfTheSet()
			{
				HashSet<string> subject = new(StringComparer.Ordinal) { "a", };

				async Task Act()
					=> await That(subject).Contains("A").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSetContainsItemAccordingToItsComparer_ShouldSucceed()
			{
				HashSet<int> subject = new(new ModuloComparer(10)) { 1, };

				async Task Act()
					=> await That(subject).Contains(11);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSetComparerThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("comparer failed");
				HashSet<string> subject = new(new ThrowingComparer(exception)) { "a", };

				async Task Act()
					=> await That(subject).Contains("b");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "b" at least once,
					             but the comparer did throw an InvalidOperationException:
					               comparer failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenSetDoesNotContainItemAccordingToItsComparer_ShouldFail()
			{
				HashSet<string> subject = new(StringComparer.Ordinal) { "a", };

				async Task Act()
					=> await That(subject).Contains("A");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains "A" at least once,
					             but it did not contain it

					             Collection:
					             [
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenStringSetContainsItemAccordingToItsComparer_ShouldSucceed()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", };

				async Task Act()
					=> await That(subject).Contains("A");

				await That(Act).DoesNotThrow();
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task WithDefaultComparer_ForAFrozenSet_ShouldCompareNumbersOfDifferentTypesByValue()
			{
				FrozenSet<object> subject = new object[] { 1, }.ToFrozenSet();

				async Task Act()
					=> await That(subject).Contains(1L);

				await That(Act).DoesNotThrow();
			}
#endif

			[Fact]
			public async Task WithDefaultComparer_ShouldCompareNumbersOfDifferentTypesByValue()
			{
				HashSet<object> subject = [1,];

				async Task Act()
					=> await That(subject).Contains(1L);

				await That(Act).DoesNotThrow()
					.Because("the set would not find a long in a set of ints, but the default equality compares by value");
			}

			[Fact]
			public async Task WithDefaultComparer_ShouldNotMatchDateTimesOfIncompatibleKind()
			{
				DateTime utc = new(2026, 9, 24, 12, 0, 0, DateTimeKind.Utc);
				DateTime local = DateTime.SpecifyKind(utc, DateTimeKind.Local);
				HashSet<DateTime> subject = [utc,];

				async Task Act()
					=> await That(subject).Contains(local);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains an item equal to 2026-09-24T12:00:00.0000000* at least once,
					             but it did not contain it

					             Collection:
					             [
					               2026-09-24T12:00:00.0000000Z
					             ]
					             """).AsWildcard()
					.Because("the set would match any kind, but the default equality never matches a local and a UTC value");
			}

			private sealed class ModuloComparer(int modulus) : IEqualityComparer<int>
			{
				public bool Equals(int x, int y) => x % modulus == y % modulus;

				public int GetHashCode(int obj) => obj % modulus;
			}

			private sealed class ThrowingForNullComparer : IEqualityComparer<string?>
			{
				public bool Equals(string? x, string? y)
					=> x is null || y is null ? throw new ArgumentNullException(x is null ? nameof(x) : nameof(y)) : x == y;

				public int GetHashCode(string? obj) => 0;
			}

			private sealed class ThrowingComparer(Exception exception) : IEqualityComparer<string>
			{
				public bool Equals(string? x, string? y) => throw exception;

				public int GetHashCode(string obj) => 0;
			}

#if NET8_0_OR_GREATER
			private sealed class ReadOnlySetWrapper<T>(HashSet<T> inner) : IReadOnlySet<T>
			{
				public int Count => inner.Count;
				public bool Contains(T item) => inner.Contains(item);
				public IEnumerator<T> GetEnumerator() => inner.GetEnumerator();
				IEnumerator IEnumerable.GetEnumerator() => inner.GetEnumerator();
				public bool IsProperSubsetOf(IEnumerable<T> other) => inner.IsProperSubsetOf(other);
				public bool IsProperSupersetOf(IEnumerable<T> other) => inner.IsProperSupersetOf(other);
				public bool IsSubsetOf(IEnumerable<T> other) => inner.IsSubsetOf(other);
				public bool IsSupersetOf(IEnumerable<T> other) => inner.IsSupersetOf(other);
				public bool Overlaps(IEnumerable<T> other) => inner.Overlaps(other);
				public bool SetEquals(IEnumerable<T> other) => inner.SetEquals(other);
			}
#endif
		}
	}
}
