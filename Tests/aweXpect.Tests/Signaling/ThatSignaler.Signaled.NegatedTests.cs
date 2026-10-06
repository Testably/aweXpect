using aweXpect.Core;
using aweXpect.Results;
using aweXpect.Signaling;

namespace aweXpect.Tests;

public sealed partial class ThatSignaler
{
	public sealed partial class Signaled
	{
		public sealed class NegatedTests
		{
			[Test]
			[Arguments("Default", 1, "has never recorded the callback", "recorded once")]
			[Arguments("Never", 0, "has recorded the callback at least once", "never recorded")]
			[Arguments("Once", 1, "has recorded the callback not exactly once", "recorded once")]
			[Arguments("Twice", 2, "has recorded the callback not exactly twice", "recorded twice")]
			[Arguments("Exactly3", 3, "has recorded the callback not exactly 3 times", "recorded 3 times")]
			[Arguments("AtLeast2", 2, "has recorded the callback fewer than twice", "recorded twice")]
			[Arguments("AtMost1", 1, "has recorded the callback more than once", "only recorded once")]
			[Arguments("MoreThan1", 2, "has recorded the callback at most once", "recorded twice")]
			[Arguments("LessThan3", 2, "has recorded the callback at least 3 times", "only recorded twice")]
			[Arguments("Between1And3", 2, "has recorded the callback not between 1 and 3 times", "recorded twice")]
			public async Task WhenPositiveExpectationIsMet_ShouldFailWithTheComplementaryExpectation(
				string quantifier, int signalCount, string expectation, string result)
			{
				Signaler signaler = new();
				for (int i = 0; i < signalCount; i++)
				{
					signaler.Signal();
				}

				async Task Act() =>
					await That(signaler).DoesNotComplyWith(s
						=> Quantify(s.Signaled(), quantifier).Within(40.Milliseconds()));

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that signaler
					              {expectation} within 0:00.040,
					              but it was {result} *
					              """).AsWildcard()
					.Because("the result says \"only\" exactly when more signals would meet the negated expectation");
			}

			[Test]
			public async Task WhenPositiveExpectationWithParameterIsMet_ShouldNotClaimTooFewSignals()
			{
				Signaler<int> signaler = new();
				signaler.Signal(1);
				signaler.Signal(2);

				async Task Act() =>
					await That(signaler).DoesNotComplyWith(s
						=> s.Signaled().Between(1).And(3.Times()).Within(40.Milliseconds()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that signaler
					             has recorded the callback not between 1 and 3 times within 0:00.040,
					             but it was recorded twice in [
					               1,
					               2
					             ] within 0:*
					             """).AsWildcard()
					.Because("the count lies inside the range, so it is not too low");
			}

			private static SignalCountResult Quantify(SignalCountResult result, string quantifier)
				=> quantifier switch
				{
					"Never" => result.Never(),
					"Once" => result.Once(),
					"Twice" => result.Twice(),
					"Exactly3" => result.Exactly(3.Times()),
					"AtLeast2" => result.AtLeast(2.Times()),
					"AtMost1" => result.AtMost(1.Times()),
					"MoreThan1" => result.MoreThan(1.Times()),
					"LessThan3" => result.LessThan(3.Times()),
					"Between1And3" => result.Between(1).And(3.Times()),
					_ => result,
				};
		}
	}
}
