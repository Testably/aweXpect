using System;
using System.Threading;

namespace aweXpect.Customization;

/// <summary>
///     The lifetime of a customization setting.
/// </summary>
public sealed class CustomizationLifetime(Action callback) : IDisposable
{
	private Action? _callback = callback;

	/// <inheritdoc cref="IDisposable.Dispose()" />
	/// <remarks>
	///     Only the first call has an effect.
	/// </remarks>
	public void Dispose() => Interlocked.Exchange(ref _callback, null)?.Invoke();
}
