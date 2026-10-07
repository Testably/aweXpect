using aweXpect.Chronology;
using aweXpect.Core.Internal;
using aweXpect.Core.Tests.TestHelpers;

namespace aweXpect.Core.Tests.Core.Sources;

public class DelegateValueSourceTests
{
	[Test]
	public async Task ForExecutionTime_ShouldUseElapsedFromTimeSystem()
	{
		TimeSystemMock timeSystem = new TimeSystemMock().SetElapsed(1100.Milliseconds());

		async Task Act() =>
			await That(() => 1).ExecutesIn().AtLeast(1000.Milliseconds())
				.WithTimeSystem(timeSystem);

		await That(Act).DoesNotThrow();
	}
}
