using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatDictionary
{
	public sealed class PercentTests
	{
		[Test]
		public async Task ContainsValue_WhenTheValueLiesOutsideThePercentage_ShouldFail()
		{
			Dictionary<string, double> subject = new()
			{
				["a"] = 211.0,
			};

			async Task Act()
				=> await That(subject).ContainsValue(200.0).Within(5.0).Percent();

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains value 200.0 ± 5.0%,
				             but it did not contain value 200.0

				             Dictionary:
				             {["a"] = 211.0}
				             """);
		}

		[Test]
		public async Task ContainsValue_WhenTheValueLiesWithinThePercentage_ShouldSucceed()
		{
			Dictionary<string, double> subject = new()
			{
				["a"] = 209.0,
			};

			async Task Act()
				=> await That(subject).ContainsValue(200.0).Within(5.0).Percent();

			await That(Act).DoesNotThrow();
		}

		[Test]
		public async Task IsEqualTo_ShouldApplyThePercentageToEachExpectedValue()
		{
			Dictionary<string, double> subject = new()
			{
				["a"] = 104.0,
				["b"] = 209.0,
			};

			async Task Act()
				=> await That(subject).IsEqualTo(new Dictionary<string, double>
				{
					["a"] = 100.0,
					["b"] = 200.0,
				}).Within(5.0).Percent();

			await That(Act).DoesNotThrow();
		}
	}
}
