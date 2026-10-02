using System.Diagnostics;
using System.Threading;
using aweXpect.Customization;

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
					             but it could not be verified, because the evaluation was already canceled
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
					             but the predicate did throw a NullReferenceException:
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
					             but the predicate did throw an InvalidOperationException:
					               predicate failed
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(exception));
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
					             but the predicate did throw an InvalidOperationException:
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
					             but the predicate did throw an OperationCanceledException:
					               nothing was canceled
					             """)
					.Because("only a cancellation that was actually requested may abort the evaluation");
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
			public async Task WhenCanceledShortlyBeforeTheTimeout_ShouldFailWithTheResult()
			{
				Other subject = new();
				using CancellationTokenSource cts = new();
				using ManualResetEventSlim firstCheck = new();
				Task cancellation = Task.Run(() =>
				{
					firstCheck.Wait(10.Seconds());
					Stopwatch stopwatch = Stopwatch.StartNew();
					while (stopwatch.Elapsed < 49.Milliseconds())
					{
						Thread.SpinWait(10);
					}

					cts.Cancel();
				});

				bool StartsTheClock(Other _)
				{
					firstCheck.Set();
					return false;
				}

				async Task Act()
					=> await That(subject).Satisfies(StartsTheClock).Within(50.Milliseconds())
						.CheckEvery(1.Hours()).WithCancellation(cts.Token);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             satisfies StartsTheClock within 0:00.050,
					             but it was ThatGeneric.Other {
					               Value = 0
					             }
					             """)
					.Because("a cancellation at about the timeout must not hide the result of the last check");
				await cancellation;
			}

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
					             but it could not be verified, because the evaluation was already canceled
					             """).WithTimeout(10.Seconds());
			}

			[Fact]
			public async Task WhenCancellationIsRequestedWhileRetryingWithALongerGlobalTimeout_ShouldBeInconclusive()
			{
				Other subject = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act()
					=> await That(subject).Satisfies(_ => false).Within(30.Seconds())
						.WithTimeout(60.Seconds()).WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that subject
					             satisfies _ => false within 0:30,
					             but it could not be verified, because the evaluation was already canceled
					             """).WithTimeout(10.Seconds())
					.Because("only a timeout, not the cancellation by the caller, leaves the decision to the last check");
			}

			[Fact]
			public async Task WhenDefaultIntervalIsNotPositive_ShouldCheckWithoutWaiting()
			{
				int count = 0;
				Other subject = new();

				async Task Act()
					=> await That(subject).Satisfies(_ => ++count > 3).Within(5.Seconds());

				using (((IAwexpectCustomization)Customize.aweXpect).Set("aweXpect.Settings.DefaultCheckInterval",
					       TimeSpan.FromMilliseconds(-5)))
				{
					await That(Customize.aweXpect.Settings().DefaultCheckInterval.Get())
						.IsEqualTo(TimeSpan.FromMilliseconds(-5))
						.Because("the interval must be stored under the key that the setting reads");
					await That(Act).DoesNotThrow().WithTimeout(10.Seconds())
						.Because("an interval that bypassed the validation is treated like Eventually() treats it");
				}

				await That(count).IsEqualTo(4);
			}

			[Fact]
			public async Task WhenGlobalTimeoutEqualsTheTimeout_ShouldFailWithTheResult()
			{
				Other subject = new();

				async Task Act()
					=> await That(subject).Satisfies(_ => false).Within(200.Milliseconds()).CheckEvery(1.Hours())
						.WithTimeout(200.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             satisfies _ => false within 0:00.200,
					             but it was ThatGeneric.Other {
					               Value = 0
					             }
					             """)
					.Because("the timeout elapsed at the same time as the global timeout");
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

			[Fact]
			public async Task WhenIntervalExceedsTheTimeout_ShouldCheckAgainAtTheTimeout()
			{
				int count = 0;
				Other subject = new();

				async Task Act()
					=> await That(subject).Satisfies(_ => ++count > 1).Within(200.Milliseconds())
						.CheckEvery(1.Hours());

				await That(Act).DoesNotThrow().WithTimeout(10.Seconds())
					.Because("the wait is shortened to the remaining time, so the last check is made at the timeout");
				await That(count).IsEqualTo(2);
			}

			[Fact]
			public async Task WhenIntervalExceedsTheTimeout_ShouldNotCountASuccessAfterTheTimeout()
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				Other subject = new();

				async Task Act()
					=> await That(subject).Satisfies(_ => stopwatch.Elapsed >= 3.Seconds()).Within(100.Milliseconds())
						.CheckEvery(6.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             satisfies _ => stopwatch.Elapsed >= 3.Seconds() within 0:00.100,
					             but it was ThatGeneric.Other {
					               Value = 0
					             }
					             """)
					.Because("no check is made after the timeout");
			}

			[Fact]
			public async Task WhenIntervalExceedsTheTimerLimit_ShouldWaitUntilCanceled()
			{
				Other subject = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act()
					=> await That(subject).Satisfies(_ => false).Within(System.Threading.Timeout.InfiniteTimeSpan)
						.CheckEvery(100.Days())
						.WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that subject
					             satisfies _ => false,
					             but it could not be verified, because the evaluation was already canceled
					             """).WithTimeout(10.Seconds())
					.Because("an interval beyond the limit of a timer is capped instead of rejected");
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
					             but the predicate did throw an InvalidOperationException:
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
					             but the predicate did throw an InvalidOperationException:
					               predicate failed again
					             """).And
					.Whose(e => e.InnerException, i => i.IsSameAs(lastException));
				await That(count).IsGreaterThan(1)
					.Because("an exception is only a failed attempt, so the predicate is retried until the time runs out");
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
			public async Task
				WhenTestCancellationTimeoutIsShorterAndWithTimeoutIsLonger_ShouldFailWithTheTestCancellationTimeout()
			{
				Other subject = new();
				Exception? exception;
				using (IDisposable _ = Customize.aweXpect.Settings().TestCancellation
					       .Set(TestCancellation.FromTimeout(300.Milliseconds())))
				{
					async Task Act()
						=> await That(subject).Satisfies(_ => false).Within(2.Seconds()).WithTimeout(10.Seconds());

					exception = await Record.ExceptionAsync(Act);
				}

				await That(exception).IsExactly<XunitException>().And
					.HasMessage("""
					            Expected that subject
					            satisfies _ => false within 0:02,
					            but it did not finish within 0:00.300
					            """).And
					.HasInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.300."))
					.Because("the effective timeout is the tighter of WithTimeout and TestCancellation");
			}

			[Fact]
			public async Task WhenTestCancellationTimeoutIsShorter_ShouldFailWithTheTestCancellationTimeout()
			{
				Other subject = new();
				Exception? exception;
				using (IDisposable _ = Customize.aweXpect.Settings().TestCancellation
					       .Set(TestCancellation.FromTimeout(300.Milliseconds())))
				{
					async Task Act()
						=> await That(subject).Satisfies(_ => false).Within(2.Seconds());

					exception = await Record.ExceptionAsync(Act);
				}

				await That(exception).IsExactly<XunitException>().And
					.HasMessage("""
					            Expected that subject
					            satisfies _ => false within 0:02,
					            but it did not finish within 0:00.300
					            """).And
					.HasInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.300."))
					.Because("a TestCancellation timeout that is shorter than Within ends the checks");
			}

			[Fact]
			public async Task WhenTimeoutIsInfinite_ShouldNotMentionTheTimeout()
			{
				Other subject = new();
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act()
					=> await That(subject).Satisfies(_ => false).Within(System.Threading.Timeout.InfiniteTimeSpan)
						.WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that subject
					             satisfies _ => false,
					             but it could not be verified, because the evaluation was already canceled
					             """).WithTimeout(10.Seconds())
					.Because("an infinite timeout imposes no limit, so only the cancellation ends the retries");
			}

			[Fact]
			public async Task WhenTimeoutIsInfinite_ShouldRetryUntilThePredicateIsSatisfied()
			{
				int count = 0;
				Other subject = new();

				async Task Act()
					=> await That(subject).Satisfies(_ => ++count > 2).Within(System.Threading.Timeout.InfiniteTimeSpan)
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
