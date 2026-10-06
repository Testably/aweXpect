namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsALetter
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
				[Arguments('\u4E50')]
				public async Task WhenSubjectIsALetter_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).IsALetter();

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
				public async Task WhenSubjectIsNoLetter_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).IsALetter();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is a letter,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsALetter();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is a letter,
						             but it was <null>
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
				[Arguments('\u4E50')]
				public async Task WhenSubjectIsALetter_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsALetter());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not a letter,
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
				public async Task WhenSubjectIsNoLetter_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsALetter());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsALetter());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not a letter,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
