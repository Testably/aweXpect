namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsAnAsciiHexDigit
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
				public async Task WhenSubjectIsAnAsciiHexDigit_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).IsAnAsciiHexDigit();

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
				public async Task WhenSubjectIsNoAsciiHexDigit_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).IsAnAsciiHexDigit();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is an ASCII hex digit,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsAnAsciiHexDigit();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is an ASCII hex digit,
						             but it was <null>
						             """);
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
				public async Task WhenSubjectIsAnAsciiHexDigit_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsAnAsciiHexDigit());

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
				public async Task WhenSubjectIsNoAsciiHexDigit_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsAnAsciiHexDigit());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsAnAsciiHexDigit());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not an ASCII hex digit,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
