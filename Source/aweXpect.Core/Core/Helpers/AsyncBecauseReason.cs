using System;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Helpers;

/// <remarks>
///     A class, so that the message resolved by one use is kept for every later use and the reason is awaited at most
///     once.
/// </remarks>
internal sealed class AsyncBecauseReason(Task<string?> reason) : IBecauseReason
{
	private bool _isResolved;
	private string? _message;

	private static string CreateMessage(string reason)
	{
		const string prefix = "because";
		string message = reason.Trim();

		return !message.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
			? $", {prefix} {message}"
			: $", {message}";
	}

	/// <summary>
	///     Marks a faulted <paramref name="task" /> as observed, so that a reason that is never awaited cannot surface
	///     later as an <see cref="TaskScheduler.UnobservedTaskException" />.
	/// </summary>
	private static void ObserveExceptions(Task<string?> task)
		=> task.ContinueWith(static t => _ = t.Exception,
			TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

	public async ValueTask<ConstraintResult>
		ApplyTo(ConstraintResult result)
	{
		// The reason is only needed for a failure message, so a broken reason provider must not fail a met expectation.
		if (result.Outcome == Outcome.Success)
		{
			if (!_isResolved)
			{
				ObserveExceptions(reason);
			}

			return result;
		}

		await Resolve();
		if (_message is not { } message)
		{
			return result;
		}

		return result.AppendExpectationText(e => e.Append(message));
	}

	/// <summary>
	///     Awaits the reason, unless it was already resolved, and caches its message.
	/// </summary>
	public async Task Resolve()
	{
		if (_isResolved)
		{
			return;
		}

		string? resolvedReason;
		try
		{
			resolvedReason = await reason.ConfigureAwait(false);
		}
		catch (Exception exception)
		{
			resolvedReason =
				$"the reason did throw {Formatter.Format(exception.GetType()).PrependAOrAn()}: {exception.Message}";
		}

		if (!string.IsNullOrEmpty(resolvedReason))
		{
			_message = CreateMessage(resolvedReason);
		}

		_isResolved = true;
	}

	/// <summary>
	///     The message of the reason, or an empty string while it is not resolved.
	/// </summary>
	public override string ToString()
		=> _message ?? "";
}
