namespace aweXpect.TestHelpers;

/// <summary>
///     Captures the exception that a callback throws, for tests that inspect it next to other observations.
/// </summary>
public static class Catch
{
	/// <summary>
	///     Returns the exception that the <paramref name="callback" /> throws, or <see langword="null" />.
	/// </summary>
	public static Exception? Exception(Func<object?> callback)
	{
		try
		{
			_ = callback();
			return null;
		}
		catch (Exception exception)
		{
			return exception;
		}
	}

	/// <summary>
	///     Returns the exception that the <paramref name="callback" /> throws, or <see langword="null" />.
	/// </summary>
	public static async Task<Exception?> ExceptionAsync(Func<Task> callback)
	{
		try
		{
			await callback();
			return null;
		}
		catch (Exception exception)
		{
			return exception;
		}
	}
}
