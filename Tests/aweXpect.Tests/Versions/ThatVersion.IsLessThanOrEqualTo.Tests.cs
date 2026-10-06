namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class IsLessThanOrEqualTo
	{
		public sealed class Tests
		{
			[Test]
			public async Task WhenExpectedIsNull_AndNegated_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version? expected = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsLessThanOrEqualTo(expected));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not less than or equal to <null>,
					             but it was 1.2
					             """)
					.Because("nothing can be ordered against null, so the negation fails as well");
			}

			[Test]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version? expected = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is less than or equal to <null>,
					             but it was 1.2
					             """);
			}

			[Test]
			public async Task WhenSubjectIsGreater_ShouldFail()
			{
				Version? subject = new(2, 0);
				Version expected = new(1, 2);

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected)
						.Because("we want to test the failure");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is less than or equal to 1.2, because we want to test the failure,
					             but it was 2.0
					             """);
			}

			[Test]
			public async Task WhenSubjectIsLess_ShouldSucceed()
			{
				Version? subject = new(1, 2);
				Version expected = new(2, 0);

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(new Version(1, 2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is less than or equal to 1.2,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsTheSame_ShouldSucceed()
			{
				Version? subject = new(1, 2, 3, 4);
				Version expected = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).IsLessThanOrEqualTo(expected);

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenSubjectIsGreater_ShouldSucceed()
			{
				Version subject = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsLessThanOrEqualTo(new Version(1, 2)));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsTheSame_ShouldFail()
			{
				Version subject = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsLessThanOrEqualTo(new Version(1, 2, 3, 4)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not less than or equal to 1.2.3.4,
					             but it was 1.2.3.4
					             """);
			}
		}
	}
}
