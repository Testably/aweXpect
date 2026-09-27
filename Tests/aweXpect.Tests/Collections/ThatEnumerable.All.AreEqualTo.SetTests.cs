using System.Collections;
using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class All
	{
		public sealed partial class AreEqualTo
		{
			public sealed class SetTests
			{
				[Fact]
				public async Task ForADoubleSet_ShouldUseTheComparerOfTheSet()
				{
					HashSet<double> subject = new(new RoundingComparer()) { 1.0, };

					async Task Act()
						=> await That(subject).All().AreEqualTo(1.2);

					await That(Act).DoesNotThrow()
						.Because("an element type that allows a tolerance keeps the comparer of the set until one is specified");
				}

				[Fact]
				public async Task ForAnUntypedSetOfObjects_ShouldUseTheComparerOfTheSet()
				{
					IEnumerable subject = new HashSet<object>(new AllEqualComparer()) { 1, };

					async Task Act()
						=> await That(subject).All().AreEqualTo(2);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task IgnoringCase_ShouldIgnoreTheComparerOfTheSet()
				{
					HashSet<string> subject = new(new AllDifferentComparer()) { "a", };

					async Task Act()
						=> await That(subject).All().AreEqualTo("A").IgnoringCase();

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task None_WhenSetContainsTheItemAccordingToItsComparer_ShouldFail()
				{
					HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

					async Task Act()
						=> await That(subject).None().AreEqualTo("A");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "A" using the subject's StringComparer.OrdinalIgnoreCase for no items,
						             but 1 of 2 were

						             Matching items:
						             [
						               "a"
						             ]

						             Collection:
						             [
						               "a",
						               "b"
						             ]
						             """);
				}

				[Fact]
				public async Task ShouldUseTheComparerOfTheSet()
				{
					HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", };

					async Task Act()
						=> await That(subject).All().AreEqualTo("A");

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task Using_ShouldOverrideTheComparerOfTheSet()
				{
					HashSet<int> subject = new(new ModuloComparer(10)) { 1, };

					async Task Act()
						=> await That(subject).All().AreEqualTo(11).Using(EqualityComparer<int>.Default);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 11 using GenericEqualityComparer<int> for all items,
						             but none of 1 were

						             Not matching items:
						             [1]

						             Collection:
						             [1]
						             """)
						.Because("the default comparer forces the default equality");
				}

				[Fact]
				public async Task UsingTheComparerOfTheSet_ShouldUseIt()
				{
					List<int> subject = [1, 21,];
					HashSet<int> set = new(new ModuloComparer(10));

					async Task Act()
						=> await That(subject).All().AreEqualTo(11).Using(set.Comparer);

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenExpectedIsNull_ShouldNotNameTheComparerOfTheSet()
				{
					HashSet<string?> subject = new(StringComparer.OrdinalIgnoreCase) { "a", };

					async Task Act()
						=> await That(subject).All().AreEqualTo(null);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to <null> for all items,
						             but none of 1 were

						             Not matching items:
						             [
						               "a"
						             ]

						             Collection:
						             [
						               "a"
						             ]
						             """)
						.Because("the comparer never decides for null, which only equals null");
				}

				[Fact]
				public async Task WhenSetContainsOtherItemsAccordingToItsComparer_ShouldNameTheComparer()
				{
					HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

					async Task Act()
						=> await That(subject).All().AreEqualTo("A");

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to "A" using the subject's StringComparer.OrdinalIgnoreCase for all items,
						             but only 1 of 2 were

						             Not matching items:
						             [
						               "b"
						             ]

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

				private sealed class RoundingComparer : IEqualityComparer<double>
				{
					public bool Equals(double x, double y) => Math.Round(x) == Math.Round(y);

					public int GetHashCode(double obj) => Math.Round(obj).GetHashCode();
				}
			}
		}
	}
}
