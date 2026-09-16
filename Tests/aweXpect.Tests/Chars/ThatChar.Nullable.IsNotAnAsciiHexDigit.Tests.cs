namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsNotAnAsciiHexDigit
		{
			public sealed class Tests
			{
				[Theory]
				[InlineData('g')]
				[InlineData('G')]
				[InlineData('z')]
				[InlineData(' ')]
				[InlineData('/')]
				[InlineData(':')]
				[InlineData('\u0663')]
				public async Task WhenSubjectIsNoAsciiHexDigit_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).IsNotAnAsciiHexDigit();

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData('0')]
				[InlineData('9')]
				[InlineData('a')]
				[InlineData('f')]
				[InlineData('A')]
				[InlineData('F')]
				public async Task WhenSubjectIsAnAsciiHexDigit_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).IsNotAnAsciiHexDigit();

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not an ASCII hex digit,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsNotAnAsciiHexDigit();

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not an ASCII hex digit,
						             but it was <null>
						             """);
				}
			}

			public sealed class NegatedTests
			{
				[Theory]
				[InlineData('g')]
				[InlineData('G')]
				[InlineData('z')]
				[InlineData(' ')]
				[InlineData('/')]
				[InlineData(':')]
				[InlineData('\u0663')]
				public async Task WhenSubjectIsNoAsciiHexDigit_ShouldFail(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotAnAsciiHexDigit());

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is an ASCII hex digit,
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Theory]
				[InlineData('0')]
				[InlineData('9')]
				[InlineData('a')]
				[InlineData('f')]
				[InlineData('A')]
				[InlineData('F')]
				public async Task WhenSubjectIsAnAsciiHexDigit_ShouldSucceed(char? subject)
				{
					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotAnAsciiHexDigit());

					await That(Act).DoesNotThrow();
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsNotAnAsciiHexDigit());

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is an ASCII hex digit,
						             but it was <null>
						             """);
				}
			}
		}
	}
}
