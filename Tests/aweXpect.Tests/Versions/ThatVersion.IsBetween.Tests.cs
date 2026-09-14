namespace aweXpect.Tests;

public sealed partial class ThatVersion
{
	public sealed class IsBetween
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMaximumIsNull_ShouldFail()
			{
				Version? subject = new(1, 5);
				Version? maximum = null;

				async Task Act()
					=> await That(subject).IsBetween(new Version(1, 2)).And(maximum);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is between 1.2 and <null>,
					             but it was 1.5
					             """);
			}

			[Fact]
			public async Task WhenMinimumIsGreaterThanMaximum_ShouldFail()
			{
				Version? subject = new(1, 5);

				async Task Act()
					=> await That(subject).IsBetween(new Version(2, 0)).And(new Version(1, 2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is between 2.0 and 1.2,
					             but it was 1.5
					             """)
					.Because("an inverted range can never be satisfied");
			}

			[Fact]
			public async Task WhenMinimumIsNull_ShouldFail()
			{
				Version? subject = new(1, 5);
				Version? minimum = null;

				async Task Act()
					=> await That(subject).IsBetween(minimum).And(new Version(2, 0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is between <null> and 2.0,
					             but it was 1.5
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsGreaterThanMaximum_ShouldFail()
			{
				Version? subject = new(2, 1);

				async Task Act()
					=> await That(subject).IsBetween(new Version(1, 2)).And(new Version(2, 0))
						.Because("we want to test the failure");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is between 1.2 and 2.0, because we want to test the failure,
					             but it was 2.1
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsLessThanMinimum_ShouldFail()
			{
				Version? subject = new(1, 1);

				async Task Act()
					=> await That(subject).IsBetween(new Version(1, 2)).And(new Version(2, 0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is between 1.2 and 2.0,
					             but it was 1.1
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Version? subject = null;

				async Task Act()
					=> await That(subject).IsBetween(new Version(1, 2)).And(new Version(2, 0));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is between 1.2 and 2.0,
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsTheMaximum_ShouldSucceed()
			{
				Version? subject = new(2, 0);

				async Task Act()
					=> await That(subject).IsBetween(new Version(1, 2)).And(new Version(2, 0));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsTheMinimum_ShouldSucceed()
			{
				Version? subject = new(1, 2);

				async Task Act()
					=> await That(subject).IsBetween(new Version(1, 2)).And(new Version(2, 0));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsWithinTheRange_ShouldSucceed()
			{
				Version? subject = new(1, 5);

				async Task Act()
					=> await That(subject).IsBetween(new Version(1, 2)).And(new Version(2, 0));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
