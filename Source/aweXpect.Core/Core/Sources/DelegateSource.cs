using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.TimeSystem;

namespace aweXpect.Core.Sources;

internal class DelegateSource : IValueSource<DelegateValue>
{
	private readonly Action<CancellationToken>? _action;

	public DelegateSource(Action<CancellationToken>? action)
	{
		ThrowIfAsyncVoid(action?.GetMethodInfo(), "Func<CancellationToken, Task>");
		_action = action;
	}

	public DelegateSource(Action? action)
	{
		ThrowIfAsyncVoid(action?.GetMethodInfo(), "Func<Task>");
		_action = action is null ? null : _ => action();
	}

	#region IValueSource<DelegateValue> Members

	public Task<DelegateValue> GetValue(ITimeSystem timeSystem,
		CancellationToken cancellationToken)
	{
		if (_action is null)
		{
			return Task.FromResult(new DelegateValue(null, TimeSpan.Zero, true));
		}

		IStopwatch sw = timeSystem.Stopwatch.New();
		try
		{
			sw.Start();
			_action(cancellationToken);
			sw.Stop();
			return Task.FromResult(new DelegateValue(null, sw.Elapsed));
		}
		catch (Exception ex)
		{
			return Task.FromResult(new DelegateValue(ex, sw.Elapsed));
		}
	}

	#endregion

	private static void ThrowIfAsyncVoid(MethodInfo? method, string replaceType)
	{
		if (method is not null &&
		    Attribute.IsDefined(method, typeof(AsyncStateMachineAttribute), true))
		{
			throw Tracing.WriteException(
				new InvalidOperationException(
					$"Cannot use aweXpect on an async void method: Use {replaceType} instead."));
		}
	}
}
