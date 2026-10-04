using aweXpect.Core.Constraints;
using aweXpect.Options;

namespace aweXpect.Core.Tests.Collections;

public sealed class QuantifiedCollectionConstraintBaseTests
{
	[Fact]
	public async Task StartEvaluation_AfterACompletedEvaluation_ShouldNotShowItsItems()
	{
		AreEvenConstraint sut = new(EnumerableQuantifier.All());
		sut.IsMetBy([1, 3,]);

		ConstraintResult result = sut.IsMetBy(null);

		await That(ResultContextCollector.Capture(result)).IsNull()
			.Because("the items of the earlier evaluation do not describe the null subject");
	}

	[Fact]
	public async Task StartEvaluation_AfterAnEvaluationThatStoppedEarly_ShouldNotCountItsItems()
	{
		AreEvenConstraint sut = new(EnumerableQuantifier.Exactly(2));
		sut.IsMetBy([2, 4, -1,]);

		ConstraintResult result = sut.IsMetBy([2, 4,]);

		await That(result.Outcome).IsEqualTo(Outcome.Success)
			.Because("the items that the earlier evaluation recorded before it stopped do not count");
	}

	private sealed class AreEvenConstraint(EnumerableQuantifier quantifier)
		: QuantifiedCollectionConstraint<int[]?, int>("it", ExpectationGrammars.None, quantifier,
			_ => "is even", "were")
	{
		public AreEvenConstraint IsMetBy(int[]? actual)
		{
			StartEvaluation();
			Actual = actual;
			if (actual is null)
			{
				return this;
			}

			foreach (int item in actual)
			{
				if (item < 0)
				{
					Outcome = Outcome.FailureBothWays;
					return this;
				}

				Record(item, item % 2 == 0);
			}

			Complete();
			return this;
		}
	}
}
