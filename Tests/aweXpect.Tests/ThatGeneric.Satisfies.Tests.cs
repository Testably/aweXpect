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
			public async Task WhenPredicateCancelsTheEvaluation_ShouldNotReportAFailedExpectation()
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

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Error evaluating*constraint*evaluation canceled").AsWildcard().And
					.WithInner<OperationCanceledException>(inner => inner.HasMessage("evaluation canceled"))
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
					.WithMessage("The predicate cannot be null.").AsPrefix();
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
			public async Task WhenPredicateThrows_ShouldFailWithoutRetrying()
			{
				int count = 0;
				Other subject = new();

				bool ThrowingPredicate(Other _)
				{
					count++;
					throw new InvalidOperationException("predicate failed");
				}

				async Task Act()
					=> await That(subject).Satisfies(ThrowingPredicate).Within(30.Seconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             satisfies ThrowingPredicate within 0:30,
					             but it did throw an InvalidOperationException:
					               predicate failed
					             """);
				await That(count).IsEqualTo(1)
					.Because("a predicate that throws cannot turn true later on, so retrying it is pointless");
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
