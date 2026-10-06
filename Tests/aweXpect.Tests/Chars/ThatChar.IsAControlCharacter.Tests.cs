namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsAControlCharacter
	{
		public sealed class Tests
		{
			[Test]
			[Arguments('\0')]
			[Arguments('\t')]
			[Arguments('\n')]
			[Arguments('\r')]
			[Arguments('\u001B')]
			[Arguments('\u007F')]
			[Arguments('\u0085')]
			public async Task WhenSubjectIsAControlCharacter_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).IsAControlCharacter();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments('a')]
			[Arguments('1')]
			[Arguments(' ')]
			[Arguments('@')]
			[Arguments('\u00A0')]
			public async Task WhenSubjectIsNoControlCharacter_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).IsAControlCharacter();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is a control character,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments('\0')]
			[Arguments('\t')]
			[Arguments('\n')]
			[Arguments('\r')]
			[Arguments('\u001B')]
			[Arguments('\u007F')]
			[Arguments('\u0085')]
			public async Task WhenSubjectIsAControlCharacter_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAControlCharacter());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not a control character,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments('a')]
			[Arguments('1')]
			[Arguments(' ')]
			[Arguments('@')]
			[Arguments('\u00A0')]
			public async Task WhenSubjectIsNoControlCharacter_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAControlCharacter());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
