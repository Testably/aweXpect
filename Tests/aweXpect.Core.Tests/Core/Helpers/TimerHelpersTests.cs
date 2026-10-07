using System.Threading;
using aweXpect.Chronology;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Tests.Core.Helpers;

public sealed class TimerHelpersTests
{
	[Test]
	public async Task Tighter_WhenBothAreNull_ShouldBeNull()
	{
		TimeSpan? result = TimerHelpers.Tighter(null, null);

		await That(result).IsNull();
	}

	[Test]
	public async Task Tighter_WhenFirstIsInfinite_ShouldBeTheSecond()
	{
		TimeSpan? result = TimerHelpers.Tighter(Timeout.InfiniteTimeSpan, 1.Seconds());

		await That(result).IsEqualTo(1.Seconds());
	}

	[Test]
	public async Task Tighter_WhenFirstIsInfiniteAndSecondIsNull_ShouldBeInfinite()
	{
		TimeSpan? result = TimerHelpers.Tighter(Timeout.InfiniteTimeSpan, null);

		await That(result).IsEqualTo(Timeout.InfiniteTimeSpan);
	}

	[Test]
	public async Task Tighter_WhenFirstIsLonger_ShouldBeTheSecond()
	{
		TimeSpan? result = TimerHelpers.Tighter(2.Seconds(), 1.Seconds());

		await That(result).IsEqualTo(1.Seconds());
	}

	[Test]
	public async Task Tighter_WhenFirstIsShorter_ShouldBeTheFirst()
	{
		TimeSpan? result = TimerHelpers.Tighter(1.Seconds(), 2.Seconds());

		await That(result).IsEqualTo(1.Seconds());
	}

	[Test]
	public async Task Tighter_WhenSecondIsInfinite_ShouldBeTheFirst()
	{
		TimeSpan? result = TimerHelpers.Tighter(1.Seconds(), Timeout.InfiniteTimeSpan);

		await That(result).IsEqualTo(1.Seconds());
	}

	[Test]
	public async Task Tighter_WhenSecondIsNull_ShouldBeTheFirst()
	{
		TimeSpan? result = TimerHelpers.Tighter(1.Seconds(), null);

		await That(result).IsEqualTo(1.Seconds());
	}
}
