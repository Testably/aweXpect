namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsNotWhiteSpace
	{
		public sealed class Tests
		{
			[Test]
			[Arguments(' ')]
			[Arguments('\t')]
			[Arguments('\r')]
			[Arguments('\n')]
			public async Task WhenSubjectIsNotWhiteSpace_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).IsNotWhiteSpace();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not whitespace,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments('0')]
			[Arguments('1')]
			[Arguments('4')]
			[Arguments('9')]
			[Arguments('a')]
			[Arguments('d')]
			[Arguments('z')]
			[Arguments('A')]
			[Arguments('M')]
			[Arguments('Z')]
			[Arguments('\u4E50')]
			[Arguments('@')]
			[Arguments('[')]
			[Arguments(']')]
			[Arguments('{')]
			[Arguments('}')]
			public async Task WhenSubjectIsNotWhiteSpace_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).IsNotWhiteSpace();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments('0')]
			[Arguments('1')]
			[Arguments('4')]
			[Arguments('9')]
			[Arguments('a')]
			[Arguments('d')]
			[Arguments('z')]
			[Arguments('A')]
			[Arguments('M')]
			[Arguments('Z')]
			[Arguments('\u4E50')]
			[Arguments('@')]
			[Arguments('[')]
			[Arguments(']')]
			[Arguments('{')]
			[Arguments('}')]
			public async Task WhenSubjectIsNotWhiteSpace_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotWhiteSpace());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is whitespace,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments(' ')]
			[Arguments('\t')]
			[Arguments('\r')]
			[Arguments('\n')]
			public async Task WhenSubjectIsNotWhiteSpace_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotWhiteSpace());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
