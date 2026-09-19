using System.ComponentModel;

namespace aweXpect.Tests;

public sealed partial class ThatEventRecording
{
	private sealed class CustomEventWithoutParametersClass
	{
		public delegate void CustomEventDelegate();

		public event CustomEventDelegate? CustomEvent;

		public void NotifyCustomEvent()
			=> CustomEvent?.Invoke();

		public void NotifyCustomEvents(int notificationCount)
		{
			for (int i = 0; i < notificationCount; i++)
			{
				CustomEvent?.Invoke();
			}
		}
	}

	private sealed class CustomEventWithParametersClass<T1>
	{
		public delegate void CustomEventDelegate(T1 arg1);

		public event CustomEventDelegate? CustomEvent;

		public void NotifyCustomEvent(T1 arg1)
			=> CustomEvent?.Invoke(arg1);
	}

	private sealed class CustomEventWithParametersClass<T1, T2>
	{
		public delegate void CustomEventDelegate(T1 arg1, T2 arg2);

		public event CustomEventDelegate? CustomEvent;

		public void NotifyCustomEvent(T1 arg1, T2 arg2)
			=> CustomEvent?.Invoke(arg1, arg2);
	}

	private sealed class CustomEventWithParametersClass<T1, T2, T3>
	{
		public delegate void CustomEventDelegate(T1 arg1, T2 arg2, T3 arg3);

		public event CustomEventDelegate? CustomEvent;

		public void NotifyCustomEvent(T1 arg1, T2 arg2, T3 arg3)
			=> CustomEvent?.Invoke(arg1, arg2, arg3);
	}

	private sealed class CustomEventWithParametersClass<T1, T2, T3, T4>
	{
		public delegate void CustomEventDelegate(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

		public event CustomEventDelegate? CustomEvent;

		public void NotifyCustomEvent(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
			=> CustomEvent?.Invoke(arg1, arg2, arg3, arg4);
	}

	private sealed class CustomEventWithParametersClass<T1, T2, T3, T4, T5>
	{
		public delegate void CustomEventDelegate(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

#pragma warning disable CS0067 // Event is never used
		public event CustomEventDelegate? CustomEvent;
#pragma warning restore CS0067 // Event is never used
	}

	private sealed class PropertyChangedClass : INotifyPropertyChanged
	{
		public int MyValue { get; set; }
		public event PropertyChangedEventHandler? PropertyChanged;

		public void NotifyPropertyChanged(string propertyName)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public void NotifyPropertyChanged(object? sender, string? propertyName)
			=> PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
	}

	/// <remarks>
	///     Separate from <see cref="PropertyChangedClass" />, so that the additional members do not change
	///     the formatted output of the events recorded in the other tests.
	/// </remarks>
	private sealed class PropertyChangedWithMembersClass : INotifyPropertyChanged
	{
		public int MyField = 1;

		public PropertyChangedWithMembersClass? Inner { get; set; }

		public int MyValue { get; set; }

		public int this[int index] => index;
		public event PropertyChangedEventHandler? PropertyChanged;

		public int GetMyValue()
			=> MyValue;

		public void NotifyPropertyChanged(string? propertyName)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
