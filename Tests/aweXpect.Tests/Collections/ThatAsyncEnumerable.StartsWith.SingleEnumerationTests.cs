#if NET8_0_OR_GREATER
using System.Collections.Generic;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class StartsWith
	{
		public sealed class SingleEnumerationTests
		{
			[Fact]
			public async Task Strings_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["a", "b", "c",]);
				IEnumerable<string> expected = Factory.GetSingleUseEnumerable("a", "d");

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with expected,
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

			[Fact]
			public async Task WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<int> subject = ToAsyncEnumerable([1, 2, 3,]);
				IEnumerable<int> expected = Factory.GetSingleUseEnumerable(1, 4);

				async Task Act()
					=> await That(subject).StartsWith(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with expected,
					             but it contained item 2 at index 1 instead of 4

					             Collection:
					             [1, 2, 3]
					             """)
					.Because("the guard and the comparison share one enumeration of the expected items");
			}

			[Fact]
			public async Task Within_WhenExpectedCanOnlyBeEnumeratedOnce_ShouldFail()
			{
				IAsyncEnumerable<double> subject = ToAsyncEnumerable(1.0, 2.0, 3.0);
				IEnumerable<double> expected = Factory.GetSingleUseEnumerable(1.1, 4.0);

				async Task Act()
					=> await That(subject).StartsWith(expected).Within(0.25);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             starts with expected ± 0.25,
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
