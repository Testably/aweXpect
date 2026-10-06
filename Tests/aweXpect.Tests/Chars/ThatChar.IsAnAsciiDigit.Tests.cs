namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsAnAsciiDigit
	{
		public sealed class Tests
		{
			[Test]
			[Arguments('0')]
			[Arguments('5')]
			[Arguments('9')]
			public async Task WhenSubjectIsAnAsciiDigit_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).IsAnAsciiDigit();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments('a')]
			[Arguments('A')]
			[Arguments(' ')]
			[Arguments('/')]
			[Arguments(':')]
			[Arguments('\u0663')]
			[Arguments('\u00BD')]
			public async Task WhenSubjectIsNoAsciiDigit_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).IsAnAsciiDigit();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an ASCII digit,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments('0')]
			[Arguments('5')]
			[Arguments('9')]
			public async Task WhenSubjectIsAnAsciiDigit_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAnAsciiDigit());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not an ASCII digit,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments('a')]
			[Arguments('A')]
			[Arguments(' ')]
			[Arguments('/')]
			[Arguments(':')]
			[Arguments('\u0663')]
			[Arguments('\u00BD')]
			public async Task WhenSubjectIsNoAsciiDigit_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAnAsciiDigit());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
