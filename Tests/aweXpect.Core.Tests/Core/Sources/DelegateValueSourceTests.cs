using System.Threading;
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

	[Test]
	public async Task WhenDelegateIsNull_ShouldFail()
	{
		Func<int>? @delegate = null;

		async Task Act()
			=> await That(@delegate!).DoesNotThrow();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that @delegate
			             does not throw any exception,
			             but it was <null>
			             """);
	}

	[Test]
	public async Task WhenDelegateWithCancellationTokenIsNull_ShouldFail()
	{
		Func<CancellationToken, int>? @delegate = null;

		async Task Act()
			=> await That(@delegate!).DoesNotThrow();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that @delegate
			             does not throw any exception,
			             but it was <null>
			             """);
	}

	[Test]
	public async Task WhenDelegateWithCancellationTokenReturnsAValue_ShouldPassTheValue()
	{
		Func<CancellationToken, int> @delegate = _ => 42;

		async Task Act()
			=> await That(@delegate).DoesNotThrow().WhoseResult.IsEqualTo(42);

		await That(Act).DoesNotThrow();
	}
}
