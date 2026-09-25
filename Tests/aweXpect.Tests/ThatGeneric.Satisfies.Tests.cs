using System.Threading;

namespace aweXpect.Tests;

public sealed partial class ThatGeneric
{
	public sealed class Satisfies
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData(true)]
			[InlineData(false)]
			public async Task ShouldFailWhenPredicateResultIsFalse(bool predicateResult)
			{
				Other subject = new();

				async Task Act()
					=> await That(subject).Satisfies(_ => predicateResult);

				await That(Act).Throws<XunitException>()
					.OnlyIf(!predicateResult)
					.WithMessage("""
					             Expected that subject
					             satisfies _ => predicateResult,
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
					=> await That(subject).Satisfies(x => x is null);

				await That(Act).DoesNotThrow()
					.Because("the predicate decides about a null subject as well");
			}

			[Fact]
			public async Task WhenPredicateCancelsTheEvaluation_ShouldBeInconclusive()
			{
				using CancellationTokenSource cts = new();
				Other subject = new();

				bool CancelingPredicate(Other _)
				{
					cts.Cancel();
					throw new OperationCanceledException("evaluation canceled", cts.Token);
				}

				async Task Act()
					=> await That(subject).Satisfies(CancelingPredicate).WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that subject
					             satisfies CancelingPredicate,
					             but it could not be verified, because it was already canceled
					             """)
					.Because("a cancellation that was actually requested aborts the evaluation instead of answering the expectation");
			}

			[Fact]
			public async Task WhenPredicateDereferencesANullSubject_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).Satisfies(x => x!.Length > 3);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             satisfies x => x!.Length > 3,
					             but it did throw a NullReferenceException:
					             """).AsPrefix().And
					.Whose(e => e.InnerException, i => i.Is<NullReferenceException>())
					.Because("a careless predicate meeting a null subject must be reported as a failed expectation");
			}

			[Fact]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				Other subject = new();

				async Task Act()
					=> await That(subject).Satisfies(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate").And
					.WithMessage("The 'predicate' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenPredicateThrowsInsideDoesNotComplyWith_ShouldFail()
			{
				InvalidOperationException exception = new("predicate failed");
				Other subject = new();

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Satisfies(_ => throw exception));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not satisfy _ => throw exception,
					             but it did throw an InvalidOperationException:
					               predicate failed
					             """)
					.Because("a predicate that threw answered nothing, so it fails the negation just as it fails the expectation");
			}

			[Fact]
			public async Task WhenPredicateThrowsOperationCanceledExceptionWithoutCancellation_ShouldFail()
			{
				Other subject = new();

				async Task Act()
					=> await That(subject)
						.Satisfies(_ => throw new OperationCanceledException("nothing was canceled"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             satisfies _ => throw new OperationCanceledException("nothing was canceled"),
					             but it did throw an OperationCanceledException:
					               nothing was canceled
					             """)
					.Because("only a cancellation that was actually requested may abort the evaluation");
			}

			[Fact]
			public async Task WhenPredicateThrows_ShouldFailWithTheExceptionAsInnerException()
			{
				InvalidOperationException exception = new("predicate failed");
				Other subject = new();

				async Task Act()
					=> await That(subject).Satisfies(_ => throw exception);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             satisfies _ => throw exception,
					             but it did throw an InvalidOperationException:
					               predicate failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
			}

			[Fact]
			public async Task WhenSubjectIsNullAndPredicateExpectsNotNull_ShouldFail()
			{
				string? subject = null;

				async Task Act()
					=> await That(subject).Satisfies(x => x is not null);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             satisfies x => x is not null,
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
					=> await That(subject).Satisfies(_ => predicateResult);

				await That(Act).Throws<XunitException>()
					.OnlyIf(!predicateResult)
					.WithMessage("""
					             Expected that subject
					             satisfies _ => predicateResult,
					             but it was <null>
					             """)
					.Because("the predicate decides about a null subject as well");
			}

			[Fact]
			public async Task WhenValueTypeSubjectDoesNotSatisfyThePredicate_ShouldFail()
			{
				int subject = 42;

				async Task Act()
					=> await That(subject).Satisfies(x => x > 100);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             satisfies x => x > 100,
					             but it was 42
					             """);
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
					=> await That(subject).Satisfies(_ => false).Within(30.Seconds())
						.WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that subject
					             satisfies _ => false within 0:30,
					             but it could not be verified, because it was already canceled
					             """).WithTimeout(10.Seconds());
			}

			[Fact]
			public async Task WhenGlobalTimeoutIsApplied_ShouldFail()
			{
				int count = 0;
				Other subject = new();

				async Task Act()
					=> await That(subject).Satisfies(_ => ++count > 42).Within(30.Seconds())
						.WithTimeout(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             satisfies _ => ++count > 42 within 0:30,
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
					=> await That(subject).Satisfies(_ => true).Within(1.Seconds())
						.CheckEvery(intervalSeconds.Seconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.OnlyIf(shouldThrow)
					.WithParamName("interval").And
					.WithMessage("The interval must be positive*").AsWildcard();
			}

			[Fact]
			public async Task WhenPredicateResultTurnsTrueLaterOn_ShouldSucceed()
			{
				int count = 0;
				Other subject = new();

				async Task Act()
					=> await That(subject).Satisfies(_ => ++count > 2).Within(5.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPredicateThrowsUntilItReturnsTrue_ShouldSucceed()
			{
				int count = 0;
				Other subject = new();

				bool ThrowingPredicate(Other _)
				{
					if (++count <= 2)
					{
						throw new InvalidOperationException("not yet");
					}

					return true;
				}

				async Task Act()
					=> await That(subject).Satisfies(ThrowingPredicate).Within(5.Seconds());

				await That(Act).DoesNotThrow()
					.Because("an exception is only a failed attempt, so the predicate is retried like any other failure");
			}

			[Fact]
			public async Task WhenPredicateThrows_ShouldRetryAndFailWithTheLastException()
			{
				int count = 0;
				InvalidOperationException lastException = new("predicate failed again");
				Other subject = new();

				bool ThrowingPredicate(Other _)
				{
					if (++count == 1)
					{
						throw new InvalidOperationException("predicate failed");
					}

					throw lastException;
				}

				async Task Act()
					=> await That(subject).Satisfies(ThrowingPredicate).Within(200.Milliseconds())
						.CheckEvery(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             satisfies ThrowingPredicate within 0:00.200,
					             but it did throw an InvalidOperationException:
					               predicate failed again
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(lastException));
				await That(count).IsGreaterThan(1)
					.Because("an exception is only a failed attempt, so the predicate is retried until the time runs out");
			}

			[Fact]
			public async Task WhenPredicateThrows_WhenNegated_ShouldRetryAndFailWithTheLastException()
			{
				int count = 0;
				InvalidOperationException lastException = new("predicate failed again");
				Other subject = new();

				bool ThrowingPredicate(Other _)
				{
					if (++count == 1)
					{
						throw new InvalidOperationException("predicate failed");
					}

					throw lastException;
				}

				async Task Act()
					=> await That(subject).DoesNotSatisfy(ThrowingPredicate).Within(200.Milliseconds())
						.CheckEvery(10.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not satisfy ThrowingPredicate within 0:00.200,
					             but it did throw an InvalidOperationException:
					               predicate failed again
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(lastException));
				await That(count).IsGreaterThan(1)
					.Because("an exception is only a failed attempt, so the predicate is retried until the time runs out");
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
					=> await That(subject).Satisfies(_ => true).Within(timeoutSeconds.Seconds());

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
					=> await That(subject).Satisfies(_ => ++count > 42).Within(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             satisfies _ => ++count > 42 within 0:00.050,
					             but it was ThatGeneric.Other {
					               Value = 0
					             }
					             """);
			}
		}
	}
}
