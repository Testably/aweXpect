// ReSharper disable UnusedMember.Local

namespace aweXpect.Tests;

public sealed partial class ThatGeneric
{
	public sealed class DoesNotComplyWith
	{
		public sealed class Tests
		{
			[Theory]
			[InlineData("foo", false)]
			[InlineData("bar", true)]
			public async Task WhenValueIsDifferent_ShouldSucceed(string expectedValue, bool expectSuccess)
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(x => x.IsEqualTo(expectedValue));

				await That(Act).Throws<XunitException>()
					.OnlyIf(!expectSuccess)
					.WithMessage($"""
					              Expected that subject
					              is not equal to "{expectedValue}",
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

			[Fact]
			public async Task NotAAndBAndC_ShouldTranslateToNotAOrNotBOrNotC()
			{
				bool? subject = false;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(x => x.IsFalse().And.IsNotNull().And.IsNotTrue());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not False or is <null> or is True,
					             but it was False
					             """);
			}
		}

		public sealed class WithinTests
		{
			[Fact]
			public async Task WhenGlobalTimeoutIsApplied_ShouldFail()
			{
				MyChangingClass subject = new(42);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(x => x.IsEquivalentTo(new
					{
						HasWaitedEnough = false,
					})).Within(30.Seconds()).WithTimeout(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equivalent to { HasWaitedEnough = False } within 0:30,
					             but it was considered equivalent for ThatGeneric.DoesNotComplyWith.WithinTests.MyChangingClass {
					                 HasWaitedEnough = False
					               }
					             
					             Equivalency options:
					              - include public fields and properties
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
					=> await That(subject).DoesNotComplyWith(x => x.IsEquivalentTo(new
					{
						HasWaitedEnough = false,
					})).Within(50.Milliseconds());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not equivalent to { HasWaitedEnough = False } within 0:00.050,
					             but it was considered equivalent for ThatGeneric.DoesNotComplyWith.WithinTests.MyChangingClass {
					                 HasWaitedEnough = False
					               }
					             
					             Equivalency options:
					              - include public fields and properties
					             """);
			}

			private sealed class MyChangingClass(int numberOfChanges)
			{
				private int _iterations;
				public bool HasWaitedEnough => _iterations++ >= numberOfChanges;
			}
		}
	}
}
