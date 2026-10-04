using aweXpect.Chronology;

namespace aweXpect.Core.Tests.Delegates;

public sealed partial class ThatDelegateTests
{
	public sealed class ExecutesInTests
	{
		[Fact]
		public async Task AtLeast_WhenMinimumIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).ExecutesIn().AtLeast(-1.Seconds());

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("minimum").And
				.WithMessage("The minimum must not be negative.").AsPrefix()
				.Because("an execution can never take less than no time at all");
		}

		[Fact]
		public async Task AtMost_WhenMaximumExceedsTheTimerRange_ShouldSucceed()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).ExecutesIn().AtMost(60.Days());

			await That(Act).DoesNotThrow()
				.Because("a duration beyond the range of the cancellation timer is still a valid upper bound");
		}

		[Fact]
		public async Task AtMost_WhenMaximumIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).ExecutesIn().AtMost(-1.Seconds());

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must not be negative.").AsPrefix()
				.Because("an execution can never take less than no time at all");
		}

		[Fact]
		public async Task Between_WhenMaximumExceedsTheTimerRange_ShouldFailWithTheGivenDurations()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).ExecutesIn().Between(60.Days()).And(61.Days());

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that @delegate
				             executes in between 60.00:00:00 and 61.00:00:00,
				             but it took *
				             """).AsWildcard()
				.Because("only the cancellation timer is limited, not the durations that are compared and reported");
		}

		[Fact]
		public async Task Between_WhenMaximumIsBelowMinimum_ShouldThrowArgumentOutOfRangeException()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).ExecutesIn().Between(2.Seconds()).And(1.Seconds());

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must be greater than or equal to the minimum.").AsPrefix();
		}

		[Fact]
		public async Task Between_WhenMaximumIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).ExecutesIn().Between(TimeSpan.Zero).And(-1.Seconds());

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("maximum").And
				.WithMessage("The maximum must not be negative.").AsPrefix();
		}

		[Fact]
		public async Task Between_WhenMinimumIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).ExecutesIn().Between(-1.Seconds()).And(1.Seconds());

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("minimum").And
				.WithMessage("The minimum must not be negative.").AsPrefix();
		}

		[Fact]
		public async Task WithExpected_WhenExpectedIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).ExecutesIn(-1.Seconds()).Within(1.Seconds());

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("expected").And
				.WithMessage("The expected duration must not be negative.").AsPrefix();
		}

		[Fact]
		public async Task Within_WhenExpectedIsMaxValue_ShouldFailWithoutOverflow()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).ExecutesIn(TimeSpan.MaxValue).Within(TimeSpan.FromTicks(1));

			await That(Act).Throws<XunitException>()
				.WithMessage("""
				             Expected that @delegate
				             executes in approximately TimeSpan.MaxValue ± 0:00.0000001,
				             but it took only *
				             """).AsWildcard()
				.Because("the upper bound saturates at the maximum duration instead of overflowing");
		}

		[Fact]
		public async Task Within_WhenToleranceIsMaxValue_ShouldSucceed()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).ExecutesIn(1.Seconds()).Within(TimeSpan.MaxValue);

			await That(Act).DoesNotThrow()
				.Because("any execution time is within an unlimited tolerance");
		}

		[Fact]
		public async Task Within_WhenToleranceIsNegative_ShouldThrowArgumentOutOfRangeException()
		{
			Action @delegate = () => { };

			async Task Act()
				=> await That(@delegate).ExecutesIn(50.Milliseconds()).Within(-1.Milliseconds());

			await That(Act).Throws<ArgumentOutOfRangeException>()
				.WithParamName("tolerance").And
				.WithMessage("The tolerance must not be negative.").AsPrefix().And
				.Whose(e => e.ActualValue, v => v.IsNull())
				.Because("the tolerance is rejected like every other tolerance");
		}

		[Fact]
		public async Task Within_WithValue_WhenToleranceIsMaxValue_ShouldSucceed()
		{
			Func<int> @delegate = () => 1;

			async Task Act()
				=> await That(@delegate).ExecutesIn(1.Seconds()).Within(TimeSpan.MaxValue);

			await That(Act).DoesNotThrow()
				.Because("any execution time is within an unlimited tolerance");
		}
	}
}
