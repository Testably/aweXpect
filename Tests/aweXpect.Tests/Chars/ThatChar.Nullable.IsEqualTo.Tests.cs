namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsEqualTo
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					char? subject = 'v';

					async Task Act()
						=> await That(subject).IsEqualTo(null);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to <null>,
						             but it was 'v'
						             """);
				}

				[Fact]
				public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsEqualTo(null);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData('a', 'b')]
				[InlineData('a', null)]
				[InlineData('B', 'b')]
				[InlineData('B', null)]
				[InlineData(null, 'a')]
				[InlineData(null, 'B')]
				public async Task WhenSubjectIsDifferent_ShouldFail(char? subject, char? expected)
				{
					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsEqualTo('Z');

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 'Z',
						             but it was <null>
						             """);
				}

				[Theory]
				[InlineData('a')]
				[InlineData('X')]
				[InlineData('5')]
				[InlineData('\t')]
				public async Task WhenSubjectIsTheSame_ShouldSucceed(char? subject)
				{
					char? expected = subject;

					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
