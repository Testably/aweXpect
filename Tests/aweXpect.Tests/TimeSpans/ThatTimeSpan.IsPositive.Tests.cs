namespace aweXpect.Tests;

public sealed partial class ThatTimeSpan
{
	public sealed class IsPositive
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectIsMaxValue_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsMinValue_ShouldFail()
			{
				TimeSpan subject = TimeSpan.MinValue;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was TimeSpan.MinValue
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNegative_ShouldFail()
			{
				TimeSpan subject = -1.Seconds();

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is positive,
					             but it was -0:01
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsPositive_ShouldSucceed()
			{
				TimeSpan subject = 1.Seconds();

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsZero_ShouldFail()
			{
				TimeSpan subject = TimeSpan.Zero;

				async Task Act()
					=> await That(subject).IsPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}
		}
	}
}
