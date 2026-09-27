using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class EndsWith
	{
		public sealed class SetTests
		{
			[Fact]
			public async Task ForADoubleSet_ShouldUseTheComparerOfTheSet()
			{
				SortedSet<double> subject = new(new RoundingComparer()) { 1.0, 2.0, };

				async Task Act()
					=> await That(subject).EndsWith(1.8);

				await That(Act).DoesNotThrow()
					.Because("an element type that allows a tolerance keeps the comparer of the set until one is specified");
			}

			[Fact]
			public async Task ForAnUntypedStringSet_WithAString_ShouldUseTheComparerOfTheSet()
			{
				IEnumerable subject = new SortedSet<string>(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).EndsWith("B");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForAnUntypedStringSet_WithStrings_ShouldUseTheComparerOfTheSet()
			{
				IEnumerable subject = new SortedSet<string>(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).EndsWith(new[] { "B", });

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task IgnoringCase_ShouldIgnoreTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(StringComparer.Ordinal) { "a", };

				async Task Act()
					=> await That(subject).EndsWith("A").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldUseTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).EndsWith("B");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				SortedSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", };

				async Task Act()
					=> await That(subject).EndsWith("A").Using(new AllDifferentComparer());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with ["A"] using AllDifferentComparer,
					             but it contained item "a" at index 0 instead of "A"

					             Collection:
					             [
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task UsingTheComparerOfTheSet_ShouldUseIt()
			{
				List<int> subject = [1,];
				HashSet<int> set = new(new ModuloComparer(10));

				async Task Act()
					=> await That(subject).EndsWith(11).Using(set.Comparer);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSetDoesNotEndWithTheItemsAccordingToItsComparer_ShouldNameTheComparer()
			{
				SortedSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).EndsWith("A");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             ends with ["A"] using the subject's StringComparer.OrdinalIgnoreCase,
					             but it contained item "b" at index 1 instead of "A"

					             Collection:
					             [
					               "a",
					               "b"
					             ]
					             """);
			}

			private sealed class ModuloComparer(int modulus) : IEqualityComparer<int>
			{
				public bool Equals(int x, int y) => x % modulus == y % modulus;

				public int GetHashCode(int obj) => obj % modulus;
			}

			private sealed class RoundingComparer : IComparer<double>
			{
				public int Compare(double x, double y) => Math.Round(x).CompareTo(Math.Round(y));
			}
		}
	}
}