#if NET8_0_OR_GREATER
using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed class PercentTests
	{
		[Test]
		public async Task Contains_WhenTheItemLiesOutsideThePercentage_ShouldFail()
		{
			IAsyncEnumerable<double> subject = ToAsyncEnumerable(1.0, 211.0);

			async Task Act()
				=> await That(subject).Contains(200.0).Within(5.0).Percent();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 200.0 ± 5.0% at least once,
				             but it did not contain it

				             Collection:
				             [1.0, 211.0]
				             """);
		}

		[Test]
		public async Task Contains_WhenTheItemLiesWithinThePercentage_ShouldSucceed()
		{
			IAsyncEnumerable<double> subject = ToAsyncEnumerable(1.0, 209.0);

			async Task Act()
				=> await That(subject).Contains(200.0).Within(5.0).Percent();

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task IsEqualTo_WhenEachItemLiesWithinThePercentage_ShouldSucceed()
		{
			IAsyncEnumerable<double> subject = ToAsyncEnumerable(104.0, 209.0);

			async Task Act()
				=> await That(subject).IsEqualTo([100.0, 200.0,]).Within(5.0).Percent();

			await That(Act).DoesNotThrow();
		}
	}
}
#endif
