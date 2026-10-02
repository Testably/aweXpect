using System.Threading;
using aweXpect.Customization;
using aweXpect.Options;

namespace aweXpect.Tests;

public sealed class RepeatedCheckExtensionTests
{
	[Fact]
	public async Task CheckRepeatedly_WhenCallerCancelsWhileRetrying_ShouldBeInconclusive()
	{
		MyRepeatedCheckExtensions.Probe probe = new(() => 0);
		using CancellationTokenSource cts = new();
		cts.CancelAfter(50.Milliseconds());

		async Task Act()
			=> await That(probe).ReturnsPositive().Within(30.Seconds()).WithCancellation(cts.Token);

		await That(Act).Throws<InconclusiveException>()
			.WithMessage("""
			             Expected that probe
			             returns a positive value within 0:30,
			             but it could not be verified, because the evaluation was already canceled
			             """).WithTimeout(10.Seconds())
			.Because("an extension reports the undecided outcome of CheckRepeatedly as inconclusive");
	}

	[Fact]
	public async Task CheckRepeatedly_WhenInvertedAndProbeBecomesFalseWithinTheTimeout_ShouldSucceed()
	{
		int count = 0;
		MyRepeatedCheckExtensions.Probe probe = new(() => ++count > 2 ? 0 : 1);

		async Task Act()
			=> await That(probe).DoesNotReturnPositive().Within(30.Seconds()).CheckEvery(1.Milliseconds());

		await That(Act).DoesNotThrow();
		await That(count).IsEqualTo(3)
			.Because("a negated check is repeated until the condition is no longer met");
	}

	[Fact]
	public async Task CheckRepeatedly_WhenProbeBecomesTrueWithinTheTimeout_ShouldSucceed()
	{
		int count = 0;
		MyRepeatedCheckExtensions.Probe probe = new(() => ++count > 2 ? 1 : 0);

		async Task Act()
			=> await That(probe).ReturnsPositive().Within(30.Seconds()).CheckEvery(1.Milliseconds());

		await That(Act).DoesNotThrow();
		await That(count).IsEqualTo(3)
			.Because("the check is repeated until it succeeds and not after");
	}

	[Fact]
	public async Task CheckRepeatedly_WhenProbeStaysFalse_ShouldCheckAgainAtTheTimeoutAndFail()
	{
		int count = 0;
		MyRepeatedCheckExtensions.Probe probe = new(() =>
		{
			count++;
			return 0;
		});

		async Task Act()
			=> await That(probe).ReturnsPositive().Within(500.Milliseconds()).CheckEvery(1.Hours());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that probe
			             returns a positive value within 0:00.500,
			             but it returned 0
			             """);
		await That(count).IsEqualTo(2)
			.Because("the wait is shortened to the timeout, so the last check is made at the timeout");
	}

	[Fact]
	public async Task
		CheckRepeatedly_WhenTestCancellationTimeoutIsShorterAndWithTimeoutIsLonger_ShouldFailWithTheTestCancellationTimeout()
	{
		MyRepeatedCheckExtensions.Probe probe = new(() => 0);
		Exception? exception;
		using (IDisposable _ = Customize.aweXpect.Settings().TestCancellation
			       .Set(TestCancellation.FromTimeout(300.Milliseconds())))
		{
			async Task Act()
				=> await That(probe).ReturnsPositive().Within(2.Seconds()).WithTimeout(10.Seconds());

			exception = await Record.ExceptionAsync(Act);
		}

		await That(exception).IsExactly<XunitException>().And
			.HasMessage("""
			            Expected that probe
			            returns a positive value within 0:02,
			            but it did not finish within 0:00.300
			            """).And
			.HasInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.300."))
			.Because("the effective timeout is the tighter of WithTimeout and TestCancellation");
	}

	[Fact]
	public async Task CheckRepeatedly_WithoutWithin_ShouldCheckOnceAndFail()
	{
		int count = 0;
		MyRepeatedCheckExtensions.Probe probe = new(() => ++count > 1 ? 1 : 0);

		async Task Act()
			=> await That(probe).ReturnsPositive();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that probe
			             returns a positive value,
			             but it returned 0
			             """);
		await That(count).IsEqualTo(1)
			.Because("without a timeout the check is not repeated");
	}

	[Fact]
	public async Task IsRepeated_WhenWithinIsNotSpecified_ShouldBeFalse()
	{
		RepeatedCheckOptions options = new();

		await That(options.IsRepeated).IsFalse();
	}

	[Theory]
	[InlineData(0, false)]
	[InlineData(1, true)]
	[InlineData(-1, true)]
	public async Task IsRepeated_WhenWithinIsSpecified_ShouldBeTrueForAPositiveOrInfiniteTimeout(
		int timeoutMilliseconds, bool expected)
	{
		RepeatedCheckOptions options = new();

		options.Within(TimeSpan.FromMilliseconds(timeoutMilliseconds));

		await That(options.IsRepeated).IsEqualTo(expected)
			.Because("a zero timeout leaves no time for another check, and -1 ms is the infinite timeout");
	}
}
