namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class IsEqualTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenChainedWithAnd_ShouldKeepTheVersionSubject()
			{
				Version? subject = new(1, 2);

				async Task Act()
					=> await That(subject).IsEqualTo(new Version(1, 2)).And.HasMinor(3);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1.2 and has minor equal to 3,
					             but it had minor 2
					             """);
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(1, 2);

				async Task Act()
					=> await That(subject).IsEqualTo(null);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to <null>,
					             but it was 1.2
					             """);
			}

			[Test]
			public async Task WhenSubjectAndExpectedAreNull_ShouldSucceed()
			{
				Version? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(null);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsDifferent_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version expected = new(1, 3);

				async Task Act()
					=> await That(subject).IsEqualTo(expected)
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1.3, because we want to test the failure,
					             but it was 1.2
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;

				async Task Act()
					=> await That(subject).IsEqualTo(new Version(1, 2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1.2,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsTheSame_ShouldSucceed()
			{
				Version? subject = new(1, 2, 3, 4);
				Version expected = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectOmitsAComponentOfExpected_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version expected = new(1, 2, 0);

				async Task Act()
					=> await That(subject).IsEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1.2.0,
					             but it was 1.2
					             """)
					.Because("an unset component is not treated as 0");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsDifferent_ShouldSucceed()
			{
				Version? subject = new(1, 2);
				Version expected = new(1, 3);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsTheSame_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version expected = new(1, 2);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsEqualTo(expected));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 1.2,
					             but it was 1.2
					             """);
			}
		}
	}
}
