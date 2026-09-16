namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsNotAnAsciiDigit
		{
			public sealed class Tests
			{
				[Theory]
				[InlineData('a')]
				[InlineData('A')]
				[InlineData(' ')]
				[InlineData('/')]
				[InlineData(':')]
				[InlineData('\u0663')]
				[InlineData('\u00BD')]
				public async Task WhenSubjectIsNoAsciiDigit_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).IsNotAnAsciiDigit();

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData('0')]
				[InlineData('5')]
				[InlineData('9')]
				public async Task WhenSubjectIsAnAsciiDigit_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).IsNotAnAsciiDigit();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not an ASCII digit,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsNotAnAsciiDigit();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not an ASCII digit,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Theory]
				[InlineData('a')]
				[InlineData('A')]
				[InlineData(' ')]
				[InlineData('/')]
				[InlineData(':')]
				[InlineData('\u0663')]
				[InlineData('\u00BD')]
				public async Task WhenSubjectIsNoAsciiDigit_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotAnAsciiDigit());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is an ASCII digit,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Theory]
				[InlineData('0')]
				[InlineData('5')]
				[InlineData('9')]
				public async Task WhenSubjectIsAnAsciiDigit_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotAnAsciiDigit());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotAnAsciiDigit());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is an ASCII digit,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
