namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsNotEqualTo
		{
			public sealed class Tests
			{
				[Fact]
				public async Task WhenSubjectAndUnexpectedAreNull_ShouldFail()
				{
					char? subject = null;
					char? unexpected = null;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to <null>,
						             but it was <null>
						             """);
				}

				[Theory]
				[InlineData('a', 'b')]
				[InlineData('a', null)]
				[InlineData('B', 'b')]
				[InlineData('B', null)]
				[InlineData(null, 'a')]
				[InlineData(null, 'B')]
				public async Task WhenSubjectIsDifferent_ShouldSucceed(char? subject,
					char? unexpected)
				{
					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Theory]
				[InlineData('a')]
				[InlineData('X')]
				[InlineData('5')]
				[InlineData('\t')]
				public async Task WhenSubjectIsTheSame_ShouldFail(char? subject)
				{
					char? unexpected = subject;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<XunitException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Fact]
				public async Task WhenUnexpectedIsNull_ShouldSucceed()
				{
					char? subject = 'B';

					async Task Act()
						=> await That(subject).IsNotEqualTo(null);

					await That(Act).DoesNotThrow();
				}
			}
		}
	}
}
