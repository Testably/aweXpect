namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed class IsNotUpperCased
	{
		public sealed class Tests
		{
			[Test]
			[Arguments('a')]
			[Arguments('z')]
			[Arguments('\u00E4')]
			[Arguments('1')]
			[Arguments(' ')]
			[Arguments('@')]
			[Arguments('\u4E50')]
			public async Task WhenSubjectIsNotUpperCased_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments('A')]
			[Arguments('M')]
			[Arguments('Z')]
			[Arguments('\u00C4')]
			[Arguments('\u03A9')]
			public async Task WhenSubjectIsUpperCased_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).IsNotUpperCased();

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is not upper-cased,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			[Arguments('a')]
			[Arguments('z')]
			[Arguments('\u00E4')]
			[Arguments('1')]
			[Arguments(' ')]
			[Arguments('@')]
			[Arguments('\u4E50')]
			public async Task WhenSubjectIsNotUpperCased_ShouldFail(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotUpperCased());

				await That(Act).Throws<FailException>()
					.WithMessage($"""
					              Expected that subject
					              is upper-cased,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Test]
			[Arguments('A')]
			[Arguments('M')]
			[Arguments('Z')]
			[Arguments('\u00C4')]
			[Arguments('\u03A9')]
			public async Task WhenSubjectIsUpperCased_ShouldSucceed(char subject)
			{
				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotUpperCased());

				await That(Act).DoesNotThrow();
			}
		}
	}
}
