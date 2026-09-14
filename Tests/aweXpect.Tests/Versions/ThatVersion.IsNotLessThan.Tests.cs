namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class IsNotLessThan
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectIsGreater_ShouldSucceed()
			{
				Version? subject = new(2, 0);
				Version unexpected = new(1, 2);

				async Task Act()
					=> await That(subject).IsNotLessThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsLess_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version unexpected = new(2, 0);

				async Task Act()
					=> await That(subject).IsNotLessThan(unexpected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not less than 2.0, because we want to test the failure,
					             but it was 1.2
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;

				async Task Act()
					=> await That(subject).IsNotLessThan(new Version(1, 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not less than 1.2,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsTheSame_ShouldSucceed()
			{
				Version? subject = new(1, 2, 3, 4);
				Version unexpected = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).IsNotLessThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenUnexpectedIsNull_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotLessThan(unexpected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not less than <null>,
					             but it was 1.2
					             """)
					.Because("a version can neither be less nor not less than nothing");
			}
		}
	}
}
