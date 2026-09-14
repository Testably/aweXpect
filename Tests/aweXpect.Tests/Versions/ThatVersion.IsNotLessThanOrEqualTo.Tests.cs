namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class IsNotLessThanOrEqualTo
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectIsGreater_ShouldSucceed()
			{
				Version? subject = new(2, 0);
				Version unexpected = new(1, 2);

				async Task Act()
					=> await That(subject).IsNotLessThanOrEqualTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsLess_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version unexpected = new(2, 0);

				async Task Act()
					=> await That(subject).IsNotLessThanOrEqualTo(unexpected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not less than or equal to 2.0, because we want to test the failure,
					             but it was 1.2
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;

				async Task Act()
					=> await That(subject).IsNotLessThanOrEqualTo(new Version(1, 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not less than or equal to 1.2,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsTheSame_ShouldFail()
			{
				Version? subject = new(1, 2, 3, 4);
				Version unexpected = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).IsNotLessThanOrEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not less than or equal to 1.2.3.4,
					             but it was 1.2.3.4
					             """);
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotLessThanOrEqualTo(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not less than or equal to <null>,
					             but it was 1.2
					             """)
					.Because("a version can neither be less nor not less than nothing");
			}
		}
	}
}
