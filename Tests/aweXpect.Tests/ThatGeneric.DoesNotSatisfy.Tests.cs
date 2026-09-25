using System.Threading;

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
			public async Task WhenNullableValueTypeSubjectIsNull_ShouldUsePredicateResult()
			{
				int? subject = null;

				async Task Act()
					=> await That(subject).DoesNotSatisfy(x => x is not null);

				await That(Act).DoesNotThrow()
					.Because("the predicate decides about a null subject as well");
			}

			[Fact]
			public async Task WhenPredicateDereferencesANullSubject_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).DoesNotSatisfy(x => x!.Length > 5);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not satisfy x => x!.Length > 5,
					             but it did throw a NullReferenceException:
					             """).AsPrefix().And
					.Whose(e => e.InnerException, i => i.Is<NullReferenceException>());
			}

			[Fact]
			public async Task WhenPredicateGuardsAgainstNull_ShouldSucceedForANullSubject()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).DoesNotSatisfy(x => x?.Length > 5);

				await That(Act).DoesNotThrow()
					.Because("a null-safe predicate answers false for a null subject, which is exactly what the expectation asks for");
			}

			[Fact]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				Other subject = new();

				async Task Act()
					=> await That(subject).DoesNotSatisfy(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenSubjectIsNullAndPredicateExpectsNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).DoesNotSatisfy(x => x is null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not satisfy x => x is null,
					             but it was <null>
					             """);
			}

			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task WhenSubjectIsNull_ShouldUsePredicateResult(bool predicateResult)
			{
				Other? subject = null;

				async Task Act()
					=> await That(subject).DoesNotSatisfy(_ => predicateResult);

				await That(Act).Throws<XunitException>()
					.OnlyIf(predicateResult)
					.WithMessage("""
					             Expected that subject
					             does not satisfy _ => predicateResult,
					             but it was <null>
					             """)
					.Because("the predicate decides about a null subject as well");
			}
		}

		public sealed class WithinTests
		{
			[Fact]
			public async Task WhenCancellationIsRequestedWhileRetrying_ShouldBeInconclusive()
			{
				Other subject = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act()
					=> await That(subject).DoesNotSatisfy(_ => true).Within(30.Seconds())
						.WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that subject
					             does not satisfy _ => true within 0:30,
					             but it could not be verified, because it was already canceled
					             """).WithTimeout(10.Seconds());
			}

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
					             but it did not finish within 0:00.050
					             """).And
					.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."));
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

			[Fact]
			public async Task WhenTimeoutIsInfinite_ShouldNotMentionTheTimeout()
			{
				Other subject = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act()
					=> await That(subject).DoesNotSatisfy(_ => true).Within(System.Threading.Timeout.InfiniteTimeSpan)
						.WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that subject
					             does not satisfy _ => true,
					             but it could not be verified, because it was already canceled
					             """).WithTimeout(10.Seconds())
					.Because("an infinite timeout imposes no limit, so only the cancellation ends the retries");
			}

			[Fact]
			public async Task WhenTimeoutIsInfinite_ShouldRetryUntilThePredicateIsNoLongerSatisfied()
			{
				int count = 0;
				Other subject = new();

				async Task Act()
					=> await That(subject).DoesNotSatisfy(_ => ++count <= 2)
						.Within(System.Threading.Timeout.InfiniteTimeSpan)
						.CheckEvery(10.Milliseconds());

				await That(Act).DoesNotThrow();
				await That(count).IsEqualTo(3);
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
