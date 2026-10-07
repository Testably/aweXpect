// ReSharper disable UnusedMember.Local

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;
using aweXpect.Customization;

namespace aweXpect.Tests;

public sealed partial class ThatGeneric
{
	public sealed class CompliesWith
	{
		public sealed class Tests
		{
			[Test]
			public async Task AllowsNestedIs()
			{
				Base subject = new Derived
				{
					Name = "foo",
				};

				async Task Act()
					=> await That(subject).CompliesWith(it => it.Is<Derived>()
						.Whose(d => d.Name, it => it.IsEqualTo("foo")));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				int subject = 1;

				async Task Act()
					=> await That(subject).CompliesWith(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenInnerExpectationHasAnotherValueType_ShouldReturnTheSubject()
			{
				int[] subject = [1,];

				int[]? result = await That(subject).CompliesWith(it => it.HasSingle());

				await That(result).IsSameAs(subject)
					.Because("the value of the inner expectation (the single item) is no int[]");
			}

			[Test]
			public async Task WhenInnerExpectationHasAsyncReason_AndAnotherExpectationFails_ShouldAppendIt()
			{
				int subject = 1;

				async Task Act()
					=> await That(subject)
						.CompliesWith(x => x.IsEqualTo(1).Because(Task.FromResult<string?>("of reasons")))
						.And.IsEqualTo(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 1, because of reasons and is equal to 2,
					             but it was 1, which differs by -1
					             """)
					.Because("a reason that must be awaited is shown like a string reason, although its expectation is met");
			}

			[Test]
			public async Task WhenInnerExpectationHasAsyncReason_InASkippedOrOperand_ShouldNotAwaitTheReason()
			{
				bool reasonWasResolved = false;
				Task<string?> becauseTask = Task.Delay(TimeSpan.FromSeconds(30)).ContinueWith(_ =>
				{
					reasonWasResolved = true;
					return (string?)"of reasons";
				});
				int subject = 1;

				await That(subject).IsEqualTo(1).Or.CompliesWith(x => x.IsEqualTo(2).Because(becauseTask));

				await That(reasonWasResolved).IsFalse()
					.Because("a met expectation never builds a failure message, so it must not wait for the reason");
			}

			[Test]
			public async Task WhenInnerExpectationHasAsyncReason_InASkippedOrOperand_WhenNegated_ShouldAppendIt()
			{
				Task<string?> becauseTask = Task.Delay(TimeSpan.FromMilliseconds(50))
					.ContinueWith(_ => (string?)"of reasons");
				int subject = 1;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it
						.IsEqualTo(1).Or
						.CompliesWith(x => x.IsEqualTo(2).Because(becauseTask)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 1 and is not equal to 2, because of reasons,
					             but it was 1
					             """)
					.Because("the text of the skipped operand is shown, so its reason is awaited for the failure");
			}

			[Test]
			public async Task WhenInnerExpectationHasReason_ShouldAppendItAfterTheInnerExpectation()
			{
				int subject = 1;

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEqualTo(2).Because("of reasons").Or.IsEqualTo(3));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2 or is equal to 3, because of reasons,
					             but it was 1, which differs by -1 and was 1, which differs by -2
					             """);
			}

			[Test]
			public async Task WhenInnerExpectationHasValueOfTheSubjectType_ShouldReturnTheSubject()
			{
				object[] inner = [1,];
				object[] subject = [inner,];

				object[]? result = await That(subject).CompliesWith(it => it.HasSingle());

				await That(result).IsSameAs(subject)
					.Because("the single item has the subject's type, but the result of CompliesWith is the subject");
			}

			[Test]
			public async Task WhenUsedForTheItemsOfAnEmptyCollection_ShouldDescribeTheExpectations()
			{
				int[] subject = [];

				async Task Act()
					=> await That(subject).HasItemThat(item => item.CompliesWith(it => it.IsEqualTo(1)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an item that is equal to 1,
					             but it had no item

					             Collection:
					             []
					             """)
					.Because("without any item the expectations are described without being evaluated");
			}

			[Test]
			[Arguments(1, true)]
			[Arguments(2, false)]
			public async Task WhenValueIsDifferent_ShouldFail(int expectedValue, bool expectSuccess)
			{
				Other subject = new()
				{
					Value = 1,
				};

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEquivalentTo(new { Value = expectedValue, }));

				await That(Act).Throws<FailException>()
					.OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             is equivalent to new { Value = expectedValue, },
					             but it was not:
					               Property Value differed:
					                   Actual: 1
					                 Expected: 2

					             Equivalency options:
					              - include public fields and properties
					             """);
			}
		}

		public sealed class WithinTests
		{
			[Test]
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
					=> await That(subject).CompliesWith(x => x.Satisfies(StartsTheClock))
						.Within(50.Milliseconds()).CheckEvery(1.Hours()).WithCancellation(cts.Token);

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenCancellationIsRequestedWhileRetrying_ShouldBeInconclusive()
			{
				int subject = 1;
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEqualTo(2)).Within(30.Seconds())
						.WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2 within 0:30,
					             but it could not be verified, because the evaluation was already canceled
					             """).WithTimeout(10.Seconds());
			}

			[Test]
			public async Task WhenDefaultIntervalIsNotPositive_ShouldCheckWithoutWaiting()
			{
				int count = 0;
				Other subject = new();

				async Task Act()
					=> await That(subject).CompliesWith(x => x.Satisfies(_ => ++count > 3)).Within(5.Seconds());

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

			[Test]
			public async Task WhenGlobalTimeoutEqualsTheTimeout_ShouldFailWithTheResult()
			{
				int subject = 1;

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEqualTo(2)).Within(200.Milliseconds())
						.CheckEvery(1.Hours()).WithTimeout(200.Milliseconds())
						.WithTimeSystem(new VirtualTimeSystem());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2 within 0:00.200,
					             but it was 1, which differs by -1
					             """)
					.Because("the timeout elapsed at the same time as the global timeout");
			}

			[Test]
			public async Task WhenGlobalTimeoutIsApplied_ShouldFail()
			{
				MyChangingClass subject = new(42);

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEquivalentTo(new { HasWaitedEnough = true, }))
						.Within(30.Seconds()).WithTimeout(50.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to new { HasWaitedEnough = true, } within 0:30,
					             but it did not finish within 0:00.050
					             """).And
					.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."));
			}

			[Test]
			public async Task WhenInnerExpectationHasReason_ShouldAppendItAfterTheTimeout()
			{
				int subject = 1;

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEqualTo(2).Because("of reasons"))
						.Within(0.Seconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2 within 0:00, because of reasons,
					             but it was 1, which differs by -1
					             """);
			}

			[Test]
			public async Task WhenInnerExpectationHasValueOfTheSubjectType_ShouldReturnTheSubject()
			{
				object[] inner = [1,];
				object[] subject = [inner,];

				object[]? result = await That(subject).CompliesWith(it => it.HasSingle()).Within(1.Seconds());

				await That(result).IsSameAs(subject)
					.Because("the single item has the subject's type, but the result of CompliesWith is the subject");
			}

			[Test]
			public async Task WhenIntervalExceedsTheTimeout_ShouldCheckAgainAtTheTimeout()
			{
				int count = 0;
				Other subject = new();

				async Task Act()
					=> await That(subject).CompliesWith(x => x.Satisfies(_ => ++count > 1))
						.Within(200.Milliseconds()).CheckEvery(1.Hours());

				await That(Act).DoesNotThrow().WithTimeout(10.Seconds())
					.Because("the wait is shortened to the remaining time, so the last check is made at the timeout");
				await That(count).IsEqualTo(2);
			}

			[Test]
			public async Task WhenIntervalExceedsTheTimeout_ShouldNotCountASuccessAfterTheTimeout()
			{
				int count = 0;
				Other subject = new();

				async Task Act()
					=> await That(subject).CompliesWith(x => x.Satisfies(_ => ++count > 2))
						.Within(100.Milliseconds()).CheckEvery(1.Hours())
						.WithTimeSystem(new VirtualTimeSystem());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             satisfies _ => ++count > 2 within 0:00.100,
					             but it was ThatGeneric.Other {
					                 Value = 0
					               }
					             """).WithTimeout(30.Seconds())
					.Because("no check is made after the timeout");
				await That(count).IsEqualTo(2)
					.Because("the timeout only allows the first check and the check at its end");
			}

			[Test]
			public async Task WhenIntervalExceedsTheTimerLimit_ShouldWaitUntilCanceled()
			{
				int subject = 1;
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEqualTo(2))
						.Within(System.Threading.Timeout.InfiniteTimeSpan).CheckEvery(100.Days())
						.WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2,
					             but it could not be verified, because the evaluation was already canceled
					             """).WithTimeout(10.Seconds())
					.Because("an interval beyond the limit of a timer is capped instead of rejected");
			}

			[Test]
			[Arguments(1, false)]
			[Arguments(0, true)]
			[Arguments(-1, true)]
			public async Task WhenIntervalIsNotPositive_ShouldThrowArgumentOutOfRangeException(int intervalSeconds,
				bool shouldThrow)
			{
				Other subject = new();

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsNotNull()).Within(1.Seconds())
						.CheckEvery(intervalSeconds.Seconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.OnlyIf(shouldThrow)
					.WithParamName("interval").And
					.WithMessage("The interval must be positive*").AsWildcard();
			}

			[Test]
			public async Task WhenPredicateResultTurnsTrueLaterOn_ShouldSucceed()
			{
				MyChangingClass subject = new(2);

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEquivalentTo(new
					{
						HasWaitedEnough = true,
					})).Within(5.Seconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task
				WhenTestCancellationTimeoutIsShorterAndWithTimeoutIsLonger_ShouldFailWithTheTestCancellationTimeout()
			{
				int subject = 1;
				Exception? exception;
				using (IDisposable _ = Customize.aweXpect.Settings().TestCancellation
					       .Set(TestCancellation.FromTimeout(300.Milliseconds())))
				{
					async Task Act()
						=> await That(subject).CompliesWith(x => x.IsEqualTo(2)).Within(30.Seconds())
							.WithTimeout(60.Seconds())
							.WithTimeSystem(new VirtualTimeSystem());

					exception = await Catch.ExceptionAsync(Act);
				}

				await That(exception).IsExactly<FailException>().And
					.HasMessage("""
					            Expected that subject
					            is equal to 2 within 0:30,
					            but it did not finish within 0:00.300
					            """).And
					.HasInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.300."))
					.Because("the effective timeout is the tighter of WithTimeout and TestCancellation");
			}

			[Test]
			public async Task WhenTestCancellationTimeoutIsShorter_ShouldFailWithTheTestCancellationTimeout()
			{
				int subject = 1;
				Exception? exception;
				using (IDisposable _ = Customize.aweXpect.Settings().TestCancellation
					       .Set(TestCancellation.FromTimeout(300.Milliseconds())))
				{
					async Task Act()
						=> await That(subject).CompliesWith(x => x.IsEqualTo(2)).Within(30.Seconds())
							.WithTimeSystem(new VirtualTimeSystem());

					exception = await Catch.ExceptionAsync(Act);
				}

				await That(exception).IsExactly<FailException>().And
					.HasMessage("""
					            Expected that subject
					            is equal to 2 within 0:30,
					            but it did not finish within 0:00.300
					            """).And
					.HasInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.300."))
					.Because("a TestCancellation timeout that is shorter than Within ends the checks");
			}

			[Test]
			public async Task WhenTheSourceThrowsAtFirst_ShouldReadItAgain()
			{
				int enumerations = 0;

				IEnumerable<int> Items()
				{
					if (++enumerations == 1)
					{
						throw new MyException("not yet");
					}

					yield return 1;
				}

				IEnumerable<int> subject = Items();

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsNotEmpty())
						.Within(30.Seconds()).CheckEvery(1.Milliseconds());

				await That(Act).DoesNotThrow()
					.Because("an exception of the source must be retried instead of being replayed in every check");
				await That(enumerations).IsEqualTo(2);
			}

			[Test]
			public async Task WhenTheSubjectGrows_ShouldSeeTheNewItems()
			{
				int enumerations = 0;

				IEnumerable<int> Items()
				{
					if (++enumerations >= 3)
					{
						yield return 1;
					}
				}

				IEnumerable<int> subject = Items();

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsNotEmpty())
						.Within(30.Seconds()).CheckEvery(1.Milliseconds());

				await That(Act).DoesNotThrow()
					.Because("each check must read the lazy subject again instead of replaying the first snapshot");
				await That(enumerations).IsEqualTo(3);
			}

			[Test]
			public async Task WhenTimeoutIsInfinite_ShouldNotMentionTheTimeout()
			{
				int subject = 1;
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEqualTo(2))
						.Within(System.Threading.Timeout.InfiniteTimeSpan)
						.WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveTestException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2,
					             but it could not be verified, because the evaluation was already canceled
					             """).WithTimeout(10.Seconds())
					.Because("an infinite timeout imposes no limit, so only the cancellation ends the retries");
			}

			[Test]
			public async Task WhenTimeoutIsInfinite_ShouldRetryUntilTheExpectationsAreMet()
			{
				MyChangingClass subject = new(2);

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEquivalentTo(new
						{
							HasWaitedEnough = true,
						})).Within(System.Threading.Timeout.InfiniteTimeSpan)
						.CheckEvery(10.Milliseconds());

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments(1, false)]
			[Arguments(0, false)]
			[Arguments(-1, true)]
			public async Task WhenTimeoutIsNegative_ShouldThrowArgumentOutOfRangeException(int timeoutSeconds,
				bool shouldThrow)
			{
				Other subject = new();

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsNotNull()).Within(timeoutSeconds.Seconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.OnlyIf(shouldThrow)
					.WithParamName("timeout").And
					.WithMessage("The timeout must not be negative*").AsWildcard();
			}

			[Test]
			public async Task WhenTimeoutIsTooShort_ShouldFail()
			{
				MyChangingClass subject = new(42);

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEquivalentTo(new { HasWaitedEnough = true, }))
						.Within(50.Milliseconds());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to new { HasWaitedEnough = true, } within 0:00.050,
					             but it was not:
					               Property HasWaitedEnough differed:
					                   Actual: False
					                 Expected: True

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Test]
			public async Task WhenTimeoutIsZero_ShouldMentionTheTimeout()
			{
				int subject = 1;

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEqualTo(2)).Within(TimeSpan.Zero);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2 within 0:00,
					             but it was 1, which differs by -1
					             """)
					.Because("an explicit timeout is named like on a signaler, even when it is zero");
			}

			private sealed class MyChangingClass(int numberOfChanges)
			{
				private int _iterations;
				public bool HasWaitedEnough => _iterations++ >= numberOfChanges;
			}
		}

		public sealed class NegatedTests
		{
			[Test]
			public async Task WhenExpectationsAreMet_ShouldFail()
			{
				int subject = 1;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.CompliesWith(x => x.IsEqualTo(1)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 1,
					             but it was 1
					             """);
			}

			[Test]
			public async Task WhenExpectationsAreNotMet_ShouldSucceed()
			{
				int subject = 1;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.CompliesWith(x => x.IsEqualTo(2)));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
