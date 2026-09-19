namespace aweXpect.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed partial class ThatDateTime
{
	/// <summary>
	///     Use a fixed random time in each test run to ensure, that the tests don't rely on special times.
	/// </summary>
	private static readonly Lazy<DateTime> CurrentTimeLazy = new(
		() => DateTime.MinValue.AddSeconds(new Random().Next(100, 100000)));

	private static DateTime CurrentTime()
		=> CurrentTimeLazy.Value;

	private static DateTime CurrentTime(DateTimeKind kind)
		=> DateTime.SpecifyKind(CurrentTime(), kind);

	private static DateTime EarlierTime(int seconds = 1)
		=> CurrentTime().AddSeconds(-1 * seconds);

	private static DateTime EarlierTime(int seconds, DateTimeKind kind)
		=> DateTime.SpecifyKind(EarlierTime(seconds), kind);

	private static DateTime LaterTime(int seconds = 1)
		=> CurrentTime().AddSeconds(seconds);

	private static DateTime LaterTime(int seconds, DateTimeKind kind)
		=> DateTime.SpecifyKind(LaterTime(seconds), kind);
}
