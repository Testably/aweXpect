// ReSharper disable UnusedMember.Local

using System.Threading;

namespace aweXpect.Tests;

public sealed partial class ThatGeneric
{
	public sealed class CompliesWith
	{
		public sealed class Tests
		{
			[Fact]
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

			[Fact]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				int subject = 1;

				async Task Act()
					=> await That(subject).CompliesWith(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenInnerExpectationHasReason_ShouldAppendItAfterTheInnerExpectation()
			{
				int subject = 1;

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEqualTo(2).Because("of reasons").Or.IsEqualTo(3));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2 or is equal to 3, because of reasons,
					             but it was 1, which differs by -1 and it was 1, which differs by -2
					             """);
			}

			[Theory]
			[InlineData(1, true)]
			[InlineData(2, false)]
			public async Task WhenValueIsDifferent_ShouldFail(int expectedValue, bool expectSuccess)
			{
				Other subject = new()
				{
					Value = 1,
				};

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEquivalentTo(new
					{
						Value = expectedValue,
					}));

				await That(Act).Throws<XunitException>()
					.OnlyIf(!expectSuccess)
					.WithMessage("""
					             Expected that subject
					             is equivalent to {
					                 Value = 2
					               },
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
			[Fact]
			public async Task WhenCancellationIsRequestedWhileRetrying_ShouldBeInconclusive()
			{
				int subject = 1;
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEqualTo(2)).Within(30.Seconds())
						.WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2 within 0:30,
					             but it could not be verified, because it was already canceled
					             """).WithTimeout(10.Seconds());
			}

			[Fact]
			public async Task WhenGlobalTimeoutIsApplied_ShouldFail()
			{
				MyChangingClass subject = new(42);

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEquivalentTo(new
					{
						HasWaitedEnough = true,
					})).Within(30.Seconds()).WithTimeout(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to {
					                 HasWaitedEnough = True
					               } within 0:30,
					             but it did not finish within 0:00.050

					             Equivalency options:
					              - include public fields and properties
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
					=> await That(subject).CompliesWith(x => x.IsNotNull()).Within(1.Seconds())
						.CheckEvery(intervalSeconds.Seconds());

				await That(Act).Throws<ArgumentOutOfRangeException>()
					.OnlyIf(shouldThrow)
					.WithParamName("interval").And
					.WithMessage("The interval must be positive*").AsWildcard();
			}

			[Fact]
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

			[Fact]
			public async Task WhenTimeoutIsInfinite_ShouldNotMentionTheTimeout()
			{
				int subject = 1;
				using CancellationTokenSource cts = new();
				cts.CancelAfter(50.Milliseconds());

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEqualTo(2))
						.Within(System.Threading.Timeout.InfiniteTimeSpan)
						.WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that subject
					             is equal to 2,
					             but it could not be verified, because it was already canceled
					             """).WithTimeout(10.Seconds())
					.Because("an infinite timeout imposes no limit, so only the cancellation ends the retries");
			}

			[Fact]
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

			[Theory]
			[InlineData(1, false)]
			[InlineData(0, false)]
			[InlineData(-1, true)]
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

			[Fact]
			public async Task WhenTimeoutIsTooShort_ShouldFail()
			{
				MyChangingClass subject = new(42);

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEquivalentTo(new
					{
						HasWaitedEnough = true,
					})).Within(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to {
					                 HasWaitedEnough = True
					               } within 0:00.050,
					             but it was not:
					               Property HasWaitedEnough differed:
					                   Actual: False
					                 Expected: True

					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Fact]
			public async Task WhenTimeoutIsZero_ShouldMentionTheTimeout()
			{
				int subject = 1;

				async Task Act()
					=> await That(subject).CompliesWith(x => x.IsEqualTo(2)).Within(TimeSpan.Zero);

				await That(Act).Throws<XunitException>()
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
	}
}
