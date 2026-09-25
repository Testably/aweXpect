namespace aweXpect.Tests;

public sealed partial class ThatBool
{
	public sealed class DoesNotImply
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenAntecedentDoesNotImplyConsequent_ShouldSucceed()
			{
				bool antecedent = true;
				bool consequent = false;

				async Task Act()
					=> await That(antecedent).DoesNotImply(consequent);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(false, false)]
			[InlineData(false, true)]
			[InlineData(true, true)]
			public async Task WhenAntecedentImpliesConsequent_ShouldFail(bool antecedent, bool consequent)
			{
				async Task Act()
					=> await That(antecedent).DoesNotImply(consequent)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that antecedent
					              does not imply {Formatter.Format(consequent)}, because we want to test the failure,
					              but it did
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenAntecedentDoesNotImplyConsequent_ShouldFail()
			{
				bool antecedent = true;
				bool consequent = false;

				async Task Act()
					=> await That(antecedent).DoesNotComplyWith(b => b.DoesNotImply(consequent))
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that antecedent
					             implies False, because we want to test the failure,
					             but it did not
					             """);
			}

			[Theory]
			[InlineData(false, false)]
			[InlineData(false, true)]
			[InlineData(true, true)]
			public async Task WhenAntecedentImpliesConsequent_ShouldSucceed(bool antecedent, bool consequent)
			{
				async Task Act()
					=> await That(antecedent).DoesNotComplyWith(b => b.DoesNotImply(consequent));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
