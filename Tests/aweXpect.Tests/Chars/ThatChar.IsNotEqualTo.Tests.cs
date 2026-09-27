namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectDiffersOnlyInCase_AndIgnoringCase_ShouldFail()
			{
				char subject = 'a';

				async Task Act()
					=> await That(subject).IsNotEqualTo('A').IgnoringCase();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 'A' ignoring case,
					             but it was 'a'
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsDifferent_AndIgnoringCase_ShouldSucceed()
			{
				char subject = 'a';

				async Task Act()
					=> await That(subject).IsNotEqualTo('B').IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData('a', 'b')]
			[InlineData('B', 'b')]
			public async Task WhenSubjectIsDifferent_ShouldSucceed(char subject, char unexpected)
			{
				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData('a')]
			[InlineData('X')]
			[InlineData('5')]
			[InlineData('\t')]
			public async Task WhenSubjectIsTheSame_ShouldFail(char subject)
			{
				char unexpected = subject;

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not equal to {Formatter.Format(unexpected)},
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				char subject = 'X';

				async Task Act()
					=> await That(subject).IsNotEqualTo(null);

				await That(Act).DoesNotThrow();
			}
		}
	}
}
