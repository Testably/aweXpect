namespace aweXpect.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed partial class ThatTimeSpan
{
	/// <summary>
	///     Use a fixed random time in each test run to ensure, that the tests don't rely on special times.
	/// </summary>
	private static readonly Lazy<TimeSpan> CurrentTimeLazy = new(() => new Random().Next(100, 100000).Seconds());

	private static TimeSpan CurrentTime()
		=> CurrentTimeLazy.Value;

	private static TimeSpan EarlierTime(int seconds = 1)
		=> CurrentTime() - seconds.Seconds();

	private static TimeSpan LaterTime(int seconds = 1)
		=> CurrentTime() + seconds.Seconds();
}
