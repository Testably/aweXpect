#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class EndsWith
	{
		public sealed class SingleEnumerationTests
		{
			[Test]
			public async Task Strings_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<string> expected = Factory.GetSingleUseEnumerable("d", "c");

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected,
					             but it contained item "b" at index 1 instead of "d"

					             Collection:
					             [
					               "a",
					               "b",
					               "c"
					             ]
					             """)
					.Because("the guard and the comparison share one enumeration of the expected items");
			}

			[Test]
			public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable(1, 2, 3);
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(4, 3);

				async Task Act()
					=> await That(subject).EndsWith(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected,
					             but it contained item 2 at index 1 instead of 4

					             Collection:
					             [1, 2, 3]
					             """)
					.Because("the guard and the comparison share one enumeration of the expected items");
			}

			[Test]
			public async Task Within_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<double> subject = ToAsyncEnumerable(1.0, 2.0, 3.0);
				IEnumerable<double> expected = Factory.GetSingleUseEnumerable(4.0, 2.9);

				async Task Act()
					=> await That(subject).EndsWith(expected).Within(0.25);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             ends with expected ± 0.25,
					             but it contained item 2.0 at index 1 instead of 4.0

					             Collection:
					             [1.0, 2.0, 3.0]
					             """)
					.Because("the guard and the comparison share one enumeration of the expected items");
			}
		}
	}
}
#endif
