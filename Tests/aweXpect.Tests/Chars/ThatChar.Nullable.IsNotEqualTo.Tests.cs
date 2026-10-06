namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsNotEqualTo
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenSubjectAndUnexpectedAreNull_ShouldFail()
				{
					char? subject = null;
					char? unexpected = null;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to <null>,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectDiffersOnlyInCase_AndIgnoringCase_ShouldFail()
				{
					char? subject = 'a';

					async Task Act()
						=> await That(subject).IsNotEqualTo('A').IgnoringCase();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not equal to 'A' ignoring case,
						             but it was 'a'
						             """);
				}

				[Test]
				[Arguments('a', 'b')]
				[Arguments('a', null)]
				[Arguments('B', 'b')]
				[Arguments('B', null)]
				[Arguments(null, 'a')]
				[Arguments(null, 'B')]
				public async Task WhenSubjectIsDifferent_ShouldSucceed(char? subject,
					char? unexpected)
				{
					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsNull_AndIgnoringCase_ShouldSucceed()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsNotEqualTo('a').IgnoringCase();

					await That(Act).DoesNotThrow();
				}

				[Test]
				[Arguments('a')]
				[Arguments('X')]
				[Arguments('5')]
				[Arguments('\t')]
				public async Task WhenSubjectIsTheSame_ShouldFail(char? subject)
				{
					char? unexpected = subject;

					async Task Act()
						=> await That(subject).IsNotEqualTo(unexpected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is not equal to {Formatter.Format(unexpected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
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
