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
}
