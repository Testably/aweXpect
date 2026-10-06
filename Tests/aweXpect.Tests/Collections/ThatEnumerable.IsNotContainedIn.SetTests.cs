using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsNotContainedIn
	{
		public sealed class SetTests
		{
			[Test]
			public async Task WhenSetIsContainedAccordingToItsComparer_ShouldFail()
			{
				HashSet<string> subject = new(StringComparer.OrdinalIgnoreCase)
				{
					"a",
					"b",
				};

				async Task Act()
					=> await That(subject).IsNotContainedIn(["C", "B", "A",]).InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not contained in collection ["C", "B", "A",] using the subject's StringComparer.OrdinalIgnoreCase in any order,
					             but it was

					             Collection:
					             [
					               "a",
					               "b"
					             ]

					             Expected:
					             [
					               "C",
					               "B",
					               "A"
					             ]
					             """);
			}
		}
	}
}
