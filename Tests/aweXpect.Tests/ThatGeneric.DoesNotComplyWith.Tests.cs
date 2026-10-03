// ReSharper disable UnusedMember.Local

using System.Collections.Generic;
using System.Threading;
using aweXpect.Customization;

namespace aweXpect.Tests;

public sealed partial class ThatGeneric
{
	public sealed class DoesNotComplyWith
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenValueIsDifferent_ShouldSucceed()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(x => x.IsEqualTo("bar"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenValueIsEqual_ShouldFail()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(x => x.IsEqualTo("foo"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to "foo",
					             but it was "foo"
					             """);
			}
		}

		public sealed class CombinationTests
		{
			[Fact]
			public async Task NotAAndB_ShouldTranslateToNotAOrNotB()
			{
				bool subject = true;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(x => x.IsTrue().And.IsTrue());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not True or is not True,
					             but it was True
					             """);
			}

			[Fact]
			public async Task NotAAndBAndC_ShouldTranslateToNotAOrNotBOrNotC()
			{
				bool? subject = false;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(x => x.IsFalse().And.IsNotNull().And.IsNotTrue());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not False or is null or is True,
					             but it was False
					             """);
			}

			[Fact]
			public async Task NotAOrB_ShouldTranslateToNotAAndNotB()
			{
				bool subject = true;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(x => x.IsTrue().Or.IsTrue());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not True and is not True,
					             but it was True
					             """);
			}

			[Fact]
			public async Task NotAOrB_WhenOnlyOneBranchFails_ShouldRenderTheResultOfTheFailingBranch()
			{
				bool subject = true;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(x => x.IsTrue().Or.IsFalse());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not True and is not False,
					             but it was True
					             """);
			}
		}

		public sealed class ContextTests
		{
			[Fact]
			public async Task Contains_ShouldIncludeTheCollectionContext()
			{
				int[] subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Contains(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item equal to 1,
					             but it contained 1 at least once

					             Collection:
					             [1, 2, 3]
					             """);
			}

			[Fact]
			public async Task Contains_WithPredicate_ShouldIncludeTheCollectionContext()
			{
				int[] subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Contains(x => x == 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item matching x => x == 1,
					             but it contained it at least once

					             Collection:
					             [1, 2, 3]
					             """);
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task Contains_OnAsyncEnumerable_ShouldIncludeTheReceivedItems()
			{
				IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Contains(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item equal to 1,
					             but it contained 1 at least once

					             Collection:
					             [1, (… and maybe more)]
					             """);
			}

			[Fact]
			public async Task Contains_OnAsyncEnumerableWithPredicate_ShouldIncludeTheReceivedItems()
			{
				IAsyncEnumerable<int> subject = ThatAsyncEnumerable.ToAsyncEnumerable([1, 2, 3,]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Contains(x => x == 1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain an item matching x => x == 1,
					             but it contained it at least once

					             Collection:
					             [1, (… and maybe more)]
					             """);
			}
#endif

			[Fact]
			public async Task ContainsKey_ShouldIncludeTheDictionaryContext()
			{
				Dictionary<int, int> subject = new()
				{
					{ 1, 1 },
				};

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.ContainsKey(1));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain key 1,
					             but it did

					             Dictionary:
					             {[1] = 1}
					             """);
			}

			[Fact]
			public async Task DoesNotContainKey_ShouldIncludeTheDictionaryContext()
			{
				Dictionary<int, int> subject = new()
				{
					{ 1, 1 },
				};

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.DoesNotContainKey(2));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             contains key 2,
					             but it did not contain key 2

					             Dictionary:
					             {[1] = 1}
					             """);
			}

			[Fact]
			public async Task HasRecursiveInnerExceptions_ShouldIncludeTheInnerExceptions()
			{
				Exception subject = new("outer", new Exception("inner"));

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.HasRecursiveInnerExceptions(e => e.IsNotEmpty()));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have recursive inner exceptions that are not empty,
					             but it had 1 recursive inner exception

					             Collection:
					             [
					               Exception: inner
					             ]
					             """);
			}

			[Fact]
			public async Task Is_ShouldIncludeTheActualContext()
			{
				object subject = "s";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Is<string>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not of type string,
					             but it was string

					             Actual:
					             "s"
					             """);
			}

			[Fact]
			public async Task Is_WithType_ShouldIncludeTheActualContext()
			{
				object subject = "s";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Is(typeof(string)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not of type string,
					             but it was string

					             Actual:
					             "s"
					             """);
			}

			[Fact]
			public async Task IsExactly_ShouldIncludeTheActualContext()
			{
				object subject = "s";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsExactly<string>());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not exactly of type string,
					             but it was string

					             Actual:
					             "s"
					             """);
			}

			[Fact]
			public async Task IsExactly_WithType_ShouldIncludeTheActualContext()
			{
				object subject = "s";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsExactly(typeof(string)));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not exactly of type string,
					             but it was string

					             Actual:
					             "s"
					             """);
			}

			[Fact]
			public async Task StringContains_ShouldIncludeTheActualContext()
			{
				string subject = "a subject with more than twenty characters";

				async Task Act()
				{
					using (Customize.aweXpect.Formatting().MaximumStringLength.Set(20))
					{
						await That(subject).DoesNotComplyWith(it => it.Contains("more"));
					}
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not contain "more",
					             but it contained "more" once in "a subject with more …"

					             Actual:
					             a subject with more than twenty characters
					             """);
			}

			[Fact]
			public async Task StringEndsWith_ShouldIncludeTheExpectedContext()
			{
				string subject = "a subject with more than twenty characters";

				async Task Act()
				{
					using (Customize.aweXpect.Formatting().MaximumStringLength.Set(20))
					{
						await That(subject).DoesNotComplyWith(it => it.EndsWith("with more than twenty characters"));
					}
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not end with "with more than twent…",
					             but it was "a subject with more …"

					             Actual:
					             a subject with more than twenty characters

					             Expected:
					             with more than twenty characters
					             """);
			}

			[Fact]
			public async Task StringIsEqualTo_ShouldIncludeTheExpectedContext()
			{
				string subject = "a subject with more than twenty characters";

				async Task Act()
				{
					using (Customize.aweXpect.Formatting().MaximumStringLength.Set(20))
					{
						await That(subject).DoesNotComplyWith(it
							=> it.IsEqualTo("a subject with more than twenty characters"));
					}
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to "a subject with more …",
					             but it was "a subject with more …"

					             Actual:
					             a subject with more than twenty characters

					             Expected:
					             a subject with more than twenty characters
					             """);
			}

			[Fact]
			public async Task StringStartsWith_ShouldIncludeTheExpectedContext()
			{
				string subject = "a subject with more than twenty characters";

				async Task Act()
				{
					using (Customize.aweXpect.Formatting().MaximumStringLength.Set(20))
					{
						await That(subject).DoesNotComplyWith(it => it.StartsWith("a subject with more than"));
					}
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not start with "a subject with more …",
					             but it was "a subject with more …"

					             Actual:
					             a subject with more than twenty characters

					             Expected:
					             a subject with more than
					             """);
			}
		}

		public sealed class ExpectedValuesContextTests
		{
			[Fact]
			public async Task IsNotOneOf_ShouldTitleTheValuesAsExpected()
			{
				char subject = 'a';
				IEnumerable<char> expected = ['b', 'c',];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsNotOneOf(expected));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is one of expected,
					             but it was 'a'

					             Expected values:
					             ['b', 'c']
					             """);
			}

			[Fact]
			public async Task IsOneOf_ForChar_ShouldTitleTheValuesAsUnexpected()
			{
				char subject = 'a';
				IEnumerable<char> unexpected = ['a', 'b',];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(unexpected));

				await That(Act).Throws<XunitException>()
					.WithMessage(Message(subject, unexpected));
			}

			[Fact]
			public async Task IsOneOf_ForDateTime_ShouldTitleTheValuesAsUnexpected()
			{
				DateTime subject = new(2024, 1, 1);
				IEnumerable<DateTime> unexpected = [subject, subject.AddDays(1),];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(unexpected));

				await That(Act).Throws<XunitException>()
					.WithMessage(Message(subject, unexpected));
			}

			[Fact]
			public async Task IsOneOf_ForDateTimeOffset_ShouldTitleTheValuesAsUnexpected()
			{
				DateTimeOffset subject = new(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
				IEnumerable<DateTimeOffset> unexpected = [subject, subject.AddDays(1),];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(unexpected));

				await That(Act).Throws<XunitException>()
					.WithMessage(Message(subject, unexpected));
			}

			[Fact]
			public async Task IsOneOf_ForEnum_ShouldTitleTheValuesAsUnexpected()
			{
				DayOfWeek subject = DayOfWeek.Monday;
				IEnumerable<DayOfWeek> unexpected = [DayOfWeek.Monday, DayOfWeek.Tuesday,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(unexpected));

				await That(Act).Throws<XunitException>()
					.WithMessage(Message(subject, unexpected));
			}

			[Fact]
			public async Task IsOneOf_ForGuid_ShouldTitleTheValuesAsUnexpected()
			{
				Guid subject = Guid.NewGuid();
				IEnumerable<Guid> unexpected = [subject, Guid.NewGuid(),];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(unexpected));

				await That(Act).Throws<XunitException>()
					.WithMessage(Message(subject, unexpected));
			}

			[Fact]
			public async Task IsOneOf_ForNumber_ShouldTitleTheValuesAsUnexpected()
			{
				int subject = 1;
				IEnumerable<int> unexpected = [1, 2,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(unexpected));

				await That(Act).Throws<XunitException>()
					.WithMessage(Message(subject, unexpected));
			}

			[Fact]
			public async Task IsOneOf_ForObject_ShouldTitleTheValuesAsUnexpected()
			{
				object subject = 1;
				IEnumerable<object> unexpected = [1, 2,];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(unexpected));

				await That(Act).Throws<XunitException>()
					.WithMessage(Message(subject, unexpected));
			}

			[Fact]
			public async Task IsOneOf_ForString_ShouldTitleTheValuesAsUnexpected()
			{
				string subject = "a";
				IEnumerable<string> unexpected = ["a", "b",];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(unexpected));

				await That(Act).Throws<XunitException>()
					.WithMessage(Message(subject, unexpected));
			}

			[Fact]
			public async Task IsOneOf_ForTimeSpan_ShouldTitleTheValuesAsUnexpected()
			{
				TimeSpan subject = 1.Seconds();
				IEnumerable<TimeSpan> unexpected = [1.Seconds(), 2.Seconds(),];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(unexpected));

				await That(Act).Throws<XunitException>()
					.WithMessage(Message(subject, unexpected));
			}

			[Fact]
			public async Task IsOneOf_ForVersion_ShouldTitleTheValuesAsUnexpected()
			{
				Version subject = new(1, 2);
				IEnumerable<Version> unexpected = [new(1, 2), new(1, 3),];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(unexpected));

				await That(Act).Throws<XunitException>()
					.WithMessage(Message(subject, unexpected));
			}

#if NET8_0_OR_GREATER
			[Fact]
			public async Task IsOneOf_ForDateOnly_ShouldTitleTheValuesAsUnexpected()
			{
				DateOnly subject = new(2024, 1, 1);
				IEnumerable<DateOnly> unexpected = [subject, subject.AddDays(1),];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(unexpected));

				await That(Act).Throws<XunitException>()
					.WithMessage(Message(subject, unexpected));
			}

			[Fact]
			public async Task IsOneOf_ForTimeOnly_ShouldTitleTheValuesAsUnexpected()
			{
				TimeOnly subject = new(1, 2);
				IEnumerable<TimeOnly> unexpected = [subject, subject.AddHours(1),];

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.IsOneOf(unexpected));

				await That(Act).Throws<XunitException>()
					.WithMessage(Message(subject, unexpected));
			}
#endif

			private static string Message<T>(T subject, IEnumerable<T> unexpected)
				=> $"""
				    Expected that subject
				    is not one of unexpected,
				    but it was {Formatter.Format(subject)}

				    Unexpected values:
				    {Formatter.Format(unexpected)}
				    """;
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
					=> await That(subject).DoesNotComplyWith(x => x.IsEqualTo(1)).Within(30.Seconds())
						.WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 1 within 0:30,
					             but it could not be verified, because the evaluation was already canceled
					             """).WithTimeout(10.Seconds());
			}

			[Fact]
			public async Task WhenGlobalTimeoutIsApplied_ShouldFail()
			{
				MyChangingClass subject = new(42);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(x => x.IsEquivalentTo(new { HasWaitedEnough = false, }))
						.Within(30.Seconds()).WithTimeout(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equivalent to new { HasWaitedEnough = false, } within 0:30,
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
					=> await That(subject).DoesNotComplyWith(x => x.IsNull()).Within(1.Seconds())
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
					=> await That(subject).DoesNotComplyWith(x => x.IsEquivalentTo(new
					{
						HasWaitedEnough = false,
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
					=> await That(subject).DoesNotComplyWith(x => x.IsEqualTo(1))
						.Within(System.Threading.Timeout.InfiniteTimeSpan)
						.WithCancellation(cts.Token);

				await That(Act).Throws<InconclusiveException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 1,
					             but it could not be verified, because the evaluation was already canceled
					             """).WithTimeout(10.Seconds())
					.Because("an infinite timeout imposes no limit, so only the cancellation ends the retries");
			}

			[Fact]
			public async Task WhenTimeoutIsInfinite_ShouldRetryUntilTheExpectationsAreNoLongerMet()
			{
				MyChangingClass subject = new(2);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(x => x.IsEquivalentTo(new
						{
							HasWaitedEnough = false,
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
					=> await That(subject).DoesNotComplyWith(x => x.IsNull()).Within(timeoutSeconds.Seconds());

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
					=> await That(subject).DoesNotComplyWith(x => x.IsEquivalentTo(new { HasWaitedEnough = false, }))
						.Within(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equivalent to new { HasWaitedEnough = false, } within 0:00.050,
					             but it was ThatGeneric.DoesNotComplyWith.WithinTests.MyChangingClass {
					                 HasWaitedEnough = False
					               }, which is considered equivalent
					             
					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			[Fact]
			public async Task WhenTimeoutIsZero_ShouldMentionTheTimeout()
			{
				int subject = 1;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(x => x.IsEqualTo(1)).Within(TimeSpan.Zero);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equal to 1 within 0:00,
					             but it was 1
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
