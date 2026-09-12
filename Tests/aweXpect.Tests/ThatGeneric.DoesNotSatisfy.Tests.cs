namespace aweXpect.Tests;

public sealed partial class ThatGeneric
{
	public sealed class DoesNotSatisfy
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task ShouldFailWhenPredicateResultIsTrue(bool predicateResult)
			{
				Other subject = new();

				async Task Act()
					=> await That(subject).DoesNotSatisfy(_ => predicateResult);

				await That(Act).Throws<XunitException>()
					.OnlyIf(predicateResult)
					.WithMessage("""
					             Expected that subject
					             does not satisfy _ => predicateResult,
					             but it was ThatGeneric.Other {
					               Value = 0
					             }
					             """);
			}

			[Fact]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				Other subject = new();

				async Task Act()
					=> await That(subject).DoesNotSatisfy(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The predicate cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Other? subject = null;

				async Task Act()
					=> await That(subject).DoesNotSatisfy(_ => false);

				await That(Act).Throws<XunitException>();
			}
		}
	}
}
