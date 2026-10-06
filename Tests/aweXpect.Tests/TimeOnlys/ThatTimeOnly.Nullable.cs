#if NET8_0_OR_GREATER
namespace aweXpect.Tests;

public sealed partial class ThatTimeOnly
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public sealed partial class Nullable
	{
		/// <summary>
		///     Use a fixed random time in each test run to ensure, that the tests don't rely on special times.
		/// </summary>
		private static readonly Lazy<TimeOnly?> CurrentTimeLazy = new(() => TimeOnly.MinValue.Add(new Random().Next(100, 86300).Seconds()));

		private static TimeOnly? CurrentTime()
			=> CurrentTimeLazy.Value;

		private static TimeOnly? EarlierTime(int seconds = 1)
			=> CurrentTime()?.Add(-seconds.Seconds());

		private static TimeOnly? LaterTime(int seconds = 1)
			=> CurrentTime()?.Add(seconds.Seconds());
	}
}
#endif
