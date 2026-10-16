using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed class PercentTests
	{
		[Test]
		public async Task AllAreEqualTo_WhenAnItemLiesOutsideThePercentage_ShouldFail()
		{
			double[] subject = [95.0, 106.0,];

			async Task Act()
				=> await That(subject).All().AreEqualTo(100.0).Within(5.0).Percent();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to 100.0 ± 5.0% for all items,
				             but only 1 of 2 were

				             Not matching items:
				             [106.0]

				             Collection:
				             [95.0, 106.0]
				             """);
		}

		[Test]
		public async Task AllAreEqualTo_WhenEachItemLiesWithinThePercentage_ShouldSucceed()
		{
			double[] subject = [95.0, 105.0,];

			async Task Act()
				=> await That(subject).All().AreEqualTo(100.0).Within(5.0).Percent();

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task Contains_ForDecimals_WhenTheItemLiesWithinThePercentage_ShouldSucceed()
		{
			decimal[] subject = [1m, 209m,];

			async Task Act()
				=> await That(subject).Contains(200m).Within(5m).Percent();

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task Contains_ForFloats_WhenTheItemLiesWithinThePercentage_ShouldSucceed()
		{
			float[] subject = [1f, 209f,];

			async Task Act()
				=> await That(subject).Contains(200f).Within(5f).Percent();

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task Contains_ForNullableDoubles_WhenTheItemLiesWithinThePercentage_ShouldSucceed()
		{
			double?[] subject = [null, 209.0,];

			async Task Act()
				=> await That(subject).Contains(200.0).Within(5.0).Percent();

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task Contains_WhenTheItemLiesOutsideThePercentage_ShouldFail()
		{
			double[] subject = [1.0, 211.0,];

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
		public async Task Contains_WithItems_ShouldApplyThePercentageToEachExpectedItem()
		{
			double[] subject = [104.0, 209.0,];

			async Task Act()
				=> await That(subject).Contains([100.0, 200.0,]).Within(5.0).Percent();

			await That(Act).DoesNotThrow()
				.Because("5% is a tolerance of 5 for 100 and of 10 for 200");
		}

		[Test]
		public async Task Contains_WithQuantifier_ShouldCountTheItemsWithinThePercentage()
		{
			double[] subject = [95.0, 105.0, 106.0,];

			async Task Act()
				=> await That(subject).Contains(100.0).Within(5.0).Percent().AtLeast(3);

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equal to 100.0 ± 5.0% at least 3 times,
				             but it contained 100.0 twice

				             Collection:
				             [95.0, 105.0, 106.0]
				             """);
		}

		[Test]
		public async Task DoesNotContain_WhenTheItemLiesWithinThePercentage_ShouldFail()
		{
			double[] subject = [1.0, 209.0,];

			async Task Act()
				=> await That(subject).DoesNotContain(200.0).Within(5.0).Percent();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             does not contain an item equal to 200.0 ± 5.0%,
				             but it contained 209.0 at least once

				             Collection:
				             [1.0, 209.0]
				             """);
		}

		[Test]
		public async Task EndsWith_WhenTheItemsLieWithinThePercentage_ShouldSucceed()
		{
			double[] subject = [1.0, 104.0, 209.0,];

			async Task Act()
				=> await That(subject).EndsWith(100.0, 200.0).Within(5.0).Percent();

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task HasItem_WhenTheItemLiesWithinThePercentage_ShouldSucceed()
		{
			double[] subject = [1.0, 209.0,];

			async Task Act()
				=> await That(subject).HasItem(200.0).Within(5.0).Percent().AtIndex(1);

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task IsContainedIn_WhenTheItemsLieWithinThePercentage_ShouldSucceed()
		{
			IEnumerable<double> subject = [104.0, 209.0,];

			async Task Act()
				=> await That(subject).IsContainedIn([1.0, 100.0, 200.0,]).Within(5.0).Percent();

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task IsEqualTo_InAnyOrder_ShouldApplyThePercentageToEachExpectedItem()
		{
			double[] subject = [209.0, 104.0,];

			async Task Act()
				=> await That(subject).IsEqualTo([100.0, 200.0,]).Within(5.0).Percent().InAnyOrder();

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task IsEqualTo_WhenAnItemLiesOutsideThePercentage_ShouldFail()
		{
			double[] subject = [104.0, 211.0,];

			async Task Act()
				=> await That(subject).IsEqualTo([100.0, 200.0,]).Within(5.0).Percent();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [100.0, 200.0,] ± 5.0% in order,
				             but it contained item 211.0 at index 1 instead of 200.0

				             Collection:
				             [104.0, 211.0]

				             Expected:
				             [100.0, 200.0]
				             """);
		}

		[Test]
		public async Task StartsWith_WhenTheItemsLieWithinThePercentage_ShouldSucceed()
		{
			double[] subject = [104.0, 209.0, 1.0,];

			async Task Act()
				=> await That(subject).StartsWith(100.0, 200.0).Within(5.0).Percent();

			await That(Act).DoesNotThrow();
		}
	}
}
