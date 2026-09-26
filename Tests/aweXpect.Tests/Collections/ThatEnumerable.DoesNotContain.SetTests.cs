using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotContain
	{
		public sealed class SetCollectionTests
		{
			[Fact]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).DoesNotContain(["A", "B",]).InAnyOrder().Using(StringComparer.Ordinal);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSetContainsTheItemsAccordingToItsComparer_ShouldFail()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).DoesNotContain(["B", "A",]).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain collection ["B", "A",] in any order,
					             but it did

					             Collection:
					             [
					               "a",
					               "b"
					             ]

					             Expected:
					             [
					               "B",
					               "A"
					             ]
					             """);
			}
		}

		public sealed class SetItemTests
		{
			[Fact]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				HashSet<int> subject = new(new ModuloComparer(10)) { 1, };

				async Task Act()
					=> await That(subject).DoesNotContain(11).Using(new AllDifferentComparer());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Using_WithStringComparer_ShouldOverrideTheComparerOfTheSet()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", };

				async Task Act()
					=> await That(subject).DoesNotContain("A").Using(StringComparer.Ordinal);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenIgnoringCase_ShouldIgnoreTheComparerOfTheSet()
			{
				HashSet<string> subject = new(StringComparer.Ordinal) { "a", };

				async Task Act()
					=> await That(subject).DoesNotContain("A").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "A" ignoring case,
					             but it contained "A" at least once

					             Collection:
					             [
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSetContainsItemAccordingToItsComparer_ShouldFail()
			{
				HashSet<int> subject = new(new ModuloComparer(10)) { 1, };

				async Task Act()
					=> await That(subject).DoesNotContain(11);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item equal to 11,
					             but it contained 11 once

					             Collection:
					             [1]
					             """);
			}

			[Fact]
			public async Task WhenSetDoesNotContainItemAccordingToItsComparer_ShouldSucceed()
			{
				HashSet<string> subject = new(StringComparer.Ordinal) { "a", };

				async Task Act()
					=> await That(subject).DoesNotContain("A");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenStringSetContainsItemAccordingToItsComparer_ShouldFail()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", };

				async Task Act()
					=> await That(subject).DoesNotContain("A");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "A",
					             but it contained "A" once

					             Collection:
					             [
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task WithDefaultComparer_ShouldNotMatchDateTimesOfIncompatibleKind()
			{
				DateTime utc = new(2026, 9, 24, 12, 0, 0, DateTimeKind.Utc);
				DateTime local = DateTime.SpecifyKind(utc, DateTimeKind.Local);
				HashSet<DateTime> subject = [utc,];

				async Task Act()
					=> await That(subject).DoesNotContain(local);

				await That(Act).DoesNotThrow()
					.Because("the set would match any kind, but the default equality never matches a local and a UTC value");
			}

			private sealed class ModuloComparer(int modulus) : IEqualityComparer<int>
			{
				public bool Equals(int x, int y) => x % modulus == y % modulus;

				public int GetHashCode(int obj) => obj % modulus;
			}
		}
	}
}
