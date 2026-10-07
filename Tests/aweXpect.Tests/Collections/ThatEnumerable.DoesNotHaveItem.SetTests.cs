using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class DoesNotHaveItem
	{
		public sealed class SetTests
		{
			[Test]
			public async Task ForADoubleSet_WhenSetHasTheItemAccordingToItsComparer_ShouldFail()
			{
				HashSet<double> subject = new(new RoundingComparer())
				{
					1.0,
				};

				async Task Act()
					=> await That(subject).DoesNotHaveItem(1.2).AtIndex(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item equal to 1.2 using the subject's ThatEnumerable.DoesNotHaveItem.SetTests.RoundingComparer at index 0,
					             but it had item 1.0 at index 0

					             Collection:
					             [1.0]
					             """);
			}

			[Test]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase)
				{
					"a",
				};

				async Task Act()
					=> await That(subject).DoesNotHaveItem("A").Using(new AllDifferentComparer()).AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSetDoesNotHaveTheItemAccordingToItsComparer_ShouldSucceed()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase)
				{
					"a",
				};

				async Task Act()
					=> await That(subject).DoesNotHaveItem("b").AtIndex(0);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSetHasTheItemAccordingToItsComparer_ShouldFail()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase)
				{
					"a",
				};

				async Task Act()
					=> await That(subject).DoesNotHaveItem("A").AtIndex(0);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an item equal to "A" using the subject's StringComparer.OrdinalIgnoreCase at index 0,
					             but it had item "a" at index 0

					             Collection:
					             [
					               "a"
					             ]
					             """);
			}

			private sealed class RoundingComparer : IEqualityComparer<double>
			{
				public bool Equals(double x, double y) => Math.Round(x) == Math.Round(y);

				public int GetHashCode(double obj) => Math.Round(obj).GetHashCode();
			}
		}
	}
}
