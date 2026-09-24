namespace aweXpect.Core.Tests.TestHelpers;

internal static class PendingTask
{
	/// <summary>
	///     A task that is still pending for as long as a test waits for it.
	/// </summary>
	/// <remarks>
	///     It faults after half a minute, so that a regression which awaits it to completion fails the test instead of
	///     hanging the test run.
	/// </remarks>
	public static Task<T> Of<T>()
		=> Task.Delay(TimeSpan.FromSeconds(30))
			.ContinueWith<T>(_ => throw new TimeoutException("The pending task was awaited to completion."));
}
