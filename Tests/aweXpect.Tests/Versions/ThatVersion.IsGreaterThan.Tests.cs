namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class IsGreaterThan
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version? expected = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than <null>,
					             but it was 1.2
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsGreater_ShouldSucceed()
			{
				Version? subject = new(2, 0);
				Version expected = new(1, 2);

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsLess_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version expected = new(2, 0);

				async Task Act()
					=> await That(subject).IsGreaterThan(expected)
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 2.0, because we want to test the failure,
					             but it was 1.2
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;

				async Task Act()
					=> await That(subject).IsGreaterThan(new Version(1, 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 1.2,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsTheSame_ShouldFail()
			{
				Version? subject = new(1, 2, 3, 4);
				Version expected = new(1, 2, 3, 4);

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 1.2.3.4,
					             but it was 1.2.3.4
					             """);
			}

			[Fact]
			public async Task WhenSubjectOmitsAComponentOfExpected_ShouldFail()
			{
				Version? subject = new(1, 2);
				Version expected = new(1, 2, 0);

				async Task Act()
					=> await That(subject).IsGreaterThan(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is greater than 1.2.0,
					             but it was 1.2
					             """)
					.Because("an unspecified build compares as less than an explicit zero");
			}
		}
	}
}
