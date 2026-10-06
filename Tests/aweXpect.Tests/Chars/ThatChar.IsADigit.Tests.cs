namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsADigit
	{
		public sealed class Tests
		{
			[Test]
			[Arguments('0')]
			[Arguments('5')]
			[Arguments('9')]
			[Arguments('\u0663')]
			[Arguments('\u096B')]
			public async Task WhenSubjectIsADigit_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).IsADigit();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments('a')]
			[Arguments('A')]
			[Arguments(' ')]
			[Arguments('@')]
			[Arguments('\u00BD')]
			[Arguments('\u2163')]
			public async Task WhenSubjectIsNoDigit_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).IsADigit();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is a digit,
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
			[Arguments('\u0663')]
			[Arguments('\u096B')]
			public async Task WhenSubjectIsADigit_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsADigit());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not a digit,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments('a')]
			[Arguments('A')]
			[Arguments(' ')]
			[Arguments('@')]
			[Arguments('\u00BD')]
			[Arguments('\u2163')]
			public async Task WhenSubjectIsNoDigit_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsADigit());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
