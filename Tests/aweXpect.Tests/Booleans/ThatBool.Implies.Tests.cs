namespace aweXpect.Tests;

public sealed partial class ThatBool
{
	public sealed class Implies
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenAntecedentDoesNotImplyConsequent_ShouldFail()
			{
				bool antecedent = true;
				bool consequent = false;

				async Task Act()
					=> await That(antecedent).Implies(consequent)
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that antecedent
					              implies {Formatter.Format(consequent)}, because we want to test the failure,
					              but it did not
					              """);
			}

			[Test]
			[Arguments(false, false)]
			[Arguments(false, true)]
			[Arguments(true, true)]
			public async Task WhenAntecedentImpliesConsequent_ShouldSucceed(bool antecedent,
				bool consequent)
			{
				async Task Act()
					=> await That(antecedent).Implies(consequent);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task ShouldUseCorrectExpectation()
			{
				bool antecedent = false;
				bool consequent = true;

				async Task Act()
					=> await That(antecedent).DoesNotComplyWith(b => b.Implies(consequent))
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that antecedent
					             does not imply True, because we want to test the failure,
					             but it did
					             """);
			}
		}
	}
}
