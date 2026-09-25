namespace aweXpect.Tests;

public sealed partial class ThatTimeSpan
{
	public sealed class IsNotPositive
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenSubjectIsMaxValue_ShouldFail()
			{
				TimeSpan subject = TimeSpan.MaxValue;

				async Task Act()
					=> await That(subject).IsNotPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not positive,
					             but it was TimeSpan.MaxValue
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsMinValue_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.MinValue;

				async Task Act()
					=> await That(subject).IsNotPositive();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNegative_ShouldSucceed()
			{
				TimeSpan subject = -1.Seconds();

				async Task Act()
					=> await That(subject).IsNotPositive();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsPositive_ShouldFail()
			{
				TimeSpan subject = 1.Seconds();

				async Task Act()
					=> await That(subject).IsNotPositive();

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              is not positive,
					              but it was {Formatter.Format(subject)}
					              """);
			}

			[Fact]
			public async Task WhenSubjectIsZero_ShouldSucceed()
			{
				TimeSpan subject = TimeSpan.Zero;

				async Task Act()
					=> await That(subject).IsNotPositive();

				await That(Act).DoesNotThrow();
			}
		}
	}
}
