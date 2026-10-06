namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsANumber
		{
			public sealed class Tests
			{
				[Test]
				[Arguments('0')]
				[Arguments('1')]
				[Arguments('4')]
				[Arguments('9')]
				[Arguments('\u00BD')]
				[Arguments('\u2163')]
				[Arguments('\u00B2')]
				public async Task WhenSubjectIsANumber_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).IsANumber();

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments('a')]
				[Arguments('d')]
				[Arguments('z')]
				[Arguments('A')]
				[Arguments('M')]
				[Arguments('Z')]
				[Arguments('\u4E50')]
				[Arguments('\t')]
				[Arguments('@')]
				[Arguments('[')]
				[Arguments(']')]
				[Arguments('{')]
				[Arguments('}')]
				public async Task WhenSubjectIsNotANumber_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).IsANumber();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is a number,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsANumber();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is a number,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				[Arguments('0')]
				[Arguments('1')]
				[Arguments('4')]
				[Arguments('9')]
				[Arguments('\u00BD')]
				[Arguments('\u2163')]
				[Arguments('\u00B2')]
				public async Task WhenSubjectIsANumber_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsANumber());

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not a number,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				[Arguments('a')]
				[Arguments('d')]
				[Arguments('z')]
				[Arguments('A')]
				[Arguments('M')]
				[Arguments('Z')]
				[Arguments('\u4E50')]
				[Arguments('\t')]
				[Arguments('@')]
				[Arguments('[')]
				[Arguments(']')]
				[Arguments('{')]
				[Arguments('}')]
				public async Task WhenSubjectIsNotANumber_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsANumber());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsANumber());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not a number,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
