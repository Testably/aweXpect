namespace aweXpect.Tests;

public sealed partial class ThatTimeSpan
{
	public sealed partial class Nullable
	{
		public sealed class IsPositive
		{
			public sealed class Tests
			{
				[Test]
				public async Task WhenSubjectIsMaxValue_ShouldSucceed()
				{
					TimeSpan? subject = TimeSpan.MaxValue;

					async Task Act()
						=> await That(subject).IsPositive();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsMinValue_ShouldFail()
				{
					TimeSpan? subject = TimeSpan.MinValue;

					async Task Act()
						=> await That(subject).IsPositive();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is positive,
						             but it was TimeSpan.MinValue
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNegative_ShouldFail()
				{
					TimeSpan? subject = -1.Seconds();

					async Task Act()
						=> await That(subject).IsPositive();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is positive,
						             but it was -0:01
						             """);
				}

				[Test]
				public async Task WhenSubjectIsNull_ShouldFail()
				{
					TimeSpan? subject = null;

					async Task Act()
						=> await That(subject).IsPositive();

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is positive,
						             but it was <null>
						             """);
				}

				[Test]
				public async Task WhenSubjectIsPositive_ShouldSucceed()
				{
					TimeSpan? subject = 1.Seconds();

					async Task Act()
						=> await That(subject).IsPositive();

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsZero_ShouldFail()
				{
					TimeSpan? subject = TimeSpan.Zero;

					async Task Act()
						=> await That(subject).IsPositive();

					await That(Act).Throws<FailException>()
						.WithMessage($"""
						              Expected that subject
						              is positive,
						              but it was {Formatter.Format(subject)}
						              """);
				}
			}

			public sealed class NegatedTests
			{
				[Test]
				public async Task WhenSubjectIsNegative_ShouldSucceed()
				{
					TimeSpan? subject = -5.Seconds();

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsPositive());

					await That(Act).DoesNotThrow();
				}

				[Test]
				public async Task WhenSubjectIsPositive_ShouldFail()
				{
					TimeSpan? subject = 5.Seconds();

					async Task Act()
						=> await That(subject).DoesNotComplyWith(it => it.IsPositive());

					await That(Act).Throws<FailException>()
						.WithMessage("""
						             Expected that subject
						             is not positive,
						             but it was 0:05
						             """);
				}
			}
		}
	}
}
