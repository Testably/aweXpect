using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class IsContainedIn
	{
		public sealed class SingleEnumerationTests
		{
			[Fact]
			public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IEnumerable<int> subject = ToEnumerable([1, 4,]);
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 2, 3);

				async Task Act()
					=> await That(subject).IsContainedIn(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is contained in collection expected in order and contiguous,
					             but it contained item 4 at index 1 that was not expected

					             Collection:
					             [1, 4]

					             Expected:
					             [
					               1,
					               2,
					               3
					             ]
					             """)
					.Because("the comparison and the message share one enumeration of the expected items");
			}
		}
	}
}
