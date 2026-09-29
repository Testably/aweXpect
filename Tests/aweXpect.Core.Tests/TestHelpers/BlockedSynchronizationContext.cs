using System.Runtime.ExceptionServices;
using System.Threading;

namespace aweXpect.Core.Tests.TestHelpers;

/// <summary>
///     A synchronization context that never runs the posted callbacks, like the UI thread of WPF or WinForms while it
///     is blocked.
/// </summary>
internal sealed class BlockedSynchronizationContext : SynchronizationContext
{
	public override void Post(SendOrPostCallback d, object? state) { }

	/// <summary>
	///     Runs the <paramref name="action" /> on a dedicated thread with a <see cref="BlockedSynchronizationContext" />
	///     and returns whether it completed.
	/// </summary>
	/// <remarks>
	///     It gives up after ten seconds, so that a deadlock fails the test instead of hanging the test run.
	/// </remarks>
	public static bool Run(Action action)
	{
		ExceptionDispatchInfo? exception = null;
		Thread thread = new(() =>
		{
			SetSynchronizationContext(new BlockedSynchronizationContext());
			try
			{
				action();
			}
			catch (Exception ex)
			{
				exception = ExceptionDispatchInfo.Capture(ex);
			}
		})
		{
			IsBackground = true,
		};
		thread.Start();
		bool completed = thread.Join(TimeSpan.FromSeconds(10));
		exception?.Throw();
		return completed;
	}
}
