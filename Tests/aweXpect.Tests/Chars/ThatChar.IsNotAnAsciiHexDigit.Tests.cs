namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsNotAnAsciiHexDigit
	{
		public sealed class Tests
		{
			[Test]
			[Arguments('0')]
			[Arguments('9')]
			[Arguments('a')]
			[Arguments('f')]
			[Arguments('A')]
			[Arguments('F')]
			public async Task WhenSubjectIsAnAsciiHexDigit_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).IsNotAnAsciiHexDigit();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not an ASCII hex digit,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments('g')]
			[Arguments('G')]
			[Arguments('z')]
			[Arguments(' ')]
			[Arguments('/')]
			[Arguments(':')]
			[Arguments('\u0663')]
			public async Task WhenSubjectIsNoAsciiHexDigit_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).IsNotAnAsciiHexDigit();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments('0')]
			[Arguments('9')]
			[Arguments('a')]
			[Arguments('f')]
			[Arguments('A')]
			[Arguments('F')]
			public async Task WhenSubjectIsAnAsciiHexDigit_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotAnAsciiHexDigit());

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments('g')]
			[Arguments('G')]
			[Arguments('z')]
			[Arguments(' ')]
			[Arguments('/')]
			[Arguments(':')]
			[Arguments('\u0663')]
			public async Task WhenSubjectIsNoAsciiHexDigit_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotAnAsciiHexDigit());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an ASCII hex digit,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
