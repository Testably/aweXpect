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
			[Theory]
			[InlineData("Default", 1, "has never recorded the callback")]
			[InlineData("Never", 0, "has recorded the callback at least once")]
			[InlineData("Once", 1, "has recorded the callback not exactly once")]
			[InlineData("Twice", 2, "has recorded the callback not exactly twice")]
			[InlineData("Exactly3", 3, "has recorded the callback not exactly 3 times")]
			[InlineData("AtLeast2", 2, "has recorded the callback fewer than twice")]
			[InlineData("AtMost1", 1, "has recorded the callback more than once")]
			[InlineData("MoreThan1", 2, "has recorded the callback at most once")]
			[InlineData("LessThan3", 2, "has recorded the callback at least 3 times")]
			[InlineData("Between1And3", 2, "has recorded the callback not between 1 and 3 times")]
			public async Task WhenPositiveExpectationIsMet_ShouldFailWithTheComplementaryExpectation(
				string quantifier, int signalCount, string expectation)
			{
				Signaler signaler = new();
				for (int i = 0; i < signalCount; i++)
				{
					signaler.Signal();
				}

				async Task Act() =>
					await That(signaler).DoesNotComplyWith(s
						=> Quantify(s.Signaled(), quantifier).Within(40.Milliseconds()));

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that signaler
					              {expectation} within 0:00.040,
					              but it was *
					              """).AsWildcard();
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
