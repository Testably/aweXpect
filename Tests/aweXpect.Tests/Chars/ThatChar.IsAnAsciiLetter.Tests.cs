namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsAnAsciiLetter
	{
		public sealed class Tests
		{
			[Test]
			[Arguments('a')]
			[Arguments('d')]
			[Arguments('z')]
			[Arguments('A')]
			[Arguments('M')]
			[Arguments('Z')]
			public async Task WhenSubjectIsAnAsciiLetter_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).IsAnAsciiLetter();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments('\t')]
			[Arguments('5')]
			[Arguments('@')]
			[Arguments('[')]
			[Arguments(']')]
			[Arguments('{')]
			[Arguments('}')]
			[Arguments('\u4E50')]
			public async Task WhenSubjectIsNoAsciiLetter_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).IsAnAsciiLetter();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is an ASCII letter,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments('a')]
			[Arguments('d')]
			[Arguments('z')]
			[Arguments('A')]
			[Arguments('M')]
			[Arguments('Z')]
			public async Task WhenSubjectIsAnAsciiLetter_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAnAsciiLetter());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not an ASCII letter,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments('\t')]
			[Arguments('5')]
			[Arguments('@')]
			[Arguments('[')]
			[Arguments(']')]
			[Arguments('{')]
			[Arguments('}')]
			[Arguments('\u4E50')]
			public async Task WhenSubjectIsNoAsciiLetter_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsAnAsciiLetter());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
