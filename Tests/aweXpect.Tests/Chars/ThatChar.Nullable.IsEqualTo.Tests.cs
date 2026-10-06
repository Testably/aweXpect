namespace aweXpect.Tests;

public sealed partial class ThatChar
{
	public sealed partial class Nullable
	{
		public sealed class IsEqualTo
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenExpectedIsNull_ShouldFail()
				{
					char? subject = 'v';

					async Task Act()
						=> await That(subject).IsEqualTo(null);

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to <null>,
						             but it was 'v'
						             """);
				}

				[Test]
				public async Task WhenSubjectAndExpectedAreNull_AndIgnoringCase_ShouldSucceed()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsEqualTo(null).IgnoringCase();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsEqualTo(null);

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectDiffersOnlyInCase_AndIgnoringCase_ShouldSucceed()
				{
					char? subject = 'a';

					async Task Act()
						=> await That(subject).IsEqualTo('A').IgnoringCase();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsDifferent_AndIgnoringCase_ShouldFail()
				{
					char? subject = 'a';

					async Task Act()
						=> await That(subject).IsEqualTo('B').IgnoringCase();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 'B' ignoring case,
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
				public async Task WhenSubjectIsDifferent_ShouldFail(char? subject, char? expected)
				{
					async Task Act()
						=> await That(subject).IsEqualTo(expected);

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is equal to {Formatter.Format(expected)},
						              but it was {Formatter.Format(subject)}
						              """);
				}

				[Test]
				public async Task WhenSubjectIsNull_AndIgnoringCase_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsEqualTo('a').IgnoringCase();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 'a' ignoring case,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					char? subject = null;

					async Task Act()
						=> await That(subject).IsEqualTo('Z');

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is equal to 'Z',
						             but it was <null>
						             """);
				}

				[Test]
				[Arguments('a')]
				[Arguments('X')]
				[Arguments('5')]
				[Arguments('\t')]
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
