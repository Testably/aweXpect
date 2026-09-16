namespace aweXpect.Tests;

public sealed partial class ThatEnum
{
	public sealed class DoesNotHaveValue
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(MyNumbers.One, 2L)]
			[InlineData(MyNumbers.Two, -7)]
			[InlineData(MyNumbers.Three, 0)]
			public async Task WhenSubjectDoesNotHaveUnexpectedValue_ShouldSucceed(MyNumbers subject,
				long unexpected)
			{
				async Task Act()
					=> await That(subject).DoesNotHaveValue(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(MyNumbers.One, 1)]
			[InlineData(MyNumbers.Two, 2)]
			[InlineData(MyNumbers.Three, 3)]
			public async Task WhenSubjectHasUnexpectedValue_ShouldFail(MyNumbers subject,
				long unexpected)
			{
				async Task Act()
					=> await That(subject).DoesNotHaveValue(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has value not equal to {Formatter.Format(unexpected)},
					              but it had value {Formatter.Format((long)subject)}
					              """);
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				MyColors subject = MyColors.Yellow;

				async Task Act()
					=> await That(subject).DoesNotHaveValue(null);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
