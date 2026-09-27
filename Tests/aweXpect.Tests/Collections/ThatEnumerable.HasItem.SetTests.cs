using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class HasItem
	{
		public sealed class SetTests
		{
			[Fact]
			public async Task ForADoubleSet_ShouldUseTheComparerOfTheSet()
			{
				HashSet<double> subject = new(new RoundingComparer()) { 1.0, };

				async Task Act()
					=> await That(subject).HasItem(1.2).AtIndex(0);

				await That(Act).DoesNotThrow()
					.Because("an element type that allows a tolerance keeps the comparer of the set until one is specified");
			}
			[Fact]
			public async Task ForAnUntypedSetOfObjects_ShouldUseTheComparerOfTheSet()
			{
				IEnumerable subject = new HashSet<object>(new AllEqualComparer()) { 1, };

				async Task Act()
					=> await That(subject).HasItem(2).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ForAnUntypedStringSet_ShouldUseTheDefaultEquality()
			{
				IEnumerable subject = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "a", };

				async Task Act()
					=> await That(subject).HasItem("A").AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an item equal to "A" at index 0,
					             but it had item "a" at index 0

					             Collection:
					             [
					               "a"
					             ]
					             """)
					.Because("the comparer of a set of another item type than the expected one cannot be read");
			}

			[Fact]
			public async Task IgnoringCase_ShouldIgnoreTheComparerOfTheSet()
			{
				HashSet<string> subject = new(new AllDifferentComparer()) { "a", };

				async Task Act()
					=> await That(subject).HasItem("A").IgnoringCase().AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task ShouldUseTheComparerOfTheSet()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", };

				async Task Act()
					=> await That(subject).HasItem("A").AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				HashSet<int> subject = new(new ModuloComparer(10)) { 1, };

				async Task Act()
					=> await That(subject).HasItem(11).Using(EqualityComparer<int>.Default).AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an item equal to 11 using GenericEqualityComparer<int> at index 0,
					             but it had item 1 at index 0

					             Collection:
					             [1]
					             """)
					.Because("the default comparer forces the default equality");
			}

			[Fact]
			public async Task UsingTheComparerOfTheSet_ShouldUseIt()
			{
				List<int> subject = [1,];
				HashSet<int> set = new(new ModuloComparer(10));

				async Task Act()
					=> await That(subject).HasItem(11).Using(set.Comparer).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSetDoesNotHaveTheItemAccordingToItsComparer_ShouldNameTheComparer()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", };

				async Task Act()
					=> await That(subject).HasItem("b").AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an item equal to "b" using the subject's StringComparer.OrdinalIgnoreCase at index 0,
					             but it had item "a" at index 0

					             Collection:
					             [
					               "a"
					             ]
					             """);
			}

			[Fact]
			public async Task WhenSetHasTheItemAccordingToItsComparer_ShouldSucceed()
			{
				HashSet<int> subject = new(new ModuloComparer(10)) { 1, };

				async Task Act()
					=> await That(subject).HasItem(11).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task Within_ShouldIgnoreTheComparerOfTheSet()
			{
				HashSet<double> subject = new(new RoundingComparer()) { 1.0, };

				async Task Act()
					=> await That(subject).HasItem(1.25).Within(0.125).AtIndex(0);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an item equal to 1.25 ± 0.125 at index 0,
					             but it had item 1.0 at index 0

					             Collection:
					             [1.0]
					             """);
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
		}
	}
}
