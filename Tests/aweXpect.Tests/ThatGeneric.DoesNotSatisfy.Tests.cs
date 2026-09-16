namespace aweXpect.Tests;

public sealed partial class ThatGeneric
{
	public sealed class DoesNotSatisfy
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task ShouldFailWhenPredicateResultIsTrue(bool predicateResult)
			{
				Other subject = new();

				async Task Act()
					=> await That(subject).DoesNotSatisfy(_ => predicateResult);

				await That(Act).Throws<XunitException>()
					.OnlyIf(predicateResult)
					.WithMessage("""
					             Expected that subject
					             does not satisfy _ => predicateResult,
					             but it was ThatGeneric.Other {
					               Value = 0
					             }
					             """);
			}

			[Fact]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				Other subject = new();

				async Task Act()
					=> await That(subject).DoesNotSatisfy(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The predicate cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Other? subject = null;

				async Task Act()
					=> await That(subject).DoesNotSatisfy(_ => false);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not satisfy _ => false,
					             but it was <null>
					             """);
			}
		}

		public sealed class WithinTests
		{
			[Fact]
			public async Task WhenGlobalTimeoutIsApplied_ShouldFail()
			{
				int count = 0;
				Other subject = new();

				async Task Act()
					=> await That(subject).DoesNotSatisfy(_ => ++count <= 42).Within(30.Seconds())
						.WithTimeout(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not satisfy _ => ++count <= 42 within 0:30,
					             but it was ThatGeneric.Other {
					               Value = 0
					             }
					             """);
			}

			[Theory]
			[InlineData(1, false)]
			[InlineData(0, true)]
			[InlineData(-1, true)]
			public async Task WhenIntervalIsNotPositive_ShouldThrowArgumentOutOfRangeException(int intervalSeconds,
				bool shouldThrow)
			{
				Other subject = new();

				async Task Act()
					=> await That(subject).DoesNotSatisfy(_ => false).Within(1.Seconds())
						.CheckEvery(intervalSeconds.Seconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.OnlyIf(shouldThrow)
					.WithParamName("interval").And
					.WithMessage("The interval must be positive*").AsWildcard();
			}

			[Fact]
			public async Task WhenPredicateResultIsFalseInitially_ShouldSucceedWithoutRetrying()
			{
				int count = 0;
				Other subject = new();

				async Task Act()
					=> await That(subject).DoesNotSatisfy(_ => ++count > 1).Within(5.Seconds());

				await That(Act).DoesNotThrow();
				await That(count).IsEqualTo(1);
			}

			[Fact]
			public async Task WhenPredicateResultTurnsFalseLaterOn_ShouldSucceed()
			{
				int count = 0;
				Other subject = new();

				async Task Act()
					=> await That(subject).DoesNotSatisfy(_ => ++count <= 2).Within(5.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData(1, false)]
			[InlineData(0, false)]
			[InlineData(-1, true)]
			public async Task WhenTimeoutIsNegative_ShouldThrowArgumentOutOfRangeException(int timeoutSeconds,
				bool shouldThrow)
			{
				Other subject = new();

				async Task Act()
					=> await That(subject).DoesNotSatisfy(_ => false).Within(timeoutSeconds.Seconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.OnlyIf(shouldThrow)
					.WithParamName("timeout").And
					.WithMessage("The timeout must not be negative*").AsWildcard();
			}

			[Fact]
			public async Task WhenTimeoutIsTooShort_ShouldFail()
			{
				int count = 0;
				Other subject = new();

				async Task Act()
					=> await That(subject).DoesNotSatisfy(_ => ++count <= 42).Within(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not satisfy _ => ++count <= 42 within 0:00.050,
					             but it was ThatGeneric.Other {
					               Value = 0
					             }
					             """);
			}
		}
	}
}
