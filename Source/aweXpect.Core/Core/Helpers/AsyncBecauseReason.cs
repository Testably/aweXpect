using System;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Helpers;

internal struct AsyncBecauseReason(Task<string?> reason) : IBecauseReason
{
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

#if NET8_0_OR_GREATER
	public async ValueTask<ConstraintResult>
#else
	public async Task<ConstraintResult>
#endif
		ApplyTo(ConstraintResult result)
	{
		if (_message is null)
		{
			// The reason is only needed for a failure message, so a broken reason provider must not fail a met expectation.
			if (result.Outcome == Outcome.Success)
			{
				ObserveExceptions(reason);
				return result;
			}

			string? resolvedReason;
			try
			{
				resolvedReason = await reason.ConfigureAwait(false);
			}
			catch (Exception exception)
			{
				resolvedReason = $"the reason could not be determined: {Formatter.Format(exception)}";
			}

			if (string.IsNullOrEmpty(resolvedReason))
			{
				return result;
			}

			_message = CreateMessage(resolvedReason);
		}

		string message = _message;
		return result.AppendExpectationText(e => e.Append(message));
	}
}
