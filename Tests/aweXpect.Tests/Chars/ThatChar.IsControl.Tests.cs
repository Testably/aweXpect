namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsControl
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData('\0')]
			[InlineData('\t')]
			[InlineData('\n')]
			[InlineData('\r')]
			[InlineData('\u001B')]
			[InlineData('\u007F')]
			[InlineData('\u0085')]
			public async Task WhenSubjectIsAControlCharacter_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).IsControl();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData('a')]
			[InlineData('1')]
			[InlineData(' ')]
			[InlineData('@')]
			[InlineData('\u00A0')]
			public async Task WhenSubjectIsNoControlCharacter_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).IsControl();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is a control character,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Theory]
			[InlineData('\0')]
			[InlineData('\t')]
			[InlineData('\n')]
			[InlineData('\r')]
			[InlineData('\u001B')]
			[InlineData('\u007F')]
			[InlineData('\u0085')]
			public async Task WhenSubjectIsAControlCharacter_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsControl());

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not a control character,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Theory]
			[InlineData('a')]
			[InlineData('1')]
			[InlineData(' ')]
			[InlineData('@')]
			[InlineData('\u00A0')]
			public async Task WhenSubjectIsNoControlCharacter_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsControl());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
