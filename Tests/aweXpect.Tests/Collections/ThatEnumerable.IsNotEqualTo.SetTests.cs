using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class SetTests
		{
			[Test]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).IsNotEqualTo(["B", "A",]).InAnyOrder().Using(StringComparer.Ordinal);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSetContainsTheItemsAccordingToItsComparer_ShouldFail()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).IsNotEqualTo(["B", "A",]).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection ["B", "A",] using the subject's StringComparer.OrdinalIgnoreCase in any order,
					             but it was

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
	}
}
