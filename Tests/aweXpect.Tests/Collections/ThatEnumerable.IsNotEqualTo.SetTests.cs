using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotEqualTo
	{
		public sealed class SetTests
		{
			[Fact]
			public async Task Using_ShouldOverrideTheComparerOfTheSet()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).IsNotEqualTo(["B", "A",]).InAnyOrder().Using(StringComparer.Ordinal);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSetContainsTheItemsAccordingToItsComparer_ShouldFail()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase) { "a", "b", };

				async Task Act()
					=> await That(subject).IsNotEqualTo(["B", "A",]).InAnyOrder();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to collection ["B", "A",] in any order,
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
