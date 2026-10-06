namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class IsNotGreaterThan
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenSubjectIsGreater_ShouldFail()
			{
				Version? subject = new(2, 0);
				Version unexpected = new(1, 2);

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected)
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not greater than 1.2, because we want to test the failure,
					             but it was 2.0
					             """);
			}

			[Test]
			public async Task WhenSubjectIsLess_ShouldSucceed()
			{
				Version? subject = new(1, 2);
				Version unexpected = new(2, 0);

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(new Version(1, 2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not greater than 1.2,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsTheSame_ShouldSucceed()
			{
				Version? subject = new(1, 2, 3, 4);
				Version unexpected = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenUnexpectedIsNull_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version? unexpected = null;

				async Task Act()
					=> await That(subject).IsNotGreaterThan(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not greater than <null>,
					             but it was 1.2
					             """)
					.Because("a version can neither be greater nor not greater than nothing");
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsGreater_ShouldSucceed()
			{
				Version subject = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotGreaterThan(new Version(1, 2)));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsLess_ShouldFail()
			{
				Version subject = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotGreaterThan(new Version(2, 0)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2.0,
					             but it was 1.2.3.4
					             """);
			}
		}
	}
}
