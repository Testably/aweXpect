using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Sources;

public class DelegateAsyncValueSourceTests
{
	[Fact]
	public async Task ForExecutionTime_ShouldUseElapsedFromTimeSystem()
	{
		TimeSystemMock timeSystem = new TimeSystemMock().SetElapsed(1100.Milliseconds());

		async Task Act() =>
			await That(() => Task.FromResult(0)).ExecutesIn().AtLeast(1000.Milliseconds())
				.UseTimeSystem(timeSystem);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenDelegateCompletesWithinTheTimeout_ShouldSucceed()
	{
		Func<Task<int>> @delegate = () => Task.Delay(50.Milliseconds()).ContinueWith(_ => 1);

		async Task Act()
			=> await That(@delegate).DoesNotThrow().WhoseResult.IsEqualTo(1).WithTimeout(5.Seconds());

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenDelegateDoesNotCompleteWithinTheTimeout_ShouldFail()
	{
		Func<Task<int>> @delegate = () => PendingTask.Of<int>();

		async Task Act()
			=> await That(@delegate).DoesNotThrow().WithTimeout(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that @delegate
			             does not throw any exception,
			             but it did not finish within 0:00.050
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
			.Because("the timeout must abandon the task instead of awaiting it to completion");
	}

	[Fact]
	public async Task WhenDelegateDoesNotCompleteWithinTheTimeout_WhoseResult_ShouldFail()
	{
		Func<Task<int>> @delegate = () => PendingTask.Of<int>();

		async Task Act()
			=> await That(@delegate).DoesNotThrow().WhoseResult.IsEqualTo(1).WithTimeout(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that @delegate
			             does not throw any exception and its result is equal to 1,
			             but it did not finish within 0:00.050
			             """).And
			.WithInner<TimeoutException>()
			.Because("the timeout is reported once, and not also as a thrown exception");
	}

	[Fact]
	public async Task WhenDelegateIgnoresTheCancellationToken_ShouldFail()
	{
		Func<CancellationToken, Task<int>> @delegate = _ => PendingTask.Of<int>();

		async Task Act()
			=> await That(@delegate).ExecutesIn().AtMost(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that @delegate
			             executes in at most 0:00.050,
			             but it did not finish within 0:00.050
			             """)
			.Because("a delegate that ignores the cancelled token must be abandoned as well");
	}
}
