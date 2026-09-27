namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class IsNotEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenChainedWithAnd_ShouldKeepTheVersionSubject()
			{
				Version? subject = new(1, 2);

				async Task Act()
					=> await That(subject).IsNotEqualTo(new Version(1, 3)).And.HasMinor(3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 1.3 and has minor equal to 3,
					             but it had minor 2
					             """);
			}

			[Fact]
			public async Task WhenSubjectAndUnexpectedAreNull_ShouldFail()
			{
				Version? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to <null>,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldSucceed()
			{
				Version? subject = new(1, 2);
				Version unexpected = new(1, 3);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldSucceed()
			{
				Version? subject = null;

				async Task Act()
					=> await That(subject).IsNotEqualTo(new Version(1, 2));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsTheSame_ShouldFail()
			{
				Version? subject = new(1, 2, 3, 4);
				Version unexpected = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 1.2.3.4, because we want to test the failure,
					             but it was 1.2.3.4
					             """);
			}

			[Fact]
			public async Task WhenSubjectOmitsAComponentOfUnexpected_ShouldSucceed()
			{
				Version? subject = new(1, 2);
				Version unexpected = new(1, 2, 0);

				async Task Act()
					=> await That(subject).IsNotEqualTo(unexpected);

				await That(Act).DoesNotThrow()
					.Because("an unset component is not treated as 0");
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldSucceed()
			{
				Version? subject = new(1, 2);

				async Task Act()
					=> await That(subject).IsNotEqualTo(null);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenSubjectIsDifferent_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version unexpected = new(1, 3);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotEqualTo(unexpected));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1.3,
					             but it was 1.2
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsTheSame_ShouldSucceed()
			{
				Version? subject = new(1, 2);
				Version unexpected = new(1, 2);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotEqualTo(unexpected));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
