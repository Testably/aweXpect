using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Sources;

public class DelegateSourceTests
{
	[Fact]
	public async Task ForExecutionTime_ShouldUseElapsedFromTimeSystem()
	{
		TimeSystemMock timeSystem = new TimeSystemMock().SetElapsed(1100.Milliseconds());

		async Task Act() =>
			await That(() => { }).ExecutesIn().AtLeast(1000.Milliseconds())
				.UseTimeSystem(timeSystem);

		await That(Act).DoesNotThrow();
	}

	[Fact]
	public async Task WhenDelegateIsCanceledByTheTimeout_ShouldReportThatItDidNotFinish()
	{
		Action<CancellationToken> @delegate = token =>
		{
			token.WaitHandle.WaitOne(30.Seconds());
			token.ThrowIfCancellationRequested();
		};

		async Task Act()
			=> await That(@delegate).ExecutesIn().AtMost(50.Milliseconds());

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that @delegate
			             executes in at most 0:00.050,
			             but it did not finish within 0:00.050
			             """).And
			.WithInner<TimeoutException>(inner => inner.HasMessage("The operation did not finish within 0:00.050."))
			.Because("a synchronous delegate that reacts to the timeout is reported the same way");
	}
}
